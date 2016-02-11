using PlanningPoker.Entity;
using PlanningPoker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.FormStates
{
    class GameStateClient : GameStateBase
    {
        public static readonly GameStateClient Instance = new GameStateClient();

        private GameStateClient() { }

        public override void callback_ExitEventHandler(object sender, WCF.UserExitEventArgs e)
        {
            if (e.ExitUser == gameInfo.Moderator)
            {
                Init();
                gameInfo.Message = string.Format("{0} has ended this game!", e.ExitUser);
            }
        }

        public override void Join(string serverIP)
        {
            bool joined = base.JoinGame(serverIP);
            gameInfo.CanConnectServer = joined;
        }

        public override bool IsModeratorExit
        {
            get
            {
                return false;
            }
        }

        public override void callback_StorySyncEventHandler(object sender, WCF.StorySyncArgs e)
        {
            gameInfo.SyncStory = e.Story;
            gameInfo.CurrentStory = e.Story;
            OnStorySyncComplete();
        }
        
        public override void callback_StoryListSyncEventHandler(object sender, WCF.StoryListSyncArgs e)
        {
            if(e.StoryList != null)
            {
                gameInfo.StoryList.Clear();

                foreach(var story in e.StoryList)
                {
                    gameInfo.StoryList.Add(story);
                }
            }

            OnStoryListSyncComplete();
        }        
    }
}
