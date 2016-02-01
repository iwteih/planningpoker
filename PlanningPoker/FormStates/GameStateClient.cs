using PlanningPoker.Entity;
using PlanningPoker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.FormStates
{
    class GameStateClient : GameStateBase
    {
        public static readonly GameStateClient Instance = new GameStateClient();

        private GameStateClient() { }

        public override void callback_ExitEventHandler(object sender, WCF.UserExitEventArgs e)
        {
            if (e.ExitUser == gameInfo.Moderator)
            {
                Init();
                gameInfo.Message = string.Format("{0} has ended this game!", e.ExitUser);
            }
        }

        public override void Join(string serverIP)
        {
            bool joined = base.JoinGame(serverIP);
            gameInfo.CanConnectServer = joined;
        }

        public override bool IsModeratorExit
        {
            get
            {
                return false;
            }
        }

        public override void Flip()
        {
            // client is not allowed to send flip command
        }

        public override void Reset()
        {
            // client is not allowed to send reset command
        }


        public override void SyncStory(Story story)
        {
            // client is not allowed to send syncstory command
        }

        public override void callback_StorySyncEventHandler(object sender, WCF.StorySyncArgs e)
        {
            gameInfo.SyncStory = e.Story;
            gameInfo.CurrentStory = e.Story;
        }
    }
}
