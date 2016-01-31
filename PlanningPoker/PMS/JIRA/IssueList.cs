using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.PMS.JIRA
{
    public class IssueList
    {
        public int StartAt { get; set; }
        public int MaxResults { get; set; }
        public int Total { get; set; }
        public List<Issue> Issues { get; set; }
    }


}
