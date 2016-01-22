using PlanningPoker.Entity;
using PlanningPoker.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlanningPoker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Story> storyList = new ObservableCollection<Story>();
        private ObservableCollection<Participant> participantsList = new ObservableCollection<Participant>();

        ConfigInfo configInfo = new ConfigInfo();

        public MainWindow()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();

            this.lbStoryList.ItemsSource = storyList;
            this.participants.ItemsSource = participantsList;
            this.DataContext = configInfo;
            configInfo.UserName = "Yiming";

            MockData();
        }

        public ConfigInfo Config
        {
            get { return configInfo; }
        }

        private void MockData()
        {
            Participant p1 = new Participant() { ParticipantName = "P1", PlayingCard = "HAT", Role="dev" };
            Participant p2 = new Participant() { ParticipantName = "P2", PlayingCard = "2" , Role="QA"};
            participantsList.Add(p1);
            participantsList.Add(p2);
            

            Story s1 = new Story { Title = "title1", URL = "url1" };
            Story s2 = new Story { Title = "title2", URL = "url2" };
            Story s3 = new Story { Title = "title3", URL = "url3" };
            storyList.Add(s1);
            storyList.Add(s2);
            storyList.Add(s3);
        }


        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            string queryString = txtQuery.Text;
            if (!String.IsNullOrEmpty(queryString))
            {
                Console.WriteLine(queryString);
                //TODO: query from jira
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            configInfo.LoadCardSequence();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            txtLocalIP.Text = string.Format("{0}:8088", IPUtil.GetLocalIP());
        }


    }
}
