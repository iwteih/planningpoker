using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PlanningPoker.StoryPointCalc
{
    interface IStoryPointCalc
    {
        string Calc(Collection<Participant> participants, Collection<String> cardSquence);
    }
}
