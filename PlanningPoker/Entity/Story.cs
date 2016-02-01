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
        [DataMember]
        public string Title
        {
            get;
            set;
        }

        [DataMember]
        public string Summary
        {
            get;
            set;
        }

        [DataMember]
        public string URL
        {
            get;
            set;
        }

        [DataMember]
        public string Assignee
        {
            get;
            set;
        }

        [DataMember]
        public string IssueType
        {
            get;
            set;
        }

        [DataMember]
        public string IssueTypeIcon
        {
            get;
            set;
        }

        [DataMember]
        public string Description
        {
            get;
            set;
        }

        [DataMember]
        public string Priority
        {
            get;
            set;
        }

    }
}
