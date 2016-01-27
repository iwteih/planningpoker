using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.WCF
{
    public class UserExitEventArgs : EventArgs
    {
        public string ExitUser { get; set; }
    }
}
