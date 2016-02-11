using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.PMS.JIRA
{
    public class SessionInfo
    {
        public Session Session { get; set; }
    }

    public class Session
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

}
