using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using TiS.Core.eFlowAPI;
using System.Text.RegularExpressions;

namespace TiS.Engineering.InputApi
{
    #region "CsmManager" class
    /// <summary>
    /// A class that conatins and manages the CSMs to access.
    /// </summary>
#if INTERNAL
    internal class CsmManager : IDisposable
#else
    public class CsmManager : IDisposable
#endif
    {
        #region class variables
       private Dictionary<String, ITisClientServicesModule> csms;
       private Dictionary<String, DateTime> lastAccessed;       
        #endregion

        #region "CsmNames" property
        /// <summary>
        /// The file types that this class contains
        /// </summary>
        [XmlIgnore]
        public virtual String[] CsmNames
        {
            get
            {
                List<String> csmNames = new List<String>();
                if (csms != null)
                {
                    foreach (KeyValuePair<String, ITisClientServicesModule> kvp in csms)
                    {
                        if (kvp.Value != null && kvp.Value.Initialized && kvp.Value.LoggedInApplication) csmNames.Add(kvp.Key);
                    }
                }
                return csmNames.ToArray();
            }
        }
        #endregion

        #region "LastAccessedCsm" property
        /// <summary>
        /// Get the last accessed CSM.
        /// </summary>
        [XmlIgnore]
        public virtual ITisClientServicesModule LastAccessedCsm
        {
            get
            {
                try
                {
                    if (csms == null || csms.Count <= 0) return null;
                    String keyname = null;
                    DateTime tm =DateTime.MinValue;

                    foreach (KeyValuePair<String, DateTime> kvp in lastAccessed)
                    {
                        if (kvp.Value > tm)
                        {
                            tm = kvp.Value;
                            keyname = kvp.Key;
                        }
                    }

                    if (keyname != null)
                    {
                        return csms[keyname];
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                    throw (ex);
                }
                return null;
            }
        }
        #endregion

        #region "CSMs" property
        /// <summary>
        /// All the CSM that this class contains
        /// </summary>
        [XmlIgnore]
        public virtual ITisClientServicesModule[] CSMs
        {
            get
            {
                List<ITisClientServicesModule> fls = new List<ITisClientServicesModule>();
                if (csms != null)
                {
                    foreach (KeyValuePair<String, ITisClientServicesModule> kvp in csms)
                    {
                        fls.Add(kvp.Value);
                    }
                }
                return fls.ToArray();
            }
        }
        #endregion

        #region "CsmCount" property
        /// <summary>
        /// Get the total CSMs in the class.
        /// </summary>
        [XmlIgnore]
        public virtual int CsmCount
        {
            get  { return   csms!=null ? csms.Count : 0; }
        }
        #endregion

        #region "MaxCsm" property
        /// <summary>
        /// Get set, how many CSMs can be opened in this 'CsmManager' instance.
        /// </summary>
        private int maxCsm;
        public virtual int MaxCsm { get { return maxCsm; } set { maxCsm = value; } }
        #endregion

        #region class constructors
        public CsmManager()
        {
        }

        public CsmManager(String appName, String stationName)
        {
            if (!String.IsNullOrEmpty(stationName)) GetCsm(appName, stationName, true);
        }
        #endregion

        #region "AddCsm" method
        /// <summary>
        /// Add a file\s to the list of files index by extension.
        /// </summary>
        /// <param name="csm">the CSM to add</param>
        /// <param name="logoutExistingMatchingCSM">When true logout already existing CSM (in the CsmManager) with the same login crdentials when true.</param>
        public virtual void AddCsm(ITisClientServicesModule csm, bool logoutExistingMatchingCSM)
        {
            try
            {
                if (csm == null) return;

                String keyname = GetCsmKey(csm);

                if (csms == null) csms = new Dictionary<String, ITisClientServicesModule>();
                else if (csms.ContainsKey(keyname))
                {
                    if (logoutExistingMatchingCSM)
                    {
                        if (csms[keyname] != null)
                        {
                            try
                            {
                                csms[keyname].Dynamic.FreeCollections(true, false);
                                csms[keyname].LogoutApplication();
                                csms[keyname] = null;
                            }
                            catch (Exception et)
                            {
                                ILog.LogError(et);
                            }
                        }
                    }
                    csms[keyname] = csm;
                }
                else
                {
                    //-- Remove old CSMs if there is a limit on CSM count --\\
                    while (MaxCsm > 0 && csms.Count + 1 >= MaxCsm) RemoveLeastAccessedCsm();
                    csms.Add(keyname,csm);
                }

                if (lastAccessed == null) lastAccessed = new Dictionary<String, DateTime>();
                else if (lastAccessed.ContainsKey(keyname)) lastAccessed[keyname] = DateTime.Now;
                else lastAccessed.Add(keyname, DateTime.Now);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
                throw (ex);
            }
        }        
        #endregion

        #region "RemoveLeastAccessedCsm" method
        /// <summary>
        /// Remove the last oldest CSM in the list (least accessed).
        /// </summary>
        public virtual bool RemoveLeastAccessedCsm()
        {
            try
            {
                if (csms==null || csms.Count<=0) return true;
                String keyname = null;
                DateTime tm=DateTime.Now;

                foreach (KeyValuePair<String, DateTime> kvp in lastAccessed)
                {
                    if (kvp.Value > tm)
                    {
                        tm = kvp.Value;
                        keyname = kvp.Key;
                    }
                }

                if (keyname != null)
                {
                    RemoveCsms(true, keyname);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
                throw (ex);
            }
            return false;
        }
        #endregion

        #region "GetCsmKey" functions
        /// <summary>
        /// Get the String that will represent this CSM key. 
        /// </summary>
        /// <param name="csm">ITisClientServicesModule</param>
        /// <returns>The key being used to index this CSM instance.</returns>
        protected virtual String GetCsmKey(ITisClientServicesModule csm)
        {
            try
            {
                return String.Format("{0}_{1}", csm.Application.AppName.ToUpper(), csm.Session.StationName.ToUpper());
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return String.Empty;
        }

        /// <summary>
        /// Get the String that will represent this Station login key.
        /// </summary>
        /// <param name="appName">The name of the application.</param>
        /// <param name="stationName">The name of the station</param>
        /// <returns>The formatted key.</returns>
        protected virtual String GetCsmKey(String appName, String stationName)
        {
            try
            {
                return String.Format("{0}_{1}", (appName ?? String.Empty).ToUpper(), (stationName ?? String.Empty).ToUpper());
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return String.Empty;
        } 
        #endregion

        #region "GetCsm" function
        /// <summary>
        /// Get a CSM object based on the specified login credentials.
        /// </summary>
        /// <param name="appName">The eFlow application name.</param>
        /// <param name="stationName">The station that this CSM has to be logged to.</param>
        /// <returns>ITisClientServicesModule when successfull, null when failed.</returns>
        public virtual ITisClientServicesModule GetCsm(String appName, String stationName, bool canCreate)
        {
            ITisClientServicesModule res = null;
            try
            {
                //-- Create a key name --\\
                String keyname = GetCsmKey(appName, stationName);

                //-- Find in existing list --\\
                if (csms != null)
                {
                    if (csms.ContainsKey(keyname))
                    {
                        res = csms[keyname];
                        canCreate = false;
                    }
                }

                //-- not foudn, create one if we can --\\
                if (canCreate && res == null)
                {
                    res = TisClientServicesModule.GetNewInstance(appName, stationName);
                    keyname = GetCsmKey(res);//-- update --\\
                }


                if (res != null)
                {
                    //-- Update or set CSM result --\\
                    if (csms == null) csms = new Dictionary<String, ITisClientServicesModule>();
                    
                    //-- Update Last accessed time --\\
                    if (!csms.ContainsKey(keyname)) csms.Add(keyname, res);
                    if (lastAccessed == null)
                    {
                        lastAccessed = new Dictionary<String, DateTime>();
                        lastAccessed.Add(keyname, DateTime.Now);
                    }
                    else if (lastAccessed.ContainsKey(keyname)) lastAccessed[keyname] = DateTime.Now;
                    else lastAccessed.Add(keyname, DateTime.Now);
                }
                else
                {
                    if (lastAccessed != null && !String.IsNullOrEmpty(keyname) && lastAccessed.ContainsKey(keyname)) lastAccessed.Remove(keyname);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
                throw (ex);
            }
            finally
            {
                if (res != null)
                {
                    //-- Hook to the CSM OnMessage event --\\
                    res.Session.OnMessage -= StationMessage;
                    res.Session.OnMessage += StationMessage;
                }

                //-- Freee and release orphaned csms --\\
                if (csms != null && res != null && !csms.ContainsKey(GetCsmKey(res)))
                {
                    try
                    {
                        res.LogoutApplication();
                        res.Dispose();
                        res = null;
                    }
                    catch (Exception ev)
                    {
                        ILog.LogError(ev);
                        throw ev;
                    }
                }
            }
            return res;
        }
        #endregion

        #region "StationMessage" event
        /// <summary>
        /// Receive and process station messages.
        /// </summary>
        /// <param name="message"></param>
        private void StationMessage(String message)
        {
            try
            {
                ILog.LogInfo("Received eflow system message: [{0}]", message);

                if (Regex.IsMatch(message, "(?i)(Stop|Disable|End|Exit|Close|Break|log(out|off?)|Terminate|Abort)"))
                {
                    //-- Stop and logoutr CSMs --\\
                    this.RemoveCsms(true, this.CsmNames);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        }
        #endregion

        #region "RemoveCsm" function
        /// <summary>
        /// Remove the specified CSM names.
        /// </summary>
        /// <param name="canLogout"></param>
        /// <param name="csmNames">The csm name\s to remove</param>
        public virtual void RemoveCsms(bool canLogout, params String[] csmNames)
        {
            try
            {
                if (csms == null || csms.Count <= 0 || csmNames == null || csmNames.Length <= 0) return;

                foreach (String csmName in csmNames)
                {
                   String keyname = csmName.ToUpper();
                   
                       if (csms.ContainsKey(keyname)) try
                        {
                            if (canLogout && csms[keyname] != null)
                           {
                               csms[keyname].Dynamic.FreeCollections(true,false);
                               csms[keyname].LogoutApplication();
                               csms[keyname].Dispose();
                               csms[keyname]=null;
                           }
                        }
                        catch (Exception ed)
                        {
                            ILog.LogError(ed);
                        }
                        csms.Remove(keyname);
                     if (csms.Count <= 0) return;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
                throw (ex);
            }
        }
        #endregion

        #region "Dispose" method
        /// <summary>
        /// dispose used objects
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (csms != null)
                {
                    this.RemoveCsms(true, this.CsmNames);
                    csms.Clear();
                    csms = null;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
                throw (ex);
            }
        }
        #endregion
    }
    #endregion            
}
