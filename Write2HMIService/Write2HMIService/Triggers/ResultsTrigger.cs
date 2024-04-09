using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Write2HMIService.Triggers
{
    class ResultsTrigger : Trigger
    {
        #region Overrides of Trigger

        public ResultsTrigger(DAL dal)
            : base(dal)
      {
          this.Screen = "results";
      }
       

        protected override string ExecuteQuery()
        {

            var newValue = TriggerDal.CheckResultsSignal();
            return newValue;
        }

        #endregion


    }
}
