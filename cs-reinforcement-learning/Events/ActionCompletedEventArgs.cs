using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning.Events
{
    public class ActionCompletedEventArgs : EventArgs
    {
        protected int mPrevStateId=-1;
        protected int mPrevActionId=-1;
        protected int mStateId=-1;
        protected double mReward;

        public double Reward
        {
            get { return mReward; }
        }

        public int StateId
        {
            get { return mStateId; }
        }

        public int PrevStateId
        {
            get { return mPrevStateId; }
            set { mPrevStateId = value; }
        }

        public int PrevActionId
        {
            get { return mPrevActionId; }
            set { mPrevActionId = value; }
        }

        public ActionCompletedEventArgs(int state_id, double reward)
        {
            mStateId = state_id;
            mReward = reward;
        }

        public ActionCompletedEventArgs(int prev_state_id, int prev_action_id, int state_id, double reward)
        {
            mPrevActionId = prev_action_id;
            mPrevStateId = prev_state_id;
            mStateId = state_id;
            mReward = reward;
        }
    }
}
