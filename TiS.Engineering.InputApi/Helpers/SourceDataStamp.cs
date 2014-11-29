using System;
using System.Collections.Generic;
using System.Text;
using TiS.Core.eFlowAPI;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace TiS.Engineering.InputApi
{
    internal  class SourceDataStamp
    {
        #region class variables.
        /// <summary>
        /// The dll stamp data without any specifc dll (to be added to a calling dll name).
        /// </summary>
        private static readonly string PartDllStampData = String.Join(", ", PartCreateStampStationDllData().ToArray());

        private static bool disabled;
        #endregion

        #region "Disabled" property
        /// <summary>
        /// Disable the Stamp dll method when true (i.e. don't stamp collections).
        /// </summary>
        [Description("Disable the Stamp dll method when true (i.e. don't stamp collections).")]
        public static bool Disabled
        {
            get { return disabled; }
            set { disabled = value; }
        }
        #endregion

        #region "PartCreateStampStationDllData" function
        private static List<String> PartCreateStampStationDllData()
        {
            //-- Create static sata for dll stamp --\\
            List<String> result = new List<String>();
            try
            {
                //-- Add environment data --\\
                result.Add("User Name=" + Environment.UserName);
                result.Add("Machine Name=" + System.Net.Dns.GetHostName());
                result.Add("OS=" + Environment.OSVersion.Platform + ":" + Environment.OSVersion.Version);

                //-- Get eFlow registry version --\\
                Microsoft.Win32.RegistryKey tisKey = Microsoft.Win32.Registry.LocalMachine;

                try
                {
                    tisKey = tisKey.OpenSubKey(@"Software\TopImageSystems\eFlow 4.5\");
                }
                catch { }

                if (tisKey == null)
                {
                    try
                    {
                        tisKey = tisKey.OpenSubKey(@"Software\TopImageSystems\eFlow 4\");
                    }
                    catch { }
                }

                if (tisKey != null)
                {
                    result.Add("Eflow version=" + tisKey.GetValue("Version"));
                    result.Add("Eflow SetupType=" + tisKey.GetValue("SetupType"));
                }

                result.Add("Command Params=" + string.Join(" ", Environment.GetCommandLineArgs()).Replace(Application.ExecutablePath, string.Empty).Trim());
                result.Add("Executable=" + Path.GetFileNameWithoutExtension(Application.ExecutablePath));

            }
            catch { }

            return result;
        }
        #endregion

        #region "GetCallingDll" function
        internal static String GetCallingDll(int frameCount)
        {
            String result = String.Empty;
            try
            {
                result = new StackTrace().GetFrames()[frameCount].GetMethod().Module.Assembly.FullName;
                Match mtc = Regex.Match(result, @"(?i)\w+,? ?\w+=\d+.\d+.\d+.\d+");
                if (mtc.Success) result = mtc.Value;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }

            return result;
        }
        #endregion

        #region "StampData" property
        public static String StampData
        {
            get { return GetCallingDll(2) + ", " + PartDllStampData; }
        } 
        #endregion

        #region "StampStationDll" functions
        /// <summary>
        /// Stamp the specified collection with the Custom Dll version..
        /// </summary>
        /// <param name="csm"></param>
        /// <param name="collection">The collection to stamp.</param>
        /// <param name="getCallingDllversionData">A flag to get the dll version data of the dll that called this function when true.</param>
        /// <returns>A string with the DLL version.</returns>
        public static string StampStationDll(ITisClientServicesModule csm, ITisCollectionData collection)
        {
            return StampStationDll(csm, collection, GetCallingDll(2) + ", " + PartDllStampData);
        }

        /// <summary>
        /// Stamp the specified collection with the CustomDll version..
        /// </summary>
        ///<param name="csm"></param>
        /// <param name="collection">The collection to stamp.</param>
        /// <param name="staticData">Data that is stored statically.</param>
        /// <returns>A string with the DLL version.</returns>
        public static string StampStationDll(ITisClientServicesModule csm, ITisCollectionData oColl, string staticData)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                if (!Disabled && oColl != null)
                {
                    //-- Get previous data --\\
                    string appName = csm != null ? csm.Session.StationName : Path.GetFileNameWithoutExtension(Application.ExecutablePath);

                    result.Append(oColl.get_NamedUserTags("DLL_VERSION_" + appName));
                    if (result.Length > 0) result.Append("\r\n");

                    //-- Add environment data --\\
                    result.Append(staticData ?? string.Empty);

                    result.Append(", Time=" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                    //-- Set DLL name and version --\\s
                    oColl.set_NamedUserTags("DLL_VERSION_" + appName, result.ToString());
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }

            return result.ToString();
        } 
        #endregion

    }
}
