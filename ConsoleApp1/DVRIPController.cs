using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class DVRIPController
    {
        private String session;

        public string Session { get => session; set => session = value; }

        public byte[] Login() {

            JObject o = JObject.FromObject(new
            { 
                    EncryptType = "MD5",
                    LoginType = "DVRIP-Xm030",
                    PassWord = "8fVv3M2n",
                    UserName =  "admin"
            }); 
            return Encoding.UTF8.GetBytes(o.ToString()); 
        }
        public byte[] KeepAlive() {   
            JObject o = JObject.FromObject(new
                {
                    Name = "KeepAlive",
                    SessionID = session
                });
            return Encoding.UTF8.GetBytes(o.ToString());
        }
        public byte[] DownloadVideo() {
            //   { "Name":"OPPlayBack","OPPlayBack":{ "Action":"DownloadStart","Parameter":{ "PlayMode":"ByName","FileName":"/idea0/2020-05-22/001/00.00.38-00.01.19[M][@1834][0].h264","Stream_Type":0,"Value":0,"TransMode":"TCP"},"StartTime":"2020-05-22 00:00:38","EndTime":"2020-05-22 00:01:19"},"SessionID":"0x%08X" % self.session})
 
            JObject o = JObject.FromObject(new
            {
                Name = "OPPlayBack",
                OPPlayBack = new
                {
                    Action = "Claim",
                    Parameter = new
                    {
                        PlayMode = "ByName",
                        FileName = "/idea0/2020-05-25/001/03.48.24-03.48.35[M][@38a][0].h264",
                        Stream_Type = 0,
                        Value = 0,
                        TransMode = "TCP"
                    },
                    StartTime = "2020-05-25 03:48:24", 
                    EndTime = "2020-05-25 03:48:35"
                },
            SessionID = "0x00"+session.Substring(2)
            });
            return Encoding.UTF8.GetBytes(o.ToString());
        }
        public byte[] DownloadStart()
        { 
            JObject o = JObject.FromObject(new
            {
                Name = "OPPlayBack",
                OPPlayBack = new
                {
                    Action = "DownloadStart",
                    Parameter = new
                    {
                        PlayMode = "ByName",
                        FileName = "/idea0/2020-05-25/001/03.48.24-03.48.35[M][@38a][0].h264",
                        Stream_Type = 0,
                        Value = 0,
                        TransMode = "TCP"
                    },
                    StartTime = "2020-05-25 03:48:24",
                    EndTime = "2020-05-25 03:48:35"
                },
                SessionID = "0x00" + session.Substring(2)
            });
            return Encoding.UTF8.GetBytes(o.ToString());
        }
        public byte[] DownloadStop()
        {
            JObject o = JObject.FromObject(new
            {
                Name = "OPPlayBack",
                OPPlayBack = new
                {
                    Action = "DownloadStop",
                    Parameter = new
                    {
                        PlayMode = "ByTime",
                        FileName = "/idea0/2015-10-20/001/00.00.00-00.00.09[M][@bff][0].h264",
                        Channel = 0,
                        Stream_Type = 0,
                        Value = 0,
                        TransMode = "TCP"
                    },
                    StartTime = "2015-10-20 01:00:00",
                    EndTime = "2015-10-20 01:00:09"
                },
                SessionID = "0x00" + session.Substring(2)
            });
            return Encoding.UTF8.GetBytes(o.ToString());
        }
        public byte[] OpMonitor(String action)
        {
            //1413
            //"Name":"OPMonitor","OPMonitor":{ "Action":"Claim","Parameter":{ "Channel":0,"CombinMode":"CONNECT_ALL","StreamType":"Main","TransMode":"TCP"} },"SessionID"
            JObject o = JObject.FromObject(new
            {
                Name = "OPMonitor",
                OPMonitor = new
                {
                    Action = "Claim",
                    Parameter = new
                    {
                        CombinMode = "CONNECT_ALL", 
                        Channel = 0,
                        StreamType = "Main", 
                        TransMode = "TCP"
                    }
                },
                SessionID = "0x00" + session.Substring(2)
            });

            return Encoding.UTF8.GetBytes(o.ToString());

        }
        public byte[] OPFileQuery()
        {
            //1413
            //"Name":"OPMonitor","OPMonitor":{ "Action":"Claim","Parameter":{ "Channel":0,"CombinMode":"CONNECT_ALL","StreamType":"Main","TransMode":"TCP"} },"SessionID"
            JObject o = JObject.FromObject(new
            {
                Name = "OPFileQuery",
                OPFileQuery = new
                {
                    BeginTime = "2020-05-25 00:00:00",
                    EndTime = "2020-05-25 23:59:59", 
                        Channel = 0,
                        DriverTypeMask = "0x0000FFFF",
                        StreamType = "0x00000000",
                        Event = "*",
                        Type = "h264"
                },
                SessionID = "0x00" + session.Substring(2)
            });

            return Encoding.UTF8.GetBytes(o.ToString());

        }
    }

}
