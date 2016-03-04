using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.FormStates
{
    class InitGameState : IGameState
    {
        public static readonly InitGameState Instance = new InitGameState();

        private InitGameState() { }

        public bool IsModeratorExit
        {
            get { return false; }
        }

        public void Init()
        {
        }

        public bool Join(string serverIP)
        {
            return false;
        }

        public void Play(string card)
        {
        }

        public void Withdraw()
        {
        }

        public void Reset()
        {
        }

        public void Flip()
        {
        }

        public void Exit()
        {
        }

        void IGameState.Join(string serverIP)
        {
        }


        public void SyncStory(Entity.Story story)
        {
        }


        public void SyncStoryList(List<Entity.Story> storyList)
        {
        }

        public bool UpdateStoryPoint(PMS.IPMSOperator pmsOperator, string username, string password)
        {
            return false;
        }

        public event EventHandler StorySyncComplete;
        public event EventHandler StoryListSyncComplete;
        public event EventHandler<WCF.StorySyncArgs> StoryPointSyncComplete;

        public void UpdateStory(PMS.IPMSOperator pmsOperator, Entity.Story story, string username, string password)
        {
        }


        public void SyncStoryPoint(Entity.Story story)
        {
        }

    }
}
