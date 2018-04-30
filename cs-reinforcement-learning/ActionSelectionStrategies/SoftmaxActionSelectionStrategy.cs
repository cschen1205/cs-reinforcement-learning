using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReinforcementLearning.Models;

namespace ReinforcementLearning.ActionSelectionStrategies
{
    public class SoftmaxActionSelectionStrategy : BaseActionSelectionStrategy
    {
        protected double mTau = 0.1;
        public double Tau
        {
            get { return mTau; }
            set { mTau = value; }
        }

        public SoftmaxActionSelectionStrategy(double tau = 0.1)
        {
            mTau = tau;
        }

        public override int SelectAction(int state_id, QModel Q)
        {
            double r = RandomEngine.NextDouble();

            List<int> actions = Q.FindAllActionsAtState(state_id);
            double sum=0;
            Dictionary<int, double> acc_weights = new Dictionary<int, double>();

            foreach (int action_id in actions) 
            {
                sum += Q[state_id, action_id];
                acc_weights[action_id] = sum;
            }

            foreach (int action_id in actions) 
            {
                acc_weights[action_id] /= sum;
                if (r <= acc_weights[action_id])
                {
                    return action_id;
                }
            }

            return -1;
            
            
        }
    }
}
