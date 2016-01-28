using log4net;
using PlanningPoker.Entity;
using PlanningPoker.Utility;
using PlanningPoker.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace PlanningPoker.FormStates
{
    class GameStateServer : GameStateBase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly GameStateServer Instance = new GameStateServer();

        private ServiceHost host = null;
        private bool isModeratorExit = false;
        
        /// <summary>
        /// An callback to close window.
        /// </summary>
        public event EventHandler CloseFormHandler;

        private GameStateServer() 
        {
            gameInfo.Moderator = gameInfo.UserName;
        }

        public void StartServer(string serverIP)
        {
            try
            {
                Uri baseAddress = new Uri(string.Format("net.tcp://{0}/{1}", serverIP, typeof(GamePlay).Name));
                NetTcpBinding netTcpBinding = new NetTcpBinding();
                netTcpBinding.MaxBufferSize = 2147483647;
                netTcpBinding.MaxReceivedMessageSize = 2147483647;
                host = new ServiceHost(typeof(GamePlay), baseAddress);
                host.AddServiceEndpoint(typeof(IGamePlay), netTcpBinding, "");
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(smb);
                host.Open();
            }
            catch (Exception exp)
            {
                log.Error(string.Format("cannot start servie, {0}", serverIP), exp);
                gameInfo.Message = exp.Message;
            }
        }

        public void CloseServer()
        {
            if(host != null)
            {
                host.Close();
            }
        }

        public bool IsConnected
        {
            get { return host != null; }
        }

        public override void callback_ExitEventHandler(object sender, WCF.UserExitEventArgs e)
        {
            // moderator exits
            if (e.ExitUser == gameInfo.Moderator)
            {
                isModeratorExit = true;
                
                if (CloseFormHandler != null)
                {
                    CloseFormHandler(null, null);
                }
            }
        }

        public override void Join(string serverIP)
        {
            bool joined = base.JoinGame(serverIP);
            gameInfo.CanStartService = joined;
        }

        public override bool IsModeratorExit
        {
            get
            {
                return isModeratorExit;
            }
        }

        public override void Flip()
        {
            // only host/server can flip cards
            if (gamePlay != null)
            {
                gamePlay.Flip();
                string score = CalcScore();
                gamePlay.ShowScore(score);
            }
        }

        private string CalcScore()
        {
            double? total = null;
            int count = 0;
            foreach (Participant p in gameInfo.ParticipantsList)
            {
                if (IsCalculatable(p.Role))
                {
                    double v;
                    bool canParse = false;

                    if (p.UnflipedPlayingCard.Contains("/"))
                    {
                        v = Utils.FractionToDouble(p.UnflipedPlayingCard);
                        canParse = true;
                    }
                    else
                    {
                        canParse = double.TryParse(p.UnflipedPlayingCard, out v);
                    }
                    if (canParse)
                    {
                        total = total == null ? v : total + v;
                        count++;
                    }
                }
            }

            if (gameInfo.CardSquence.Count == 0 || !total.HasValue)
            {
                return "-";
            }

            if (count > 0)
            {
                double average = total.Value * 1.0 / count;
                String ret = "-";

                foreach (string card in gameInfo.CardSquence)
                {
                    double c;
                    bool canParse = false;

                    if (card.Contains("/"))
                    {
                        c = Utils.FractionToDouble(card);
                        canParse = true;
                    }
                    else
                    {
                        canParse = double.TryParse(card, out c);
                    }

                    if (canParse && c >= average)
                    {
                        ret = card;
                        break;
                    }
                }

                return ret;
            }

            return "-";
        }

        private bool IsCalculatable(string role)
        {
            return Enum.GetNames(typeof(Role)).Contains(role);
        }

        public override void Reset()
        {
            // only server can reset the game
            if (gamePlay != null)// && host != null
            {
                gamePlay.Reset();
            }
        }

    }
}
