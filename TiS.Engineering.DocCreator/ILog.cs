using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;

namespace TiS.Engineering.DocCreator
{
    [XmlType(Namespace = Convert.DEF_NAMESPACE_DOCCREATOR)]
    internal static class ILog
    {
      public static TiS.Engineering.DocCreator.Convert.OnMessage DbgMsg;
      public static TiS.Engineering.DocCreator.Convert.OnMessage InfoMsg;
      public static TiS.Engineering.DocCreator.Convert.OnMessage ErrorMsg;
      public static TiS.Engineering.DocCreator.Convert.OnMessage WarningMsg;

      public static event TiS.Engineering.DocCreator.Convert.OnMessage AllMsg
      {
          add
          {
              DbgMsg += value;
              InfoMsg += value;
              ErrorMsg += value;
              WarningMsg += value;
          }

          remove
          {
              DbgMsg -= value;
              InfoMsg -= value;
              ErrorMsg -= value;
              WarningMsg -= value;
          }
      }

      public static void LogError(Exception ex)
      {
          try
          {
              if (ErrorMsg != null) ErrorMsg("Error [{0}]{1}], method [{2}]", ex.Message, !String.IsNullOrEmpty(ex.InnerException.Message) ? ", inner data: "+ex.InnerException.Message : String.Empty, new StackTrace().GetFrames()[1].GetMethod().Name);
          }
          catch { }
      }

      public static void LogError(String format, params object[] args)
      {
          try
          {
              if (ErrorMsg != null) ErrorMsg(format, args);
          }
          catch { }
      }

      public static void LogDebug(String format, params object[] args)
      {
          try
          {
              if (DbgMsg != null) DbgMsg(format, args);
          }
          catch { }
      }

      public static void LogWarning(String format, params object[] args)
      {
          try
          {
              if (WarningMsg != null) WarningMsg(format, args);
          }
          catch { }
      }

      public static void LogInfo(String format, params object[] args)
      {
          try
          {
              if (InfoMsg != null) InfoMsg(format, args);
          }
          catch { }
      }
    }
}
