using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Write2HMIService
{
    public partial class Service1 : ServiceBase
    {
        private MainClass mainClass;
        public Service1()
        {
            InitializeComponent();

            if (!Environment.UserInteractive)
            {
                // Startup as service.
            }
            else
            {
                OnStart(null); // Startup as application
            }

        }

       
        protected override void OnStart(string[] args)
        {
            Logger.WriteEventLog("In OnStart.", EventLogEntryType.Information);
            mainClass = new MainClass();
            mainClass.Start();
        }

        protected override void OnStop()
        {
            Logger.WriteEventLog("In onStop.", EventLogEntryType.Information);
        }
        protected override void OnShutdown()
        {
            Logger.WriteEventLog("In OnShut down.", EventLogEntryType.Information);
            base.OnShutdown();
        }
        protected override void OnPause()
        {
            Logger.WriteEventLog("In OnPause.", EventLogEntryType.Information);

            base.OnPause();
        }







    }
}
