using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.FormStates
{
    interface IGameState
    {
        bool IsModeratorExit{get;}
        void Init();
        void Join(string serverIP);
        void Play(string card);
        void Withdraw();
        void Reset();
        void Flip();
        void Exit();
        void SyncStory(Story story);
    }
}
