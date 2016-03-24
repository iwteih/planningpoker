using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker
{
    [Serializable]
    public class ApplicationConfig
    {
        private bool autoFlip = false;
        public bool AutoFlip
        {
            get
            {
                return autoFlip;
            }
            set
            {
                autoFlip = value;
            }
        }

        public string UserName
        {
            get;
            set;
        }

        public string Role
        {
            get;
            set;
        }

        public string QueryString
        {
            get;
            set;
        }

        public int TabIndex_ServerOrClient
        {
            get;
            set;
        }

        public string QueryUser
        {
            get;
            set;
        }
    }

}
