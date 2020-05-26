using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmEyePersonalCloud
{
    public static class Action
    {
        static String claim = "Claim";
        static String downloadStart = "DownloadStart";
        static String downloadStop = "DownloadStop";

        public static string Claim { get => claim; set => claim = value; }
        public static string DownloadStart { get => downloadStart; set => downloadStart = value; }
        public static string DownloadStop { get => downloadStop; set => downloadStop = value; }
    }
}
