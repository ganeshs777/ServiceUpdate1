using System.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Net;

namespace ServiceUpdate1.GrpcClient
{
    public static class ExtensionMethods
    {
        //public ExtensionMethods() { }

        public static bool IsAccessible(this IPAddress address )
        {
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(address, 3000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch { }
            return false;
        }
        

    }
}
