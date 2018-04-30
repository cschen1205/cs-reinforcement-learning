using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning.Models
{
    /// <summary>
    /// Utility value of a state $U(s)$ is the expected long term reward of state $s$ given the sequence of reward and the optimal policy
    /// Utility value $U(s)$ at state $s$ can be obtained by the Bellman equation
    /// Bellman Equtation states that $U(s) = R(s) + \gamma * max_a \sum_{s'} T(s,a,s')U(s')$
    /// where s' is the possible transitioned state given that action $a$ is applied at state $s$
    /// where $T(s,a,s')$ is the transition probability of $s \rightarrow s'$ given that action $a$ is applied at state $s$
    /// where $\sum_{s'} T(s,a,s')U(s')$ is the expected long term reward given that action $a$ is applied at state $s$
    /// where $max_a \sum_{s'} T(s,a,s')U(s')$ is the maximum expected long term reward given that the chosen optimal action $a$ is applied at state $s$ 
    /// </summary>
    public class UtilityModel
    {
        private double[] mU;
        private int mStateCount;
        private int mActionCount;
        private double mGamma = 0.9;

        public double Gamma
        {
            get { return mGamma; }
            set { mGamma = value; }
        }

        public UtilityModel(int state_count, int action_count, double initial_U = 0)
        {
            mStateCount = state_count;
            mU = new double[state_count];

            for (int state_id = 0; state_id < state_count; ++state_id)
            {
                mU[state_id] = initial_U;
            }
        }

        public virtual List<int> FindAllActionsAtState(int state_id)
        {
            List<int> actions = new List<int>();
            for (int action_id = 0; action_id < mActionCount; ++action_id)
            {
                actions.Add(action_id);
            }

            return actions;
        }

        public virtual List<int> FindAllStates()
        {
            List<int> states = new List<int>();
            for (int state_id = 0; state_id < mStateCount; ++state_id)
            {
                states.Add(state_id);
            }

            return states;
        }

        public virtual double this[int index]
        {
            get { return mU[index]; }
            set
            {
                mU[index] = value;
            }
        }

        public virtual void Reset(double initial_U = 0.1)
        {
            for (int state_id = 0; state_id < mStateCount; ++state_id)
            {
                mU[state_id] = initial_U;
            }
        }
    }
}
