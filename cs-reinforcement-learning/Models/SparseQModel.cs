using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning.Models
{
    /// <summary>
    /// Q is known as the quality of state-action combination, note that it is different from utility of a state
    /// </summary>
    public class SparseQModel : QModel
    {
        protected Dictionary<StateActionKey, double> mQMatrix; // Q value for (state_id, action_id) pair
        protected Dictionary<int, HashSet<int>> mActionSet=new Dictionary<int, HashSet<int>>();
        protected HashSet<int> mStateSet=new HashSet<int>();
        protected Dictionary<int, double> mV; // V value for each state, i.e., the true utility value of each state
        protected Dictionary<StateActionKey, double> mAlphaMatrix; // value for learning rate: alpha(state_id, action_id)
        protected double mInitialQ = 0;

        public override double GetMaxQForState(int state_id, out int selected_action_id)
        {
            double maxQ = double.MinValue;
            double Q = maxQ;
            selected_action_id = -1;
            foreach(int action_id in FindActionSetAtState(state_id))
            {
                Q = FindQEntry(state_id, action_id);
                if (maxQ < Q)
                {
                    selected_action_id = action_id;
                    maxQ = Q;
                }
            }

            return maxQ;
        }

        private double FindQEntry(int state_id, int action_id)
        {
            StateActionKey key=StateActionKey.Create(state_id, action_id);
            if(mQMatrix.ContainsKey(key))
            {
                return mQMatrix[key];
            }
            return mInitialQ;
        }

        private void SetQEntry(int state_id, int action_id, double Q)
        {
            mStateSet.Add(state_id);
            FindActionSetAtState(state_id).Add(action_id);
            StateActionKey key = StateActionKey.Create(state_id, action_id);
            mQMatrix[key] = Q;

        }

        public HashSet<int> FindActionSetAtState(int state_id)
        {
            HashSet<int> action_set = null;
            if (mActionSet.ContainsKey(state_id))
            {
                action_set = mActionSet[state_id];
            }
            else
            {
                action_set = new HashSet<int>();
                mActionSet[state_id] = action_set;
            }

            return action_set;
        }

        public override void Reset(double initial_Q)
        {
            mInitialQ = initial_Q;
            foreach(int state_id in mStateSet)
            {
                foreach(int action_id in FindActionSetAtState(state_id))
                {
                    SetQEntry(state_id, action_id, initial_Q);
                }
            }
        }

        public override List<int> FindAllActionsAtState(int state_id)
        {
            List<int> actions_at_state = null;
            if (TryUserDefinedFindAllActionsAtStateRequest(state_id, out actions_at_state))
            {
                return actions_at_state;
            }

            return FindActionSetAtState(state_id).ToList();
        }

        public override List<int> FindAllStates()
        {
            return mStateSet.ToList();
        }

        public SparseQModel(int[] known_states, int[] known_actions, double initial_Q = 0.1)
            : base(-1, -1, initial_Q)
        {
            mInitialQ = initial_Q;

            mQMatrix = new Dictionary<StateActionKey, double>();
            mAlphaMatrix = new Dictionary<StateActionKey, double>();

            mV = new Dictionary<int, double>();

            if (known_states != null)
            {
                foreach (int state_id in known_states)
                {
                    mStateSet.Add(state_id);

                    if (known_actions != null)
                    {
                        HashSet<int> action_set = new HashSet<int>();
                        foreach (int action_id in known_actions)
                        {
                            action_set.Add(action_id);
                        }
                        mActionSet[state_id] = action_set;
                    }
                }
            }
        }

        public SparseQModel(int[] known_states, Dictionary<int, int[]> known_actions_at_state, double initial_Q = 0.1)
            : base(-1, -1, initial_Q)
        {
            mInitialQ = initial_Q;

            mQMatrix = new Dictionary<StateActionKey, double>();
            mAlphaMatrix = new Dictionary<StateActionKey, double>();

            mV = new Dictionary<int, double>();

            if (known_states != null)
            {
                foreach (int state_id in known_states)
                {
                    mStateSet.Add(state_id);

                    if (known_actions_at_state != null)
                    {
                        if (known_actions_at_state.ContainsKey(state_id))
                        {
                            int[] known_actions = known_actions_at_state[state_id];

                            HashSet<int> action_set = new HashSet<int>();
                            foreach (int action_id in known_actions)
                            {
                                action_set.Add(action_id);
                            }
                            mActionSet[state_id] = action_set;
                        }
                    }
                }
            }
        }

        public override double GetAlpha(int state_id, int action_id)
        {
            StateActionKey key = StateActionKey.Create(state_id, action_id);
            double alpha = -1;
            if (mAlphaMatrix.ContainsKey(key))
            {
                alpha = mAlphaMatrix[key];
            }
            if (alpha < 0) return mAlphaValue;
            return alpha;
        }

        public override void SetAlpha(int state_id, int action_id, double newValue)
        {
            mStateSet.Add(state_id);
            FindActionSetAtState(state_id).Add(state_id);
            StateActionKey key = StateActionKey.Create(state_id, action_id);
            mAlphaMatrix[key] = newValue;
        }

        public override double this[int state_id, int action_id]
        {
            get
            {
                return FindQEntry(state_id, action_id);
            }
            set
            {
                SetQEntry(state_id, action_id, value);
            }
        }
    }
}
