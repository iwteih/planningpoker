using log4net;
using PlanningPoker.Entity;
using PlanningPoker.PMS.JIRA;
using PlanningPoker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace PlanningPoker.PMS
{
    class JIRAOperator : IPMSOperator
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string POST_URL = "{0}/rest/api/2/issue/{1}";

        public List<Story> Query(string user, string password, string url)
        {
            var list = new List<Story>();
            var issueList = RestUtil.Get<IssueList>(user, password, url);

            if (issueList != null)
            {
                foreach (Issue issue in issueList.Issues)
                {
                    Story story = new Story();
                    story.ID = issue.Key;
                    story.Summary = issue.Fields.Summary;
                    story.Assignee = issue.Fields.Assignee.DisplayName;
                    story.URL = string.Format("{0}/browse/{1}", issue.Self.Substring(0, issue.Self.IndexOf("/rest/api")), issue.Key);
                    story.IssueType = issue.Fields.IssueType.Name;
                    story.IssueTypeIcon = issue.Fields.IssueType.IconUrl;
                    story.Description = issue.Fields.Description;
                    story.Priority = issue.Fields.Priority.Name;

                    list.Add(story);
                }
            }
            return list;
        }

        public bool UpdateStoryPoint(string user, string password, Story story, string storyPointField)
        {
            string postString = string.Format("{0}\"fields\":{0}\"{2}\":{3}{1}{1}", "{", "}", storyPointField, story.StoryPoint);
            string url = string.Format(POST_URL, IPUtil.GetHost(story.URL), story.ID);
            //HttpStatusCode statusCode = WebUtil.PutHTTP(user, password, postString, url);
            HttpStatusCode statusCode = RestUtil.Put(user, password, postString, url);
            
            if(statusCode == HttpStatusCode.NoContent)
            {
                return true;
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
    }
}
