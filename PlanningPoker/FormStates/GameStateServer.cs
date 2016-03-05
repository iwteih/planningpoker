using log4net;
using PlanningPoker.Entity;
using PlanningPoker.StoryPointCalc;
using PlanningPoker.Utility;
using PlanningPoker.WCF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace PlanningPoker.FormStates
{
    class GameStateServer : GameStateBase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly GameStateServer Instance = new GameStateServer();

        private ServiceHost host = null;
        private bool isModeratorExit = false;
        private IStoryPointCalc storyPointCalc;

        /// <summary>
        /// An callback to close window.
        /// </summary>
        public event EventHandler CloseFormHandler;

        private GameStateServer()
        {
            gameInfo.Moderator = gameInfo.UserName;
            BuildStoryPointCalculator();
        }

        public void StartServer(string serverIP)
        {
            try
            {
                Uri baseAddress = new Uri(string.Format("net.tcp://{0}/{1}", serverIP, typeof(GamePlay).Name));
                NetTcpBinding netTcpBinding = new NetTcpBinding();
                netTcpBinding.OpenTimeout = new TimeSpan(0, 5, 0);
                netTcpBinding.SendTimeout = new TimeSpan(0, 5, 0);
                netTcpBinding.ReceiveTimeout = new TimeSpan(0, 30, 0);
                netTcpBinding.CloseTimeout = new TimeSpan(0, 0, 5);
                netTcpBinding.MaxBufferSize = 2147483647;
                netTcpBinding.MaxReceivedMessageSize = 2147483647;
                netTcpBinding.Security.Mode = SecurityMode.None;
                host = new ServiceHost(typeof(GamePlay), baseAddress);
                host.AddServiceEndpoint(typeof(IGamePlay), netTcpBinding, "");
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(smb);
                host.Open();
            }
            catch (Exception exp)
            {
                try
                {
                    CloseServer();
                }
                catch { }
                host = null;

                log.Error(string.Format("cannot start servie, {0}", serverIP), exp);
                gameInfo.Message = exp.Message;
            }
        }

        public void CloseServer()
        {
            if (host != null)
            {
                host.Close();
            }
        }

        public bool IsConnected
        {
            get { return gameInfo.CanStartService; }
        }

        public override void callback_ExitEventHandler(object sender, WCF.UserExitEventArgs e)
        {
            // moderator exits
            if (e.ExitUser == gameInfo.Moderator)
            {
                isModeratorExit = true;

                if (CloseFormHandler != null)
                {
                    CloseFormHandler(null, null);
                }
            }
        }

        public override void Join(string serverIP)
        {
            bool joined = base.JoinGame(serverIP);
            gameInfo.CanStartService = joined;
        }

        /// <summary>
        /// Flag to indicate the moderator has exited. 
        /// </summary>
        public override bool IsModeratorExit
        {
            get
            {
                return isModeratorExit;
            }
            set
            {
                isModeratorExit = value;
            }
        }

        public override void Flip()
        {
            // only host/server can flip cards
            if (gamePlay != null)
            {
                gamePlay.Flip();
                string score = CalcScore();
                gamePlay.ShowScore(score);
            }
        }

        private string CalcScore()
        {
            string point = storyPointCalc.Calc(gameInfo.ParticipantsList, gameInfo.CardSquence);
            return point;
        }

        private void BuildStoryPointCalculator()
        {
            string storyPointAlgorithm = ConfigurationManager.AppSettings["StoryPointAlgorithm"];

            if ("RoleGroup" == storyPointAlgorithm)
            {
                storyPointCalc = new RoleGroup();
            }
            else
            {
                storyPointCalc = new AllinOne();
            }
        }

        public override void Reset()
        {
            // only server can reset the game
            if (gamePlay != null)
            {
                gamePlay.Reset();
            }
        }

        public override void SyncStory(Story story)
        {
            if (gamePlay != null)
            {
                gameInfo.SyncStory = story;
                gamePlay.SyncStory(story);
            }
        }

        public override void callback_StorySyncEventHandler(object sender, StorySyncArgs e)
        {
            // unnecessary, because this can only be called once
            gameInfo.SyncStory = e.Story;
            // do not re-assign the story, bc/ server may view another story
        }

        public override void callback_StoryListSyncEventHandler(object sender, StoryListSyncArgs e)
        {

        }

        public override void SyncStoryList(List<Story> storyList)
        {
            gamePlay.SyncStoryList(storyList);
        }

        public override bool UpdateStoryPoint(PMS.IPMSOperator pmsOperator, string username, string password)
        {
            if (gameInfo.SyncStory == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(gameInfo.Score) || gameInfo.Score == Story.UnFlippedScore)
            {
                return false;
            }

            if (pmsOperator == null)
            {
                return false;
            }

            string storyPointField = ConfigurationManager.AppSettings["StoryPointField"];
            if (string.IsNullOrEmpty(storyPointField))
            {
                gameInfo.Message = "Please specify StoryPointField in config file";
                return false;
            }

            string storyPointBackup = gameInfo.SyncStory.StoryPoint;

            string score = gameInfo.Score;

            if(gameInfo.Score.IndexOf('=') != -1)
            {
                score = gameInfo.Score.Substring(gameInfo.Score.LastIndexOf('=') + 1);
            }

            gameInfo.SyncStory.StoryPoint = score;

            if (score.Contains("/"))
            {
                gameInfo.SyncStory.StoryPoint = Utils.FractionToFloat(score).ToString();
            }

            bool success = false;
            try
            {
                success = pmsOperator.UpdateStoryPoint(username, password, gameInfo.SyncStory, storyPointField);

                if (!success)
                {
                    gameInfo.SyncStory.StoryPoint = storyPointBackup;
                    gameInfo.Message = "Save story point failed!!";
                }
            }
            catch (Exception exp)
            {
                gameInfo.SyncStory.StoryPoint = storyPointBackup;
                gameInfo.Message = exp.Message;
            }

            return success;
        }
         
        public override void UpdateStory(PMS.IPMSOperator pmsOperator, Story story, string username, string password)
        {
            Story newStory = pmsOperator.QueryStory(story, username, password);

            if(newStory != null)
            {
                story.StoryPoint = newStory.StoryPoint;
            }
        }

        public override void SyncStoryPoint(Story story)
        {
            gamePlay.SyncStoryPoint(story);
        }


        public override void callback_StoryPointSyncEventHandler(object sender, StorySyncArgs e)
        {
            OnSyncStoryPoint(e.Story);
        }
    }
}
