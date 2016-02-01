using PlanningPoker.Entity;
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
        void Join(string moderator, string user, string role, Story story, string cardSequnce, Participant[] participants);

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
