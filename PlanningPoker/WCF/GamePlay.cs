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
            //bool exist = false;

            //lock (gameInfo)
            //{
            //    Participant p = gameInfo.ParticipantsList.FirstOrDefault(f => f.ParticipantName == user);
            //    if (p == null)
            //    {
            //        gameInfo.ParticipantsList.Add(
            //            new Participant()
            //        {
            //            ParticipantName = user,
            //            Role = role,
            //            PlayingCard = CardStatus.Pending.ToString()
            //        });
            //        exist = false;
            //    }
            //    else
            //    {
            //        exist = true;
            //    }
            //}

            //if (!exist)
            //{
                ChannelManager.Instance.BroadcastJoinEvent(user, role, gameInfo.ParticipantsList.ToArray());
            //}
        }

        public void Play(string user, string pokerValue)
        {
            //lock (gameInfo)
            //{
            //    Participant p = gameInfo.ParticipantsList.Where(f => f.ParticipantName == user).FirstOrDefault();

            //    if (p != null)
            //    {
            //        p.UnflipedPlayingCard = pokerValue;
            //        p.PlayingCard = CardStatus.Ready.ToString();
            //        //p.PlayingCard = pokerValue;
            //    }
            //}
            ChannelManager.Instance.BroadcastPlayEvent(user, pokerValue);
        }


        public void Withdraw(string user)
        {
            ChannelManager.Instance.BroadcastWithdrawEvent(user);
        }

        public void Exit(string user)
        {
            lock (gameInfo)
            {
                Participant p = gameInfo.ParticipantsList.Where(f => f.ParticipantName == user).FirstOrDefault();

                if (p != null)
                {
                    gameInfo.ParticipantsList.Remove(p);
                }
            }
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
            ChannelManager.Instance.BroadcaseShowScoreEvent(score);
        }
    }
}
