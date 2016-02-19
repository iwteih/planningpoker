using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PlanningPoker.Entity
{
    [DataContract]
    public class SubTask
    {
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
