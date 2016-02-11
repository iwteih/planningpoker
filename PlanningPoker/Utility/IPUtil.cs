using System;
using System.Collections.Generic;
using System.Net;

namespace PlanningPoker.Utility
{
    internal class IPUtil
    {
        public static string GetLocalIP()
        {
            string addressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    addressIP = _IPAddress.ToString();
                    return addressIP;
                }
            }
            return addressIP;
        }
    }
}
