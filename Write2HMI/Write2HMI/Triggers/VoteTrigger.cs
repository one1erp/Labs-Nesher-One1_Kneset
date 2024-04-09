using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Write2HMI.Triggers
{
    public class VoteTrigger : Trigger
    {
        #region Overrides of Trigger

        public VoteTrigger(DAL dal)
            : base(dal)          
        {
            this.Screen = "vote";
        }


        protected override string ExecuteQuery()
        {

            var newValue = TriggerDal.CheckVoteSignal();
            return newValue;
        }

        #endregion


    }
}
