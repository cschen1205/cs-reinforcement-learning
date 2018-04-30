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
    /// Implement temporal-difference learning Sarsa, which is an on-policy TD control algorithm
    /// Usage:
    /// Sarsa method=new Sarsa(state_count, action_count, alpha, gamma, initial_Q);
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
    public class Sarsa : TDLearning
    {
        public Sarsa(int state_count, int action_count, double alpha = 0.1, double gamma = 0.7, double initial_Q = 0.1)
        {
            mModel = new QModel(state_count, action_count, initial_Q);
            LearningRate = alpha;
            DiscountFactor = gamma;

            mActionSelectionStrategy = new EpsilonGreedyActionSelectionStrategy();
        }

        private void Update(int state_id, int action_id, int next_state_id, int next_action_id, double immediate_reward)
        {
            double oldQ = mModel[state_id, action_id];
            double nextStateActionQ = mModel[next_state_id, next_action_id];

            double alpha=mModel.GetAlpha();
            double gamma=mModel.Gamma;

            double newQ = oldQ + alpha * (immediate_reward + gamma * nextStateActionQ - oldQ);

            mModel[state_id, action_id] = newQ;
        }

        public override void Iterate()
        {
            Execute(mCurrentStateId, mCurrentActionId, (sender, e)=>{
                int newStateId = e.StateId;
                int newActionId = SelectAction(newStateId);
                double reward = e.Reward;

                Update(mCurrentStateId, mCurrentActionId, newStateId, newActionId, reward);

                mCurrentStateId = newStateId;
                mCurrentActionId = newActionId;

            });


        }
    }
}
