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

        public static string getLoggonUser()
        {
            return Environment.UserName;
        }

        public static double FractionToFloat(string fraction)
        {
            float result;

            if (float.TryParse(fraction, out result))
            {
                return result;
            }

            string[] split = fraction.Split(new char[] { ' ', '/' });

            if (split.Length == 2 || split.Length == 3)
            {
                int a, b;

                if (int.TryParse(split[0], out a) && int.TryParse(split[1], out b))
                {
                    if (split.Length == 2)
                    {
                        return (float)a / b;
                    }

                    int c;

                    if (int.TryParse(split[2], out c))
                    {
                        return a + (float)b / c;
                    }
                }
            }

            throw new FormatException("Not a valid fraction.");
        }
        
    }
}
