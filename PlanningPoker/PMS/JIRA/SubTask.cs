using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.PMS.JIRA
{
    public class SubTask
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public SubTaskFields Fields { get; set; }
    }
}
