using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Write2HMIService.Triggers
{
      class SpeakersTrigger : Trigger
    {
        #region Overrides of Trigger

          public SpeakersTrigger(DAL dal)
              : base(dal)             
      {
          this.Screen = "speakers";
      }
       

        protected override string ExecuteQuery()
        {

            var newValue = TriggerDal.CheckSpeakersSignal();
            return newValue;
        }

        #endregion


    }
}
