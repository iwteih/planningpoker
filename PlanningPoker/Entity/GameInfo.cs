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
        private static readonly DependencyProperty ScoreProperty;
        private static readonly DependencyProperty AutoFlipProperty;
        private static readonly DependencyProperty RoleProperty;
        private static readonly DependencyProperty QueryStringProperty;
        private static readonly DependencyProperty CanStartServiceProperty;
        private static readonly DependencyProperty CanConnectServerProperty;
        private static readonly DependencyProperty MessageProperty;
        private static readonly DependencyProperty LocalIPProperty;
        private static readonly DependencyProperty ServerIPProperty;
        private static readonly DependencyProperty SelectedCardProperty;
        private static readonly DependencyProperty CurrentStoryProperty;

        private ObservableCollection<Story> storyList = new ObservableCollection<Story>();

        public static readonly GameInfo Instance = new GameInfo();

        private GameInfo()
        {
        }

        static GameInfo()
        {
            UserNameProperty = DependencyProperty.Register("UserName", typeof(string), typeof(GameInfo));

            ScoreProperty = DependencyProperty.Register("Score",
               typeof(string),
               typeof(GameInfo),
               new PropertyMetadata("-"));

            AutoFlipProperty = DependencyProperty.Register("AutoFlip",
                 typeof(bool),
                 typeof(GameInfo),
                 new PropertyMetadata(false));

            RoleProperty = DependencyProperty.Register("Role", typeof(string), typeof(GameInfo));

            QueryStringProperty = DependencyProperty.Register("QueryString", typeof(string), typeof(GameInfo));

            CanStartServiceProperty = DependencyProperty.Register("CanStartService",
                typeof(bool),
                typeof(GameInfo),
                new PropertyMetadata(false));

            CanConnectServerProperty = DependencyProperty.Register("CanConnectServer",
                typeof(bool),
                typeof(GameInfo),
                new PropertyMetadata(false));

            MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(GameInfo));
            LocalIPProperty = DependencyProperty.Register("LocalIP", typeof(string), typeof(GameInfo));
            ServerIPProperty = DependencyProperty.Register("ServerIP", typeof(string), typeof(GameInfo));
            SelectedCardProperty = DependencyProperty.Register("SelectedCard", typeof(string), typeof(GameInfo));
            CurrentStoryProperty = DependencyProperty.Register("CurrentStory", typeof(Story), typeof(GameInfo));
        }

        public string Port
        {
            get
            {
                string port = ConfigurationManager.AppSettings["Port"];
                if (string.IsNullOrEmpty(port))
                {
                    return "8088";
                }
                return port;
            }
        }

        public void LoadCardSequence()
        {
            string defaultSequence = "0, 1/2, 1, 2, 3, 5, 8, 13, 20, 40, 100, ?, coffee";
            string selectedSequence = ConfigurationManager.AppSettings["DefaultSequence"];

            if (selectedSequence != null)
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(selectedSequence))
                {
                    defaultSequence = ConfigurationManager.AppSettings[selectedSequence];
                }
            }

            LoadCardSequence(defaultSequence);
        }

        public void LoadCardSequence(string sequence)
        {
            CardSequenceString = sequence;
            cardSquence.Clear();

            foreach (String str in sequence.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                cardSquence.Add(str.Trim());
            }
        }

        public void LoadRoleList()
        {
            string[] roles = Enum.GetNames(typeof(Role));
            roleList.Clear();

            foreach (string role in roles)
            {
                roleList.Add(role);
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

        public string Score
        {
            get
            {
                return (string)base.GetValue(ScoreProperty);
            }
            set
            {
                base.SetValue(ScoreProperty, value);
            }
        }

        public bool AutoFlip
        {
            get
            {
                return (bool)base.GetValue(AutoFlipProperty);
            }
            set
            {
                base.SetValue(AutoFlipProperty, value);
            }
        }

        public string Role
        {
            get
            {
                return (string)base.GetValue(RoleProperty);
            }
            set
            {
                base.SetValue(RoleProperty, value);
            }
        }

        public string QueryString
        {
            get
            {
                return (string)base.GetValue(QueryStringProperty);
            }
            set
            {
                base.SetValue(QueryStringProperty, value);
            }
        }

        public bool CanStartService
        {
            get
            {
                return (bool)base.GetValue(CanStartServiceProperty);
            }
            set
            {
                base.SetValue(CanStartServiceProperty, value);
            }
        }

        public bool CanConnectServer
        {
            get
            {
                return (bool)base.GetValue(CanConnectServerProperty);
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

        private ObservableCollection<string> roleList = new ObservableCollection<string>();
        public ObservableCollection<string> RoleList
        {
            get { return roleList; }
        }

        public Participant CurrentParticipant
        {
            get
            {
                if (participantsList.Count == 0)
                {
                    return null;
                }
                return participantsList.FirstOrDefault(f => f.ParticipantName == UserName);
            }
        }

        public string LocalIP
        {
            get
            {
                return (string)base.GetValue(LocalIPProperty);
            }
            set
            {
                base.SetValue(LocalIPProperty, value);
            }
        }

        public string ServerIP
        {
            get
            {
                return (string)base.GetValue(ServerIPProperty);
            }
            set
            {
                base.SetValue(ServerIPProperty, value);
            }
        }

        public string Message
        {
            get
            {
                return (string)base.GetValue(MessageProperty);
            }
            set
            {
                base.SetValue(MessageProperty, value);
            }
        }

        public string SelectedCard
        {
            get
            {
                return (string)base.GetValue(SelectedCardProperty);
            }
            set
            {
                base.SetValue(SelectedCardProperty, value);
            }
        }

        public Story CurrentStory
        {
            get
            {
                return (Story)base.GetValue(CurrentStoryProperty);
            }
            set
            {
                base.SetValue(CurrentStoryProperty, value);
            }
        }

        public string Moderator { get; set; }
        public string CardSequenceString { get; set; }

        public Story SyncStory { get; set; }

        public string PMS
        {
            get
            {
                string pms = ConfigurationManager.AppSettings["PMS"];

                if (!string.IsNullOrEmpty(pms))
                {
                    return pms.ToUpper();
                }
                return null;
            }
        }

        public ObservableCollection<Story>  StoryList
        {
            get
            {
                return storyList;
            }
        }


    }
}
