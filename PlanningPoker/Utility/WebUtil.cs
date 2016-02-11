using log4net;
using PlanningPoker.Entity;
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

        private static CredentialCache GetCredentialCache(string uri, string username, string password)
        {
            string authorization = string.Format("{0}:{1}", username, password);
            CredentialCache credCache = new CredentialCache();
            credCache.Add(new Uri(uri), "Basic", new NetworkCredential(username, password));

            return credCache;
        }

        private static string GetAuthorization(string username, string password)
        {
            string authorization = string.Format("{0}:{1}", username, password);
            return "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(authorization));
        }

        public static string QueryHTTP(string user, string password, string url)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Credentials = GetCredentialCache(url, user, password);
            request.Headers.Add("Authorization", GetAuthorization(user, password));
            request.Method = "GET";

            // Ignore Certificate validation failures (aka untrusted certificate + certificate chains)
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string content = streamReader.ReadToEnd();
                    return content;
                }
            }
        }

        public static HttpStatusCode PutHTTP(string user, string password, string payload, string url)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Credentials = GetCredentialCache(url, user, password);
            request.Headers.Add("Authorization", GetAuthorization(user, password));
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.ContentLength = payload.Length;
            
            // Ignore Certificate validation failures (aka untrusted certificate + certificate chains)
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(payload);
            }
            var response = (HttpWebResponse)request.GetResponse();

            return response.StatusCode;
        }

        public static void PostHTTP(string user, string password, string postString, string url)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            //request.Credentials = cache;
            request.Credentials = GetCredentialCache(url, user, password);
            request.Headers.Add("Authorization", GetAuthorization(user, password));
            request.Method = "Get";
            request.ContentType = "application/json";
            request.ContentLength = postString.Length;

            // Ignore Certificate validation failures (aka untrusted certificate + certificate chains)
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(postString);
            }
            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
                Console.WriteLine("Update completed");
            else
                Console.WriteLine("Error in update");
            
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    string html = sr.ReadToEnd();
                    Console.WriteLine(html);
                }
            }
        }

        private static WebClient BuildWebClient(string user, string password)
        {
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true); ;
            WebClient client = new WebClient { Credentials = new NetworkCredential(user, password), Encoding = Encoding.UTF8 };
            //client.Credentials = CredentialCache.DefaultCredentials;
            return client;
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
