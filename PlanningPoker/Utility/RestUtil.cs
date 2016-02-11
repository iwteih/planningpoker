using log4net;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace PlanningPoker.Utility
{
    public class RestUtil
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static HttpStatusCode GetEntity_Post<T>(string username, string password, string url, string payload) where T : new()
        {
            var client = new RestClient(IPUtil.GetHost(url));
            var request = new RestRequest(IPUtil.GetResource(url), Method.POST);
            request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("application/json; charset=utf-8", payload, ParameterType.RequestBody);
            request.AddBody(payload);
            request.RequestFormat = DataFormat.Json;
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                log.Error(string.Format("error post,url={0},status={1}", url, response.StatusCode));
                var exp = new ApplicationException(response.StatusDescription, response.ErrorException);
                throw exp;
            }
            return response.StatusCode;
        }

        public static T Get<T>(string user, string password, string url) where T : new()
        {
            var client = new RestClient(IPUtil.GetHost(url));
            client.Authenticator = new HttpBasicAuthenticator(user, password);
            var request = new RestRequest(IPUtil.GetResource(url), Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                log.Error(string.Format("error post,url={0},status={1}", url, response.StatusCode));
                var exp = new ApplicationException(response.StatusDescription, response.ErrorException);
                throw exp;
            }

            return response.Data;
        }

        public static HttpStatusCode Put(string user, string password, string payload, string url)
        {
            var client = new RestClient(IPUtil.GetHost(url));
            client.Authenticator = new HttpBasicAuthenticator(user, password);
            var request = new RestRequest(IPUtil.GetResource(url), Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("application/json", payload, ParameterType.RequestBody);
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                log.Error(string.Format("error post,url={0},status={1}", url, response.StatusCode));
                var exp = new ApplicationException(response.StatusDescription, response.ErrorException);
                throw exp;
            }

            return response.StatusCode;
        }
    }
}
