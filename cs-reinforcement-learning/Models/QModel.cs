using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning.Models
{
    /// <summary>
    /// Q is known as the quality of state-action combination, note that it is different from utility of a state
    /// </summary>
    public class QModel
    {
        /// <summary>
        ///  Q value for (state_id, action_id) pair
        ///  Q is known as the quality of state-action combination, note that it is different from utility of a state
        /// </summary>
        private double[,] mQMatrix; 

        /// <summary>
        ///  $\alpha[s, a]$ value for learning rate: alpha(state_id, action_id)
        /// </summary>
        private double[,] mAlphaMatrix;

        /// <summary>
        /// default value for all learning rate: alpha(state_id, action_id)
        /// </summary>
        protected double mAlphaValue = 0.1; 

        /// <summary>
        /// discount factor
        /// </summary>
        protected double mGamma = 0.7;

        /// <summary>
        /// Compute the estimate of optimal future value $max_a Q(s, a)$ given state $s$
        /// </summary>
        /// <param name="state_id">The given state $s$</param>
        /// <returns>The maximum $Q(s, a)$ at a given state $s$</returns>
        public virtual double GetMaxQForState(int state_id)
        {
            int selected_action_id;
            return GetMaxQForState(state_id, out selected_action_id);
        }

        /// <summary>
        /// Compute the estimate of optimal future value $max_a Q(s, a)$ given state $s$
        /// </summary>
        /// <param name="state_id">The given state $s$</param>
        /// <param name="selected_action_id">The action $a \in Actions(s)$ for which $Q(s, a)$ is maximum given state $s$</param>
        /// <returns>The maximum $Q(s, a)$ at a given state $s$</returns>
        public virtual double GetMaxQForState(int state_id, out int selected_action_id)
        {
            double maxQ = double.MinValue;
            double Q = maxQ;
            selected_action_id = -1;
            for (int action_id = 0; action_id < mActionCount; ++action_id)
            {
                Q = mQMatrix[state_id, action_id];
                if (maxQ < Q)
                {
                    selected_action_id = action_id;
                    maxQ = Q;
                }
            }

            return maxQ;
        }

        public double Gamma
        {
            get { return mGamma; }
            set { mGamma = value; }
        }

        private int mStateCount;
        private List<int> mStates;
        public virtual List<int> FindAllStates()
        {
            if (mStates == null)
            {
                mStates = new List<int>();
                for (int state_id = 0; state_id < mStateCount; ++state_id)
                {
                    mStates.Add(state_id);
                }
            }
            return mStates;
        }

        public delegate List<int> FindAllActionsAtStateHandler(int state);
        public event FindAllActionsAtStateHandler OnFindAllActionsAtStateRequested;

        protected bool TryUserDefinedFindAllActionsAtStateRequest(int state_id, out List<int> actions_at_state)
        {
            actions_at_state = null;
            if (OnFindAllActionsAtStateRequested != null)
            {
                actions_at_state = OnFindAllActionsAtStateRequested(state_id);
                return true;
            }
            return false;
        }

        private int mActionCount;
        private List<int> mActions;
        public virtual List<int> FindAllActionsAtState(int state)
        {
            if (OnFindAllActionsAtStateRequested != null)
            {
                return OnFindAllActionsAtStateRequested(state);
            }

            if (mActions == null)
            {
                mActions = new List<int>();
                for (int action_id = 0; action_id < mActionCount; ++action_id)
                {
                    mActions.Add(action_id);
                }
            }
            return mActions; 
        }

        public virtual void Reset(double initial_Q)
        {
            for (int state_id = 0; state_id < mStateCount; ++state_id)
            {
                for (int action_id = 0; action_id < mActionCount; ++action_id)
                {
                    mQMatrix[state_id, action_id] = initial_Q;
                }
            }
        }

        public QModel(int state_count, int action_count, double initial_Q = 0.1)
        {
            mStateCount = state_count;
            mActionCount = action_count;

            mQMatrix = new double[mStateCount, mActionCount];
            mAlphaMatrix = new double[mStateCount, mActionCount];

            for (int state_id = 0; state_id < mStateCount; ++state_id)
            {
                for (int action_id = 0; action_id < mActionCount; ++action_id)
                {
                    mQMatrix[state_id, action_id] = initial_Q;
                    mAlphaMatrix[state_id, action_id] = -1;
                }
            }
        }

        public double GetAlpha()
        {
            return mAlphaValue;
        }

        public virtual double GetAlpha(int state_id, int action_id)
        {
            if (state_id < 0 || state_id >= mStateCount)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (action_id < 0 || action_id >= mActionCount)
            {
                throw new ArgumentOutOfRangeException();
            }

            double alpha = mAlphaMatrix[state_id, action_id];
            if (alpha < 0) return mAlphaValue;
            return alpha;
        }

        public virtual void SetAlpha(int state_id, int action_id, double newValue)
        {
            if (state_id < 0 || state_id >= mStateCount)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (action_id < 0 || action_id >= mActionCount)
            {
                throw new ArgumentOutOfRangeException();
            }

            mAlphaMatrix[state_id, action_id] = newValue;
        }

        public void SetAlpha(double newValue)
        {
            mAlphaValue = newValue;
        }

        public virtual double this[int state_id, int action_id]
        {
            get
            {
                if (state_id < 0 || state_id >= mStateCount)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (action_id < 0 || action_id >= mActionCount)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return mQMatrix[state_id, action_id];
            }
            set
            {
                if (state_id < 0 || state_id >= mStateCount)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (action_id < 0 || action_id >= mActionCount)
                {
                    throw new ArgumentOutOfRangeException();
                }

                mQMatrix[state_id, action_id] = value;
            }
        }
    }
}
