using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlanningPoker.Entity;
using PlanningPoker.PMS.JIRA;
using PlanningPoker.Utility;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;

namespace PlanningPoker.PMS
{
    class JIRAOperator : IPMSOperator
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string POST_URL = "{0}/rest/api/2/issue/{1}";
        private static readonly string KEYS_URL = "{0}?jql=key in ({1})";

        private string BuildCardUrl(string baseUrl, string key)
        {
            return string.Format("{0}/browse/{1}", baseUrl.Substring(0, baseUrl.IndexOf("/rest/api")), key);
        }

        public List<Story> Query(string user, string password, string url)
        {
            var response = QueryResponse(user, password, url);

            List<Story> list = QueryStory(response);
            
            // set story point
            SetStoryPoint(list, response.Content);

            // set story points for subtasks
            SetStoryPointForSubTasks(list, user, password, url);

            return list;
        }

        private IRestResponse<IssueList> QueryResponse(string user, string password, string url)
        {
            var response = RestUtil.Get<IssueList>(user, password, url);
            return response;
        }

        private List<Story> QueryStory(IRestResponse<IssueList> response)
        {
            var list = new List<Story>();

            if (response.Data != null)
            {
                foreach (Issue issue in response.Data.Issues)
                {
                    Story story = new Story();
                    story.UUID = Guid.NewGuid();
                    story.ID = issue.Key;
                    story.Summary = issue.Fields.Summary;
                    story.Assignee = issue.Fields.Assignee == null ? string.Empty : issue.Fields.Assignee.DisplayName;
                    story.URL = BuildCardUrl(issue.Self, issue.Key);
                    story.IssueType = issue.Fields.IssueType.Name;
                    story.IssueTypeIcon = issue.Fields.IssueType.IconUrl;
                    story.Description = issue.Fields.Description;
                    story.Priority = issue.Fields.Priority.Name;
                    
                    if(issue.Fields.SubTasks != null)
                    {
                        foreach (var s in issue.Fields.SubTasks)
                        {
                            Story subTask = new Story();
                            subTask.UUID = Guid.NewGuid();
                            subTask.ID = s.Key;
                            subTask.Priority = s.Fields.Priority.Name;
                            subTask.Summary = s.Fields.Summary;
                            subTask.URL = BuildCardUrl(issue.Self, s.Key);
                            subTask.Parent = story;

                            story.SubTasks.Add(subTask);
                        }
                    }

                    list.Add(story);
                }
            }

            return list;
        }

        private void SetStoryPoint(List<Story> list, string json)
        {
            if (list.Count > 0 && !string.IsNullOrEmpty(json))
            {
                string storyPointField = ConfigurationManager.AppSettings["StoryPointField"];
                var jsObj = JsonConvert.DeserializeObject(json) as JObject;
                JArray issues = jsObj["issues"] as JArray;

                if(issues == null)
                {
                    return;
                }

                foreach (var issue in issues)
                {
                    string key = issue["key"].Value<String>();
                    var sp = issue["fields"][storyPointField];

                    if(sp == null)
                    {
                        continue;
                    }

                    string point = sp.Value<string>();

                    Story story = list.FirstOrDefault(f => f.ID.Equals(key));
                    if (story != null)
                    {
                        story.StoryPoint = point;
                    }
                }
            }
        }

        private void SetStoryPointForSubTasks(List<Story> list, string user, string password, string url)
        {
            List<Story> subTaskList = new List<Story>();
            list.ForEach(l =>
            {
                if (l.SubTasks != null)
                {
                    subTaskList.AddRange(l.SubTasks);
                }
            });

            // do not query subtasks if there are no sub tasks
            if(subTaskList.Count == 0)
            {
                return;
            }

            Uri uri = new Uri(url);
            url = uri.AbsoluteUri.Replace(uri.Query, string.Empty);
            var keyArray = subTaskList.Select(f => f.ID).ToArray<string>();
            string keys = string.Join(",", keyArray);
            url = string.Format(KEYS_URL, url, keys);

            var response = QueryResponse(user, password, url);
            var storyList = QueryStory(response);
            SetStoryPoint(storyList, response.Content);

            storyList.ForEach(story =>
            {
                Story subStory = subTaskList.FirstOrDefault(f => f.ID.Equals(story.ID));
                if (subStory != null)
                {
                    subStory.Assignee = story.Assignee;
                    subStory.StoryPoint = story.StoryPoint;
                }
            });
        }

        public bool UpdateStoryPoint(string user, string password, Story story, string storyPointField)
        {
            string postString = string.Format("{0}\"fields\":{0}\"{2}\":{3}{1}{1}", "{", "}", storyPointField, story.StoryPoint);
            string url = string.Format(POST_URL, IPUtil.GetHost(story.URL), story.ID);
            //HttpStatusCode statusCode = WebUtil.PutHTTP(user, password, postString, url);
            var response = RestUtil.Put(user, password, postString, url);
            
            if(response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
            
            if(response.StatusCode == HttpStatusCode.BadRequest)
            {
                string content = response.Content;
                throw new InvalidOperationException(content);
            }

            return false;
        }

        private string session = null;

        // not work, weird, put session returns 404
        public void NewSession(string username, string password, string url)
        {
            string host = IPUtil.GetHost(url);
            url = string.Format("{0}/jira/rest/auth/1/session", host);

            string payload = string.Format("{0}\"username\": \"{2}\", \"password\": \"{3}\"{1}", "{", "}", username, password);

            //SessionInfo sessionInfo = RestUtil.GetEntity_Post<SessionInfo>(username, password, url, payload);
            WebUtil.PostHTTP(username, password, payload, url);

            //Console.WriteLine(sessionInfo.Session.Name);
            //Console.WriteLine(sessionInfo.Session.Value);
        }

        public Story QueryStory(Story story, string username, string password)
        {
            Uri uri = new Uri(story.URL);
            string host = uri.AbsoluteUri.Replace(uri.AbsolutePath, string.Empty);
            string url = string.Format(KEYS_URL, host + "/rest/api/2/search", story.ID);
            var response = QueryResponse(username, password, url);
            var storyList = QueryStory(response);

            if(storyList != null && storyList.Count > 0)
            {
                Story newStory = storyList[0];
                SetStoryPoint(storyList, response.Content);
            }

            return null;
        }
    }
}
