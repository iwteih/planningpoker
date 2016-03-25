﻿using Aga.Controls.Tree;
using log4net;
using PlanningPoker.Control;
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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        IGameState gameState = InitGameState.Instance;

        Dictionary<string, Func<Story, string>> SortFuncDict = new Dictionary<string, Func<Story, string>>();

        public MainWindow()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();

            this.DataContext = gameInfo;
        }

        private IPMSOperator pmsOperator = null;
        private IPMSOperator PMSOpertor
        {
            get
            {
                if (pmsOperator == null)
                {
                    if (gameInfo.PMS == "JIRA")
                    {
                        pmsOperator = new JIRAOperator();
                    }
                }

                if (pmsOperator == null)
                {
                    gameInfo.Message = "Please specify PMS type in config file";
                }

                return pmsOperator;
            }
        }


        private void AddGrouping()
        {
            string groupByRole = ConfigurationManager.AppSettings["GroupByRole"];

            if (string.IsNullOrEmpty(groupByRole) || groupByRole.ToUpper() != "TRUE")
            {
                participants.GroupStyle.Clear();
                return;
            }

            CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(participants.ItemsSource);
            if (collectionView.CanGroup == true)
            {
                PropertyGroupDescription groupDescription
                    = new PropertyGroupDescription("Role");
                collectionView.GroupDescriptions.Add(groupDescription);
            }
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            string queryString = txtQuery.Text;
            if (!String.IsNullOrEmpty(queryString))
            {
                if (PMSOpertor == null)
                {
                    return;
                }

                string queryText = txtQuery.Text;
                string queryUser = txtQueryUser.Text;
                string queryPwd = txtQueryPwd.Password;

                Action action = delegate()
                {
                    List<Story> list = null;
                    try
                    {
                        list = PMSOpertor.Query(queryUser, queryPwd, queryText);
                    }
                    catch (Exception exp)
                    {
                        this.Dispatcher.BeginInvoke(
                            DispatcherPriority.Send,
                            new Action(() =>
                            {
                                processBar.Visibility = System.Windows.Visibility.Collapsed;
                                gameInfo.Message = exp.Message;
                            }));
                    }
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action<List<Story>>(UpdateStoryList), list);
                };
                action.BeginInvoke(null, null);
                processBar.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void UpdateStoryList(List<Story> list)
        {
            if (list == null)
            {
                return;
            }

            lbStoryList.Items.SortDescriptions.Clear();
            ListViewBehavior.cleanLagecySortInfo(lbStoryList);
            gameInfo.StoryList.Clear();
            foreach (Story story in list)
            {
                gameInfo.StoryList.Add(story);
            }
            BuildTreeModel(list);
            processBar.Visibility = System.Windows.Visibility.Hidden;
            expQuery.IsExpanded = false;
        }


        private void BuildTreeModel(ICollection<Story> list)
        {
            lbStoryList.Model = new StoryListModel(list);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            lbStoryList.AddHandler(ListViewBehavior.ListViewHeaderSortEvent, new RoutedEventHandler(this.ListViewHeaderClickHandler));
            AddGrouping();
        }

        private void LoadConfig()
        {
            LoadAppConfig();
            ShowWindowTitle();
            gameInfo.LoadRoleList();
            gameInfo.LoadCardSequence();
        }

        private void LoadAppConfig()
        {
            gameInfo.UserName = "Fething user name ...";
            btnConnect.IsEnabled = false;
            btnStart.IsEnabled = false;

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

        private void ShowWindowTitle()
        {
            var versionInfo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string version = String.Format("{0}.{1}.{2}", versionInfo.Major, versionInfo.Minor, versionInfo.Build);
            this.Title = string.Format(this.Title, version);
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

            if (string.IsNullOrEmpty(gameInfo.QueryString))
            {
                gameInfo.QueryString = appconfig.QueryString;
            }
            gameInfo.UserName = userName;
            gameInfo.AutoFlip = autoFlip;
            gameInfo.Role = role;
            tbctrlServerOrClient.SelectedIndex = appconfig.TabIndex_ServerOrClient;

            if (string.IsNullOrEmpty(txtQueryUser.Text))
            {
                txtQueryUser.Text = appconfig.QueryUser;
            }

            if (appconfig.SavePassword)
            {
                ckSavePassword.IsChecked = true;
                txtQueryPwd.Password = StringCipher.Decrypt(appconfig.Password, Utils.getLoggonUser() + ManagementObjectUtil.GetBiosSerialNumber());
            }

            btnConnect.IsEnabled = true;
            btnStart.IsEnabled = true;
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
            string serverIP = IPUtil.GetLocalIP();

            if (!string.IsNullOrEmpty(gameInfo.Port))
            {
                serverIP = string.Format("{0}:{1}", serverIP, gameInfo.Port);
            }

            if (gameInfo.CanStartService)
            {
                return;
            }

            gameInfo.LocalIP = serverIP;

            gameState = GameStateServer.Instance;
            gameState.StoryPointSyncComplete += gameState_StoryPointSyncComplete;
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
            if (string.IsNullOrEmpty(gameInfo.ServerIP))
            {
                return;
            }

            // if already connected
            if (gameInfo.CanConnectServer)
            {
                return;
            }

            gameState = GameStateClient.Instance;
            gameState.StorySyncComplete += gameState_StorySyncComplete;
            gameState.StoryPointSyncComplete += gameState_StoryPointSyncComplete;
            gameState.StoryListSyncComplete += gameState_StoryListSyncComplete;
            gameState.Join(gameInfo.ServerIP);
        }

        void gameState_StorySyncComplete(object sender, EventArgs e)
        {
            ScrollIntoView();
        }

        void gameState_StoryPointSyncComplete(object sender, StorySyncArgs e)
        {
            if (e.Story.Equals(gameInfo.SyncStory))
            {
                gameInfo.Score = e.Story.StoryPoint;
            }

            for (int i = 0; i < lbStoryList.Items.Count; i++)
            {
                ListViewItem listViewItem = lbStoryList.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                if (listViewItem == null)
                {
                    continue;
                }

                var item = listViewItem as TreeListItem;
                if (item == null)
                {
                    continue;
                }

                var story = item.Node.Tag as Story;

                if (story == null)
                {
                    continue;
                }

                if (e.Story.Equals(story))
                {
                    DoBackgroundAnimation(listViewItem);
                    //DoHeightAnimation(listViewItem);
                    break;
                }
            }
        }

        private void DoHeightAnimation(System.Windows.Controls.Control control)
        {
            Storyboard storyboard = new Storyboard();
            storyboard.Duration = new Duration(TimeSpan.FromSeconds(2));
            double originalHeight = control.ActualHeight;

            DoubleAnimation animation1 = new DoubleAnimation();
            animation1.BeginTime = TimeSpan.FromSeconds(0);
            animation1.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation1.From = originalHeight;
            animation1.To = originalHeight * 1.5;

            DoubleAnimation animation2 = new DoubleAnimation();
            animation2.BeginTime = TimeSpan.FromSeconds(1);
            animation2.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation2.From = originalHeight * 1.5;
            animation2.To = originalHeight;

            storyboard.Children.Add(animation1);
            storyboard.Children.Add(animation2);

            Storyboard.SetTarget(animation1, control);
            Storyboard.SetTarget(animation2, control);
            Storyboard.SetTargetProperty(animation1, new PropertyPath("Height"));
            Storyboard.SetTargetProperty(animation2, new PropertyPath("Height"));

            storyboard.Begin();
        }

        private void DoBackgroundAnimation(System.Windows.Controls.Control control)
        {
            Storyboard storyboard = new Storyboard();
            storyboard.Duration = new Duration(TimeSpan.FromSeconds(2));
            Color originColor = (Color)ColorConverter.ConvertFromString(control.Background.ToString());

            SolidColorBrush brush = new SolidColorBrush();
            ColorAnimation animation1 = new ColorAnimation();
            animation1.BeginTime = TimeSpan.FromSeconds(0);
            animation1.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation1.From = originColor;
            animation1.To = Colors.Transparent;

            ColorAnimation animation2 = new ColorAnimation();
            animation2.BeginTime = TimeSpan.FromSeconds(1);
            animation2.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation2.From = Colors.Transparent;
            animation2.To = originColor;

            storyboard.Children.Add(animation1);
            storyboard.Children.Add(animation2);

            Storyboard.SetTarget(animation1, control);
            Storyboard.SetTarget(animation2, control);
            Storyboard.SetTargetProperty(animation1, new PropertyPath("(Background).(SolidColorBrush.Color)"));
            Storyboard.SetTargetProperty(animation2, new PropertyPath("(Background).(SolidColorBrush.Color)"));

            storyboard.Begin();
        }

        void gameState_StoryListSyncComplete(object sender, EventArgs e)
        {
            BuildTreeModel(gameInfo.StoryList);
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
                // The first time when calliing ScrollIntoView
                // will not scroll the item to view, but it works
                // in the next time, so call ScrollIntoView twice 
                // to make sure the SelectedItem is in the view
                lbStoryList.ScrollIntoView(story);
                lbStoryList.ScrollIntoView(story);
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
            if (state != null && state.IsConnected && !state.IsModeratorExit)
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

            if (e.Cancel)
            {
                // in case the network has problem, server cannot trigger
                // the exit callback, that will prevent form closing, so
                // here start a timer to make sure 3 seconds later, the 
                // form can be closed.
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 3);
                timer.Tick += timer_Tick;
                timer.Start();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            GameStateServer state = gameState as GameStateServer;
            if (state != null)
            {
                log.Warn("form closing due to time out");
                state.IsModeratorExit = true;
            }

            this.Close();
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
            appconfig.QueryUser = txtQueryUser.Text;

            if (ckSavePassword.IsChecked.HasValue && ckSavePassword.IsChecked.Value)
            {
                appconfig.SavePassword = true;
                string password = StringCipher.Encrypt(txtQueryPwd.Password, Utils.getLoggonUser() + ManagementObjectUtil.GetBiosSerialNumber());
                appconfig.Password = password;
            }
            else
            {
                appconfig.SavePassword = false;
                appconfig.Password = string.Empty;
            }

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
            TreeNode treeNode = lbStoryList.SelectedItem as TreeNode;
            if (treeNode == null)
            {
                return;
            }

            Story story = treeNode.Tag as Story;
            if (story != null)
            {
                gameState.SyncStory(story);
            }
        }

        private void lbStoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var treeNode = (sender as ListBox).SelectedItem as TreeNode;

            if (treeNode == null)
            {
                return;
            }

            Story story = treeNode.Tag as Story;
            gameInfo.CurrentStory = story;
        }

        private void btnSyncStoryList_Click(object sender, RoutedEventArgs e)
        {
            gameState.SyncStoryList(gameInfo.StoryList.ToList());
        }

        private void btnQueryHelper_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(gameInfo.PMS))
            {
                string url = ConfigurationManager.AppSettings[gameInfo.PMS + "_HelpUrl"];

                if (!string.IsNullOrEmpty(url))
                {
                    Process.Start(new ProcessStartInfo(url));
                }
            }
        }

        private void btnUpateStoryPoint_Click(object sender, RoutedEventArgs e)
        {
            if (gameInfo.SyncStory == null)
            {
                return;
            }

            if (gameState is GameStateServer)
            {
                if (Validation.GetHasError(txtStoryPoint))
                {
                    return;
                }

                try
                {
                    bool success = gameState.UpdateStoryPoint(pmsOperator, txtQueryUser.Text, txtQueryPwd.Password);
                    gameState.UpdateStory(pmsOperator, gameInfo.SyncStory, txtQueryUser.Text, txtQueryPwd.Password);

                    if (success)
                    {
                        Storyboard storyboard = this.FindResource("storyboard_StoryPointSaved") as Storyboard;
                        if (storyboard != null)
                        {
                            storyboard.Begin();
                        }
                        gameState.SyncStoryPoint(gameInfo.SyncStory);
                    }

                    if (gameInfo.SyncStory.Parent == null)
                    {
                        return;
                    }

                    success = gameState.UpdateParentStoryPoint(pmsOperator, gameInfo.SyncStory, txtQueryUser.Text, txtQueryPwd.Password);

                    if (success)
                    {
                        gameState.SyncStoryPoint(gameInfo.SyncStory.Parent);
                    }
                }
                catch (Exception exp)
                {
                    log.Error(exp);
                    gameInfo.Message = exp.Message;
                }
            }
        }

        #region four method to evaluate story property's value

        // reflector
        private Func<Story, string> BuildFunc(string header)
        {
            Func<Story, string> func = delegate(Story story)
            {
                Type type = typeof(Story);
                var pi = type.GetProperty(header);
                var v = pi.GetValue(story, null);
                string value = v == null ? string.Empty : v.ToString();
                return value;
            };

            return func;
        }

        // Expression Tree
        private Func<Story, string> BuildExpressionFunc(string header)
        {
            var type = typeof(Story);
            var param = System.Linq.Expressions.Expression.Parameter(type, type.Name);
            var body = System.Linq.Expressions.Expression.Property(param, header);
            var keySelector = System.Linq.Expressions.Expression.Lambda(body, param);
            var f = (Func<Story, string>)keySelector.Compile();
            return f;
        }

        // emmit
        private Func<Story, string> BuildDynamicFunc(string header)
        {
            Type type = typeof(Story);
            var pi = type.GetProperty(header);
            var mi = pi.GetGetMethod();
            DynamicMethod dm = new DynamicMethod("method", typeof(string), new Type[] { typeof(Story) });
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, mi);
            il.Emit(OpCodes.Ret);

            Func<Story, string> f = (Func<Story, string>)dm.CreateDelegate(typeof(Func<Story, string>));
            return f;
        }

        // Delegate.CreateDelegate
        private Func<Story, string> BuildDelegateFunc(string header)
        {
            Type type = typeof(Story);
            var pi = type.GetProperty(header);
            var mi = pi.GetGetMethod();
            Func<Story, string> f = (Func<Story, string>)Delegate.CreateDelegate(typeof(Func<Story, string>), mi);
            return f;
        }
        #endregion

        private Func<Story, string> GetSortFunc(string header)
        {
            if (SortFuncDict.ContainsKey(header))
            {
                return SortFuncDict[header];
            }

            var func = BuildExpressionFunc(header);
            SortFuncDict.Add(header, func);

            return func;
        }

        private void ListViewHeaderClickHandler(object sender, RoutedEventArgs e)
        {
            object o = e.OriginalSource;
            ListViewHeaderSortEventArgs args = e as ListViewHeaderSortEventArgs;

            if (args == null)
            {
                return;
            }

            string header = args.Header;
            ListSortDirection direction = args.Direction;

            StoryListModel model = lbStoryList.Model as StoryListModel;

            if (model == null)
            {
                return;
            }

            if (model.StoryList == null && model.StoryList.Count == 0)
            {
                return;
            }

            Func<Story, string> func = GetSortFunc(header);

            List<Story> newList = null;
            if (direction == ListSortDirection.Ascending)
            {
                newList = model.StoryList.OrderBy(func).ToList();
            }
            else
            {
                newList = model.StoryList.OrderByDescending(func).ToList();
            }
            lbStoryList.Model = new StoryListModel(newList);
        }
    }
}
