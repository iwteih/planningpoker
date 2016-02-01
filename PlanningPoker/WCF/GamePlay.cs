using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PlanningPoker.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    class GamePlay : IGamePlay
    {
        GameInfo gameInfo = GameInfo.Instance;

        public void Regist()
        {
            ICallback callbackChannel = OperationContext.Current.GetCallbackChannel<ICallback>();
            ChannelManager.Instance.Register(callbackChannel);
        }

        public void UnRegist()
        {
            ICallback callbackChannel = OperationContext.Current.GetCallbackChannel<ICallback>();
            ChannelManager.Instance.UnRegister(callbackChannel);
        }

        public void Join(string user, string role)
        {

            ChannelManager.Instance.BroadcastJoinEvent(gameInfo.Moderator, user, role,
                gameInfo.SyncStory,
                gameInfo.CardSequenceString,
                gameInfo.ParticipantsList.ToArray());

            if (gameInfo.ShouldSyncStoryList)
            {
                SyncStoryList(gameInfo.StoryList.ToList());
            }
        }

        public void Play(string user, string pokerValue)
        {
            ChannelManager.Instance.BroadcastPlayEvent(user, pokerValue);
        }


        public void Withdraw(string user)
        {
            ChannelManager.Instance.BroadcastWithdrawEvent(user);
        }

        public void Exit(string user)
        {
            ChannelManager.Instance.BroadcastExitEvent(user);
        }

        public void Flip()
        {
            ChannelManager.Instance.BroadcastFlipEvent();
        }

        public void Reset()
        {
            ChannelManager.Instance.BroadcastRestEvent();
        }

        public void ShowScore(string score)
        {
            ChannelManager.Instance.BroadcastShowScoreEvent(score);
        }

        public void SyncStory(Story story)
        {
            ChannelManager.Instance.BroadcastSyncStory(story);
        }

        public void SyncStoryList(List<Story> storyList)
        {
            ChannelManager.Instance.BroadcastSyncStoryListEvent(storyList);
        }
    }
}
