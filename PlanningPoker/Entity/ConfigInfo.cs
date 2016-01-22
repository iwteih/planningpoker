using PlanningPoker.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace PlanningPoker.Entity
{
    public class ConfigInfo : DependencyObject
    {
        public static readonly DependencyProperty UserNameProperty;
        
        static ConfigInfo()
        {
            UserNameProperty = DependencyProperty.Register("UserName", typeof(string), typeof(ConfigInfo));
        }

        public void LoadCardSequence()
        {
            string defaultSequence = "0, 1/2, 1, 2, 3, 5, 8, 13, 20, 40, 100, ?, Coffee";
            string selectedSequence = ConfigurationManager.AppSettings["DefaultSequence"];

            if (selectedSequence != null)
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(selectedSequence))
                {
                    defaultSequence = ConfigurationManager.AppSettings[selectedSequence];
                }
            }

            cardSquence.Clear();
            foreach(String str in defaultSequence.Split(new String[]{","}, StringSplitOptions.RemoveEmptyEntries))
            {
                cardSquence.Add(str.Trim());
            }
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

        private ObservableCollection<string> cardSquence = new ObservableCollection<string>();
        public ObservableCollection<string> CardSquence
        {
            get { return cardSquence; }
        }


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
    }
}
