using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.Utility
{
    public class Utils
    {
        public static String GetUserName()
        {
            string name = System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName;

            if (String.IsNullOrEmpty(name))
            {
                name = Environment.UserName;
            }

            return name;
        }
    }
}
