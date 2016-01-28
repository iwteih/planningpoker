using log4net;
using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.WCF
{
    public class ChannelManager
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<ICallback> callbackChannelList = new List<ICallback>();
        public static readonly object locker = new object();

        internal static readonly ChannelManager Instance = new ChannelManager();
        
        private ChannelManager() { }

        public void Register(ICallback callbackChannel)
        {
            if (!callbackChannelList.Contains(callbackChannel))
            {
                lock (locker)
                {
                    callbackChannelList.Add(callbackChannel);
                }
            }
        }

        public void UnRegister(ICallback callbackChannel)
        {
            if (callbackChannelList.Contains(callbackChannel))
            {
                lock (locker)
                {
                    callbackChannelList.Remove(callbackChannel);
                }
            }
        }

        public void BroadcastJoinEvent(string moderator, string user, string role, string cardSequence, Participant[] participants)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var channel in callbackChannelList.ToArray())
                {
                    try
                    {
                        channel.Join(moderator, user, role, cardSequence, participants);
                    }
                    catch
                    {
                        logger.Error(string.Format("error BroadcastJoinEvent,user={0},role={1}", user, role));
                        callbackChannelList.Remove(channel);
                    }
                }
            }
        }

        public void BroadcastPlayEvent(string user, string pokerValue)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var channel in callbackChannelList.ToArray())
                {
                    try
                    {
                        channel.Play(user, pokerValue);
                    }
                    catch
                    {
                        logger.Error(string.Format("error BroadcastPlayEvent,user={0},role={1}", user, pokerValue));
                        callbackChannelList.Remove(channel);
                    }
                }
            }
        }

        public void BroadcastExitEvent(string user)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var channel in callbackChannelList.ToArray())
                {
                    try
                    {
                        channel.Exit(user);
                    }
                    catch
                    {
                        logger.Error(string.Format("error BroadcastExitEvent,user={0}", user));
                        callbackChannelList.Remove(channel);
                    }
                }
            }
        }


        public void BroadcastWithdrawEvent(string user)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var channel in callbackChannelList.ToArray())
                {
                    try
                    {
                        channel.Withdraw(user);
                    }
                    catch
                    {
                        logger.Error(string.Format("error BroadcastWithdrawEvent,user={0}", user));
                        callbackChannelList.Remove(channel);
                    }
                }
            }
        }

        internal void BroadcastFlipEvent()
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var channel in callbackChannelList.ToArray())
                {
                    try
                    {
                        channel.Flip();
                    }
                    catch
                    {
                        logger.Error("error BroadcastFlipEvent");
                        callbackChannelList.Remove(channel);
                    }
                }
            }
        }

        internal void BroadcastRestEvent()
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var channel in callbackChannelList.ToArray())
                {
                    try
                    {
                        channel.Reset();
                    }
                    catch(Exception exp)
                    {
                        logger.Error("error BroadcastRestEvent", exp);
                        callbackChannelList.Remove(channel);
                    }
                }
            }
        }

        internal void BroadcaseShowScoreEvent(string score)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var channel in callbackChannelList.ToArray())
                {
                    try
                    {
                        channel.ShowScore(score);
                    }
                    catch (Exception exp)
                    {
                        logger.Error("error BroadcastRestEvent", exp);
                        callbackChannelList.Remove(channel);
                    }
                }
            }
        }
    }
}
