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

            //MockData();
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
            LoadUserName();
        }

        private void LoadConfig()
        {
            gameInfo.LoadAppConfig();
            cbRole.ItemsSource = Enum.GetNames(typeof(Role));
        }

        private void LoadUserName()
        {
            gameInfo.LoadUserName();
        }

        private ServiceHost startServer(string serverIP)
        {
            Uri baseAddress = new Uri(string.Format("net.tcp://{0}/{1}", serverIP, typeof(GamePlay).Name));
            host = new ServiceHost(typeof(GamePlay), baseAddress);
            host.AddServiceEndpoint(typeof(IGamePlay), new NetTcpBinding(), "");
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(smb);
            host.Open();
            return host;
        }

        private IGamePlay connectServer(string serverIP)
        {
            Callback callback = new Callback();
            string baseAddress = string.Format("net.tcp://{0}/{1}", serverIP, typeof(GamePlay).Name);
            DuplexChannelFactory<IGamePlay> channel = new DuplexChannelFactory<IGamePlay>(
                new InstanceContext(callback),
                new NetTcpBinding(),
                new EndpointAddress(baseAddress));
            gamePlay = channel.CreateChannel();


            return gamePlay;
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

                this.host = startServer(serverIP);

                this.gamePlay = connectServer(serverIP);
                this.gamePlay.Regist();
                this.gamePlay.Join(txtUserName.Text.Trim(), cbRole.Text);

                btnStart.Content = "Stop";
                gameInfo.CanStartService = System.Windows.Visibility.Visible;
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtServerIP.Text) || gamePlay != null)
            {
                return;
            }

            this.gamePlay = connectServer(txtServerIP.Text.Trim());

            this.gameInfo.CanConnectServer = System.Windows.Visibility.Visible;
            this.gamePlay.Regist();
            this.gamePlay.Join(txtUserName.Text.Trim(), cbRole.Text);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            // only server can reset the game
            if (gamePlay != null && host != null)
            {
                gamePlay.Reset();
            }
        }

        private string CalcScore()
        {
            int? total = null;
            int count = 0;
            foreach (Participant p in gameInfo.ParticipantsList)
            {
                if (IsCalculatable(p.Role))
                {
                    int v;
                    bool canParse = int.TryParse(p.UnflipedPlayingCard, out v);

                    if (canParse)
                    {
                        total = total == null ? v : total + v;
                        count++;
                    }
                }
            }

            if (gameInfo.CardSquence.Count == 0 || !total.HasValue)
            {
                return "-";
            }

            if (count > 0)
            {
                int upper = (int)Math.Ceiling(total.Value * 1.0 / count);
                String ret = "-";

                foreach (string card in gameInfo.CardSquence)
                {
                    int c;
                    bool canParse = int.TryParse(card, out c);

                    if (canParse && c >= upper)
                    {
                        ret = card;
                        break;
                    }
                }

                return ret;
            }

            return "-";
        }

        private bool IsCalculatable(string role)
        {
            return Enum.GetNames(typeof(Role)).Contains(role);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (gamePlay != null)
            {
                gamePlay.Exit(txtUserName.Text.Trim());
            }
            if (host != null)
            {
                host.Close();
            }
        }

        private void lbCardSequence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbCardSequence.SelectedItem == null)
            {
                gamePlay.Withdraw(txtUserName.Text.Trim());
                return;
            }

            string cardValue = lbCardSequence.SelectedItem.ToString();
            if (gamePlay != null)
            {
                gamePlay.Play(txtUserName.Text.Trim(), cardValue);
            }
        }

        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            // only host/server can flip cards
            if (gamePlay != null && host != null)
            {
                gamePlay.Flip();
                string score = CalcScore();
                gamePlay.ShowScore(score);
            }
        }
    }
}
