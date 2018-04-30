using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReinforcementLearning.Events;
using ReinforcementLearning.ActionSelectionStrategies;

namespace ReinforcementLearning
{
    public abstract class ReinforcementLearning
    {
        protected int mCurrentStateId;
        protected int mCurrentActionId;
        protected IActionSelectionStrategy mActionSelectionStrategy;

        public delegate void ExecuteHandler(int current_state_id, int action_id, EventHandler<ActionCompletedEventArgs> on_executed);
        public event ExecuteHandler OnExecute;

        public IActionSelectionStrategy ActionSelectionStrategy
        {
            get { return mActionSelectionStrategy; }
            set { mActionSelectionStrategy = value; }
        }

        public int CurrentStateId
        {
            get { return mCurrentStateId; }
            set
            {
                mCurrentStateId = value;
                mCurrentActionId = SelectAction(mCurrentStateId);
            }
        }

        public void Initialize(int state_id)
        {
            mCurrentActionId = state_id;
            mCurrentActionId = SelectAction(state_id);
        }

        public abstract int SelectAction(int state_id);

        public virtual void Execute(int current_state_id, int action_id, EventHandler<ActionCompletedEventArgs> callback)
        {
            if (OnExecute != null)
            {
                OnExecute(current_state_id, action_id, (sender, e) =>
                {
                    if (e.PrevStateId == -1) e.PrevStateId = current_state_id;
                    if (e.PrevActionId == -1) e.PrevActionId = action_id;
                    callback(this, e);
                });
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public abstract void Iterate();
    }
}
