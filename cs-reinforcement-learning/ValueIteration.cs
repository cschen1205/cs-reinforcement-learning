using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReinforcementLearning.Models;

namespace ReinforcementLearning
{
    public class ValueIteration : ReinforcementLearning
    {
        /// <summary>
        /// Utility of each state, $mV$[$s$] is the utility value of state $s$
        /// Usage:
        /// ValueIteration method=new ValueIteration(state_count, action_count, gamma, initial_U);
        /// PrepareRewardValues(method.Model);
        /// while(!ShouldTerminate())
        /// {
        ///   method.Iterate();
        /// }
        /// Print(method.OptimalPolicy);
        /// </summary>
        protected UtilityModel mModel;
        protected Dictionary<int, int> mOptimalPolicy = new Dictionary<int, int>();
        protected Dictionary<int, double> mRewards = new Dictionary<int, double>();

        public double FindReward(int state_id)
        {
            if (mRewards.ContainsKey(state_id))
            {
                return mRewards[state_id];
            }
            return 0;
        }

        public virtual Dictionary<int, int> OptimalPolicy
        {
            get { return mOptimalPolicy; }
        }

        public UtilityModel Model
        {
            get { return mModel; }
            set { mModel = value; }
        }

        public ValueIteration(int state_count, int action_count, double gamma = 0.9, double initial_U = 0.1)
        {
            mModel = new UtilityModel(state_count, action_count, initial_U);
            mModel.Gamma = gamma;
        }

        public void Reset(double initial_U)
        {
            mModel.Reset(initial_U);
        }

        public delegate Dictionary<int, double> GetNextStateInfoHandler(int state_id, int action_id);
        public event GetNextStateInfoHandler OnGetNextStateRequested;

        public virtual Dictionary<int, double> GetNextStates(int state_id, int action_id)
        {
            if (OnGetNextStateRequested != null)
            {
                return OnGetNextStateRequested(state_id, action_id);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public override int SelectAction(int state_id)
        {
            if (mOptimalPolicy.ContainsKey(state_id))
            {
                return mOptimalPolicy[state_id];
            }
            return -1;
        }

        public override void Iterate()
        {
            //Iterate(mCurrentStateId, out mCurrentActionId);
            //Execute(mCurrentStateId, mCurrentActionId, (sender, e) =>
            //{
            //    mCurrentStateId = e.StateId;
            //});

            UpdateUtility();
        }

        public void UpdateUtility()
        {
            List<int> states = mModel.FindAllStates();
            foreach (int state_id in states)
            {
                int selected_action_id = -1;
                UpdateUtilityAtState(state_id, out selected_action_id);
            }
        }

        public void UpdateUtilityAtState(int current_state_id, out int current_action_id)
        {
            current_action_id = -1;

            List<int> actions_at_state = mModel.FindAllActionsAtState(current_state_id);

            int best_action = -1;
            double maxNewVal = double.MinValue;
            foreach (int action_id in actions_at_state)
            {
                Dictionary<int, double> next_state_info_matrix = GetNextStates(current_state_id, action_id);
                double newVal = 0;
                foreach (int newStateId in next_state_info_matrix.Keys)
                {
                    double transition_prob = next_state_info_matrix[newStateId];
                    double newU = mModel[newStateId];
                    newVal += (transition_prob * newU);
                }

                if (newVal > maxNewVal)
                {
                    maxNewVal = newVal;
                    best_action = action_id;
                }
            }

            mModel[current_state_id] = FindReward(current_state_id) + maxNewVal;
            current_action_id = best_action;
            mOptimalPolicy[current_state_id] = current_action_id;

           
        }
    }
}
