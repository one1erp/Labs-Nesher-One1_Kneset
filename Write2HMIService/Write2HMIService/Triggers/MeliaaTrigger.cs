namespace Write2HMIService.Triggers
{
   class MeliaaTrigger : Trigger
    {
     //  private DAL dal;

        #region Overrides of Trigger

      public MeliaaTrigger(DAL dal):base( dal)
      {
          this.Screen = "maliaa";
      }

 
       

        protected override string ExecuteQuery()
        {

            var newValue = TriggerDal.CheckMaliaaSignal();
            return newValue;
        }

        #endregion


    }
}