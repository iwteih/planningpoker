using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.WCF
{
    /// <summary>
    /// Callback methods occur on client side
    /// </summary>
    public class Callback : ICallback
    {
        GameInfo gameInfo = GameInfo.Instance;

        public void Join(string user, string role, Participant[] participants)
        {
            lock (gameInfo)
            {
                List<Participant> toAppend = new List<Participant>();
                foreach (Participant remoteP in participants)
                {
                    bool exist = false;
                    foreach (Participant localP in gameInfo.ParticipantsList)
                    {
                        if (remoteP.ParticipantName == localP.ParticipantName)
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (!exist)
                    {
                        toAppend.Add(remoteP);
                    }
                }

                foreach (Participant toAdd in toAppend)
                {
                    gameInfo.ParticipantsList.Add(
                        new Participant()
                        {
                            ParticipantName = toAdd.ParticipantName,
                            Role = toAdd.Role,
                            PlayingCard = toAdd.PlayingCard,
                            UnflipedPlayingCard = toAdd.UnflipedPlayingCard
                        });
                }

                Participant p = gameInfo.ParticipantsList.FirstOrDefault(f => f.ParticipantName == user);
                if (p == null)
                {
                    p = new Participant()
                        {
                            ParticipantName = user,
                            Role = role,
                        };
                    p.Reset();
                    gameInfo.ParticipantsList.Add(p);
                }
            }
        }

        public void Play(string user, string pokerValue)
        {
            lock (gameInfo)
            {
                Participant p = gameInfo.ParticipantsList.Where(f => f.ParticipantName == user).FirstOrDefault();

                if (p != null)
                {
                    p.Play(pokerValue);
                }
            }
        }

        public void Withdraw(string user)
        {
            lock (gameInfo)
            {
                Participant p = gameInfo.ParticipantsList.Where(f => f.ParticipantName == user).FirstOrDefault();

                if (p != null)
                {
                    p.Reset();
                }
            }
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
            //ChannelManager.Instance.BroadcastExitEvent(user);
        }


        public void Flip()
        {
            lock (gameInfo)
            {
                foreach (Participant p in gameInfo.ParticipantsList)
                {
                    p.Flip();
                }
            }
        }

        public void Reset()
        {
            lock (gameInfo)
            {
                foreach (Participant p in gameInfo.ParticipantsList)
                {
                    p.Reset();
                }
            }
        }

        public void ShowScore(string score)
        {
            gameInfo.Score = score;
        }
    }
}
