using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace TiS.Engineering.TiffDll90
{
  internal static class ILog
    {
      //public delegate void OnMessage(String format, params object[] args);      
      public static TiS.Engineering.TiffDll90.TiffDll_Methods.OnMessage DbgMsg;
      public static TiS.Engineering.TiffDll90.TiffDll_Methods.OnMessage InfoMsg;
      public static TiS.Engineering.TiffDll90.TiffDll_Methods.OnMessage ErrorMsg;
      public static TiS.Engineering.TiffDll90.TiffDll_Methods.OnMessage WarningMsg;



      public static void LogError(Exception ex)
      {
          try
          {
              if (ErrorMsg != null) ErrorMsg("Error [{0}]{1}], method [{2}]", ex.Message, ex.InnerException!=null&& !String.IsNullOrEmpty(ex.InnerException.Message) ? ", inner data: "+ex.InnerException.Message : String.Empty, new StackTrace().GetFrames()[1].GetMethod().Name);
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
