using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Kneset_terminal
{
    public static class Logger
    {


        const string sLog = "AgendaApp";
        const string sSource = "AgendaApp";
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
