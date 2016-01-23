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
    public class GameInfo : DependencyObject
    {
        private static readonly DependencyProperty UserNameProperty;
        private static readonly DependencyProperty CanStartServiceProperty;
        private static readonly DependencyProperty CanConnectServerProperty;

        public static readonly GameInfo Instance = new GameInfo();

        private GameInfo() { }

        static GameInfo()
        {
            UserNameProperty = DependencyProperty.Register("UserName", typeof(string), typeof(GameInfo));
            CanStartServiceProperty = DependencyProperty.Register("CanStartService", 
                typeof(Visibility), 
                typeof(GameInfo), 
                new PropertyMetadata(Visibility.Hidden));
            CanConnectServerProperty = DependencyProperty.Register("CanConnectServer", 
                typeof(Visibility), 
                typeof(GameInfo), 
                new PropertyMetadata(Visibility.Hidden));
        }

        public string Port
        {
            get
            {
                string port = ConfigurationManager.AppSettings["Port"];
                if(string.IsNullOrEmpty(port))
                {
                    return "8088";
                }
                return port;
            }
        }

        public void LoadAppConfig()
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
            foreach (String str in defaultSequence.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries))
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

        public Visibility CanStartService
        {
            get
            {
                return (Visibility)base.GetValue(CanStartServiceProperty);
            }
            set
            {
                base.SetValue(CanStartServiceProperty, value);
            }
        }

        public Visibility CanConnectServer
        {
            get
            {
                return (Visibility)base.GetValue(CanConnectServerProperty);
            }
            set
            {
                base.SetValue(CanConnectServerProperty, value);
            }
        }

        private ObservableCollection<string> cardSquence = new ObservableCollection<string>();
        public ObservableCollection<string> CardSquence
        {
            get { return cardSquence; }
        }

        private ObservableCollection<Participant> participantsList = new ObservableCollection<Participant>();
        public ObservableCollection<Participant> ParticipantsList
        {
            get { return participantsList; }
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
