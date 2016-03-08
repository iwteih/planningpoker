using PlanningPoker.Entity;
using PlanningPoker.PMS;
using PlanningPoker.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.FormStates
{
    interface IGameState
    {
        bool IsModeratorExit{get;}
        void Init();
        void Join(string serverIP);
        void Play(string card);
        void Withdraw();
        void Reset();
        void Flip();
        void Exit();
        void SyncStory(Story story);
        void SyncStoryList(List<Story> storyList);
        bool UpdateStoryPoint(IPMSOperator pmsOperator, string username, string password);

        event EventHandler StorySyncComplete;
        event EventHandler<StorySyncArgs> StoryPointSyncComplete;
        event EventHandler StoryListSyncComplete;

        void UpdateStory(IPMSOperator pmsOperator, Story story, string username, string password);

        void SyncStoryPoint(Story story);

        bool UpdateParentStoryPoint(IPMSOperator pmsOperator, Story story, string username, string password);
    }
}
