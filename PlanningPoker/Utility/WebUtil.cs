using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PlanningPoker.Utility
{
    class WebUtil
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string QueryBasicAuth(string user, string password, string url)
        {
            string usernamePassword = user + ":" + password;
            CredentialCache cache = new CredentialCache();
            cache.Add(new Uri(url), "Basic", new NetworkCredential(user, password));

            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Credentials = cache;
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(usernamePassword)));
            request.Method = "GET";

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string content = streamReader.ReadToEnd();
                    return content;
                }
            }
        }

        public static string Query(string user, string password, string url)
        {
            try
            {
                var client = new WebClient { Credentials = new NetworkCredential(user, password), Encoding = Encoding.UTF8 };
                var response = client.DownloadString(url);
                return response;
            }
            catch(Exception exp)
            {
                log.Error(string.Format("cannot get query resopnse,query text={0}", url), exp); 
            }
            return string.Empty;
        }

        public static string Query2()
        {
            string json = null;
            using (StreamReader sr = new StreamReader(@"..\..\..\jira.json"))
            {
                json = sr.ReadToEnd();
            }

            return json;
        }
    }
}
