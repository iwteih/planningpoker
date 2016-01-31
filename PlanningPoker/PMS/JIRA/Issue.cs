using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.PMS.JIRA
{
    public class Issue
    {
        public string Key { get; set; }
        public string Self { get; set; }
        public Fields Fields { get; set; }
    }
}
