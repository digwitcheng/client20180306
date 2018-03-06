using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace client20710711
{
    class ILog
    {
        //public  void Debug(string methodName, string message);
        //public  void Error(string methodName, string message);
        //public  void Fatal(string methodName, string message);
        //public  void Info(string methodName, string message);
        //public  void Warn(string methodName, string message);


        public void Error(string methodName, Exception message) { }
        public void ErrorFormat(string str, IPEndPoint endPoint, SslPolicyErrors err) { }
        public void DebugFormat(string str) { }


    }
}
