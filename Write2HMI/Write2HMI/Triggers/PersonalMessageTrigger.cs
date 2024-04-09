using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Write2HMI.Triggers
{
    class PersonalMessageTrigger : Trigger
    {
        private DAL dal;
        private List<kmember> list;

  
        #region Overrides of Trigger


        public PersonalMessageTrigger(DAL dal, List<kmember> list)
            : base(dal)
        {
            this.Screen = "personalMessages";  
            this.dal = dal;
            this.list = list;
        }

   

        protected override string ExecuteQuery()
        {

            var newValue = TriggerDal.CheckPersonalMessageSignal(list);
            return newValue;

        }

        #endregion


    }
}