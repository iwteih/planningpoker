using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PlanningPoker.Entity
{
    // Set IsReference = true to support Cyclic reference
    [DataContract(IsReference = true)]
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
        public Guid UUID
        {
            get;
            set;
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

        [DataMember]
        private List<Story> subTasks = new List<Story>();
        
        public List<Story> SubTasks
        {
            get { return subTasks; }
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

        [DataMember]
        public string StoryPoint { get; set; }

        public bool HasSubTasks
        {
            get
            {
                return subTasks.Count > 0;
            }
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            Story story = obj as Story;
            if(story == null)
            {
                return false;
            }

            if(this.UUID == story.UUID)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
