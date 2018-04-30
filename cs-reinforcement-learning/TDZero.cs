using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning.Models
{
    /// <summary>
    /// implement temporal-difference learning TD(0)
    /// </summary>
    public class TDZero : TDLearning
    {
        /// <summary>
        /// Utility of each state, $mV$[$s$] is the utility value of state $s$
        /// </summary>
        protected double[] mV;
        protected int mStateCount;
        
        public TDZero(int state_count, int action_count, double alpha = 0.1, double gamma=0.9, double initial_V=0.1)
        {
            mStateCount = state_count;
            mV = new double[mStateCount];
            for (int state_id = 0; state_id < mStateCount; ++state_id)
            {
                mV[state_id] = initial_V;
            }
            LearningRate = alpha;
            DiscountFactor = gamma;
        }

        public int StateCount
        {
            get { return mStateCount; }
        }
        
        public override void Reset(double initial_V)
        {
            for (int state_id = 0; state_id < mStateCount; ++state_id)
            {
                mV[state_id] = initial_V;
            }
        }

        public void Update(int state_id, int next_state_id, double immediate_reward)
        {
            double oldV = mV[state_id];

            double gamma = DiscountFactor;
            double alpha = LearningRate;

            double nextStateV = mV[next_state_id];

            // learned_value = immediate_reward + gamma * nextStateV
            // old_value = oldV
            // temporal_difference = learned_value - old_value
            // new_value = old_value + learning_rate * temporal_difference
            double newV = oldV + alpha * (immediate_reward + gamma * nextStateV - oldV); 

            mV[state_id] = newV;
        }

        public override void Iterate()
        {
            Execute(mCurrentStateId, mCurrentActionId, (sender, e) =>
            {
                int newStateId = e.StateId;
                int newActionId = SelectAction(newStateId);
                double reward = e.Reward;

                Update(mCurrentStateId, newStateId, reward);

                mCurrentStateId = newStateId;
                mCurrentActionId = newActionId;

            });


        }
    }
}
