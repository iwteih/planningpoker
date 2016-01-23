using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PlanningPoker.WCF
{
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void BroadcastJoinEvent(string user, string role);

        [OperationContract(IsOneWay = true)]
        void BroadcastPlayEvent(string user, string pokerValue);

        [OperationContract(IsOneWay = true)]
        void BroadcastExitEvent(string user);
    }
}
