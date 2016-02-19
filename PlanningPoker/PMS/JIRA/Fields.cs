using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.PMS.JIRA
{
    public class Fields
    {
        public IssueType IssueType { get; set; }
        public Assignee Assignee { get; set; }
        public Status Status { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public Priority Priority { get; set; }
        public List<SubTask> SubTasks { get; set; }
    }
}
