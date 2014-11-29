using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using TiS.Core.eFlowAPI;
using System.Text.RegularExpressions;

namespace TiS.Engineering.InputApi
{
    #region "ILog" class code
    /// <summary>
    /// A class that contains  basic Logging metods to the eFlow logger.
    /// </summary>
    internal class ILog
    {
        /// <summary>
        /// TiS logger object for debug anf all messaging.
        /// </summary>
        private static TiS.Core.eFlowAPI.TisSimpleLogger _logger = new TiS.Core.eFlowAPI.TisSimpleLogger();

        /// <summary>
        /// Call source.
        /// </summary>
        private static readonly String _src = Path.GetFileNameWithoutExtension(Application.ExecutablePath);

        /// <summary>
        /// A delegate function that will be used in the OnLog event.
        /// </summary>
        /// <param name="message">The message that is being displayed.</param>
        /// <param name="severity">The message severirt.</param>
        public delegate void OnLogEvent(String message, TIS_SEVERITY severity);

        /// <summary>
        /// The OnLog event that the ILog class calls every time it is being used.
        /// </summary>
        public static event OnLogEvent OnLog;

        #region "ThrowExceptions" property
        /// <summary>
        /// Log and then throw all exceptions when set to true.
        /// </summary>
        private static bool throwExceptions;
        public static bool ThrowExceptions { get { return throwExceptions; } set { throwExceptions = value; } }
        #endregion

        #region class ctor'
        public ILog()
        {
            ThrowExceptions = Regex.IsMatch(Environment.CommandLine, "Throw.?(All.?)?Exceptions?", RegexOptions.IgnoreCase);
        }
        #endregion

        #region log messages methods
        /// <summary>
        /// Log an error message to the eFlow logger.
        /// </summary>
        /// <param name="ex">the exception to log it's info'.</param>
        /// <param name="message">Additional message.</param>
        public static void LogError(Exception ex, String message)
        {
            if (ex != null)
            {
                try
                {
                    LogMsg(String.Format(message + " {0}\r\n{1}, {2}, {3}, {4}.", ex.ToString(), ex.Message, ex.Source.ToString(), ex.StackTrace.ToString(), ex.TargetSite), TIS_SEVERITY.TIS_ERROR);
                }
                catch { }
            }
        }

        /// <summary>
        /// Log an error message to the eFlow logger.
        /// </summary>
        /// <param name="ex">the exception to log it's info'.</param>
        /// <param name="message">Additional message.</param>
        /// <param name="canThrowException">After logging the exception throw it when set to true..</param>
        public static void LogError(Exception ex, String message, bool canThrowException)
        {
            if (ex != null)
            {
                try
                {
                    LogMsg(String.Format(message + " {0}\r\n{1}, {2}, {3}, {4}.", ex.ToString(), ex.Message, ex.Source.ToString(), ex.StackTrace.ToString(), ex.TargetSite), TIS_SEVERITY.TIS_ERROR);
                }
                catch { }
            }
            if (canThrowException) throw ex;
        }

        /// <summary>
        /// Log an error message to the eFlow logger.
        /// </summary>
        /// <param name="ex">the exception to log it's info'.</param>
        public static void LogError(Exception ex)
        {
            String callee = String.Empty;
            try { callee = "Error in: " + new StackTrace().GetFrames()[1].GetMethod().Name; }
            catch { }
            LogError(ex, callee);
        }

        /// <summary>
        /// Log an error message to the eFlow logger.
        /// </summary>
        /// <param name="ex">the exception to log it's info'.</param>
        /// <param name="canThrowException">After logging the exception throw it when set to true..</param>
        public static void LogError(Exception ex, bool canThrowException)
        {
            String callee = String.Empty;
            try { callee = "Error in: " + new StackTrace().GetFrames()[1].GetMethod().Name; }
            catch { }
            LogError(ex, callee);
            if (canThrowException) throw ex;
        }

        /// <summary>
        /// Log an error message to the eFlow logger.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogError(String message)
        {
            LogMsg(message, TIS_SEVERITY.TIS_ERROR);
        }

        /// <summary>
        /// Log an info error to the eFlow logger.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="StringFormat">The Strings to format</param>
        public static void LogError(String msg, params object[] stringFormat)
        {
            try
            {
                if (stringFormat != null)
                    LogMsg(String.Format(msg ?? String.Empty, stringFormat), TIS_SEVERITY.TIS_ERROR);
                else
                    LogMsg(msg ?? String.Empty, TIS_SEVERITY.TIS_ERROR);
            }
            catch { }
        }

        /// <summary>
        /// Log a warning message to the eFlow logger.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogWarning(String message)
        {
            LogMsg(message, TIS_SEVERITY.TIS_WARNING);
        }

        /// <summary>
        /// Log a warning message to the eFlow logger.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="stringFormat">The strings to format</param>
        public static void LogWarning(String msg, params object[] stringFormat)
        {
            try
            {
                if (stringFormat != null)
                    LogMsg(String.Format(msg ?? String.Empty, stringFormat), TIS_SEVERITY.TIS_WARNING);
                else
                    LogMsg(msg ?? String.Empty, TIS_SEVERITY.TIS_WARNING);
            }
            catch { }
        }


        /// <summary>
        /// Log an info message to the eFlow logger.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogInfo(String message)
        {
            LogMsg(message, TIS_SEVERITY.TIS_INFORMATORY);
        }

        /// <summary>
        /// Log an info message to the eFlow logger.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="stringFormat">The strings to format</param>
        public static void LogInfo(String msg, params object[] stringFormat)
        {
            try
            {
                if (stringFormat != null)
                    LogMsg(String.Format(msg ?? String.Empty, stringFormat), TIS_SEVERITY.TIS_INFORMATORY);
                else
                    LogMsg(msg ?? String.Empty, TIS_SEVERITY.TIS_INFORMATORY);
            }
            catch { }
        }


        /// <summary>
        /// Log a debugmessage to the eFlow logger.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogDebug(String message)
        {
            LogMsg(message, TIS_SEVERITY.TIS_DEBUG);
        }

        /// <summary>
        /// Log a debug message to the eFlow logger.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="stringFormat">The strings to format</param>
        public static void LogDebug(String msg, params object[] stringFormat)
        {
            try
            {
                if (stringFormat != null)
                    LogMsg(String.Format(msg ?? String.Empty, stringFormat), TIS_SEVERITY.TIS_DEBUG);
                else
                    LogMsg(msg ?? String.Empty, TIS_SEVERITY.TIS_DEBUG);
            }
            catch { }
        }

        /// <summary>
        /// Log a message to the eFlow logger.
        /// </summary>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="msg">The message to log.</param>
        /// <param name="stringFormat">The strings to format</param>
        public static void LogMsg(TiS.Core.eFlowAPI.TIS_SEVERITY severity, String msg, params object[] stringFormat)
        {
            try
            {
                if (stringFormat != null)
                    LogMsg(String.Format(msg ?? String.Empty, stringFormat), severity);
                else
                    LogMsg(msg ?? String.Empty, severity);
            }
            catch { }
        }


        /// <summary>
        /// Log a message to the eFlow logger.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="severity">The severity of the message.</param>
        public static void LogMsg(String message, TiS.Core.eFlowAPI.TIS_SEVERITY severity)
        {
            if (OnLog != null) OnLog(message, severity);

            if (message != null && _logger != null)
            {
                String smsg = String.Format("[[{0}]] {1}", _src, message);

                _logger.RequestMessageLog(smsg, Path.GetFileNameWithoutExtension(Application.ExecutablePath), severity, 0, 0);
#if  DEBUG
                System.Diagnostics.Debug.WriteLine(smsg + ".  " + severity.ToString() + " - " + Path.GetFileNameWithoutExtension(Application.ExecutablePath));
#endif
            }
        }
        #endregion log messages methods
    }
    #endregion "ILog" class code
}
