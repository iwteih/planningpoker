using log4net;
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
using System.Windows.Threading;

namespace PlanningPoker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        }

        private void LoadConfig()
        {
            LoadAppConfig();
            gameInfo.LoadCardSequence();
            cbRole.ItemsSource = Enum.GetNames(typeof(Role));
        }

        private void LoadAppConfig()
        {
            gameInfo.UserName = "Fething user name ...";
            Action action = delegate()
            {
                ApplicationConfig appconfig = IOUtil.LoadIsolatedData();
                string userName = appconfig.UserName;

                if (string.IsNullOrEmpty(userName))
                {
                    appconfig.UserName = Utils.GetUserName();
                    IOUtil.SaveIsolatedData(appconfig);
                }

                this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action<ApplicationConfig>(UpdateUI), appconfig);
            };
            action.BeginInvoke(null, null);
        }

        private void UpdateUI(ApplicationConfig appconfig)
        {
            bool autoFlip = appconfig.AutoFlip;
            string userName = appconfig.UserName;
            string role = appconfig.Role;
            if (String.IsNullOrEmpty(appconfig.Role))
            {
                role = Role.Dev.ToString();
            }
            string queryString = appconfig.QueryString;

            gameInfo.UserName = userName;
            gameInfo.AutoFlip = autoFlip;
            gameInfo.Role = role;
            gameInfo.QueryString = queryString;
        }

        private void LoadUserName()
        {
            Action action = delegate()
            {
                String userName = Utils.GetUserName();
                this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action<string>(UpdateUserName), userName);
            };
            action.BeginInvoke(null, null);
        }

        private void UpdateUserName(string userName)
        {
            gameInfo.UserName = userName;
        }

        private ServiceHost startServer(string serverIP)
        {
            //TODO: try catch
            Uri baseAddress = new Uri(string.Format("net.tcp://{0}/{1}", serverIP, typeof(GamePlay).Name));
            host = new ServiceHost(typeof(GamePlay), baseAddress);
            host.AddServiceEndpoint(typeof(IGamePlay), new NetTcpBinding(), "");
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(smb);
            host.Open();
            return host;
        }

        private IGamePlay ConnectServer(string serverIP)
        {
            //TODO: try catch
            Callback callback = new Callback();
            string baseAddress = string.Format("net.tcp://{0}/{1}", serverIP, typeof(GamePlay).Name);
            DuplexChannelFactory<IGamePlay> channel = new DuplexChannelFactory<IGamePlay>(
                new InstanceContext(callback),
                new NetTcpBinding(),
                new EndpointAddress(baseAddress));
            gamePlay = channel.CreateChannel();

            callback.PlayEventHandler += callback_PlayEventHandler;
            callback.ResetEventHandler += callback_ResetEventHandler;

            return gamePlay;
        }

        void callback_PlayEventHandler(object sender, EventArgs e)
        {
            if (!gameInfo.AutoFlip)
            {
                return;
            }

            lock (gameInfo.ParticipantsList)
            {
                bool allCardsFlipped = gameInfo.ParticipantsList.All(
                    p => p.PlayingCard == CardStatus.Ready.ToString());

                if (allCardsFlipped)
                {
                    Flip();
                }
            }
        }

        private void Withdraw()
        {
            lbCardSequence.SelectedItem = null;
        }

        void callback_ResetEventHandler(object sender, EventArgs e)
        {
            Withdraw();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            string localIP = IPUtil.GetLocalIP();
            string serverIP = string.Format("{0}:{1}", localIP, gameInfo.Port);
            txtLocalIP.Text = serverIP;


            this.host = startServer(serverIP);

            //this.gamePlay = connectServer(serverIP);
            //this.gamePlay.Regist();
            //this.gamePlay.Join(txtUserName.Text.Trim(), cbRole.Text);
            //Withdraw();
            Join(serverIP);

            gameInfo.CanStartService = System.Windows.Visibility.Visible;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            // if already connected
            if (string.IsNullOrEmpty(txtServerIP.Text) || gamePlay != null)
            {
                return;
            }

            //this.gamePlay = connectServer(txtServerIP.Text.Trim());
            //this.gamePlay.Regist();
            //this.gamePlay.Join(txtUserName.Text.Trim(), cbRole.Text);
            //Withdraw();
            Join(txtServerIP.Text.Trim());

            this.gameInfo.CanConnectServer = System.Windows.Visibility.Visible;
        }

        private void Join(String serverIP)
        {
            Withdraw();
            this.gamePlay = ConnectServer(serverIP);
            this.gamePlay.Regist();
            this.gamePlay.Join(txtUserName.Text.Trim(), cbRole.Text);
        }

        private void Reset()
        {
            // only server can reset the game
            if (gamePlay != null && host != null)
            {
                gamePlay.Reset();
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private string CalcScore()
        {
            double? total = null;
            int count = 0;
            foreach (Participant p in gameInfo.ParticipantsList)
            {
                if (IsCalculatable(p.Role))
                {
                    double v;
                    bool canParse = false;

                    if (p.UnflipedPlayingCard.Contains("/"))
                    {
                        v = Utils.FractionToDouble(p.UnflipedPlayingCard);
                        canParse = true;
                    }
                    else
                    {
                        canParse = double.TryParse(p.UnflipedPlayingCard, out v);
                    }
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
                double average = total.Value * 1.0 / count;
                String ret = "-";

                foreach (string card in gameInfo.CardSquence)
                {
                    double c;
                    bool canParse = false;

                    if (card.Contains("/"))
                    {
                        c = Utils.FractionToDouble(card);
                        canParse = true;
                    }
                    else
                    {
                        canParse = double.TryParse(card, out c);
                    }

                    if (canParse && c >= average)
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
            ApplicationConfig appconfig = IOUtil.LoadIsolatedData();
            appconfig.AutoFlip = ckAutoFlip.IsChecked.Value;
            appconfig.QueryString = txtQuery.Text;
            if (cbRole.Text != null)
            {
                appconfig.Role = cbRole.Text.Trim();
            }
            appconfig.UserName = txtUserName.Text.Trim();
            IOUtil.SaveIsolatedData(appconfig);

            if (gamePlay != null)
            {
                try
                {
                    gamePlay.Exit(txtUserName.Text.Trim());
                }
                catch (CommunicationObjectFaultedException exp)
                {
                    logger.Error("error close window", exp);
                }
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
                if (gamePlay != null)
                {
                    try
                    {
                        gamePlay.Withdraw(txtUserName.Text.Trim());
                    }
                    catch (CommunicationObjectFaultedException exp)
                    {
                        logger.Error("error withdraw card", exp);
                    }
                }
                return;
            }

            string cardValue = lbCardSequence.SelectedItem.ToString();
            if (gamePlay != null)
            {
                try
                {
                    gamePlay.Play(txtUserName.Text.Trim(), cardValue);
                }
                catch (CommunicationObjectFaultedException exp)
                {
                    logger.Error("error play card", exp);
                }
            }
        }

        private void Flip()
        {
            // only host/server can flip cards
            if (gamePlay != null && IsServer)
            {
                gamePlay.Flip();
                string score = CalcScore();
                gamePlay.ShowScore(score);
            }
        }

        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            Flip();
        }

        private bool IsServer
        {
            get
            {
                return host != null;
            }
        }
    }
}
