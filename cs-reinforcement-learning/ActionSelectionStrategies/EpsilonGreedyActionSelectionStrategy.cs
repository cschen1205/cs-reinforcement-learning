using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReinforcementLearning.Models;

namespace ReinforcementLearning.ActionSelectionStrategies
{
    public class EpsilonGreedyActionSelectionStrategy : BaseActionSelectionStrategy
    {
        protected double mEpsilon = 0.1;
        public double Epsilon
        {
            get { return mEpsilon; }
            set { mEpsilon = value; }
        }

        public EpsilonGreedyActionSelectionStrategy(double epsilon = 0.1)
        {
            mEpsilon = epsilon;
        }

        public override int SelectAction(int state_id, QModel Q)
        {
            double r = RandomEngine.NextDouble();

            if (r < 1 - mEpsilon)
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
            else
            {
                List<int> action_list = Q.FindAllActionsAtState(state_id);
                int action_count = action_list.Count;
                return action_list[(int)(r * action_count) % action_count];
            }

            
        }
    }
}
