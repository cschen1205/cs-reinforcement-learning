using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReinforcementLearning.Models;

namespace ReinforcementLearning
{
    /// <summary>
    /// Implement QLearning(lambda) (eligibility traces)
    /// Usage:
    /// QLearningLambda method=new QLearningLambda(state_count, action_count, alpha, gamma, lambda, initial_Q);
    /// method.OnExecute+=(current_state, current_action, callback)=>
    ///   {
    ///     // DoTask is a user-defined function which apply current_action at current_state, and returns immediate_reward and the arrived_state after the action
    ///     DoTask(current_state, current_action, (immediate_reward, arrived_state)=>
    ///     {
    ///         ActionCompletedEventArgs e=new ActionCompletedEventArgs();
    ///         e.PrevStateId = current_state;
    ///         e.PrevActionId = current_action;
    ///         e.StateId = arrived_state;
    ///         e.Reward = immediate_reward;
    ///         
    ///         PrintProgress(e); //PrintProgress is a user-defined function
    ///         callback(method, e); //Remember to invoke callback method at the end
    ///     }); 
    ///     
    ///   });
    /// method.Initialize(state0);
    /// while(!ShouldTerminate())
    /// {
    ///   method.Iterate();
    /// }
    /// </summary>
    public class QLearningLambda : QLearning
    {
        protected Dictionary<StateActionKey, double> mEligibilityTraces;
        protected double mLambda = 0.9;

        public double Lambda
        {
            get { return mLambda; }
            set { mLambda = value; }
        }

        public QLearningLambda(int state_count, int action_count, double alpha = 0.1, double gamma = 0.7, double lambda=0.9, double initial_Q = 0.1)
            : base(state_count, action_count, alpha, gamma, initial_Q)
        {
            mLambda = lambda;
            mEligibilityTraces = new Dictionary<StateActionKey, double>();
        }

        public QLearningLambda(SparseQModel model, double lambda=0.9)
            : base(model)
        {
            mLambda = lambda;
            mEligibilityTraces = new Dictionary<StateActionKey, double>();
        }

        public double FindEligibilityTrace(int state_id, int action_id)
        {
            StateActionKey key = StateActionKey.Create(state_id, action_id);
            if (mEligibilityTraces.ContainsKey(key))
            {
                return mEligibilityTraces[key];
            }
            return 0;
        }

        public void SetEligilibityTrace(int state_id, int action_id, double value)
        {
            StateActionKey key = StateActionKey.Create(state_id, action_id);
            mEligibilityTraces[key] = value;
        }

        public void IncrementEligibilityTrace(int state_id, int action_id, double increment)
        {
            SetEligilibityTrace(state_id, action_id, FindEligibilityTrace(state_id, action_id) + increment);
        }

        private void Update(int state_id, int action_id, int next_state_id, int next_action_id, double immediate_reward)
        {
            // old_value is $Q_t(s_t, a_t)$
            double oldQ = mModel[state_id, action_id];

            // estimate_of_optimal_future_value is $max_a Q_t(s_{t+1}, a)$
            int actionStar;
            double actionStarQ=GetMaxQForState(next_state_id, out actionStar);

            // learning_rate
            double alpha = mModel.GetAlpha();

            // discount_rate
            double gamma = mModel.Gamma;

            // learned_value = immediate_reward + gamma * estimate_of_optimal_future_value
            // old_value = oldQ
            // temporal_difference = learned_value - old_value
            double delta = immediate_reward + gamma * actionStarQ - oldQ;

            IncrementEligibilityTrace(state_id, action_id, 1);

            List<int> state_list = mModel.FindAllStates();

            foreach(int state in state_list)
            {
                List<int> action_list = mModel.FindAllActionsAtState(state);
                foreach(int action in action_list)
                {
                    mModel[state, action] = mModel[state, action] + delta * alpha * FindEligibilityTrace(state, action);
                    if (next_action_id == actionStar)
                    {
                        SetEligilibityTrace(state, action, gamma * mLambda * FindEligibilityTrace(state, action));
                    }
                    else
                    {
                        SetEligilibityTrace(state, action, 0);
                    }
                }
            }
        }
    }
}
