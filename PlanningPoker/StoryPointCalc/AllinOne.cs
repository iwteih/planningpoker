using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PlanningPoker.StoryPointCalc
{
    public class AllinOne : StoryPointCalcBase
    {
        public override string Calc(Collection<Participant> participants, Collection<string> cardSquence)
        {
            return CalcFunc(participants.ToList(), cardSquence);
        }
    }
}
