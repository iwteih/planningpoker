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

        public static string GetHost(string url)
        {
            Uri uri = new Uri(url);
            return uri.GetLeftPart(UriPartial.Authority);
        }

        public static string GetResource(string url)
        {
            Uri uri = new Uri(url);
            string host = uri.GetLeftPart(UriPartial.Authority);
            string resource = url.Replace(host, string.Empty);
            return resource;
        }
    }
}
