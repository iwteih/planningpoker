using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PlanningPoker.Entity
{
    [DataContract]
    public class Story : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        [DataMember]
        public string ID
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


        private bool isSyncStory;
        [DataMember]

        public bool IsSyncStory
        {
            get
            {
                return isSyncStory;
            }
            set
            {
                if (value != isSyncStory)
                {
                    isSyncStory = value;
                    OnPropertyChanged("IsSyncStory");
                }
            }
        }
    }
}
