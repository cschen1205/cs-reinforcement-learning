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
    /// R-learning is a standard TD control method based on off-policy GPI, much like Q-learning
    /// </summary>
    class RLearning : TDLearning
    {
        protected double mRho;
        protected double mBeta;

        public double Rho
        {
            get { return mRho; }
            set { mRho = value; }
        }

        public double Beta
        {
            get { return mBeta; }
            set { mBeta = value; }
        }

        public RLearning(int state_count, int action_count, double alpha = 0.1, double beta=0.1, double rho = 0.7, double initial_Q = 0.1)
        {
            mModel = new QModel(state_count, action_count, initial_Q);
            LearningRate = alpha;
            mRho = rho;
            mBeta = beta;

            mActionSelectionStrategy = new EpsilonGreedyActionSelectionStrategy();
        }

        private void Update(int state_id, int action_id, int next_state_id, double immediate_reward)
        {
            double oldQ = mModel[state_id, action_id];

            double alpha = mModel.GetAlpha(state_id, action_id); // learning rate;

            double maxQ = GetMaxQForState(next_state_id);

            double newQ = oldQ + alpha * (immediate_reward - mRho + maxQ - oldQ);

            double maxQAtCurrentState=GetMaxQForState(state_id);
            if (newQ == maxQAtCurrentState)
            {
                mRho = mRho + mBeta * (immediate_reward - mRho + maxQ - maxQAtCurrentState);
            }

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
