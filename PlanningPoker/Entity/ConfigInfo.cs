using PlanningPoker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PlanningPoker.Entity
{
    public class ConfigInfo : DependencyObject
    {
        public static readonly DependencyProperty UserNameProperty;

        static ConfigInfo()
        {
            UserNameProperty = DependencyProperty.Register("UserName", typeof(string), typeof(ConfigInfo));
        }

        public string UserName
        {
            get
            {
                return (string)base.GetValue(UserNameProperty);
            }
            set
            {
                base.SetValue(UserNameProperty, value);
            }
        }

    }
}
