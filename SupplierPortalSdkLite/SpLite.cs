#region "about"

//
// eFLOW Supplier Portal SDK
// 2013 (c) - Top Image Systems (a project initiated by the UK branch)
//
// The purpose of this SDK is to communicate with the Supplier Portal for eFlow Invoices.
// Developed by: Eduardo Freitas
//

#endregion "about"

#region "using"

using System;
using System.Collections.Generic;
using System.Text;

using eFlow.SupplierPortalCore;
using eFlow.CollectionManagement;
using SupplierPortalCommon;

using System.IO;
using System.Reflection;
using System.Drawing;

using TiS.Core.Application;
using TiS.Core.Common;
using TiS.Core.Domain;
using TiS.Core.TisCommon;
using TiS.Core.Application.DataModel.Dynamic;
using TiS.Core.Application.Interfaces;

using TiS.Engineering.InputApi;

using TiS.Engineering.TiffDll90;

using System.Data;
using System.Drawing.Imaging;
using System.Data.SqlClient;
using Microsoft.Win32;

#endregion "using"

#region "SupplierPortalSdk Lite NS"

namespace eFlow.SupplierPortalLite
{
    #region "SpLite Class"

    /// <summary>
    /// "SpLite" Class --> Supplier Portal 'Lite' SDK.
    /// Easy-to-use and simple way to interact with the Supplier Portal. 
    /// Wraps up all the complexity of the Supplier Portal Core SDK.   
    /// </summary>
    public class SpLite : IDisposable
    {
        #region "Private declarations"

        /// <summary>
        /// When set to true indicates that the instance of the class has been disposed
        /// </summary>
        protected bool disposed;

        /// <summary>
        /// CSM object instance
        /// </summary>
        protected ITisClientServicesModule csm = null;

        #endregion "Private declarations"

        #region "Constructor-Finalizer-Dispose"

        #region "Constructor"
        /// <summary>
        /// [Constructor] SpLite() --> Initializes an SpLite object instance.
        /// </summary>
        /// <example><code>SpLite s = new SpLite();</code></example>
        public SpLite()
        {
        }
        #endregion "Constructor"

        #region "Destructor / Finalizers"

        #region "Destructor"
        /// <summary>
        /// [Destructor] SpLite() --> Releases unmanaged resources and performs other cleanup operations before the is reclaimed by garbage collection.
        /// </summary>
        ~SpLite()
        {
            // Our finalizer should call our Dispose(bool) method with false
            this.Dispose(false);
        }
        #endregion "Destructor"

        #region "protected Dispose"
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        /// <remarks>
        /// If the main class was marked as sealed, we could just make this a private void Dispose(bool).  Alternatively, we could (in this case) put
        /// all of our logic directly in Dispose().
        /// </remarks>
        public virtual void Dispose(bool disposing)
        {
            // Use our disposed flag to allow us to call this method multiple times safely.
            // This is a requirement when implementing IDisposable
            if (!this.disposed)
            {
                if (disposing)
                {
                    // If we have any managed, IDisposable resources, Dispose of them here.
                    // In this case, we don't, so this was unneeded.
                    // Later, we will subclass this class, and use this section.


                }

                // Always dispose of undisposed unmanaged resources in Dispose(bool)

            }
            // Mark us as disposed, to prevent multiple calls to dispose from having an effect, 
            // and to allow us to handle ObjectDisposedException
            this.disposed = true;
        }
        #endregion "protected Dispose"

        #region "Dispose"
        /// <summary>
        /// Dispose() --> Performs SpLite defined tasks associated with freeing, releasing, or resetting managed and unmanaged resources.
        /// </summary>
        /// <example><code>s.Dispose();</code></example>
        public void Dispose()
        {
            // We start by calling Dispose(bool) with true
            this.Dispose(true);

            // Now suppress finalization for this object, since we've already handled our resource cleanup tasks
            GC.SuppressFinalize(this);
        }
        #endregion "Dispose"

        #endregion "Destructor / Finalizers"

        #endregion "Constructor-Finalizer-Dispose"

        #region "Exposed/Public methods"

        #region "CreateCollectionFromImportFolder"
        /// <summary>
        /// CreateCollectionFromImportFolder() --> Creates an eFlow collection from data collected from the import folder 
        /// (retrieved by the Supplier Portal Service). Returns the name of the created collection
        /// </summary>
        /// <param name="eFlowAppName">The name of the eFlow App</param>
        /// <param name="csm">The name of the CSM object</param>
        /// <param name="nextStation">The name of next station where the collections will be sent to</param>
        /// <example><code>string collectionName = CreateCollectionFromImportFolder("IRSupplierPortal", csm, "PageOCR");</code></example>
        public string CreateCollectionFromImportFolder(string eFlowAppName, ITisClientServicesModule csm, string nextStation)
        {
            string collectionName = String.Empty;
            
            try
            {
                List<string> apps = new List<string>();
                List<string> iFolders = new List<string>();

                apps.Add(Common.GetSetting(CommonConst.app01WhoCanSeeInPortal));
                apps.Add(Common.GetSetting(CommonConst.app02WhoCanSeeInPortal));
                apps.Add(Common.GetSetting(CommonConst.app03WhoCanSeeInPortal));
                apps.Add(Common.GetSetting(CommonConst.app04WhoCanSeeInPortal));
                apps.Add(Common.GetSetting(CommonConst.app05WhoCanSeeInPortal));
                apps.Add(Common.GetSetting(CommonConst.app06WhoCanSeeInPortal));
                apps.Add(Common.GetSetting(CommonConst.app07WhoCanSeeInPortal));
                apps.Add(Common.GetSetting(CommonConst.app08WhoCanSeeInPortal));
                apps.Add(Common.GetSetting(CommonConst.app09WhoCanSeeInPortal));
                apps.Add(Common.GetSetting(CommonConst.app10WhoCanSeeInPortal));

                iFolders.Add(Common.GetSetting(CommonConst.app01ImportPath));
                iFolders.Add(Common.GetSetting(CommonConst.app02ImportPath));
                iFolders.Add(Common.GetSetting(CommonConst.app03ImportPath));
                iFolders.Add(Common.GetSetting(CommonConst.app04ImportPath));
                iFolders.Add(Common.GetSetting(CommonConst.app05ImportPath));
                iFolders.Add(Common.GetSetting(CommonConst.app06ImportPath));
                iFolders.Add(Common.GetSetting(CommonConst.app07ImportPath));
                iFolders.Add(Common.GetSetting(CommonConst.app08ImportPath));
                iFolders.Add(Common.GetSetting(CommonConst.app09ImportPath));
                iFolders.Add(Common.GetSetting(CommonConst.app10ImportPath));

                string importFolder = String.Empty;
                string[] folders = iFolders.ToArray();

                int index = 0;
                foreach (string app in apps)
                {
                    if (app.ToLower().Contains(eFlowAppName.ToLower()))
                    {
                        importFolder = folders[index];
                        break;
                    }

                    index++;
                }

                if (importFolder != String.Empty && Directory.Exists(importFolder))
                {
                    CCConfiguration.CCConfigurationData UploadProfilePdf = new CCConfiguration.CCConfigurationData();
                    UploadProfilePdf.SearchExtensions = new string[] { "pdf" };
                    UploadProfilePdf.SearchPaths = new string[] { importFolder };
                    UploadProfilePdf.FlowType = csm.Setup.GetFlowByIndex(0).Name;
                    UploadProfilePdf.MaxFilesLock = 10;
                    UploadProfilePdf.ErrorFolderPath = Path.Combine(importFolder, "Error");

                    CCConfiguration.CCConfigurationData UploadProfileTiff = new CCConfiguration.CCConfigurationData();
                    UploadProfileTiff.SearchExtensions = new string[] { "tif" };
                    UploadProfileTiff.SearchPaths = new string[] { importFolder };
                    UploadProfileTiff.FlowType = csm.Setup.GetFlowByIndex(0).Name;
                    UploadProfileTiff.MaxFilesLock = 10;
                    UploadProfileTiff.ErrorFolderPath = Path.Combine(importFolder, "Error");

                    //-- Search PDF files and convert them to tiff --\\
                    SearchAndConvertPdfToTiff(csm, UploadProfilePdf, UploadProfileTiff, nextStation);

                    //-- Search image files and convert them to tiff CCITT4 300 Dpi --\\
                    SearchAndConvertImagesToTiffCCITT4(csm, UploadProfileTiff, nextStation);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return collectionName;
        }
        #endregion "CreateCollectionFromImportFolder"

        #region "RemoveDataFromPortal"

        /// <summary>
        /// RemoveDataFromPortal() --> Removes the collection data from the Supplier Portal for 'eFlowCollectionName'. 
        /// The collection can be 'Locked' on eFlow. 
        /// </summary>
        /// <param name="eFlowAppName">The name of the eFlow application</param>
        /// <param name="eFlowCollectionName">The name of the eFlow collection</param>
        /// <param name="customerDomain">The name of the customer assigned to this eFlowAppName</param>
        /// <example><code>s.RemoveDataFromPortal("CLS", "00000323", "speedyservices.com");</code></example>
        public string RemoveDataFromPortal(string eFlowAppName, string eFlowCollectionName, string customerDomain)
        {
            string result = String.Empty;
            
            try
            {
                result = RemoveDataFromPortalCore(eFlowAppName, eFlowCollectionName, customerDomain);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        #endregion "RemoveDataFromPortal"

        #region "GetDataFromPortal"

        /// <summary>
        /// GetDataFromPortal() --> Gets collection data from the Supplier Portal for 'eFlowAppName', 
        /// 'eFlowStationName' and 'eFlowCollectionName'. The collection must not 'Locked' on eFlow. 
        /// </summary>
        /// <param name="eFlowAppName">The name of the eFlow application</param>
        /// <param name="eFlowStationName">The name of the eFlow station</param>
        /// <param name="eFlowCollectionName">The name of the eFlow collection</param>
        /// <param name="customerDomain">The name of the customer assigned to this eFlowAppName</param>
        /// <example><code>s.GetDataFromPortal("CLS", "Completion", "00000323", "speedyservices.com");</code></example>
        public bool GetDataFromPortal(string eFlowAppName, string eFlowStationName, string eFlowCollectionName,
            string customerDomain)
        {
            bool changed = false;
            
            try
            {
                using (Batch b = new Batch(eFlowAppName, eFlowStationName))
                {
                    ITisCollectionData cd = b.Get(eFlowCollectionName);

                    changed = GetDataFromPortalCore(ref cd, eFlowAppName, eFlowStationName, eFlowCollectionName, customerDomain);

                    if (changed)
                        b.Put(cd);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return changed;
        }

        /// <summary>
        /// GetDataFromPortal() --> Gets collection data from the Supplier Portal for 'eFlowAppName', 
        /// 'eFlowStationName' and 'eFlowCollectionName'. The collection can be 'Locked' on eFlow. 
        /// </summary>
        /// <param name="cd">The ITisCollectionData object which contains the collection data</param>
        /// <param name="eFlowAppName">The name of the eFlow application</param>
        /// <param name="eFlowStationName">The name of the eFlow station</param>
        /// <param name="eFlowCollectionName">The name of the eFlow collection</param>
        /// <param name="customerDomain">The name of the customer assigned to this eFlowAppName</param>
        /// <example><code>s.GetDataFromPortal(ref collectionData, "CLS", "Completion", "00000323", "speedyservices.com");</code></example>
        public bool GetDataFromPortal(ref ITisCollectionData cd, string eFlowAppName, string eFlowStationName, 
            string eFlowCollectionName, string customerDomain)
        {
            bool changed = false;
            
            try
            {
                changed = GetDataFromPortalCore(ref cd, eFlowAppName, eFlowStationName, eFlowCollectionName, customerDomain);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return changed;
        }

        #endregion "GetDataFromPortal"

        #region "SendDataToPortal"

        /// <summary>
        /// SendDataToPortal() --> Send collection data to the Supplier Portal from 'eFlowAppName', 
        /// 'eFlowStationName' and 'eFlowCollectionName'. The collection must not 'Locked' on eFlow. 
        /// </summary>
        /// <param name="eFlowAppName">The name of the eFlow application</param>
        /// <param name="eFlowStationName">The name of the eFlow station</param>
        /// <param name="eFlowCollectionName">The name of the eFlow collection</param>
        /// <param name="requiresOnlineInput">Indicates if this collection requires online data entry / user input</param>
        /// <param name="index">Indicates App's 'stationsToCheckForFields' setting on the SupplierPortalCommon.dll.config file on the eFlow\Bin</param>
        /// <example><code>string response = s.SendDataToPortal("CLS", "Completion", "00000323", 1);</code></example>
        public string SendDataToPortal(string eFlowAppName, string eFlowStationName, string eFlowCollectionName, bool requiresOnlineInput, int index)
        {
            string result = String.Empty;

            try
            {
                using (Batch b = new Batch(eFlowAppName, eFlowStationName))
                {
                    ITisCollectionData cd = b.Get(eFlowCollectionName);

                    result = SendDataToPortalCore(ref cd, eFlowAppName, eFlowStationName, eFlowCollectionName, requiresOnlineInput, index);

                    b.Free(cd);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// SendDataToPortal() --> Send collection data to the Supplier Portal from 'eFlowAppName', 
        /// 'eFlowStationName' and 'eFlowCollectionName'. The collection can be 'Locked' on eFlow. 
        /// </summary>
        /// <param name="cd">The ITisCollectionData object which contains the collection data</param>
        /// <param name="eFlowAppName">The name of the eFlow application</param>
        /// <param name="eFlowStationName">The name of the eFlow station</param>
        /// <param name="eFlowCollectionName">The name of the eFlow collection</param>
        /// <param name="requiresOnlineInput">Indicates if this collection requires online data entry / user input</param>
        /// <param name="index">Indicates App's 'stationsToCheckForFields' setting on the SupplierPortalCommon.dll.config file on the eFlow\Bin</param>
        /// <example><code>string response = s.SendDataToPortal(ref collectionData, "CLS", "Completion", "00000323", 1);</code></example>
        public string SendDataToPortal(ref ITisCollectionData cd, string eFlowAppName, string eFlowStationName, string eFlowCollectionName,
            bool requiresOnlineInput, int index)
        {
            string result = String.Empty;

            try
            {
                result = SendDataToPortalCore(ref cd, eFlowAppName, eFlowStationName, eFlowCollectionName, requiresOnlineInput, index);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// SendDataToPortal() --> Send collection data to the Supplier Portal from 'eFlowAppName', 
        /// 'eFlowStationName' and 'eFlowCollectionName'. The collection can be 'Locked' on eFlow. 
        /// </summary>
        /// <param name="cd">The ITisCollectionData object which contains the collection data</param>
        /// <param name="eFlowAppName">The name of the eFlow application</param>
        /// <param name="eFlowStationName">The name of the eFlow station</param>
        /// <param name="eFlowCollectionName">The name of the eFlow collection</param>
        /// <param name="requiresOnlineInput">Indicates if this collection requires online data entry / user input</param>
        /// <param name="index">Indicates App's 'stationsToCheckForFields' setting on the SupplierPortalCommon.dll.config file on the eFlow\Bin</param>
        /// <example><code>string response = s.SendDataToPortal(collectionData, "CLS", "Completion", "00000323", 1);</code></example>
        public string SendDataToPortal(ITisCollectionData cd, string eFlowAppName, string eFlowStationName, string eFlowCollectionName,
            bool requiresOnlineInput, int index)
        {
            string result = String.Empty;

            try
            {
                result = SendDataToPortalCore(ref cd, eFlowAppName, eFlowStationName, eFlowCollectionName, requiresOnlineInput, index);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        #endregion "SendDataToPortal"

        #region "getSetting"

        /// <summary>
        /// _getSetting() --> Gets the value of 'settingname' from 'SupplierPortalCommon.dll.config'.
        /// </summary>
        /// <param name="settingname">The setting from the 'SupplierPortalCommon.dll.config' file</param>
        /// <example><code>string setting = s._getSetting("app01WhoCanSeeInPortal");</code></example>
        public string _getSetting(string settingname)
        {
            string result = String.Empty;

            try
            {
                result = Common.GetSetting(settingname);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        #endregion "getSetting"

        #region "ForceUnlock"

        /// <summary>
        /// ForceUnlock() --> Forces an unlock on a locked collection.
        /// </summary>
        /// <param name="appName">The name of the eFlow App to where to get the collections from</param>
        /// <param name="stationName">The name of the eFlow station to where to get the collections from</param>
        /// <param name="cName">The name of the collection to unlock</param>
        /// <example><code>string setting = s.ForceUnlock("IRSupplierPortal", "SPortalCompl", "0000001");</code></example>
        public bool ForceUnlock(string appName, string stationName, string cName)
        {
            bool result = false;

            try
            {
                string selectString = "SELECT WFUnit_FKID FROM E_WFUnitMetaTags WHERE (E_WFUnitMetaTags.BatchName = '" + cName + "')";
                string updateString = "UPDATE R_WFQueueUnit SET Status = 1, LockSession = 0 WHERE (Status <> 0) AND (FK_WFUnitId = @wfUnitId)";

                List<string> dbConn = new List<string>();
                List<string> appNames = new List<string>();

                string file = Path.Combine(GetBin(), Constants.cStrFullDotConfig);
                if (File.Exists(file))
                {
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr01));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr02));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr03));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr04));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr05));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr06));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr07));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr08));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr09));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr10));

                    Common.GetCustomers(ref appNames);
                }

                int index = -1;
                string[] apps = appNames.ToArray();

                foreach (string itm in dbConn)
                {
                    index++;

                    if (itm != String.Empty && apps[index] != String.Empty && apps[index].ToUpper() == appName.ToUpper())
                    {
                        string DB_CONN_STRING = itm;

                        using (SqlConnection connection = new SqlConnection(DB_CONN_STRING))
                        {
                            object wfUnitId = null;
                            string strUnitId = String.Empty;
                            
                            using (SqlCommand mySqlCommand = new SqlCommand(selectString))
                            {
                                mySqlCommand.Connection = connection;
                                connection.Open();

                                using (SqlDataReader rdr = mySqlCommand.ExecuteReader())
                                {
                                    if (rdr.HasRows)
                                    {
                                        while (rdr.Read())
                                        {
                                            wfUnitId = (rdr.GetValue(0) as object);

                                            if (wfUnitId != null)
                                            {
                                                strUnitId = wfUnitId.ToString();
                                                break;
                                            }
                                        }
                                    }
                                }

                                connection.Close();
                            }

                            using (SqlCommand command = new SqlCommand(updateString))
                            {
                                command.Connection = connection;
                                command.CommandText = updateString;
                                command.Parameters.AddWithValue("@wfUnitId", strUnitId);

                                connection.Open();
                                int rows = command.ExecuteNonQuery();
                                connection.Close();
                            }
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        #endregion "ForceUnlock"

        #region "GetBin"

        /// <summary>
        /// GetBin() --> Gets the location of the eFlow\Bin folder.
        /// </summary>
        /// <example><code>string setting = s.GetBin();</code></example>
        public string GetBin()
        {
            string result = String.Empty;
            
            try
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(Constants.cStrDynamicReg);

                if (regKey != null)
                {
                    result = Path.Combine((string)regKey.GetValue(Constants.cStrEFLOW), Constants.cStrBIN);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        #endregion "GetBin"

        #region "GetCollectionsFromStation"

        /// <summary>
        /// GetCollectionsFromStation() --> Gets the names of the collections existing on a station / application.
        /// </summary>
        /// <param name="appName">The name of the eFlow App to where to get the collections from</param>
        /// <param name="stationName">The name of the eFlow station to where to get the collections from</param>
        /// <example><code>string setting = s.GetCollectionsFromStation("IRSupplierPortal", "SPortalCompl");</code></example>
        public string[] GetCollectionsFromStation(string appName, string stationName)
        {
            List<string> collections = new List<string>();

            try
            {
                string selectString = "SELECT E_WFUnitMetaTags.BatchName FROM E_WFUnitMetaTags INNER JOIN R_WFQueueUnit ON E_WFUnitMetaTags.WFUnit_FKID = R_WFQueueUnit.FK_WFUnitId INNER JOIN E_WFQueue ON R_WFQueueUnit.FK_WFQueueId = E_WFQueue.PKID WHERE (E_WFQueue.QueueName = '" 
                    + stationName + "')";

                List<string> dbConn = new List<string>();
                List<string> appNames = new List<string>();

                string file = Path.Combine(GetBin(), Constants.cStrFullDotConfig);
                if (File.Exists(file))
                {
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr01));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr02));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr03));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr04));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr05));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr06));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr07));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr08));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr09));
                    dbConn.Add(Common.GetSetting(file, CommonConst.connectionStr10));

                    Common.GetCustomers(ref appNames);
                }

                int index = -1;
                string[] apps = appNames.ToArray();

                foreach (string itm in dbConn)
                {
                    index++;

                    if (itm != String.Empty && apps[index] != String.Empty && apps[index].ToUpper() == appName.ToUpper())
                    {
                        string DB_CONN_STRING = itm;

                        using (SqlConnection mySqlConnection = new SqlConnection(DB_CONN_STRING))
                        {
                            using (SqlCommand mySqlCommand = new SqlCommand(selectString))
                            {
                                mySqlCommand.Connection = mySqlConnection;

                                mySqlConnection.Open();
                                using (SqlDataReader rdr = mySqlCommand.ExecuteReader())
                                {
                                    if (rdr.HasRows)
                                    {
                                        while (rdr.Read())
                                        {
                                            string batchName = (rdr.GetValue(0) as string).Trim();
                                            collections.Add(batchName);
                                        }
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return collections.ToArray();
        }

        #endregion "GetCollectionsFromStation"

        #endregion "Exposed/Public methods"

        #region "Private/Protected methods"

        /// <summary>
        /// RemoveDataFromPortalCore() --> Removes the collection data from the Supplier Portal for 'eFlowCollectionName'. 
        /// The collection can be 'Locked' on eFlow. 
        /// </summary>
        /// <param name="eFlowAppName">The name of the eFlow application</param>
        /// <param name="eFlowCollectionName">The name of the eFlow collection</param>
        /// <param name="customerDomain">The name of the customer assigned to this eFlowAppName</param>
        /// <example><code>s.RemoveDataFromPortalCore("CLS", "00000323", "speedyservices.com");</code></example>
        private string RemoveDataFromPortalCore(string eFlowAppName, string eFlowCollectionName, string customerDomain)
        {
            string result = String.Empty;

            try
            {
                using (Sp p = new Sp())
                {
                    string domain = String.Empty;
                    int port = 80;
                    bool https = false;

                    Common.GetFlexibleDbParams(out domain, out port, out https);

                    if (domain != String.Empty) p.SetDomain(domain);
                    
                    result = p.HttpPostCollectionDeleteAll(customerDomain, eFlowAppName, eFlowCollectionName, port, https); 
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// GetDataFromPortalCore() --> Gets collection data from the Supplier Portal for 'eFlowAppName', 
        /// 'eFlowStationName' and 'eFlowCollectionName'.  
        /// </summary>
        /// <param name="cd">The ITisCollectionData object which contains the collection data</param>
        /// <param name="eFlowAppName">The name of the eFlow application</param>
        /// <param name="eFlowStationName">The name of the eFlow station</param>
        /// <param name="eFlowCollectionName">The name of the eFlow collection</param>
        /// <param name="customerDomain">The name of the customer assigned to this eFlowAppName</param>
        /// <example><code>s.GetDataFromPortalCore(ref collectionData, "CLS", "Completion", "00000323", "speedyservices.com");</code></example>
        private bool GetDataFromPortalCore(ref ITisCollectionData cd, string eFlowAppName, string eFlowStationName,
            string eFlowCollectionName, string customerDomain)
        {
            bool changed = false;
            
            try
            {
                if (cd != null)
                {
                    string excludedStations = this._getSetting(CommonConst.excluded);

                    if (!Common.ExistsInExcludedStations(eFlowStationName, excludedStations))
                    {
                        string fn = String.Empty;
                        string[] customers = Common.GetCustomersForApp(eFlowAppName);

                        if (customers != null && customers.Length > 0)
                        {
                            foreach (string cust in customers)
                            {
                                if (customerDomain == String.Empty || cust.ToLower() == customerDomain.ToLower())
                                {
                                    using (Sp p = new Sp())
                                    {
                                        string domain = String.Empty;
                                        int port = 80;
                                        bool https = false;

                                        Common.GetFlexibleDbParams(out domain, out port, out https);

                                        if (domain != String.Empty) p.SetDomain(domain);
                                        
                                        string res = p.HttpPostCollectionQry(cust, eFlowAppName, Constants.
                                            cStrHttpPostCollectionDatacollectionNameQryCln, eFlowCollectionName,
                                            port, https);

                                        if (!res.ToLower().Contains(Constants.cStrEmptyJsonResponse))
                                        {
                                            SpClasses.SpTableResponse tbls = SpClasses.SpClassParser.FromJson<SpClasses.SpTableResponse>(res);

                                            try
                                            {
                                                if (tbls != null && tbls.result != null && tbls.result.Length > 0)
                                                {
                                                    foreach (SpClasses.SpTable tbl in tbls.result)
                                                    {
                                                        string[] flds = SpClasses.SpUtils.ParseStrDataToFields(tbl.strData);

                                                        if (flds != null && flds.Length > 0)
                                                        {
                                                            bool statusChanged = false;
                                                            bool fieldsChanged = AssignFieldsToCollection(ref cd, flds);

                                                            if (res.ToLower().Contains("rc=0"))
                                                                statusChanged = true;

                                                            if (statusChanged || fieldsChanged)
                                                                changed = true;
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Logging.WriteLog(ex.ToString());
                                            }
                                        }
                                        else
                                            changed = true;
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return changed;
        }

        #region "Internal private"

        #region "WriteToStationWindow" procedure code
        /// <summary>
        /// Write a message to a station's window.
        /// </summary>
        /// <param name="csm">ITisClientServicesModule object.</param>
        /// <param name="sFormat">The format of the message.</param>
        /// <param name="Params">The message to send.</param>
        /// <example><code>WriteToStationWindow(csm,"G","YourMessage");</code></example>
        public static void WriteToStationWindow(ITisClientServicesModule csm, String sFormat, params object[] Params)
        {
            if (csm != null)
            {
                String sMessage = String.Format(sFormat, Params);
                if (csm.ModuleAccess == null) return;
                else csm.ModuleAccess.DoAction("ReportMessage", String.Format("{0} {1} - {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), sMessage));
            }
        }

        /// <summary>
        /// Write a message to a station's window.
        /// </summary>
        /// <param name="csm">ITisClientServicesModule object.</param>
        /// <param name="sMessage">The message to send.</param>
        /// <example><code>WriteToStationWindow(csm,"YourMessage");</code> </example>
        public static void WriteToStationWindow(ITisClientServicesModule csm, String sMessage)
        {
            if (csm != null)
            {
                if (csm.ModuleAccess == null) return;
                else csm.ModuleAccess.DoAction("ReportMessage", String.Format("{0} {1} - {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), sMessage));
            }
        }

        /// <summary>
        /// Write a message to a station's window.
        /// </summary>
        /// <param name="csm">ITisClientServicesModule object.</param>
        /// <param name="sMessage">The message to send.</param>
        /// <param name="simpleString">A dummy variable to differentiate this function (write a simple String message)</param>
        /// <example><code>WriteToStationWindow(csm,"YourMessage",true);</code></example>
        public static void WriteToStationWindow(ITisClientServicesModule csm, String sMessage, bool simpleString)
        {
            if (csm != null)
            {
                if (csm.ModuleAccess == null) return;
                else csm.ModuleAccess.DoAction("ReportMessage", sMessage);
            }
        }

        /// <summary>
        /// Write a message to a station's window.
        /// </summary>
        /// <param name="csm">ITisClientServicesModule object.</param>
        /// <param name="scMessages">The messages to send.</param>        
        /// <example><code>
        /// StringCollection messages = new StringCollection();
        /// messages.Add("YourMessage_1");
        /// messages.Add("YourMessage_2");
        /// messages.Add("YourMessage_3");
        /// WriteToStationWindow(csm, messages);
        /// </code> </example>
        public static void WriteToStationWindow(ITisClientServicesModule csm, params String[] scMessages)
        {
            if (csm != null && scMessages != null)
            {
                foreach (String msg in scMessages)
                {
                    if (msg != null)
                    {
                        if (csm.ModuleAccess == null) return;
                        else csm.ModuleAccess.DoAction("ReportMessage", msg);
                    }
                }
            }
        }
        #endregion "WriteToStationWindow" method code

        #region "GetImagePageCount" function
        /// <summary>
        /// Get the page count of the specified image
        /// </summary>
        /// <param name="sourceImage">The image  path to get size from.</param>
        private static int GetImagePageCount(String sourceImage)
        {
            int result = 0;
            try
            {
                using (FileStream fs = new FileStream(sourceImage, FileMode.Open, FileAccess.Read))
                {
                    using (Image img = Image.FromStream(fs))
                    {
                        result = 1;
                        //-- lLoad input name if valid --\\
                        Guid objGuid = img.FrameDimensionsList[0];
                        FrameDimension objDimension = new FrameDimension(objGuid);

                        //-- Gets the total number of frames in the file --\\
                        result = img.GetFrameCount(objDimension);
                    }
                }
            }
            catch //(Exception ex)
            {
                //-- not interested in error, if image was ooened it has one page probably.
                // ILog.LogInfo(ex.ToString());
            }
            return result;
        }
        #endregion

        #region "getSupplietXmlInfo"
        private void getSupplietXmlInfo(string fn, out string supplierPortalDomain, out string supplierPortalSupplierId, out string supplierPortalPONumber, 
            out string supplierPortalDateTime)
        {
            supplierPortalDomain = String.Empty;
            supplierPortalSupplierId = String.Empty;
            supplierPortalPONumber = String.Empty;
            supplierPortalDateTime = String.Empty;
            
            try
            {
                string xmlFn = fn.Replace(Path.GetExtension(fn), ".xml");

                if (File.Exists(xmlFn))
                {
                    string contents = File.ReadAllText(xmlFn);

                    using (Sp p = new Sp())
                    {
                        supplierPortalDomain = p.ExtractStrInBetweenClean(contents, CommonConst.DomainNameSXmlTag, CommonConst.DomainNameEXmlTag);
                        supplierPortalSupplierId = p.ExtractStrInBetweenClean(contents, CommonConst.SupplierIdSXmlTag, CommonConst.SupplierIdEXmlTag);
                        supplierPortalPONumber = p.ExtractStrInBetweenClean(contents, CommonConst.PONumberSXmlTag, CommonConst.PONumberEXmlTag);
                        supplierPortalDateTime = p.ExtractStrInBetweenClean(contents, CommonConst.DateTimeSXmlTag, CommonConst.DateTimeEXmlTag);
                    }

                    File.Delete(xmlFn);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }
        #endregion

        #region "SearchAndConvertImagesToTiffCCITT4"
        private void SearchAndConvertImagesToTiffCCITT4(ITisClientServicesModule csm, CCConfiguration.CCConfigurationData cfg,
            string nextStation)
        {
            try
            {
                CCFileList[] searchdFiles = CCFileList.SearchFiles(cfg);

                if (searchdFiles != null && searchdFiles.Length > 0)
                {
                    //-- Iterate file search match --\\
                    foreach (CCFileList cfls in searchdFiles)
                    {
                        #region create and initialize variables
                        String[] tifFiles = cfls.Files;
                        string errMsg = null;
                        #endregion

                        #region
                        foreach (String sTif in tifFiles)
                        {
                            string tmpTiffFile = sTif.Replace(".tifExtProc", ".tif");

                            cfls.AddFiles(tmpTiffFile);
                            cfls.AddFiles(sTif);

                            #region //-- Create a table for the collection definitions and initialize it --\\
                            DataTable db = new DataTable();
                            int totalPagesInBatch = GetImagePageCount(sTif);

                            //-- Create table columns --\\
                            db.Columns.Add(TiS.Engineering.InputApi.CCEnums.CCTableColumns.Level.ToString());//-- Item level column --\\
                            db.Columns.Add(TiS.Engineering.InputApi.CCEnums.CCTableColumns.DataType.ToString());//-- Item type column --\\
                            db.Columns.Add(TiS.Engineering.InputApi.CCEnums.CCTableColumns.Key.ToString());//-- item key column --\\
                            db.Columns.Add(TiS.Engineering.InputApi.CCEnums.CCTableColumns.Data.ToString());//-- Item value  column --\\

                            //-- Set collection level (0) definitions --\\
                            db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.ImagePath.ToString(), null, sTif);//-- REQUIRED: Set collection image path --\\
                            db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.TotalPagesInBatch.ToString(), null, totalPagesInBatch.ToString());//-- REQUIRED: Set expected pages count --\\

                            string supplierPortalDomain = String.Empty;
                            string supplierPortalSupplierId = String.Empty;
                            string supplierPortalPONumber = String.Empty;
                            string supplierPortalDateTime = String.Empty;

                            getSupplietXmlInfo(tmpTiffFile, out supplierPortalDomain, out supplierPortalSupplierId, out supplierPortalPONumber, out supplierPortalDateTime);

                            //-- Set Named user tags --\\                        
                            db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.MetaData.ToString(), Tags.ImageSourceTag, tmpTiffFile);
                            db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.MetaData.ToString(), Tags.SupplierPortalTag, Path.GetFileName(tmpTiffFile));
                            db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.MetaData.ToString(), Tags.SupplierPortalDomainTag, supplierPortalDomain);
                            db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.MetaData.ToString(), Tags.SupplierPortalSupplierIdTag, supplierPortalSupplierId);
                            db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.MetaData.ToString(), Tags.SupplierPortalPONumberTag, supplierPortalPONumber);
                            db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.MetaData.ToString(), Tags.SupplierPortalDateTimeTag, supplierPortalDateTime);

                            #endregion

                            int errrCode = 0;

                            //--<< Call the API to create the collection >>--\\
                            bool ok = CollectionCreator.CreateCsmCollection(nextStation, csm, db, cfg, out errrCode, out errMsg);

                            if (ok)
                            {
                                cfls.DeleteFiles();
                                WriteToStationWindow(csm, string.Format("Done creating collection: [{0}] from file [{1}]", errMsg, tmpTiffFile));
                            }
                            else
                            {
                                CCFileList.MoveToFolder(cfg.ErrorFolderPath, cfls.Files);
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }
        #endregion "SearchAndConvertImagesToTiffCCITT4"

        #region "SearchAndConvertPdfToTiff"
        private void SearchAndConvertPdfToTiff(ITisClientServicesModule csm, CCConfiguration.CCConfigurationData cfg,
            CCConfiguration.CCConfigurationData cfgTif, string nextStation)
        {
            try
            {
                CCFileList[] searchdFiles = CCFileList.SearchFiles(cfg);

                if (searchdFiles != null && searchdFiles.Length > 0)
                {
                    //-- Iterate file search match --\\
                    foreach (CCFileList cfls in searchdFiles)
                    {
                        #region create and initialize variables
                        String[] pdfFiles = cfls.Files;
                        string errMsg = null;
                        #endregion

                        #region //-- Convert PDF to TIFF --\\
                        foreach (String sPdf in pdfFiles)
                        {
                            string tmpTiffFile = sPdf.Replace(".pdfExtProc", ".tif");
                            String errMsgPdf = null;

                            bool converted = TiS.Engineering.DocCreator.Convert.PdfToTiff(sPdf, tmpTiffFile, 300, out errMsgPdf);

                            if (converted)
                            {
                                cfls.AddFiles(tmpTiffFile);
                                cfls.AddFiles(sPdf);

                                #region //-- Create a table for the collection definitions and initialize it --\\
                                DataTable db = new DataTable();
                                int totalPagesInBatch = GetImagePageCount(tmpTiffFile);

                                //-- Create table columns --\\
                                db.Columns.Add(TiS.Engineering.InputApi.CCEnums.CCTableColumns.Level.ToString());//-- Item level column --\\
                                db.Columns.Add(TiS.Engineering.InputApi.CCEnums.CCTableColumns.DataType.ToString());//-- Item type column --\\
                                db.Columns.Add(TiS.Engineering.InputApi.CCEnums.CCTableColumns.Key.ToString());//-- item key column --\\
                                db.Columns.Add(TiS.Engineering.InputApi.CCEnums.CCTableColumns.Data.ToString());//-- Item value  column --\\

                                //-- Set collection level (0) definitions --\\
                                db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.ImagePath.ToString(), null, tmpTiffFile);//-- REQUIRED: Set collection image path --\\
                                db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.TotalPagesInBatch.ToString(), null, totalPagesInBatch.ToString());//-- REQUIRED: Set expected pages count --\\

                                string supplierPortalDomain = String.Empty;
                                string supplierPortalSupplierId = String.Empty;
                                string supplierPortalPONumber = String.Empty;
                                string supplierPortalDateTime = String.Empty;

                                getSupplietXmlInfo(tmpTiffFile, out supplierPortalDomain, out supplierPortalSupplierId, out supplierPortalPONumber, out supplierPortalDateTime);

                                //-- Set Named user tags --\\                        
                                db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.MetaData.ToString(), Tags.ImageSourceTag, tmpTiffFile);
                                db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.MetaData.ToString(), Tags.SupplierPortalTag, Path.GetFileName(tmpTiffFile));
                                db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.MetaData.ToString(), Tags.SupplierPortalDomainTag, supplierPortalDomain);
                                db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.MetaData.ToString(), Tags.SupplierPortalSupplierIdTag, supplierPortalSupplierId);
                                db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.MetaData.ToString(), Tags.SupplierPortalPONumberTag, supplierPortalPONumber);
                                db.Rows.Add("0", TiS.Engineering.InputApi.CCEnums.CCHedaerDataType.MetaData.ToString(), Tags.SupplierPortalDateTimeTag, supplierPortalDateTime);

                                #endregion

                                int errrCode = 0;

                                //--<< Call the API to create the collection >>--\\
                                bool ok = CollectionCreator.CreateCsmCollection(nextStation, csm, db, cfgTif, out errrCode, out errMsg);

                                if (ok)
                                {
                                    cfls.DeleteFiles();
                                    WriteToStationWindow(csm, string.Format("Done creating collection: [{0}] from file [{1}]", errMsg, tmpTiffFile));
                                }
                                else
                                {
                                    CCFileList.MoveToFolder(cfg.ErrorFolderPath, cfls.Files);
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }
        #endregion       

        private bool AssignFieldsToCollection(ref ITisCollectionData cd, string[] flds)
        {
            bool changed = false;
            
            try
            {
                if (cd != null)
                {
                    foreach (ITisFormData fd in cd.Forms)
                    {
                        if (Array.Exists<string>(flds, 
                            delegate(string s) 
                            { 
                                return s.ToLower().Contains(fd.Name.ToLower()); 
                            }))
                        {
                            foreach (ITisFieldData fld in fd.Fields)
                            {
                                string found = Array.Find<string>(flds,
                                    delegate(string s)
                                    {
                                        return (s.ToLower().Contains(fld.Name.ToLower()) && s.ToLower().Contains(fd.Name.ToLower()));
                                    });

                                if (found != null && found != String.Empty)
                                {
                                    string fldValue = found.Substring(found.IndexOf("=") + 1);

                                    if (fldValue != String.Empty)
                                    {
                                        if (fld.Contents != fldValue)
                                        {
                                            fld.Contents = fldValue;
                                            changed = true;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (ITisFieldData fld in fd.Fields)
                            {
                                string found = Array.Find<string>(flds,
                                    delegate(string s)
                                    {
                                        return s.ToLower().Contains(fld.Name.ToLower());
                                    });

                                if (found != null && found != String.Empty)
                                {
                                    string fldValue = found.Substring(found.IndexOf("=") + 1);

                                    if (fldValue != String.Empty)
                                    {
                                        if (fld.Contents != fldValue)
                                        {
                                            fld.Contents = fldValue;
                                            changed = true;
                                        }
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return changed;
        }

        #endregion "Internal private"

        /// <summary>
        /// SendDataToPortalCore() --> Send collection data to the Supplier Portal from 'eFlowAppName', 
        /// 'eFlowStationName' and 'eFlowCollectionName'.
        /// </summary>
        /// <param name="cd">The ITisCollectionData object which contains the collection data</param>
        /// <param name="eFlowAppName">The name of the eFlow application</param>
        /// <param name="eFlowStationName">The name of the eFlow station</param>
        /// <param name="eFlowCollectionName">The name of the eFlow collection</param>
        /// <param name="requiresOnlineInput">Indicates if this collection requires online data entry / user input</param>
        /// <param name="index">Indicates App's 'stationsToCheckForFields' setting on the SupplierPortalCommon.dll.config file on the eFlow\Bin</param>
        /// <example><code>string response = s.SendDataToPortalCore(ref collectionData, "CLS", "Completion", "00000323", 1);</code></example>
        private string SendDataToPortalCore(ref ITisCollectionData cd, string eFlowAppName, string eFlowStationName, string eFlowCollectionName,
            bool requiresOnlineInput, int index)
        {
            string result = String.Empty;
            string origin = String.Empty;

            try
            {
                if (cd != null)
                {
                    string excludedStations = this._getSetting(CommonConst.excluded);

                    if (!Common.ExistsInExcludedStations(eFlowStationName, excludedStations))
                    {
                        string fn = String.Empty;
                        string[] customers = Common.GetCustomersForApp(eFlowAppName);

                        if (customers != null && customers.Length > 0)
                        {
                            string[] fields = SpClasses.SpUtils.GetFieldBatchInfo(cd, 
                                this._getSetting(CommonConst.tempFileUploadFolder),
                                eFlowAppName, eFlowStationName, eFlowCollectionName,
                                this._getSetting(CommonConst.fieldsToCheck + (index).ToString()), out fn, out origin);

                            string setting = this._getSetting(CommonConst.stationsToCheckForFields + (index).ToString());

                            if (SpClasses.SpUtils.ExistsInPipeStr(false, eFlowStationName, setting))
                            {
                                foreach (string cust in customers)
                                {
                                    string bInfo = String.Empty;
                                    string fnUp = String.Empty;

                                    string fDomain = this._getSetting(CommonConst.fileDomain);

                                    SpClasses.SpUtils.GetExtendedFieldBatchInfo(origin, fn,
                                        Common.GetCreationDate(cd.CreationTime), fDomain, fields, eFlowCollectionName,
                                        eFlowStationName, "1", cust, eFlowAppName, out bInfo, out fnUp);

                                    if (requiresOnlineInput && bInfo.Contains(Constants.cStrOnlineInputNotRequired))
                                        bInfo = bInfo.Replace(Constants.cStrOnlineInputNotRequired, 
                                            Constants.cStrOnlineInputRequired);

                                    string nfn = fnUp.Substring(fnUp.LastIndexOf("/") + 1);

                                    if (fn != String.Empty)
                                    {
                                        if ((Common.IsAdmin(cust, customers)) || ((cust.ToLower() == origin.ToLower() ||
                                            origin == Constants.eFlowOrigin)))
                                            Common.SendFileWebRepository(cust, eFlowAppName, fn, nfn);
                                    }

                                    string alias = String.Empty;
                                    if (Common.IsAliasStation(setting, eFlowStationName, out alias))
                                    {
                                        if (alias != String.Empty)
                                            bInfo = bInfo.Replace(eFlowStationName, alias);
                                    }

                                    // Only sync back collection data to the portal that is from the same origin as the customer
                                    // (which means that is was submitted by the customer through the portal itself)
                                    // or collection data which is originally from eFlow (not submitted through the portal)
                                    // and is related to the main domain (admin domain = when firstTime is equal to 0).
                                    if ((Common.IsAdmin(cust, customers) || ((cust.ToLower() == origin.ToLower()) ||
                                        (origin == eFlow.SupplierPortalCore.Constants.eFlowOrigin))))
                                    {
                                        Common.SendToPortal(cust, eFlowAppName, eFlowCollectionName, bInfo);
                                    }
                                }

                                if (File.Exists(fn))
                                    File.Delete(fn);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        #endregion "Private/Protected methods"

        #region "Properties"
        #endregion "Properties"
    }

    #endregion "SpLite Class"
}

#endregion "SupplierPortalSdk Lite NS"
