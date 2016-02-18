using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.WCF
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    class GameServiceClient : System.ServiceModel.ClientBase<IGamePlay>, IGamePlay
    {
        public GameServiceClient()
        {
        }

        public GameServiceClient(string endpointConfigurationName)
            :
                base(endpointConfigurationName)
        {
        }

        public GameServiceClient(string endpointConfigurationName, string remoteAddress)
            :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public GameServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress)
            :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public GameServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress)
            :
                base(binding, remoteAddress)
        {
            
        }



        void Regist()
        {
            base.Channel.Regist();
        }

        void UnRegist()
        {
            base.Channel.UnRegist();
        }

        void Join(string user, string role)
        {
            base.Channel.Join(user, role);
        }

        void Play(string user, string pokerValue)
        {
            base.Channel.Play(user,pokerValue);
        }

        void Withdraw(string user)
        {
            base.Channel.Withdraw(user);
        }

        public void Exit(string user)
        {
            base.Channel.Exit(user);
        }

        void Flip()
        {
            base.Channel.Flip();
        }

        void Reset()
        {
            base.Channel.Reset();

        }

        void ShowScore(string score)
        {

            base.Channel.ShowScore(score);
        }

        void SyncStory(Story story)
        {

            base.Channel.SyncStory(story);
        }

        void SyncStoryList(List<Story> storyList)
        {
            base.Channel.SyncStoryList( storyList);

        }
    }
}
