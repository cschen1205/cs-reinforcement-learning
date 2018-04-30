using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReinforcementLearning.Models;
using ReinforcementLearning.ActionSelectionStrategies;
using ReinforcementLearning.Events;

namespace ReinforcementLearning
{
    /// <summary>
    /// Implement temporal-difference learning Q-Learning, which is an off-policy TD control algorithm
    /// Q is known as the quality of state-action combination, note that it is different from utility of a state
    /// Usage:
    /// QLearning method=new QLearning(state_count, action_count, alpha, gamma, initial_Q);
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
    /// 
    /// </summary>
    public class QLearning : TDLearning
    {
        public QLearning(int state_count, int action_count, double alpha = 0.1, double gamma = 0.7, double initial_Q=0.1)
        {
            mModel = new QModel(state_count, action_count, initial_Q);
            LearningRate = alpha;
            DiscountFactor = gamma;

            mActionSelectionStrategy = new EpsilonGreedyActionSelectionStrategy();
        }

        public QLearning(SparseQModel model)
        {
            mModel = model;
            mActionSelectionStrategy = new EpsilonGreedyActionSelectionStrategy();
        }

        public void Update(int state_id, int action_id, int next_state_id, double immediate_reward)
        {
            // old_value is $Q_t(s_t, a_t)$
            double oldQ = mModel[state_id, action_id];

            // learning_rate;
            double alpha = mModel.GetAlpha(state_id, action_id); 

            // discount_rate;
            double gamma = mModel.Gamma; 

            // estimate_of_optimal_future_value is $max_a Q_t(s_{t+1}, a)$
            double maxQ = GetMaxQForState(next_state_id); 

            // learned_value = immediate_reward + gamma * estimate_of_optimal_future_value
            // old_value = oldQ
            // temporal_difference = learned_value - old_value
            // new_value = old_value + learning_rate * temporal_difference
            double newQ = oldQ + alpha * (immediate_reward + gamma * maxQ - oldQ);

            // new_value is $Q_{t+1}(s_t, a_t)$
            mModel[state_id, action_id] = newQ;
        }

        public override void Iterate()
        {
            Execute(mCurrentStateId, mCurrentActionId, (sender, e) =>
            {
                int newStateId = e.StateId;
                double reward = e.Reward;

                Update(mCurrentStateId, mCurrentActionId, newStateId, reward);

                mCurrentStateId = newStateId;
                mCurrentActionId = SelectAction(mCurrentStateId);

            });
        }
    }
}
