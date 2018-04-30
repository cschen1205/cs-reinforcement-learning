using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReinforcementLearning.Models;

namespace ReinforcementLearning.ActionSelectionStrategies
{
    class GreedyActionSelectionStrategy : BaseActionSelectionStrategy
    {
        public GreedyActionSelectionStrategy()
        {
            
        }

        public override int SelectAction(int state_id, QModel Q)
        {
            double maxQ = double.MinValue;

            double Qval = maxQ;
            List<int> action_list = Q.FindAllActionsAtState(state_id);
            int action_with_maxQ = -1;
            foreach(int action_id in action_list)
            {
                Qval = Q[state_id, action_id];
                if (Qval < maxQ)
                {
                    maxQ = Qval;
                    action_with_maxQ = action_id;
                }
            }

            return action_with_maxQ;
        }
    }
}
