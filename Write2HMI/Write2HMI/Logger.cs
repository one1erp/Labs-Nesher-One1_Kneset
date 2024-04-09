using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Write2HMI
{
    public class Logger
    {

        //windows log event properties 
        const string sSource = "WinGPWriter";
        const string sLog = "WinGPWriter";
        public static void WriteEventLog(string sEvent, EventLogEntryType level)
        {

            if (!EventLog.SourceExists(sSource))
            {
                EventLog.CreateEventSource(sSource, sLog);
            }

            EventLog.WriteEntry(sSource, sEvent, level, 0);
        }
    }
}
