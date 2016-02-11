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
            //string usernamePassword = user + ":" + password;
            //CredentialCache cache = new CredentialCache();
            //cache.Add(new Uri(url), "Basic", new NetworkCredential(user, password));

            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            //request.Credentials = cache;
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

        public static void PutHTTP(string user, string password, string postString, string url)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            //request.Credentials = cache;
            request.Credentials = GetCredentialCache(url, user, password);
            request.Headers.Add("Authorization", GetAuthorization(user, password));
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.ContentLength = postString.Length;
            
            // Ignore Certificate validation failures (aka untrusted certificate + certificate chains)
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            byte[] bytes = Encoding.UTF8.GetBytes(postString);
            //using (var requestStream = request.GetRequestStream())
            //{
            //    requestStream.Write(bytes, 0, bytes.Length);
            //}
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(postString);
            }
            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
                Console.WriteLine("Update completed");
            else
                Console.WriteLine( "Error in update");
        }

        //public static string PostHttp(string user, string password, string url, string body, string contentType)
        //{
        //    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        //    request.Credentials = GetCredentialCache(url, user, password);

        //    httpWebRequest.ContentType = contentType;
        //    httpWebRequest.Method = "POST";
        //    httpWebRequest.Timeout = 20000;

        //    byte[] btBodys = Encoding.UTF8.GetBytes(body);
        //    httpWebRequest.ContentLength = btBodys.Length;
        //    httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

        //    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //    StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
        //    string responseContent = streamReader.ReadToEnd();

        //    httpWebResponse.Close();
        //    streamReader.Close();
        //    httpWebRequest.Abort();
        //    httpWebResponse.Close();

        //    return responseContent;
        //}


        private static WebClient BuildWebClient(string user, string password)
        {
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true); ;
            WebClient client = new WebClient { Credentials = new NetworkCredential(user, password), Encoding = Encoding.UTF8 };
            //client.Credentials = CredentialCache.DefaultCredentials;
            return client;
        }

        public static string Query(string user, string password, string url)
        {
            WebClient client = BuildWebClient(user, password);
            try
            {
                // Inject this string as the Authorization header
                client.Headers[HttpRequestHeader.Authorization] = GetAuthorization(user, password);
                client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache);
                var response = client.DownloadString(url);
                return response;
            }
            catch (Exception exp)
            {
                log.Error(string.Format("cannot get query resopnse,query text={0}", url), exp);
                throw exp;
            }
            finally
            {
                client.Dispose();
            }
        }

        public static void Put(string user, string password, string postString, string url)
        {
            WebClient client = BuildWebClient(user, password);
            try
            {
                client.Headers[HttpRequestHeader.Authorization] = GetAuthorization(user, password);
                byte[] postData = Encoding.UTF8.GetBytes(postString);
                //client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("Content-Type", "application/json");
                //client.UploadString(url, "PUT", postString);
                using (Stream stream = client.OpenWrite(url, "PUT"))
                {
                    using (StreamWriter streamWriter = new StreamWriter(stream))
                    {
                        streamWriter.WriteLine(postString);
                    }
                    //string response = Encoding.UTF8.GetString(responseData);

                    //return response;
                }
            }
            catch (Exception exp)
            {
                log.Error(string.Format("cannot get query resopnse,query text={0}", url), exp);
                throw exp;
            }
            finally
            {
                client.Dispose();
            }
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
