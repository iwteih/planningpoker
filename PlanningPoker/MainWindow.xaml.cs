using PlanningPoker.Entity;
using PlanningPoker.Utility;
using PlanningPoker.WCF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
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
        ServiceHost host = null;
        private ObservableCollection<Story> storyList = new ObservableCollection<Story>();

        GameInfo gameInfo = GameInfo.Instance;
        IGamePlay gamePlay = null;

        public MainWindow()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();

            this.lbStoryList.ItemsSource = storyList;
            this.participants.ItemsSource = gameInfo.ParticipantsList;
            this.DataContext = gameInfo;
            gameInfo.UserName = "Yiming";

            MockData();
        }

        public GameInfo Config
        {
            get { return gameInfo; }
        }

        private void MockData()
        {
            Participant p1 = new Participant() { ParticipantName = "P1", PlayingCard = "HAT", Role = "dev" };
            Participant p2 = new Participant() { ParticipantName = "P2", PlayingCard = "2", Role = "QA" };
            gameInfo.ParticipantsList.Add(p1);
            gameInfo.ParticipantsList.Add(p2);
            gameInfo.ParticipantsList.Add(p2);


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
            gameInfo.LoadAppConfig();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (host != null)
            {
                if (host.State != CommunicationState.Closed
                    && host.State != CommunicationState.Closing)
                {
                    host.Close();
                    host = null;
                    btnStart.Content = "Start";
                    txtLocalIP.Text = string.Empty;
                    gameInfo.CanStartService = System.Windows.Visibility.Hidden;
                }
            }
            else
            {
                string localIP = IPUtil.GetLocalIP();
                string serverIP = string.Format("{0}:{1}", localIP, gameInfo.Port);
                txtLocalIP.Text = serverIP;

                Uri baseAddress = new Uri(string.Format("net.tcp://{0}/{1}", serverIP, typeof(GamePlay).Name));
                host = new ServiceHost(typeof(GamePlay), baseAddress);
                host.AddServiceEndpoint(typeof(IGamePlay), new NetTcpBinding(), "");
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(smb);
                host.Open();


                //host = new ServiceHost(typeof(GamePlay));
                //host.Open();

                btnStart.Content = "Stop";
                gameInfo.CanStartService = System.Windows.Visibility.Visible;
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            lock (gameInfo)
            {
                foreach (Participant p in gameInfo.ParticipantsList)
                {
                    p.PlayingCard = CardStatus.Pending.ToString();
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(gamePlay != null)
            {
                gamePlay.Exit(txtUserName.Content.ToString());
            }
            if (host != null)
            {
                host.Close();
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if(gamePlay != null)
            {
                return;
            }

            string baseAddress = string.Format("net.tcp://{0}/{1}", txtServerIP.Text.Trim(), typeof(GamePlay).Name);
            DuplexChannelFactory<IGamePlay> channel = new DuplexChannelFactory<IGamePlay>(
                new InstanceContext(new Callback()), 
                new NetTcpBinding(), 
                new EndpointAddress(baseAddress));
            gamePlay = channel.CreateChannel();

            gameInfo.CanConnectServer = System.Windows.Visibility.Visible;

            gamePlay.Regist();
            gamePlay.Join(txtUserName.Content.ToString(), cbRole.Text);
        }
    }
}
