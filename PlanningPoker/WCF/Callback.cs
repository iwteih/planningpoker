using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.WCF
{
    public class Callback : ICallback
    {
        GameInfo gameInfo = GameInfo.Instance;

        public void BroadcastJoinEvent(string user, string role)
        {
            lock (gameInfo)
            {
                Participant p = gameInfo.ParticipantsList.FirstOrDefault(f => f.ParticipantName == user);
                if (p == null)
                {
                    gameInfo.ParticipantsList.Add(
                        new Participant()
                        {
                            ParticipantName = user,
                            Role = role,
                            PlayingCard = CardStatus.Pending.ToString()
                        });
                }
            }
        }

        public void BroadcastPlayEvent(string user, string pokerValue)
        {
            lock (gameInfo)
            {
                Participant p = gameInfo.ParticipantsList.Where(f => f.ParticipantName == user).FirstOrDefault();

                if (p != null)
                {
                    p.PlayingCard = pokerValue;
                }
            }
        }

        public void BroadcastExitEvent(string user)
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
    }
}
