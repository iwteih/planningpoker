using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.FormStates
{
    class MockGameState : IGameState
    {
        public static readonly MockGameState Instance = new MockGameState();

        private MockGameState() { }

        public bool IsModeratorExit
        {
            get { return false; }
        }

        public void Init()
        {
        }

        public bool Join(string serverIP)
        {
            return false;
        }

        public void Play(string card)
        {
        }

        public void Withdraw()
        {
        }

        public void Reset()
        {
        }

        public void Flip()
        {
        }

        public void Exit()
        {
        }

        void IGameState.Join(string serverIP)
        {
        }
    }
}
