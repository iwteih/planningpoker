using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PlanningPoker.WCF
{
    //[ServiceContract(CallbackContract = typeof(ICallback))]
    //[ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICallback))]
    [ServiceContract(CallbackContract = typeof(ICallback))]
    interface IGamePlay
    {
        [OperationContract(IsOneWay = true)]
        void Regist();

        [OperationContract(IsOneWay = true)]
        void UnRegist();

        [OperationContract(IsOneWay = true)]
        void Join(string user, string role);

        [OperationContract(IsOneWay = true)]
        void Play(string user, string pokerValue);

        [OperationContract(IsOneWay = true)]
        void Withdraw(string user);

        [OperationContract(IsOneWay = true)]
        void Exit(string user);

        [OperationContract(IsOneWay = true)]
        void Flip();

        [OperationContract(IsOneWay = true)]
        void Reset();

        [OperationContract(IsOneWay = true)]
        void ShowScore(string score);

        [OperationContract(IsOneWay = true)]
        void SyncStory(Story story);

        [OperationContract(IsOneWay = true)]
        void SyncStoryList(List<Story> storyList);
    }
}
