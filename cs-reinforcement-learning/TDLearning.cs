using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReinforcementLearning.Models;
using ReinforcementLearning.ActionSelectionStrategies;
using ReinforcementLearning.Events;

namespace ReinforcementLearning
{
    public abstract class TDLearning : ReinforcementLearning
    {
        protected QModel mModel;
        
        
        public QModel Model
        {
            get { return mModel; }
            set { mModel = value; }
        }

       

        public double DiscountFactor
        {
            get { return mModel.Gamma; }
            set { mModel.Gamma = value; }
        }

        public double LearningRate
        {
            get { return mModel.GetAlpha(); }
            set { mModel.SetAlpha(value); }
        }

       

        public virtual void Reset(double initial_Q)
        {
            mModel.Reset(initial_Q);
        }

        

        public double GetMaxQForState(int state_id)
        {
            return mModel.GetMaxQForState(state_id);
        }

        public double GetMaxQForState(int state_id, out int selected_action_id)
        {
            return mModel.GetMaxQForState(state_id, out selected_action_id);
        }

        public double GetQ(int state_id, int action_id)
        {
            return mModel[state_id, action_id];
        }

        public void SetQ(int state_id, int action_id, double Q)
        {
            mModel[state_id, action_id] = Q;
        }

        public override int SelectAction(int state_id)
        {
            return mActionSelectionStrategy.SelectAction(state_id, mModel);
        }

       

    }
}
