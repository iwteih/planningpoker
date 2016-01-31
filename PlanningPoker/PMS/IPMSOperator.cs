using PlanningPoker.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningPoker.PMS
{
    interface IPMSOperator
    {
        List<Story> Query(string user, string password, string url);
    }
}
