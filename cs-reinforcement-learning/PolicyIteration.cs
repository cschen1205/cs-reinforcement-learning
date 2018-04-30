using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning
{
    /// <summary>
    /// PolicyIteration method=new PolicyIteration(state_count, action_count, gamma, initial_U);
    /// method.Initialize(initial_policy);
    /// PrepareRewardValues(method.Model);
    /// while(!ShouldTerminate())
    /// {
    ///   method.Iterate();
    /// }
    /// Print(e.OptimalPolicy);
    /// </summary>
    public class PolicyIteration : ValueIteration
    {
        private Dictionary<int, int> mPolicy = new Dictionary<int, int>();

        public PolicyIteration(int state_count, int action_count, double gamma = 0.9, double initial_U = 0.1)
            : base(state_count, action_count, gamma, initial_U)
        {

        }

        public void Initialize(Dictionary<int, int> initial_policy)
        {
            mPolicy.Clear();
            foreach (int state_id in initial_policy.Keys)
            {
                mPolicy[state_id] = initial_policy[state_id];
            }
        }

        public void EvaluatePolicy()
        {
            List<int> states = mModel.FindAllStates();
            foreach (int state_id in states)
            {
                EvaluateState(state_id);
            }
        }

        public override void Iterate()
        {
            EvaluatePolicy();
            ImprovePolicy();
        }

        public void EvaluateState(int current_state_id)
        {
            Dictionary<int, double> next_state_info_matrix = GetNextStates(current_state_id, mPolicy[current_state_id]);
            double newVal = 0;
            foreach (int newStateId in next_state_info_matrix.Keys)
            {
                double transition_prob = next_state_info_matrix[newStateId];
                double newU = mModel[newStateId];
                newVal += (transition_prob * newU);
            }

            mModel[current_state_id] = FindReward(current_state_id) + newVal;
        }

        public void ImprovePolicy()
        {
            UpdateUtility();
            List<int> states=mModel.FindAllStates();
            foreach (int state_id in states)
            {
                mPolicy[state_id] = SelectAction(state_id);
            }
        }
    }
}
