using PlanningPoker.Entity;
using PlanningPoker.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PlanningPoker.StoryPointCalc
{
    public abstract class StoryPointCalcBase : IStoryPointCalc
    {
        public abstract string Calc(Collection<Participant> participants, Collection<string> cardSquence);

        public string CalcFunc(List<Participant> participants, Collection<string> cardSquence)
        {
            double? total = null;
            int count = 0;
            foreach (Participant p in participants)
            {
                if (!IsCalculatableRole(p.Role))
                {
                    continue;
                }

                double v;
                bool canParse = false;

                if (p.UnflipedPlayingCard.Contains("/"))
                {
                    v = Utils.FractionToFloat(p.UnflipedPlayingCard);
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

            if (cardSquence.Count == 0 || !total.HasValue)
            {
                return Story.UnFlippedScore;
            }

            if (count > 0)
            {
                double average = total.Value * 1.0 / count;
                String ret = Story.UnFlippedScore;

                foreach (string card in cardSquence)
                {
                    double c;
                    bool canParse = false;

                    if (card.Contains("/"))
                    {
                        c = Utils.FractionToFloat(card);
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

            return Story.UnFlippedScore;
        }

        private bool IsCalculatableRole(string role)
        {
            bool flag = Enum.GetNames(typeof(Role)).Contains(role);

            if (!flag)
            {
                return false;
            }

            Role r = (Role)Enum.Parse(typeof(Role), role, true);
            if(GetCalculatableRoles().Contains(r))
            {
                return true;
            }

            return false;
        }

        protected Role[] GetCalculatableRoles()
        {
            return new Role[]{Role.Dev, Role.QA};
        }

        protected string[] GetCalculatableRoleStrings()
        {
            return GetCalculatableRoles().Select(r => r.ToString()).ToArray();
        }
    }
}
