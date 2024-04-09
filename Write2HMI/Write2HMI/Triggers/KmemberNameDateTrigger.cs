using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Write2HMI.Triggers
{
    class KmemberNameDateTrigger : Trigger
    {
        #region Overrides of Trigger

        public KmemberNameDateTrigger
          (DAL dal)
            : base(dal)
        {
            this.Screen = "KmemberNameDate";
            indicator = 5;
        }

        private int indicator;
        public override bool SignalChanged()
        {



            if (indicator == 5)
            {
                indicator = 0;
                return true;
            }
            else
            {
                indicator++;
                return false;
            }
        }
        protected override string ExecuteQuery()
        {
            return null;
        }

        #endregion


    }
}
