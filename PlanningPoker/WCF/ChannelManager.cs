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

        public void BroadcastSyncStoryListEvent( List<Story> storyList)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var callback in callbackChannelList.ToArray())
                {
                    try
                    {
                        callback.SyncStoryList(storyList);
                    }
                    catch
                    {
                        logger.Error("error BroadcastSyncStoryListEvent");
                        callbackChannelList.Remove(callback);
                    }
                }
            }
        }

        public void BroadcastJoinEvent(string moderator, string user, string role, Story story, string cardSequence, Participant[] participants)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var callback in callbackChannelList.ToArray())
                {
                    try
                    {
                        callback.Join(moderator, user, role, story, cardSequence, participants);
                    }
                    catch
                    {
                        logger.Error(string.Format("error BroadcastJoinEvent,user={0},role={1}", user, role));
                        callbackChannelList.Remove(callback);
                    }
                }
            }
        }

        public void BroadcastPlayEvent(string user, string pokerValue)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var callback in callbackChannelList.ToArray())
                {
                    try
                    {
                        callback.Play(user, pokerValue);
                    }
                    catch
                    {
                        logger.Error(string.Format("error BroadcastPlayEvent,user={0},role={1}", user, pokerValue));
                        callbackChannelList.Remove(callback);
                    }
                }
            }
        }

        public void BroadcastExitEvent(string user)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var callback in callbackChannelList.ToArray())
                {
                    try
                    {
                        callback.Exit(user);
                    }
                    catch
                    {
                        logger.Error(string.Format("error BroadcastExitEvent,user={0}", user));
                        callbackChannelList.Remove(callback);
                    }
                }
            }
        }


        public void BroadcastWithdrawEvent(string user)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var callback in callbackChannelList.ToArray())
                {
                    try
                    {
                        callback.Withdraw(user);
                    }
                    catch
                    {
                        logger.Error(string.Format("error BroadcastWithdrawEvent,user={0}", user));
                        callbackChannelList.Remove(callback);
                    }
                }
            }
        }

        internal void BroadcastFlipEvent()
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var callback in callbackChannelList.ToArray())
                {
                    try
                    {
                        callback.Flip();
                    }
                    catch
                    {
                        logger.Error("error BroadcastFlipEvent");
                        callbackChannelList.Remove(callback);
                    }
                }
            }
        }

        internal void BroadcastRestEvent()
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var callback in callbackChannelList.ToArray())
                {
                    try
                    {
                        callback.Reset();
                    }
                    catch(Exception exp)
                    {
                        logger.Error("error BroadcastRestEvent", exp);
                        callbackChannelList.Remove(callback);
                    }
                }
            }
        }

        internal void BroadcastShowScoreEvent(string score)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var callback in callbackChannelList.ToArray())
                {
                    try
                    {
                        callback.ShowScore(score);
                    }
                    catch (Exception exp)
                    {
                        logger.Error("error BroadcastRestEvent", exp);
                        callbackChannelList.Remove(callback);
                    }
                }
            }
        }

        internal void BroadcastSyncStory(Story story)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var callback in callbackChannelList.ToArray())
                {
                    try
                    {
                        callback.SyncStory(story);
                    }
                    catch (Exception exp)
                    {
                        logger.Error("error BroadcastSyncStory", exp);
                        callbackChannelList.Remove(callback);
                    }
                }
            }
        }

    }
}
