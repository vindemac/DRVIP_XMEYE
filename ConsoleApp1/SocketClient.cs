using BaseLibS.Parse.Endian;
using ConsoleApp1;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;  
using System.Net.Sockets;
using System.Numerics;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using XmEyePersonalCloud;

// Client app is the one sending messages to a Server/listener.   
// Both listener and client can send messages back and forth once a   
// communication is established.  
public class SocketClient  
{
    
    public static int Main(String[] args)  
    {  
        StartClient();  
        return 0;  
    }

    static DVRIPController dvrip = new DVRIPController();
    // State object for receiving data from remote device.  
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }


    public static byte[] FOME(String msg1)
    {
        var message = Encoding.ASCII.GetBytes(msg1);

        var lenc = new LittleEndianBitConverter();

        var bytes = new List<byte[]>(new[]
        {
                lenc.GetBytes(message.LongLength),
                message
            });

        var msg = new byte[bytes.Sum(barray => barray.LongLength)];
        int offset = 0;
        foreach (var bArray in bytes)
        {
            Buffer.BlockCopy(bArray, 0, msg, offset, bArray.Length);
            offset = bArray.Length;
        }

        Console.WriteLine(BitConverter.ToString(msg).Replace("-", ":"));
        return msg;
    }
    public static JObject parseJson(byte[] bytes) {
        byte[] response = new byte[bytes.Length - 20];
        Array.Copy(bytes, 20, response, 0, bytes.Length - 20);
        Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(response, 0, response.Length));
        return JObject.Parse(Encoding.UTF8.GetString(response, 0, response.Length)); 
    }
    public static void toFile(byte[] bytes,String filename) {
  
        Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytes.Length));
        using (var fs = new FileStream("C:\\Projects\\"+filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            fs.Write(bytes, 0, bytes.Length);

        }
    }
    public static byte[] Concat(byte[] a, byte[] b)
    {
        byte[] output = new byte[a.Length + b.Length];
        for (int i = 0; i < a.Length; i++)
            output[i] = a[i];
        for (int j = 0; j < b.Length; j++)
            output[a.Length + j] = b[j];
        return output;
    }
    private static int StringToHexToInt(string hexstring)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char t in hexstring)
        {
            //Note: X for upper, x for lower case letters
            sb.Append(Convert.ToInt32(t).ToString("x"));
        }
#if DEBUG
        Console.WriteLine(sb.ToString());
#endif
        return Convert.ToInt32(sb.ToString(), 64); 
    }
    public static byte[] PrepareStatement(MessageType msgType, String session, byte[] message, int packet_count) {
        Object[] ob = { (byte)255, (byte)0, (byte)0, (byte)0, (int) (session !=  null ? Convert.ToInt32(session.Substring(2), 16) : 0 ), (int)packet_count, (byte)0, (byte)0, (short)msgType, (int)message.Length };

        byte[] end = { (byte)0 };
        return Concat(Concat(StructConverter.Pack(ob), message), end);
    }
    public static byte[] PrepareStatement2(MessageType msgType, String session, byte[] message, int packet_count)
    {
        Object[] ob = { (byte)255, (byte)1, (byte)0, (byte)0, (int)(session != null ? Convert.ToInt32(session.Substring(2), 16) : 0), (int)packet_count, (byte)0, (byte)0, (short)msgType, (int)351};

        byte[] end = { (byte)0 };
        return Concat(Concat(StructConverter.Pack(ob), message), end);
    }
    public static void StartClient()  
    {  
        byte[] bytes = new byte[1024];  
  
        try  
        {  
            // Connect to a Remote server  
            // Get Host IP Address that is used to establish a connection  
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
            // If a host has multiple addresses, you will get a list of addresses
 
     
            IPAddress ipAddress = System.Net.IPAddress.Parse("192.168.1.72");
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 34567);
 
            // Create a TCP/IP  socket.    
            Socket sender = new Socket(AddressFamily.InterNetwork,    SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.    
            try  
            {  

                // Connect to Remote EndPoint  
                sender.Connect(remoteEP);  
  
                Console.WriteLine("Socket connected to {0}",  
                    sender.RemoteEndPoint.ToString()); 
 
                int bytesSent = sender.Send(PrepareStatement(MessageType.Login, dvrip.Session, dvrip.Login(), 0));  
    
                // Receive the response from the remote device.    
                int bytesRec = sender.Receive(bytes);
                toFile(bytes, "login.txt");

                JObject o = parseJson(bytes);
                dvrip.Session = o.SelectToken("SessionID").ToString();
  
                Console.WriteLine("-------------------------------------");
 

                bytesSent = sender.Send(PrepareStatement(MessageType.OPMonitor, dvrip.Session.ToString().Substring(2), dvrip.OpMonitor("Claim"), 24));
                /// Receive the response from the remote device.    
                bytes = new byte[4096];
                bytesRec = sender.Receive(bytes);
                toFile(bytes,"opmonitor.txt");

                /*
                bytesSent = sender.Send(PrepareStatement(MessageType.OPFileQuery, dvrip.Session.ToString().Substring(2), dvrip.OPFileQuery(), 152));
                /// Receive the response from the remote device.    
                bytes = new byte[40960];
                bytesRec = sender.Receive(bytes); 
                using (var fs = new FileStream("C:\\Projects\\filequery.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    while ((bytesRec = sender.Receive(bytes)) > 0)
                    {

                        fs.Write(bytes, 0, bytesRec);
                        if (bytesRec < 4096) {
                            break;
                        }
                    }
                }*/
                ///------------start
                bytesSent = sender.Send(PrepareStatement(MessageType.OPPlayback, dvrip.Session.ToString().Substring(2), dvrip.DownloadStart(), 64));
                /// Receive the response from the remote device.
                int read;
                byte[] buffer = new byte[4096];
                using (var fs = new FileStream("C:\\Projects\\start.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    while ((read = sender.Receive(buffer)) > 0)
                    {

                        fs.Write(buffer, 0, read);
                        if (read < 4096)
                        {
                            break;
                        }
                    }
                }
                //--------END
                bytesSent = sender.Send(PrepareStatement(MessageType.OPPlayback, dvrip.Session.ToString().Substring(2), dvrip.DownloadStop(), 72));
                /// Receive the response from the remote device.

                byte[] buffer2 = new byte[4096];
                using (var fs = new FileStream("C:\\Projects\\stop.mp4", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    while ((read = sender.Receive(buffer2)) > 0)
                    {

                        fs.Write(buffer2, 0, read);
                        if (read < 4096)
                        {
                            break;
                        }
                    }
                }

                toFile(PrepareStatement2(MessageType.OPPlayback, dvrip.Session.ToString().Substring(2), dvrip.DownloadVideo(), 24),"eita.txt");
                Console.WriteLine(Encoding.UTF8.GetString(PrepareStatement2(MessageType.OPPlayback, dvrip.Session.ToString().Substring(2), dvrip.DownloadVideo(), 24)));

                bytesSent = sender.Send(PrepareStatement2(MessageType.OPPlayback, dvrip.Session.ToString().Substring(2), dvrip.DownloadVideo(), 24));
                /// Receive the response from the remote device.

                byte[] buffer3 = new byte[4096];
                using (var fs = new FileStream("C:\\Projects\\aff3.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    while ((read = sender.Receive(buffer3)) > 0)
                    {
                    
                         fs.Write(buffer3, 0, read);
                        if (read < 4096)
                        {
                            break;
                        }
                    } 
                 }

                // Array.Copy(bytes, 20, response1, 0, bytes.Length - 20);
                //  Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(response, 0, bytesRec));

                //Release the socket.    
                sender.Shutdown(SocketShutdown.Both);  
                  sender.Close();  

            }  
            catch (ArgumentNullException ane)  
            {  
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());  
            }  
            catch (SocketException se)  
            {  
                Console.WriteLine("SocketException : {0}", se.ToString());  
            }  
            catch (Exception e)  
            {  
                Console.WriteLine("Unexpected exception : {0}", e.ToString());  
            }  
  
        }  
        catch (Exception e)  
        {  

            Console.WriteLine(e.ToString());  
        }  
    } 
        // The port number for the remote device.  
        private const int port = 34567;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.  
        private static String response = String.Empty;
 
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);
        Console.WriteLine(BitConverter.ToString(byteData));
        // Begin sending the data to the remote device.  
        client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
 
}  