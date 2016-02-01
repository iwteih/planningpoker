using PlanningPoker.Entity;
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

    public class StorySyncArgs :EventArgs
    {
        public Story Story { get; set; }
    }

    public class StoryListSyncArgs :EventArgs
    {
        public List<Story> StoryList { get; set; }
    }
}