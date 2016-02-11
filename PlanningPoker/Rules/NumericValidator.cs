using log4net;
using PlanningPoker.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PlanningPoker.Rules
{
    class NumericValidator : ValidationRule
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            bool canParse = false;
            double c;
            string point = value.ToString();
            if (point.Contains("/"))
            {
                try
                {
                    c = Utils.FractionToFloat(point);
                    canParse = true;
                }
                catch(Exception exp)
                {
                    log.Error(string.Format("error when convert {0} to double,exp={1}", point, exp.Message));
                    canParse = false;
                }
            }
            else
            {
                canParse = double.TryParse(point, out c);
            }

            if (!canParse)
            {
                return new ValidationResult(false, "Please input numeric value");
            }

            return new ValidationResult(true, null);
        }
    }
}
