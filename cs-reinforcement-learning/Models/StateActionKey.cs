using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning.Models
{
    public class StateActionKey
    {
        protected int mStateId;
        protected int mActionId;
        protected int mUniqueId;

        private StateActionKey(int state_id, int action_id)
        {
            mStateId = state_id;
            mActionId = action_id;
            mUniqueId = Tuple.Create<int, int>(state_id, action_id).GetHashCode();
        }

        public static StateActionKey Create(int state_id, int action_id)
        {
            StateActionKey key = new StateActionKey(state_id, action_id);
            return key;
        }

        public override bool Equals(object obj)
        {
            if (obj is StateActionKey)
            {
                StateActionKey rhs = obj as StateActionKey;
                return mUniqueId == rhs.GetHashCode();
            }
            return false;
        }

        public override int GetHashCode()
        {
            return mUniqueId;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", mStateId, mActionId);
        }
    }
}
