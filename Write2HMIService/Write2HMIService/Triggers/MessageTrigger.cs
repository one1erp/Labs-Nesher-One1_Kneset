namespace Write2HMIService.Triggers
{
    class MessageTrigger : Trigger
    {
        #region Overrides of Trigger


        public MessageTrigger(DAL dal):base( dal)
        {
            this.Screen = "messages";
                
        }

        protected override string ExecuteQuery()
        {

            var newValue = TriggerDal.CheckMessageSignal();
            return newValue;

        }

        #endregion


    }
}