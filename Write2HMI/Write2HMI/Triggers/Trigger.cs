using System.Collections.Generic;
using System.Windows.Forms;

namespace Write2HMI.Triggers
{
    public abstract class Trigger
    {
        public string Screen { get; set; }
        //    public List<string> Screens2Refresh { get; set; }
        public string LastValue { get; set; }

        protected Trigger(DAL triggerDal)
        {
            this.TriggerDal = triggerDal;
            LastValue = "";
          
        }
        //מפעיל את השאילתא שבודקת האם השתנה הדגל
        public virtual bool SignalChanged()
        {
            try
            {

                string newValue = this.ExecuteQuery();
            
                if (newValue != LastValue)
                {
                    LastValue = newValue;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Logger.WriteEventLog(e.Message + " " + e.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }

        }
        protected abstract string ExecuteQuery();

        public DAL TriggerDal { get; set; }



    }
}
