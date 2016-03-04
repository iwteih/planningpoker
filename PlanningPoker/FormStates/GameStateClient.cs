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
            set
            {
                throw new NotSupportedException("cannot set IsModeratorExit in client side");
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

        public override void callback_StoryPointSyncEventHandler(object sender, WCF.StorySyncArgs e)
        {
            Story syncStory = e.Story;

            foreach (var story in gameInfo.StoryList)
            {
                if (syncStory.Equals(story))
                {
                    story.StoryPoint = syncStory.StoryPoint;
                    break;
                }
                else
                {
                    bool found = false;
                    foreach (var subTask in story.SubTasks)
                    {
                        if (syncStory.Equals(subTask))
                        {
                            subTask.StoryPoint = syncStory.StoryPoint;
                            found = true;
                        }
                    }

                    if(found)
                    {
                        break;
                    }
                }
            }

            OnSyncStoryPoint(syncStory);
        }
    }
}
