using log4net;
using PlanningPoker.Entity;
using PlanningPoker.FormStates;
using PlanningPoker.PMS;
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
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        GameInfo gameInfo = GameInfo.Instance;
        IGameState gameState = MockGameState.Instance;

        public MainWindow()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();

            //this.lbStoryList.ItemsSource = storyList;
            this.DataContext = gameInfo;
            //MockData();
        }

        //private void MockData()
        //{
        //    Participant p1 = new Participant() { ParticipantName = "P1", PlayingCard = "HAT", Role = "dev" };
        //    Participant p2 = new Participant() { ParticipantName = "P2", PlayingCard = "2", Role = "QA" };
        //    gameInfo.ParticipantsList.Add(p1);
        //    for (int i = 0; i < 100; i++)
        //    {
        //        gameInfo.ParticipantsList.Add(p2);
        //    }


        //    Story s1 = new Story { Title = "title1", URL = "url1" };
        //    Story s2 = new Story { Title = "title2", URL = "url2" };
        //    Story s3 = new Story { Title = "title3", URL = "url3" };
        //    storyList.Add(s1);
        //    storyList.Add(s2);
        //    storyList.Add(s3);
        //}

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            string queryString = txtQuery.Text;
            //if (!String.IsNullOrEmpty(queryString))
            {
                IPMSOperator op = null;

                if (gameInfo.PMS == "JIRA")
                {
                    op = new JIRAOperator();
                }

                if (op == null)
                {
                    gameInfo.Message = "Please specify a PMS type in config file";
                    return;
                }

                string queryText = txtQuery.Text;
                string queryUser = txtQueryUser.Text;
                string queryPwd = txtQueryPwd.Password;

                Action action = delegate()
                {
                    List<Story> list = op.Query(queryUser, queryPwd, queryText);
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action<List<Story>>(UpdateStoryList), list);
                };
                action.BeginInvoke(null, null);
                processBar.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void UpdateStoryList(List<Story> list)
        {
            gameInfo.StoryList.Clear();
            foreach (Story story in list)
            {
                gameInfo.StoryList.Add(story);
            }
            processBar.Visibility = System.Windows.Visibility.Hidden;
            expQuery.IsExpanded = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            LoadAppConfig();
            gameInfo.LoadRoleList();
            gameInfo.LoadCardSequence();
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
            tbctrlServerOrClient.SelectedIndex = appconfig.TabIndex_ServerOrClient;
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

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            string localIP = IPUtil.GetLocalIP();
            string serverIP = string.Format("{0}:{1}", localIP, gameInfo.Port);

            // IP not changed
            if (txtLocalIP.Text.Trim() == serverIP)
            {
                return;
            }

            gameInfo.LocalIP = serverIP;

            gameState = GameStateServer.Instance;
            GameStateServer.Instance.CloseFormHandler += Instance_CloseFormHandler;
            GameStateServer.Instance.StartServer(gameInfo.LocalIP);
            gameState.Join(serverIP);
        }

        void Instance_CloseFormHandler(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            // if already connected
            if (string.IsNullOrEmpty(txtServerIP.Text) || gameState is GameStateClient)
            {
                return;
            }

            gameState = GameStateClient.Instance;
            gameState.StorySyncComplete += gameState_StorySyncComplete;
            gameState.StoryListSyncComplete += gameState_StoryListSyncComplete;
            gameState.Join(gameInfo.ServerIP);
        }

        void gameState_StorySyncComplete(object sender, EventArgs e)
        {
            ScrollIntoView();
        }

        void gameState_StoryListSyncComplete(object sender, EventArgs e)
        {
            ScrollIntoView();
            expQuery.IsExpanded = false;
        }

        void ScrollIntoView()
        {
            if (gameInfo.StoryList == null)
            {
                return;
            }

            Story story = gameInfo.SyncStory;
            if (story != null)
            {
                lbStoryList.ScrollIntoView(story);
                lbStoryList.SelectedItem = story;
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            gameState.Reset();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // if moderator wants to exit, notify all clients
            GameStateServer state = gameState as GameStateServer;
            if (state != null && state.IsConnected && !gameState.IsModeratorExit)
            {
                e.Cancel = true;
            }
            // if paticipants want to exit, go ahead
            else
            {
                e.Cancel = false;
            }

            // notify exit
            gameState.Exit();
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
            appconfig.TabIndex_ServerOrClient = tbctrlServerOrClient.SelectedIndex;
            IOUtil.SaveIsolatedData(appconfig);

            GameStateServer state = gameState as GameStateServer;
            if (state != null)
            {
                state.CloseServer();
            }
        }

        private void lbCardSequence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbCardSequence.SelectedItem == null)
            {
                gameState.Withdraw();
            }
            else
            {
                gameState.Play(gameInfo.SelectedCard);
            }
        }

        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            gameState.Flip();
        }

        private void btnMessage_Click(object sender, RoutedEventArgs e)
        {
            gameInfo.Message = string.Empty;
        }

        private void ListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Story story = lbStoryList.SelectedItem as Story;

            if (story != null)
            {
                gameState.SyncStory(story);
            }
        }

        private void lbStoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Story story = ((sender as ListBox).SelectedItem as Story);
            gameInfo.CurrentStory = story;
        }

        private void btnSyncStoryList_Click(object sender, RoutedEventArgs e)
        {
            gameState.SyncStoryList(gameInfo.StoryList.ToList());
        }
    }
}
