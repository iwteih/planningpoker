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
        public static string Query(string user, string password, string url)
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

        public static string Query1(string user, string password, string url)
        {
            var client = new WebClient { Credentials = new NetworkCredential(user, password) };
            var response = client.DownloadString(url);
            return response;
        }

        public static string Query2()
        {
            string json = null;
            using (StreamReader sr = new StreamReader(@"D:\code\my_proj\planningpoker\jira.json"))
            {
                json = sr.ReadToEnd();
            }

            return json;
        }
    }
}
