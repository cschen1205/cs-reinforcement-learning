using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReinforcementLearning.Models;

namespace ReinforcementLearning.ActionSelectionStrategies
{
    public abstract class BaseActionSelectionStrategy : IActionSelectionStrategy
    {
        public virtual int SelectAction(int state_id, UtilityModel model)
        {
            return -1;
        }

        public abstract int SelectAction(int state_id, QModel model);
    }
}
