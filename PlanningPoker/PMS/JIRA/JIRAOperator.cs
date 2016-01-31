using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlanningPoker.Entity;
using PlanningPoker.PMS.JIRA;
using PlanningPoker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.PMS
{
    class JIRAOperator : IPMSOperator
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<Story> Query(string user, string password, string url)
        {
            var list = new List<Story>();
            //string json = WebUtil.Query(user, password, url);
            string json = WebUtil.Query2();

            if (string.IsNullOrEmpty(json))
            {
                return list;
            }

            var issueList = JsonConvert.DeserializeObject<JIRA.IssueList>(json);

            if (issueList != null)
            {
                foreach (Issue issue in issueList.Issues)
                {
                    Story story = new Story();
                    story.Title = issue.Key;
                    story.Summary = issue.Fields.Summary;
                    story.Assignee = issue.Fields.Assignee.DisplayName;
                    story.URL = string.Format("{0}/{1}", issue.Self.Substring(0, issue.Self.IndexOf("/rest/api")), issue.Key);
                    story.IssueType = issue.Fields.IssueType.Name;
                    story.IssueTypeIcon = issue.Fields.IssueType.IconUrl;
                    story.Description = issue.Fields.Description;
                    story.Priority = issue.Fields.Priority.Name;

                    list.Add(story);
                }
            }
            return list;
        }


    }
}
