using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReinforcementLearning.Models;

namespace ReinforcementLearning.ActionSelectionStrategies
{
    public interface IActionSelectionStrategy
    {
        int SelectAction(int state_id, QModel model);
        int SelectAction(int state_id, UtilityModel model);
    }
}
