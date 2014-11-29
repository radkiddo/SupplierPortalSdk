#region "about"

//
// eFLOW Supplier Portal SDK
// 2013 (c) - Top Image Systems (a project initiated by the UK branch)
//
// The purpose of this SDK is to communicate with the Supplier Portal for eFlow Invoices.
// Developed by: Eduardo Freitas
//

#endregion "about"

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace eFlow.SupplierPortalCore
{
    public class Logging
    {
        private static EventLog log = null;

        public static void CreateLog()
        {
            if (!EventLog.SourceExists(Constants.cStrLoggerName))
            {
                EventLog.CreateEventSource(Constants.cStrLoggerName, Constants.cStrLoggerName);
            }
        }

        public static void InfoLog(string str)
        {
            using (log = new EventLog())
            {
                log.Source = Constants.cStrLoggerName;

                try
                {
                    log.WriteEntry(str);
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("full"))
                    {
                        log.Clear();
                        log.WriteEntry(str);
                    }
                }
            }
        }

        public static void WriteLog(string str)
        {
            using (log = new EventLog())
            {
                log.Source = Constants.cStrLoggerName;

                try
                {
                    log.WriteEntry(str, EventLogEntryType.Error);
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("full"))
                    {
                        log.Clear();
                        log.WriteEntry(str);
                    }
                }
            }
        }
    }
}
