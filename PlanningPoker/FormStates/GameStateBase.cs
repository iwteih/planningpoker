using log4net;
using PlanningPoker.Entity;
using PlanningPoker.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace PlanningPoker.FormStates
{
    abstract class GameStateBase : IGameState
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IGamePlay gamePlay;
        protected GameInfo gameInfo = GameInfo.Instance;

        public abstract bool IsModeratorExit { get; set; }
        public virtual void Flip() { }
        public virtual void Reset() { }
        public virtual void Join(string serverIP) { }
        public virtual void SyncStory(Story story) { }
        public virtual void SyncStoryList(List<Story> storyList) { }
        public virtual bool UpdateStoryPoint(PMS.IPMSOperator pmsOperator, string username, string password) { return false; }
        public virtual void UpdateStory(PMS.IPMSOperator pmsOperator, Story story, string username, string password){ }
        public abstract void callback_ExitEventHandler(object sender, UserExitEventArgs e);
        public abstract void callback_StorySyncEventHandler(object sender, StorySyncArgs e);
        public abstract void callback_StoryListSyncEventHandler(object sender, StoryListSyncArgs e);

        public bool JoinGame(String serverIP)
        {
            WithdrawAllCards();
            this.gamePlay = ConnectServer(serverIP);

            try
            {
                this.gamePlay.Regist();
                this.gamePlay.Join(gameInfo.UserName, gameInfo.Role);
                return true;
            }
            catch (Exception exp)
            {
                this.gamePlay = null;
                log.Error("join failed", exp);
                gameInfo.Message = exp.Message;
            }

            return false;
        }

        private IGamePlay ConnectServer(string serverIP)
        {
            try
            {
                string baseAddress = string.Format("net.tcp://{0}/{1}", serverIP, typeof(GamePlay).Name);
                Callback callback = new Callback();
                NetTcpBinding netTcpBinding = new NetTcpBinding();
                netTcpBinding.MaxBufferSize = 2147483647;
                netTcpBinding.MaxReceivedMessageSize = 2147483647;
                netTcpBinding.Security.Mode = SecurityMode.None;
                DuplexChannelFactory<IGamePlay> channel = new DuplexChannelFactory<IGamePlay>(
                    new InstanceContext(callback),
                    netTcpBinding,
                    new EndpointAddress(baseAddress));
                gamePlay = channel.CreateChannel();

                callback.ExitEventHandler += callback_ExitEventHandler;
                callback.PlayEventHandler += callback_PlayEventHandler;
                callback.ResetEventHandler += callback_ResetEventHandler;
                callback.StorySyncEventHandler += callback_StorySyncEventHandler;
                callback.StoryListSyncEventHandler += callback_StoryListSyncEventHandler;

                return gamePlay;
            }
            catch (Exception exp)
            {
                log.Error(string.Format("cannot connect to server {0}", serverIP), exp);
                gameInfo.Message = exp.Message;
                return null;
            }
        }

        public void callback_ResetEventHandler(object sender, EventArgs e)
        {
            WithdrawAllCards();
            gameInfo.Score = "-";
        }

        public void callback_PlayEventHandler(object sender, EventArgs e)
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

        public void Init()
        {
            gamePlay = null;
            WithdrawAllCards();
            gameInfo.CanConnectServer = false;
            gameInfo.Score = "-";
            gameInfo.ParticipantsList.Clear();
        }

        protected void WithdrawAllCards()
        {
            gameInfo.SelectedCard = null;
        }

        public void Exit()
        {
            if (gamePlay != null)
            {
                gamePlay.Exit(gameInfo.UserName);
            }
        }

        public void Withdraw()
        {
            if (gamePlay != null)
            {
                //try
                //{
                gamePlay.Withdraw(gameInfo.UserName);
                //}
                //catch (CommunicationObjectFaultedException exp)
                //{
                //    logger.Error("error withdraw card", exp);
                //}
            }
        }


        public void Play(string card)
        {
            if (gamePlay != null)
            {
                //try
                //{
                gamePlay.Play(gameInfo.UserName, gameInfo.SelectedCard);
                //}
                //catch (CommunicationObjectFaultedException exp)
                //{
                //    logger.Error("error play card", exp);
                //}
            }
        }


        public event EventHandler StorySyncComplete;

        protected void OnStorySyncComplete()
        {
            if (StorySyncComplete != null)
            {
                StorySyncComplete(null, null);
            }
        }

        public event EventHandler StoryListSyncComplete;

        protected void OnStoryListSyncComplete()
        {
            if (StoryListSyncComplete != null)
            {
                StoryListSyncComplete(null, null);
            }
        }
    }
}
