using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TiS.Core.eFlowAPI;
using System.Reflection;

namespace TiS.Engineering.InputApi
{
    /// <summary>
    /// The external\exposed API class
    /// </summary>
    public class CollectionCreator
    {
        #region class variables
       private CCConfiguration.CCConfigurationData currCfg;
       private CCConfiguration config;
       private CCTimerSearch timerSearch;
       private CsmManager csmManager;
        #endregion

        public delegate void OnCollectionCreatedEvent(ITisClientServicesModule csm, ITisCollectionData collection);

        public static OnCollectionCreatedEvent OnCollectionCreated; 

           //CCDelegates.OnCollectionCreatedEvt OnCollectionCreated;

        #region class ctors'
        public CollectionCreator():this(null)
        {
        }

#if INTERNAL
        internal CollectionCreator(String settingsFile)
#else
        public CollectionCreator(String settingsFile)
#endif
        {
            try
            {
                csmManager = new CsmManager();

                if (String.IsNullOrEmpty(settingsFile)) settingsFile = CCUtils.GetSettingsFilePath();
                if (File.Exists(settingsFile)) config = CCConfiguration.FromXml(settingsFile);

                if (config == null)
                {
                    String  errMsg = String.Format("{0}: [{1}], error code [{2}]", CCConstants.E0100, settingsFile ?? String.Empty,  (int)CCEnums.CCErrorCodes.E0100);
                    throw new Exception(errMsg);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
                throw (ex);
            }
        } 
        #endregion

        #region "CreateCollection" functions
        /*
        /// <summary>
        /// Create an eFlow collection from the specified DataTable.
        /// </summary>
        /// <param name="collectionData">The collection data in the specified data table.</param>
        /// <param name="applicationName">The eFlow application name to insert the collection to.</param>
        /// <param name="copySourceFiles">Copy the source file when true (as opposed to move).</param>
        /// <param name="errMsg">Will return the error message if any error occured.</param>
        /// <returns>The error code, 1 = sucsess.</returns>
        public bool TestCreateCollection(String filePath, String stationName, String applicationName)
        {
            try
            {
                List<ITisPageParams> pages = new List<ITisPageParams>();
                ITisClientServicesModule csm = csmManager.GetCsm(applicationName, stationName, true);
                pages.Add(csm.Setup.get_FlowByIndex(0).get_FormByIndex(0).get_PageByIndex(0));
                CCCollection.CCPage pg = new CCCollection.CCPage(null, pages[0], "Demo");
                CCCollection col = new CCCollection();
                for (int i = 0; i < 7; i++)
                {
                    col.AddForm(new CCCollection.CCForm(null, null, csm.Setup.get_FlowByIndex(0).get_FormByIndex(0).Name, pg));
                }
                col.ImagePath = filePath;// @"C:\Program Files\TIS\eFlow 4.5\Sample Applications\SimpleDemo\Office7Pages.TIF";
               // col.FlowType = "OfficeStore";
                         
                //-- Create and define the Creator class --\\
                using (CCreator crt = new CCreator())
                {
                    crt.ValidatePagePerImagePage = false;
                    crt.CurrentProfile = CCConfiguration.CCConfigurationData.FromXml("Default2");
                    int errCode=0;
                   String[] res= crt.CreateCollections(csm,  out errCode, col);
                   if (res.Length > 0)
                   {
                       return true;
                   }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return false;
        }
        */

        /// <summary>
        /// Create an eFlow collection from the specified DataTable.
        /// </summary>
        /// <param name="collectionData">The collection data in the specified data table.</param>
        /// <param name="applicationName">The eFlow application name to insert the collection to.</param>
        /// <param name="copySourceFiles">Copy the source file\s when true (as opposed to move).</param>
        /// <param name="errMsg">Will return the error message if any error occured.</param>
        /// <returns>The error code, 1 = sucsess.</returns>
        public bool CreateCollection(DataTable collectionData, String applicationName, bool copySourceFiles, out int errCode, out String errMsg)
        {
            //-- Set returning values --\\
            errCode = (int)CCEnums.CCErrorCodes.E0000;
            errMsg = null;
            String[] createdFiles = null;
            CCCollection coll = null;

            try
            {
                #region //-- Load and validate profile --\\
                if (config == null)
                {
                    errCode = (int)CCEnums.CCErrorCodes.E0101;
                    errMsg = String.Format("{0}:, error code [{1}]", CCConstants.E0101, errCode);
                    throw new Exception(errMsg);
                }

                if (String.IsNullOrEmpty(applicationName)) applicationName = CCEnums.CCNames.Default.ToString();
                currCfg = config.GetConfiguration(applicationName);

                if (currCfg == null)
                {
                    errCode = (int)CCEnums.CCErrorCodes.E0102;
                    errMsg = String.Format("{0}: [{1}], error code [{2}]", CCConstants.E0102, applicationName, errCode);
                    throw new Exception(errMsg);
                }
                #endregion

                //-- Create and define the Creator class --\\
                using (CCreator crt = new CCreator())
                {
                    crt.CurrentProfile.CopySourceFiles = copySourceFiles;
                    if (currCfg != null) crt.CurrentProfile.LockExtension = currCfg.LockExtension;

                    ITisClientServicesModule csm = csmManager.GetCsm(String.IsNullOrEmpty(currCfg.LoginApplication) ? applicationName : currCfg.LoginApplication, currCfg.LoginStation, true);

                    if (csm == null)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0210;
                        errMsg = String.Format("{0}, Application name: [{1}], Station name: [{2}] error code [{3}]", CCConstants.E0210, String.IsNullOrEmpty(currCfg.LoginApplication) ? applicationName ?? String.Empty : currCfg.LoginApplication ?? String.Empty, currCfg.LoginStation ?? String.Empty, errCode);
                        throw new Exception(errMsg);
                    }

                    //-- Load CCCollection from a data table (and validate all the data) --\\
                    coll = CCDataTable.FromDataTable(currCfg, out errCode, out errMsg, copySourceFiles, out createdFiles, collectionData);                    

                    if (errCode != 1)
                    {
                        errMsg = String.Format("{0}, table name: [{1}], error code [{2}]", CCConstants.E0091, collectionData != null ? collectionData.TableName ?? String.Empty : String.Empty, errCode);
                        throw new Exception(errMsg);
                    }

                    if (coll == null)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0091;
                        errMsg = String.Format("{0}, table name: [{1}], error code [{2}]", CCConstants.E0091, collectionData != null ? collectionData.TableName ?? String.Empty : String.Empty, errCode);
                        throw new Exception(errMsg);
                    }

                    try
                    {
                        //--<< Call eFlow to create the collection >>--\\
                        String[] resCols = crt.CreateCollections(csm, out errCode, coll);
                        if (resCols == null || resCols.Length != 1)
                        {
                            errCode = errCode<=0 ? (int)CCEnums.CCErrorCodes.E0092: errCode;
                            errMsg = crt.LastErrors;
                            throw new Exception(String.Format("{0}, {1}, error code [{2}]", CCConstants.E0092, errMsg, errCode));
                        }
                        else
                        {
                            ILog.LogInfo("Done creating collection [{0}] in eFlow system", resCols[0]);
                            errCode = (int)CCEnums.CCErrorCodes.E0001;
                            errMsg = resCols[0];//-- Return the name of the created collection --\\
                            //\\ return errCode == (int)CCEnums.CCErrorCodes.E0001;
                        }
                    }
                    catch (Exception ec)
                    {

                        errCode = (int)CCEnums.CCErrorCodes.E0092;
                        errMsg = crt.LastErrors;
                        if (String.IsNullOrEmpty(errMsg)) errMsg = CCConstants.E0092 + "," + ec.Message;
                        throw new Exception(String.Format("{0}, {1}, {2}, error code [{3}]", CCConstants.E0092, errMsg, ec.Message, errCode));
                    }
                }
            }
            catch (Exception ex)
            {
                //\\ if (errCode < (int)CCEnums.CCErrorCodes.E0000) errCode = (int)CCEnums.CCErrorCodes.E0000;
                ILog.LogError(ex);
                //-- Move collections to errror folder --\\
                if (coll != null && coll.Files.Length>0 && currCfg!=null && !String.IsNullOrEmpty(currCfg.ErrorFolderPath))
                {
                    CCFileList.MoveToFolder(currCfg.ErrorFolderPath, coll.Files);
                }
                if (currCfg == null || currCfg.ThrowAllExceptions) throw ex;
            }
            finally
            {
                //-- Delete files created by this method --\\ 
                if (copySourceFiles && createdFiles != null && createdFiles.Length > 0)
                {
                    foreach (String sf in createdFiles)
                    {
                        try
                        {
                            if (File.Exists(sf ?? String.Empty))
                            {
                                File.SetAttributes(sf, FileAttributes.Normal);
                                File.Delete(sf);
                            }
                        }
                        catch (Exception ex)
                        {
                            ILog.LogError(ex);
                            throw ex;
                        }
                    }
                }
            }
          //\\  return false;
            return errCode == (int)CCEnums.CCErrorCodes.E0001;
        }

        /// <summary>
        /// Create an eFlow collection from the specified source file.
        /// </summary>
        /// <param name="applicationName">The eFlow application name.</param>
        /// <param name="copySourceFiles">Copy the source file when true, move when false.</param>
        /// <param name="collectionDataPath">The path to the collection data file.</param>
        /// <param name="errCode">The output error code.</param>
        /// <param name="errMsg">The output error String</param>
        /// <returns>The error code, 1 when successfull, any other value when not.</returns>
#if INTERNAL  && !EXPOSE_CCColection
        internal int CreateCollection(String applicationName, bool copySourceFiles, String collectionDataPath, out int errCode, out String errMsg)
#else
        public int CreateCollection(String applicationName, bool copySourceFiles, String collectionDataPath, out int errCode, out String errMsg)
#endif
        {
            errCode = -1;
            errMsg = null;

            try
            {
                //-- Create \ initilaize configuration data --\\
                CCConfiguration.CCConfigurationData cfg = CCConfiguration.CCConfigurationData.FromXml(applicationName);

                //-- Create a collection creator class --\\
                using (CCreator crt = new CCreator(cfg))
                {
                    crt.CurrentProfile.CopySourceFiles = copySourceFiles;                    

                    //-- Create the collection definition --\\
                    CCCollection coll = CCCollection.FromXml(collectionDataPath, !copySourceFiles);

                    if (coll == null)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0091;
                        errMsg = String.Format("{0}, file name: [{1}], error code [{2}]", CCConstants.E0091, collectionDataPath ?? String.Empty, errCode);
                        throw new Exception(errMsg);
                    }


                    String[] resCols = crt.CreateCollections(null, out errCode, coll);
                    if (resCols == null || resCols.Length != 1)
                    {
                        errCode = errCode<=0 ? (int)CCEnums.CCErrorCodes.E0092: errCode;
                        errMsg = String.Format("{0}, {1}, Source file path [{1}], error code [{2}]", CCConstants.E0092, collectionDataPath ?? String.Empty, errCode);
                        throw new Exception(errMsg);
                    }
                    else
                    {
                        ILog.LogInfo("Done creating collection [{0}] from file [{1}] in eFlow system", resCols[0], collectionDataPath);
                        errCode = (int)CCEnums.CCErrorCodes.E0001;
                    }
                }
            }
            catch (Exception ex)
            {
                if (String.IsNullOrEmpty(errMsg)) errMsg = ex.Message;
                ILog.LogError(ex);
                if (currCfg == null || currCfg.ThrowAllExceptions) throw ex;
            }
            return errCode;
        } 
        #endregion

        #region "CreateCsmCollection" functions
        /// <summary>
        /// Create an eFlow collection from the specified DataTable, using an existing\specified CSM object.
        /// </summary>
        /// <param name="nextStation">The name of the next station.</param>
        /// <param name="csm">ITisClientServicesModule object.</param>
        /// <param name="collectionData">The collection data in the a data table.</param>
        /// <param name="dataCfg">The InputApi profile to use to create the collection.</param>
        /// <param name="copySourceFiles">Copy the source file when true (as opposed to move).</param>
        /// <param name="errMsg">Will return the error message if any error occured.</param>
        /// <returns>The error code, 1 = sucsess.</returns>
        public static bool CreateCsmCollection(string nextStation, ITisClientServicesModule csm, DataTable collectionData, CCConfiguration.CCConfigurationData dataCfg, out int errCode, out String errMsg)
        {
            //-- Set returning values --\\
            errCode = (int)CCEnums.CCErrorCodes.E0000;
            errMsg = null;
            String[] createdFiles = null;

            try
            {
                #region //-- Validate profile and CSM --\\
                if (csm == null)
                {
                    errCode = (int)CCEnums.CCErrorCodes.E0210;
                    errMsg = String.Format("{0}, error code [{1}]", CCConstants.E0210, "?Unknown?", errCode);
                    throw new Exception(errMsg);
                }

                if (dataCfg == null)
                {
                    errCode = (int)CCEnums.CCErrorCodes.E0102;
                    errMsg = String.Format("{0}: [{1}], error code [{2}]", CCConstants.E0102, csm.Application.AppName, errCode);
                    throw new Exception(errMsg);
                }
                #endregion

                //-- Create and define the Creator class --\\
                using (CCreator crt = new CCreator(dataCfg))
                {
                    //-- Load CCCollection from a data table (and validate all the data) --\\
                    CCCollection coll = CCDataTable.FromDataTable(dataCfg, out errCode, out errMsg, dataCfg.CopySourceFiles, out createdFiles, collectionData);

                    if (errCode != 1)
                    {
                        errMsg = String.Format("{0}, table name: [{1}], error code [{2}]", CCConstants.E0091, collectionData != null ? collectionData.TableName ?? String.Empty : String.Empty, errCode);
                        throw new Exception(errMsg);
                    }

                    if (coll == null)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0091;
                        errMsg = String.Format("{0}, table name: [{1}], error code [{2}]", CCConstants.E0091, collectionData != null ? collectionData.TableName ?? String.Empty : String.Empty, errCode);
                        throw new Exception(errMsg);
                    }

                    try
                    {
                        //--<< Call eFlow to create the collection >>--\\
                        String[] resCols = crt.CreateCollections(nextStation, csm, out errCode, coll);
                        if (resCols == null || resCols.Length != 1)
                        {
                            errCode = errCode <= 0 ? (int)CCEnums.CCErrorCodes.E0092 : errCode;
                            errMsg = crt.LastErrors;
                            throw new Exception(String.Format("{0}, {1}, error code [{2}]", CCConstants.E0092, errMsg, errCode));
                        }
                        else
                        {
                            ILog.LogInfo("Done creating collection [{0}] in eFlow system", resCols[0]);
                            errCode = (int)CCEnums.CCErrorCodes.E0001;
                            errMsg = resCols[0];//-- Return the name of the created collection --\\
                        }
                    }
                    catch (Exception ec)
                    {

                        errCode = (int)CCEnums.CCErrorCodes.E0092;
                        errMsg = crt.LastErrors;
                        if (String.IsNullOrEmpty(errMsg)) errMsg = CCConstants.E0092 + "," + ec.Message;
                        throw new Exception(String.Format("{0}, {1}, {2}, error code [{3}]", CCConstants.E0092, errMsg, ec.Message, errCode));
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
                if (dataCfg == null || dataCfg.ThrowAllExceptions) throw ex;
            }
            finally
            {
                //-- Delete files created by this method --\\ 
                if (dataCfg != null && dataCfg.CopySourceFiles && createdFiles != null && createdFiles.Length > 0)
                {
                    foreach (String sf in createdFiles)
                    {
                        try
                        {
                            if (File.Exists(sf ?? String.Empty))
                            {
                                File.SetAttributes(sf, FileAttributes.Normal);
                                File.Delete(sf);
                            }
                        }
                        catch (Exception ex)
                        {
                            ILog.LogError(ex);
                            throw ex;
                        }
                    }
                }
            }
            return errCode == (int)CCEnums.CCErrorCodes.E0001;
        }

        /// <summary>
        /// Create an eFlow collection from the specified source file, using an existing\specified CSM object..
        /// </summary>
        /// <param name="nextStation">The name of the next station.</param>
        /// <param name="csm">ITisClientServicesModule object.</param>
        /// <param name="dataCfg">The InputApi profile to use to create the collection.</param>
        /// <param name="copySourceFiles">Copy the source file when true, move when false.</param>
        /// <param name="collectionDataPath">The path to the collection data file.</param>
        /// <param name="errCode">The output error code.</param>
        /// <param name="errMsg">The output error String</param>
        /// <returns>The error code, 1 when successfull, any other value when not.</returns>
#if INTERNAL  && !EXPOSE_CCColection        
        internal static int CreateCsmCollection(string nextStation, ITisClientServicesModule csm, CCConfiguration.CCConfigurationData dataCfg, bool copySourceFiles, String collectionDataPath, out int errCode, out String errMsg)
#else
        public static int CreateCsmCollection(string nextStation, ITisClientServicesModule csm, CCConfiguration.CCConfigurationData dataCfg, bool copySourceFiles, String collectionDataPath, out int errCode, out String errMsg)
#endif
        {
            errCode = -1;
            errMsg = null;

            try
            {
                #region //-- Validate profile and CSM --\\
                if (csm == null)
                {
                    errCode = (int)CCEnums.CCErrorCodes.E0210;
                    errMsg = String.Format("{0}, error code [{1}]", CCConstants.E0210, "?Unknown?", errCode);
                    throw new Exception(errMsg);
                }

                if (dataCfg == null)
                {
                    errCode = (int)CCEnums.CCErrorCodes.E0102;
                    errMsg = String.Format("{0}: [{1}], error code [{2}]", CCConstants.E0102, csm.Application.AppName, errCode);
                    throw new Exception(errMsg);
                }
                #endregion


                //-- Create a collection creator class --\\
                using (CCreator crt = new CCreator(dataCfg))
                {
                    crt.CurrentProfile.CopySourceFiles = copySourceFiles;

                    //-- Create the collection definition --\\
                    CCCollection coll = CCCollection.FromXml(collectionDataPath, !copySourceFiles);

                    if (coll == null)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0091;
                        errMsg = String.Format("{0}, file name: [{1}], error code [{2}]", CCConstants.E0091, collectionDataPath ?? String.Empty, errCode);
                        throw new Exception(errMsg);
                    }


                    String[] resCols = crt.CreateCollections(nextStation, csm, out errCode, coll);
                    if (resCols == null || resCols.Length != 1)
                    {
                        errCode = errCode <= 0 ? (int)CCEnums.CCErrorCodes.E0092 : errCode;
                        errMsg = String.Format("{0}, {1}, Source file path [{1}], error code [{2}]", CCConstants.E0092, collectionDataPath ?? String.Empty, errCode);
                        throw new Exception(errMsg);
                    }
                    else
                    {
                        ILog.LogInfo("Done creating collection [{0}] from file [{1}] in eFlow system", resCols[0], collectionDataPath);
                        errCode = (int)CCEnums.CCErrorCodes.E0001;
                    }
                }
            }
            catch (Exception ex)
            {
                if (String.IsNullOrEmpty(errMsg)) errMsg = ex.Message;
                ILog.LogError(ex);
                if (dataCfg == null || dataCfg.ThrowAllExceptions) throw ex;
            }
            return errCode;
        } 
        #endregion

        #region "StartFilesSearch" method.
        /// <summary>
        /// Start a file search using the specified propfile\application name.
        /// </summary>
        /// <param name="profileName">the configuration profile to use for the search.</param>
#if INTERNAL
        internal void StartFilesSearch(String profileName)
#else
        public void StartFilesSearch(String profileName)
#endif
        {
            try
            {
                if (!String.IsNullOrEmpty(profileName))
                {
                    CCConfiguration.CCConfigurationData cfData = config.GetConfiguration(profileName);
                    if (cfData != null)
                    {
                        //-- Check if reset required --\\
                        if (timerSearch != null && String.Compare(profileName, timerSearch.CurrentProfile.Name, true) != 0)
                        {
                            //-- Reset --\\
                            timerSearch.Enabled = false;
                            timerSearch.Dispose();
                            timerSearch = null;
                        }

                        //-- Create a new timer search object if necessary --\\
                        if (timerSearch == null) timerSearch = new CCTimerSearch(csmManager.GetCsm(String.IsNullOrEmpty(cfData.LoginApplication) ? profileName : cfData.LoginApplication, cfData.LoginStation, true), cfData);
                        timerSearch.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        } 
        #endregion

        #region "StopFilesSearch" method
        /// <summary>
        /// Stop file search action if any is enabled and running.
        /// </summary>
#if INTERNAL
        internal void StopFilesSearch()
#else
        public void StopFilesSearch()
#endif
        {
            try
            {
                if (timerSearch != null && timerSearch.Enabled)
                {
                    timerSearch.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        } 
        #endregion
    }
}
