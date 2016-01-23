using log4net;
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
                    logger.Debug("channel registered");
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
                    logger.Debug("channel unregistered");
                }
            }
        }

        public void BroadcastJoinEvent(string user, string role)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var channel in callbackChannelList.ToArray())
                {
                    try
                    {
                        channel.BroadcastJoinEvent(user, role);
                    }
                    catch
                    {
                        logger.Error(string.Format("error BroadcastJoinEvent,user={0},role={1}", user, role));
                        callbackChannelList.Remove(channel);
                    }
                }
            }
        }

        public void BroadcastPlayEvent(string user, string role)
        {
            if (callbackChannelList.Count > 0)
            {
                foreach (var channel in callbackChannelList.ToArray())
                {
                    try
                    {
                        channel.BroadcastPlayEvent(user, role);
                    }
                    catch
                    {
                        logger.Error(string.Format("error BroadcastPlayEvent,user={0},role={1}", user, role));
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
                        channel.BroadcastExitEvent(user);
                    }
                    catch
                    {
                        logger.Error(string.Format("error BroadcastExitEvent,user={0}", user));
                        callbackChannelList.Remove(channel);
                    }
                }
            }
        }

    }
}
