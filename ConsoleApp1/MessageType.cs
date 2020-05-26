using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmEyePersonalCloud
{
    public enum MessageType : short
	{ 
            Login = 1000,
            AlarmInfo=1504,
            AlarmSet=1500,
            KeepAlive=1006,
            ChannelTitle=1046,
            OPTimeQuery=1452,
            OPTimeSetting=1450,
            OPMailTest=1636,
            OPMachine=1450,
            OPMonitor=1413,
            OPTalk=1434,
            OPPTZControl=1400,
            OPNetKeyboard=1550,
            SystemFunction=1360,
            EncodeCapability=1360,
            OPSystemUpgrade=0x5f5,
            OPSendFile=0x5f2,
            OPPlayback=1424,
            OPFileQuery=1440
    }
}
