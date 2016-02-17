using log4net;
using PlanningPoker.Entity;
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

        /// <summary>
        /// An callback to close window.
        /// </summary>
        public event EventHandler CloseFormHandler;

        private GameStateServer()
        {
            gameInfo.Moderator = gameInfo.UserName;
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
            double? total = null;
            int count = 0;
            foreach (Participant p in gameInfo.ParticipantsList)
            {
                if (!IsCalculatableRole(p.Role))
                {
                    continue;
                }

                double v;
                bool canParse = false;

                if (p.UnflipedPlayingCard.Contains("/"))
                {
                    v = Utils.FractionToFloat(p.UnflipedPlayingCard);
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
                        c = Utils.FractionToFloat(card);
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

        private bool IsCalculatableRole(string role)
        {
            bool flag = Enum.GetNames(typeof(Role)).Contains(role);

            if (!flag)
            {
                return false;
            }

            Role r = (Role)Enum.Parse(typeof(Role), role, true);
            if (r == Role.Dev || r == Role.QA)
            {
                return true;
            }

            return false;
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

            if (string.IsNullOrEmpty(gameInfo.Score) || gameInfo.Score == "-")
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

            gameInfo.SyncStory.StoryPoint = gameInfo.Score;

            if (gameInfo.Score.Contains("/"))
            {
                gameInfo.SyncStory.StoryPoint = Utils.FractionToFloat(gameInfo.Score).ToString();
            }

            bool success = false;
            try
            {
                success = pmsOperator.UpdateStoryPoint(username, password, gameInfo.SyncStory, storyPointField);

                if (!success)
                {
                    gameInfo.Message = "Save story point failed!!";
                }
            }
            catch (Exception exp)
            {
                gameInfo.Message = exp.Message;
            }

            return success;
        }
    }
}
