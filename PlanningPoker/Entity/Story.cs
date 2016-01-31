using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PlanningPoker.Entity
{
    [DataContract]
    public class Story
    {
        public string Title
        {
            get;
            set;
        }

        public string Summary
        {
            get;
            set;
        }
        public string URL
        {
            get;
            set;
        }

        public string Assignee
        {
            get;
            set;
        }

        public string IssueType
        {
            get;
            set;
        }

        public string IssueTypeIcon
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Priority
        {
            get;
            set;
        }

    }
}
