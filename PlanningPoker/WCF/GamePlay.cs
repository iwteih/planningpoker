using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PlanningPoker.WCF
{
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    //[ServiceContract(CallbackContract=typeof(IStockQuoteCallback), SessionMode= SessionMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    class GamePlay : IGamePlay
    {
        GameInfo gameInfo = GameInfo.Instance;

        public void Regist()
        {
            ICallback callbackChannel = OperationContext.Current.GetCallbackChannel<ICallback>();
            ChannelManager.Instance.Register(callbackChannel);

            OperationContext ctx = OperationContext.Current;
            ICallback callback = ctx.GetCallbackChannel<ICallback>();
            //callback..SendQuoteResponse(symbol, value);
        }

        public void UnRegist()
        {
            ICallback callbackChannel = OperationContext.Current.GetCallbackChannel<ICallback>();
            ChannelManager.Instance.UnRegister(callbackChannel);
        }

        public void Join(string user, string role)
        {
            //ChannelManager.Instance.BroadcastJoinEvent(gameInfo.Moderator, user, role,
            //    gameInfo.SyncStory,
            //    gameInfo.CardSequenceString,
            //    gameInfo.ParticipantsList.ToArray());
            OperationContext ctx = OperationContext.Current;
            ICallback callback = ctx.GetCallbackChannel<ICallback>();
            callback.Join(gameInfo.Moderator, user, role,
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
            //ChannelManager.Instance.BroadcastPlayEvent(user, pokerValue);

            OperationContext ctx = OperationContext.Current;
            ICallback callback = ctx.GetCallbackChannel<ICallback>();
            callback.Play(user, pokerValue);
        }

        public void Withdraw(string user)
        {
            OperationContext ctx = OperationContext.Current;
            ICallback callback = ctx.GetCallbackChannel<ICallback>();
            //ChannelManager.Instance.BroadcastWithdrawEvent(user);
            callback.Withdraw(user);
        }

        public void Exit(string user)
        {
            //ChannelManager.Instance.BroadcastExitEvent(user);
            OperationContext ctx = OperationContext.Current;
            ICallback callback = ctx.GetCallbackChannel<ICallback>();
            callback.Exit(user);
        }

        public void Flip()
        {
            //ChannelManager.Instance.BroadcastFlipEvent();

            OperationContext ctx = OperationContext.Current;
            ICallback callback = ctx.GetCallbackChannel<ICallback>();
            callback.Flip();
        }


        public void Reset()
        {
            //ChannelManager.Instance.BroadcastRestEvent();
            OperationContext ctx = OperationContext.Current;
            ICallback callback = ctx.GetCallbackChannel<ICallback>();
            callback.Reset();
        }

        public void ShowScore(string score)
        {
            //ChannelManager.Instance.BroadcastShowScoreEvent(score);

            OperationContext ctx = OperationContext.Current;
            ICallback callback = ctx.GetCallbackChannel<ICallback>();
            callback.ShowScore(score);
        }

        public void SyncStory(Story story)
        {
            //ChannelManager.Instance.BroadcastSyncStory(story);
            
            OperationContext ctx = OperationContext.Current;
            ICallback callback = ctx.GetCallbackChannel<ICallback>();
            callback.SyncStory(story);
        }

        public void SyncStoryList(List<Story> storyList)
        {
            //ChannelManager.Instance.BroadcastSyncStoryListEvent(storyList);
            OperationContext ctx = OperationContext.Current;
            ICallback callback = ctx.GetCallbackChannel<ICallback>();
            callback.SyncStoryList(storyList);
        }
    }
}
