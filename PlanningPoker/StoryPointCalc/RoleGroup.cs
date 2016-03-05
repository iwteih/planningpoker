using PlanningPoker.Entity;
using PlanningPoker.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PlanningPoker.StoryPointCalc
{
    class RoleGroup : StoryPointCalcBase
    {
        public override string Calc(Collection<Participant> participants, Collection<string> cardSquence)
        {
            //Role[] roles = GetCalculatableRoles();
            //string[] points = new string[roles.Length];

            //for (int i = 0; i < roles.Length; i++ )
            //{
            //    Role role = roles[i];
            //    List<Participant> list = participants.Where(p => string.Compare(p.Role, role.ToString(), true) == 0).ToList();
            //    points[i] = CalcFunc(list, cardSquence);
            //}
            string[] roles = GetCalculatableRoleStrings();

            List<string> currentRoles = participants.Select(p => p.Role).Intersect(roles).ToList();//.Distinct().Where(a=> roles..ToList();
            string[] points = new string[currentRoles.Count];
            for (int i = 0; i < currentRoles.Count; i++)
            {
                string role = currentRoles[i];
                List<Participant> list = participants.Where(p => string.Compare(p.Role, role, true) == 0).ToList();
                points[i] = CalcFunc(list, cardSquence);
            }

            string storyPoint = string.Join(" + ", points);
            double? value = null;

            foreach (string point in points)
            {
                double p;

                if (point.Contains("/"))
                {
                    p = Utils.FractionToFloat(point);
                }
                else
                {
                    bool canParse = double.TryParse(point, out p);
                    if (!canParse)
                    {
                        continue;
                    }
                }
                value = value == null ? p : value.Value + p;
            }

            string sum = string.Empty;

            if (points.Length > 1)
            {
                storyPoint = string.Format("{0} = {1}", storyPoint, value == null ? Story.UnFlippedScore : value.Value.ToString());
            }
            else // only one group, show story point directly
            {
                storyPoint = points[0];
            }
            return storyPoint;
        }
    }
}
