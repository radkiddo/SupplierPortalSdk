using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using TiS.Core.eFlowAPI;
using TiS.Core.PlatformRuntime;
using TiS.Core.Workflow.WFCommon;
using TiS.Imaging;
using TiS.Imaging.FileOP;
using System.Threading;

namespace TiS.Engineering.InputApi
{
    /// <summary>
    /// A class to serialize desrialize create and input collections
    /// </summary>
#if INTERNAL
    internal class CCreator :ICCCretaor
#else
    public class CCreator :ICCCretaor
#endif
    {
        #region class variables
        private CsmManager csmMangager = new CsmManager();
        private CCConfiguration.CCBaseConfigurationData currentProfile;
        //private CCCollection currentCollection;
        private List<String> errors;
        //private bool validateEmptyForms;
        //private bool validateCollectionImagePath;
        //private bool validateCollectionExists;
        //private bool validateFlow;
        //private bool validateSinglePagePerForm;
        //private bool validateMultiplePagePerForm;
        //private bool validateMoreThanOnePagePerForm;
        //private bool validatePagePerImagePage;
        //private bool validateFormType;
        //private bool validationsDisabled;
        #endregion 

        #region events
        /// <summary>
        /// On collection created event.
        /// </summary>
        public event CCDelegates.OnCollectionCreatedEvt OnCollectionCreated;

        /// <summary>
        /// On page read event.
        /// </summary>
        public event CCDelegates.OnPageReadEvt OnPageRead;

        /// <summary>
        /// On custom source file read event.
        /// </summary>
        public event CCDelegates.OnCustomSourceFileReadEvt OnCustomSourceFileRead;

        #region "CollectionCreated" event function
        /// <summary>
        /// The internal method that fires the collection created event.
        /// </summary>
        /// <param name="icsm">The csm object used to create the collection.</param>
        /// <param name="collection">The collection that was created.</param>
        /// <returns>true when successfull, false when failed.</returns>
        private bool CollectionCreated(ITisClientServicesModule icsm, ITisCollectionData collection)
        {
            bool result = collection != null;
            try
            {
                if (OnCollectionCreated != null) OnCollectionCreated(null, this, icsm, collection, ref result);
                if (!result)
                {
                    try
                    {
                        icsm.Dynamic.RemoveSpecificCollection(collection);
                        collection = null;
                    }
                    catch (Exception en)
                    {
                        ILog.LogError(en);
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result;
        }
        #endregion 
        #endregion

        #region class ctor's
        /// <summary>
        /// Class basic ctor.
        /// </summary>
        public CCreator()
        {
            ValidateCollectionImagePath = true;
            ValidateCollectionExists = true;
        }

        /// <summary>
        /// class extended ctor.
        /// </summary>
        /// <param name="config">The configuration\profile to use when creating collections.</param>
        public CCreator(CCConfiguration.CCBaseConfigurationData config):this()
        {
            currentProfile = config;
        } 
        #endregion

        #region "AddErrMsg" method
        /// <summary>
        /// Add an error message to the list of messages
        /// </summary>
        /// <param name="erMsg">The error message to add to the existing list of errors.</param>
        protected void AddErrMsg(String erMsg)
        {
            try
            {
                if (errors == null) errors = new List<String>();
                if (erMsg != null) errors.Add(erMsg);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        } 
        #endregion

        #region "Dispose" method
        /// <summary>
        /// Dispose class used objects and resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (csmMangager != null)
                {
                    csmMangager.Dispose();
                    csmMangager = null;
                }
            }
            catch (Exception ex)
            {
                AddErrMsg(ex.Message);
                ILog.LogError(ex);
                if (CurrentProfile.ThrowAllExceptions) throw ex;
            }
        }
        #endregion

        #region "GetFormName" function
        /// <summary>
        /// Create an eFlow form name
        /// </summary>
        /// <param name="formIndex">The form index in collection to create the from name with.</param>
        /// <returns>The created name when successfull, null when failed.</returns>
        public String GetFormName(int formIndex)
        {
            try
            {
                return String.Format("P{0}", formIndex.ToString("X").PadLeft(4, '0'));
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return null;
        } 
        #endregion

        #region "GetPageName" function
        /// <summary>
        /// Create am eFlow page name
        /// </summary>
        /// <param name="pageIndex">The page index in collection to create the page name with.</param>
        /// <returns>The created name when successfull, null when failed.</returns>
        public String GetPageName(int pageIndex)
        {
            try
            {
                return String.Format("P{0}", pageIndex.ToString("X").PadLeft(4, '0'));
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return null;
        } 
        #endregion

        #region class properties
        /// <summary>
        /// Get the last errors that occured.
        /// </summary>
        public String LastErrors
        {
            get { return errors!=null && errors.Count>0 ? String.Join("|", errors.ToArray()) : String.Empty;}
        }

        /// <summary>
        /// Defines wether to create a collection or update an exsiting item
        /// </summary>
        private TiS.Engineering.InputApi.CCEnums.CCUpdateState updateState;
        public TiS.Engineering.InputApi.CCEnums.CCUpdateState UpdateState { get { return updateState; } set { updateState = value; } }

        /// <summary>
        /// Holds the current collection (in process).
        /// </summary>
        private CCCollection currentCollection;
        public CCCollection CurrentCollection { get { return currentCollection; } set { currentCollection = value; } }

        /// <summary>
        /// Class profile profile data.
        /// </summary>
        public CCConfiguration.CCBaseConfigurationData CurrentProfile
        {
            get
            {
                if (currentProfile == null) currentProfile = new CCConfiguration.CCBaseConfigurationData();
                return currentProfile;
            }
            set { currentProfile = value; }
        }

        /// <summary>
        /// Returns true if any validation is enabled (regardless of the state of <see cref="ValidationsDsiabled"/> proeprty.
        /// </summary>
        public virtual bool HasValidations
        {
            get { return ValidateCollectionImagePath || ValidateCollectionExists || ValidateEmptyForms || ValidateFlow || ValidateFormType || ValidateMoreThanOnePagePerForm || ValidateMultiplePagePerForm || ValidatePagePerImagePage || ValidateSinglePagePerForm; }
        }

        /// <summary>
        /// Validate collection image path.
        /// </summary>
        private bool validateCollectionImagePath;
        public virtual bool ValidateCollectionImagePath { get { return validateCollectionImagePath; } set { validateCollectionImagePath = value; } }

        /// <summary>
        /// Validate collection already exists (by name).
        /// </summary>
        private bool validateCollectionExists;
        public virtual bool ValidateCollectionExists { get { return validateCollectionExists; } set { validateCollectionExists = value; } }

        /// <summary>
        /// Validate Empty forms (collection data must have forms).
        /// </summary>
        private bool validateEmptyForms;
        public virtual bool ValidateEmptyForms { get { return validateEmptyForms; } set { validateEmptyForms = value; } }

        /// <summary>
        /// Validate flow.
        /// </summary>
        private bool validateFlow;
        public virtual bool ValidateFlow { get { return validateFlow; } set { validateFlow = value; } }

        /// <summary>
        ///  Validate that a only one per form exists.
        /// </summary>
        private bool validateSinglePagePerForm;
        public virtual bool ValidateSinglePagePerForm { get { return validateSinglePagePerForm; } set { validateSinglePagePerForm = value; } }

        /// <summary>
        ///   Validate one or more pages exists in each form.
        /// </summary>
        private bool validateMultiplePagePerForm;
        public virtual bool ValidateMultiplePagePerForm { get { return validateMultiplePagePerForm; } set { validateMultiplePagePerForm = value; } }

        /// <summary>
        /// Validate that each form has more than one page.
        /// </summary>
        private bool validateMoreThanOnePagePerForm;
        public virtual bool ValidateMoreThanOnePagePerForm { get { return validateMoreThanOnePagePerForm; } set { validateMoreThanOnePagePerForm = value; } }

        /// <summary>
        /// Validtae that each image page has an eFlow page (page counts must be the same).
        /// </summary>
        private bool validatePagePerImagePage;
        public virtual bool ValidatePagePerImagePage { get { return validatePagePerImagePage; } set { validatePagePerImagePage = value; } }
        
        /// <summary>
        /// Validate that all forms have a valid form type.
        /// </summary>
        private bool validateFormType;
        public virtual bool ValidateFormType { get { return validateFormType; } set { validateFormType = value; } }

        /// <summary>
        /// Get set the validations enabled\disabled state.
        /// </summary>
        private bool validationsDisabled;
        public virtual bool ValidationsDisabled { get { return validationsDisabled; } set { validationsDisabled = value; } }
        #endregion        

        #region "ValidateCollection" function
        /// <summary>
        /// Validate a collection, according to the validate settings.
        /// </summary>
        /// <param name="csm">Only needed to validate that the collection flow type exists in this eFlow application and that all forms has valid form types.</param>
        /// <param name="ccData">the collection data to validate.</param>
        /// <param name="errCode">The error code returned by this method</param>
        /// <returns>true when valid false when not, will throw an exception when failed.</returns>
        protected virtual bool ValidateCollection(ITisClientServicesModule csm, CCCollection ccData, ref int errCode)
        {
            try
            {
                if (ValidationsDisabled || !HasValidations) return true;//-- Break if validations disabled --\\

                int formIdx = 0;

                //-- Validate flow  --\\
                if (ValidateFlow)
                {
                    if (String.IsNullOrEmpty(ccData.FlowType))
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0212;
                        throw new Exception(String.Format("{0}, Collection [{1}], Error code [{2}], set 'CCReator.ValidateFlow' to false to avoid this messsage", CCConstants.E0212, ccData.Name, errCode));
                    }

                    if (csm != null)
                    {
                        ITisFlowParams flowParams = csm.Setup.get_Flow(ccData.FlowType);
                        if (flowParams == null)
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0232;
                            throw new Exception(String.Format("{0}, Flow [{1}], Application [{2}], Error code [{3}], set 'CCReator.ValidateFlow' to false to avoid this messsage", CCConstants.E0232, ccData.FlowType, csm.Application.AppName, errCode));
                        }
                    }
                }

                //-- Validate collection image path --\\
                if (ValidateCollectionImagePath)
                {
                    if (!File.Exists(ccData.ImagePath))
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0060;
                        throw new Exception(String.Format("{0}, Collection [{1}], Image file [{2}], Error code [{3}], to avoid this message set 'CCreator.ValidateCollectionImagePath' to false.", CCConstants.E0060, ccData.Name, ccData.ImagePath, errCode));
                    }
                }

                //-- Validate Empty forms --\\
                if (ValidateEmptyForms)
                {
                    if (ccData.Forms == null || ccData.Forms.Length <= 0)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0233;
                        throw new Exception(String.Format("{0}, Collection [{1}], Error code [{2}], to avoid this message set 'CCreator.ValidateEmptyForms' to false.", CCConstants.E0233, ccData.Name, errCode));
                    }

                    formIdx = 0;
                    foreach (CCCollection.CCForm fr in ccData.Forms)
                    {
                        if (fr.Pages == null || fr.Pages.Length <= 0)
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0234;
                            throw new Exception(String.Format("{0}, Collection [{1}], Form index [{2}], Error code [{3}], to avoid this message set 'CCreator.ValidateEmptyForms' to false.", CCConstants.E0234, ccData.Name, formIdx, errCode));
                        }
                        formIdx++;
                    }
                }

                //-- Validate that a only one per form exists --\\
                if (ValidateSinglePagePerForm)
                {
                    formIdx = 0;
                    foreach (CCCollection.CCForm fr in ccData.Forms)
                    {
                        if (fr.Pages == null || fr.Pages.Length <= 0)
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0234;
                            throw new Exception(String.Format("{0}, Collection [{1}], Form index [{2}], Error code [{3}], to avoid this message set 'CCreator.ValidateSinglePagePerForm' to false.", CCConstants.E0234, ccData.Name, formIdx, errCode));
                        }
                        else if (fr.Pages.Length > 1)
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0235;
                            throw new Exception(String.Format("{0}, Collection [{1}], Form index [{2}], Page count [{2}], Error code [{3}], to avoid this message set 'CCreator.ValidateSinglePagePerForm' to false.", CCConstants.E0235, ccData.Name, formIdx, fr.Pages.Length, errCode));
                        }

                        formIdx++;
                    }
                }

                //-- Validate one or more pages exists in each form --\\
                if (ValidateMultiplePagePerForm)
                {
                    formIdx = 0;
                    foreach (CCCollection.CCForm fr in ccData.Forms)
                    {
                        if (fr.Pages == null || fr.Pages.Length <= 0)
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0234;
                            throw new Exception(String.Format("{0}, Collection [{1}], Form index [{2}], Error code [{3}], to avoid this message set 'CCreator.ValidateMultiplePagePerForm' to false.", CCConstants.E0234, ccData.Name, formIdx, errCode));
                        }
                        formIdx++;
                    }
                }

                //-- Validate that each form has more than one page --\\
                if (ValidateMoreThanOnePagePerForm)
                {
                    formIdx = 0;
                    foreach (CCCollection.CCForm fr in ccData.Forms)
                    {
                        if (fr.Pages == null || fr.Pages.Length <= 0)
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0234;
                            throw new Exception(String.Format("{0}, Collection [{1}], Form index [{2}], Error code [{3}], to avoid this message set 'CCreator.ValidateMoreThanOnePagePerForm' to false.", CCConstants.E0234, ccData.Name, formIdx, errCode));
                        }
                        else if (fr.Pages.Length <= 1)
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0236;
                            throw new Exception(String.Format("{0}, Collection [{1}], Form index [{2}] Page count [{2}] Error code [{3}], to avoid this message set 'CCreator.ValidateMoreThanOnePagePerForm' to false.", ccData.Name, formIdx, fr.Pages.Length));
                        }

                        formIdx++;
                    }
                }

                #region   //-- Validate that the specified name does not exist when update state is create new --\\
                if (ValidateCollectionExists && UpdateState == CCEnums.CCUpdateState.CreateNew && !ccData.UpdateExistingCollection && !String.IsNullOrEmpty(ccData.Name))
                {
                    ITisDynamicQueryResult[] dq = csm.DynamicQuery.QueryCollections(csm.DynamicQuery.StationQueues.GetAsArray(), null, UnitStatus.UNDEFINED, String.Format("Name='{0}'", ccData.Name), 0, 1);
                    if (dq != null && dq.Length > 0)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0230;
                        throw new Exception(String.Format("{0}, Collection [{1}], Error code [{2}]", CCConstants.E0230, ccData.Name, errCode));
                    }
                }
                #endregion

                //-- Validtae that each image page has an eFlow page (page counts must be the same) --\\
                if (ValidatePagePerImagePage)
                {
                    if (!File.Exists(ccData.ImagePath))
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0060;
                        throw new Exception(String.Format("{0}, Collection [{1}], Image file [{2}], Error code [{3}], to avoid this message set 'CCreator.ValidatePagePerImagePage' to false.", CCConstants.E0060, ccData.Name, ccData.ImagePath, errCode));
                    }

                    if (ccData.Pages == null || ccData.Pages.Length <= 0)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0237;
                        throw new Exception(String.Format("{0}, Collection [{1}],  Error code [{2}], to avoid this message set 'CCreator.ValidatePagePerImagePage' to false.", CCConstants.E0237, ccData.Name, errCode));
                    }

                    int pageCount = CCUtils.GetImagePageCount(ccData.ImagePath, null);
                    if (pageCount <= 0)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0061;
                        throw new Exception(String.Format("{0}, Collection [{1}], Image [{2}], Error code [{3}], to avoid this message set 'CCreator.ValidatePagePerImagePage' to false.",CCConstants.E0061, ccData.Name, ccData.ImagePath, errCode));
                    }
                    if (pageCount != ccData.Pages.Length)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0238;
                        throw new Exception(String.Format("{0}, Collection [{1}], Page count [{2}], Image [{3}], Image page count [{4}], Error code [{5}], to avoid this message set 'CCreator.ValidatePagePerImagePage' to false.", CCConstants.E0238, ccData.Name, ccData.Pages.Length, ccData.ImagePath, pageCount, errCode));
                    }
                }

                //-- Validate that all forms have a valid form type --\\
                if (ValidateFormType)
                {
                    //-- Validate flow --\\
                    ITisFlowParams flowParams = csm!=null ? csm.Setup.get_Flow(ccData.FlowType):null;
                    if (flowParams == null && csm != null)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0232;
                        throw new Exception(String.Format("{0}, Collection [{1}], Flow type [{2}], Application [{3}], Error code [{4}], set 'CCReator.ValidateFormType' to false to avoid this messsage", CCConstants.E0232, ccData.Name, ccData.FlowType, csm.Application.AppName, errCode));
                    }

                    formIdx = 0;
                    foreach (CCCollection.CCForm fr in ccData.Forms)
                    {
                        if (String.IsNullOrEmpty(fr.FormType))
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0239;
                            throw new Exception(String.Format("{0}, Collection [{1}], Form index [{2}] Error code [{3}], to avoid this message set CCreator.ValidateFormType to false.",CCConstants.E0239, ccData.Name, formIdx,errCode));
                        }
                        if (flowParams != null)
                        {
                            ITisFormParams fp = flowParams.get_Form(fr.FormType);
                            if (fp == null)
                            {
                                errCode = (int)CCEnums.CCErrorCodes.E0240;
                                throw new Exception(String.Format("{0}, Collection [{1}], Form index [{2}], Form type [{3}] Flow [{4}], Error code [{5}], to avoid this message set CCreator.ValidateFormType to false.", CCConstants.E0240, ccData.Name, formIdx, fr.FormType, ccData.FlowType, errCode));
                            }
                        }

                        formIdx++;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion

        #region "LockOrMoveCollectionFiles" function
        /// <summary>
        /// Lock or moe collection files (to the work directory).
        /// </summary>
        /// <param name="csm">ITisClientServicesModule object.</param>
        /// <param name="coll">The collection definition in this action.</param>
        /// <param name="collectionFilesLock">The collection files to lock \ move.</param>
        /// <param name="pagesFilesLock">Pages files \attachments to lock</param>
        /// <param name="errCode">The return error code.</param>
        /// <param name="collectionName">The collection name to create.</param>
        /// <returns>true when successfull, false when not.</returns>
        private bool LockOrMoveCollectionFiles(ITisClientServicesModule csm, CCCollection coll, CCFileList collectionFilesLock, ref List<CCFileList> pagesFilesLock, ref int errCode, String collectionName)
        {
            List<String> allFiles = new List<String>();

            try
            {
                allFiles.AddRange(collectionFilesLock.Files);                

                #region //-- Add page attachments --\\
                int pgCount = 0;
                String workDir = csm.PathLocator.get_Path(CCEnums.CCFilesExt.TIF.ToString());

                if (String.IsNullOrEmpty(collectionName))
                {
                    foreach (CCCollection.CCPage pg in coll.Pages)
                    {
                        pagesFilesLock.Add(new CCFileList());

                        if (pg.Attachments.Length > 0)
                        {
                            pgCount++;
                            using (CCFileList fLock = new CCFileList((pg.Attachments)))
                            {
                                try
                                {
                                    if (String.IsNullOrEmpty(collectionName) && !fLock.LockFiles(CurrentProfile.LockExtension, false, CurrentProfile.CopySourceFiles, false))
                                    {
                                        errCode = (int)CCEnums.CCErrorCodes.E0220;
                                        throw new Exception(String.Format("{0}, page number [{1}], error code [{2}]", CCConstants.E0220, pgCount, errCode));
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(collectionName) && !fLock.MoveFilesToWorkDirectory(workDir, collectionName, false, pgCount, CurrentProfile.LockExtension))
                                        {
                                            errCode = (int)CCEnums.CCErrorCodes.E0221;
                                            throw new Exception(String.Format("{0}, page number [{1}], work directory [{2}], error code [{3}]", CCConstants.E0221, pgCount, workDir, errCode));
                                        }
                                    }

                                    pagesFilesLock[pagesFilesLock.Count - 1].AddFiles(fLock.Files);
                                }
                                catch (Exception em)
                                {
                                    if (fLock != null) fLock.RollbackLock(CurrentProfile.LockExtension);
                                    throw em;
                                }
                            }
                        }
                    }

                    if (String.IsNullOrEmpty(collectionName)) collectionFilesLock.AddFiles(coll.Attachments);
                }
                else
                {
                    for (int icc = 0; icc < pagesFilesLock.Count; icc++)
                    {
                        if (pagesFilesLock[icc] != null && pagesFilesLock[icc].FileCount > 0)
                        {
                            pagesFilesLock[icc].MoveFilesToWorkDirectory(workDir, collectionName, false, icc + 1, CurrentProfile.LockExtension);
                        }
                    }
                }
                #endregion

                #region "//-- Copy collection attachments --\\            
                try
                {
                    if (String.IsNullOrEmpty(collectionName) && !collectionFilesLock.LockFiles(CurrentProfile.LockExtension, false, CurrentProfile.CopySourceFiles, false))
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0222;
                        throw new Exception(String.Format("{0}, collection [{1}], error code [{2}]", CCConstants.E0222, 0, errCode));
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(collectionName) && !collectionFilesLock.MoveFilesToWorkDirectory(workDir, collectionName, false, 0, CurrentProfile.LockExtension))
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0223;
                            throw new Exception(String.Format("{0}, collection [{1}], work directory [{2}], error code [{3}]", CCConstants.E0223, pgCount, workDir, errCode));
                        }
                        else if (!String.IsNullOrEmpty(collectionName) && pagesFilesLock != null && pagesFilesLock.Count>0)
                        {
                            //-- NF: 2012-07-07, make sure that attachment pages are added to the final list --\\
                            for (int icc = 0; icc < pagesFilesLock.Count; icc++)
                            {
                                if (pagesFilesLock[icc] != null && pagesFilesLock[icc].FileCount > 0)
                                {
                                    collectionFilesLock.AddFiles(pagesFilesLock[icc].Files);
                                }
                            }
                        }
                    }
                }
                catch (Exception en)
                {
                    if (collectionFilesLock != null) collectionFilesLock.RollbackLock(CurrentProfile.LockExtension);
                    throw en;
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
                if (!String.IsNullOrEmpty(CurrentProfile.ErrorFolderPath) && allFiles != null && allFiles.Count > 0)
                {
                    allFiles.AddRange(collectionFilesLock.Files);
                    CCFileList.MoveToFolder(CurrentProfile.ErrorFolderPath, allFiles.ToArray());
                }
                throw ex;
            }
        } 
        #endregion

        #region "CreateCollections" function
        /// <summary>
        /// Create collection\s.
        /// </summary>
        /// <param name="nextStation">Indicates the name of the next station</param>
        /// <param name="csm">ITisClientServicesModule object.</param>
        /// <param name="errCode">Returns the error code from thos function.</param>
        /// <param name="coll_info">The collection\s information to create</param>
        /// <returns>A String array with the names of the collections that were created.</returns>
        public String[] CreateCollections(string nextStation, ITisClientServicesModule csm , out int errCode, params CCCollection[] coll_info)
        {
            return CreateCollectionsBase(nextStation, ref csm, coll_info, out errCode);
        }

        /// <summary>
        /// Create collection\s.
        /// </summary>
        /// <param name="csm">ITisClientServicesModule object.</param>
        /// <param name="errCode">Returns the error code from thos function.</param>
        /// <param name="coll_info">The collection\s information to create</param>
        /// <returns>A String array with the names of the collections that were created.</returns>
        public String[] CreateCollections(ITisClientServicesModule csm, out int errCode, params CCCollection[] coll_info)
        {
            return CreateCollectionsBase(String.Empty, ref csm, coll_info, out errCode);
        }

        private string[] CreateCollectionsBase(string nextStation, ref ITisClientServicesModule csm, CCCollection[] coll_info, out int errCode)
        {
            #region //-- function variables --\\
            errCode = -1;
            CurrentCollection = null;
            List<String> result = new List<String>();
            List<ITisCollectionData> colls = new List<ITisCollectionData>();
            #endregion

            try
            {
                foreach (CCCollection coll in coll_info)
                {
                    //-- Prepare variables --\\
                    if (String.IsNullOrEmpty(coll.FlowType) && !String.IsNullOrEmpty(CurrentProfile.FlowType)) coll.FlowType = CurrentProfile.FlowType;
                    CurrentCollection = coll;
                    ITisCollectionData res = null;

                    CCFileList filesLock = new CCFileList(coll.ImagePath);
                    List<CCFileList> pagesLock = new List<CCFileList>();
                    bool canFree = true;

                    try
                    {
                        //-- NF: 2013-02-06 Fix FlowType if no Validation required, and flow type does not exist or isn't valid; select the first available flow --\\
                        String flowFixMsg = String.Empty;
                        if (!ValidateFlow && String.IsNullOrEmpty(coll.FlowType) || csm.Setup.get_Flow(coll.FlowType) == null)
                        {
                            if (csm.Setup.NumberOfFlows > 0)
                            {
                                String prvFlowName = coll.FlowType;
                                coll.FlowType = csm.Setup.get_FlowByIndex(0).Name;
                                flowFixMsg = String.Format("Collection [{0}] FlowType was either not valid or not defined, using flow [{1}] instead of [{2}], to throw an error instead o correcting this issue set the Collection Creator 'ValidateFlow' property to true, or set the FlowType in the InputApi profile you are using (method: [{3}]).", "!NAME_REPLACE!", coll.FlowType, prvFlowName, MethodBase.GetCurrentMethod().Name);
                            }
                        }

                        //-- Validate the collection data --\\
                        if (!ValidateCollection(csm, coll, ref errCode)) throw new Exception(String.Format("Collection [{0}] has invalid data, Error code [{1}]", coll.Name, errCode));

                        #region   //-- Check if required login to a different application --\\
                        bool canLogin = !String.IsNullOrEmpty(coll.LoginApplication) && !String.IsNullOrEmpty(coll.LoginStation) &&
                            (csm == null || (String.Compare(csm.Session.StationName, coll.LoginStation, true) != 0 && String.Compare(csm.Application.AppName, coll.LoginApplication, true) != 0));

                        if ((csm == null && canLogin) || canLogin)
                        {
                            csm = csmMangager.GetCsm(coll.LoginApplication, coll.LoginStation, true);
                        }
                        else
                        {
                            csmMangager.AddCsm(csm, false);
                        }

                        if (csm == null)
                        {
                            AddErrMsg(CCConstants.E0211 + MethodBase.GetCurrentMethod().Name);
                            ILog.LogError(CCConstants.E0211 + MethodBase.GetCurrentMethod().Name);
                            continue;
                        }
                        #endregion

                        //-- Lock collection files --\\
                        if (!LockOrMoveCollectionFiles(csm, coll, filesLock, ref pagesLock, ref errCode, null))
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0222;
                            throw new Exception(String.Format("{0}, error code [{1}]", CCConstants.E0222, errCode));
                        }

                        #region //-- Get \ create collection and validate the collection exists --\\
                        res = coll.UpdateExistingCollection || UpdateState != CCEnums.CCUpdateState.CreateNew ? csm.Dynamic.GetSpecificCollection(coll.Name, true) :
                                                                                                    String.IsNullOrEmpty(coll.Name) ? csm.Dynamic.CreateCollection(true) :
                                                                                                    csm.Dynamic.CreateSpecificCollection(coll.Name, true);

                        //-- if collection was not found and can create, create a collection --\\
                        if (res == null && UpdateState == CCEnums.CCUpdateState.CreateOrUpdate) res = String.IsNullOrEmpty(coll.Name) ? csm.Dynamic.CreateCollection(true) :
                                                                                                    csm.Dynamic.CreateSpecificCollection(coll.Name, true);


                        //-- Check that we have a collection --\\
                        if (res == null)
                        {
                            if (coll.UpdateExistingCollection || UpdateState == CCEnums.CCUpdateState.UpdateOnly)
                            {
                                ILog.LogInfo("Collection [{0}] does not exist and will not e updated in method [{1}]", coll.Name ?? String.Empty, MethodBase.GetCurrentMethod().Name);
                                continue;
                            }
                            else
                            {
                                errCode = (int)CCEnums.CCErrorCodes.E0231;
                                String errMessage = String.Format("{0}, collection name [{1}], error code [{2}]", CCConstants.E0231, coll.Name ?? String.Empty, MethodBase.GetCurrentMethod().Name, errCode);
                                ILog.LogInfo(errMessage);
                                throw new Exception(errMessage);
                            }
                        }
                        else
                        {
                            colls.Add(res);
                            if (!String.IsNullOrEmpty(flowFixMsg))
                            {
                                flowFixMsg = flowFixMsg.Replace("!NAME_REPLACE!", res.Name);
                                ILog.LogDebug(flowFixMsg);
                            }
                        }
                        #endregion

                        #region Set collection data + name, add stamp data
                        res.FlowType = coll.FlowType;

                        //-- Add stamp data if flagged --\\
                        if (CurrentProfile.UseSourceNamedUserTags)
                        {
                            string srcCaller = this.GetType().Name;
                            if (!coll.NamedUserTags.NativeDictionary.ContainsKey(srcCaller))
                            {
                                coll.NamedUserTags.NativeDictionary.Add(srcCaller, String.Format("{0}={1}", res.Name ?? String.Empty, coll.Name ?? String.Empty));
                            }

                            if (!coll.NamedUserTags.NativeDictionary.ContainsKey("SOURCE:" + srcCaller))
                            {
                                coll.NamedUserTags.NativeDictionary.Add("SOURCE:" + srcCaller, SourceDataStamp.GetCallingDll(4) + ", " + SourceDataStamp.StampData);
                            }
                        }

                        if (!CurrentProfile.IgnoreExceptions) CCUtils.SetSpecialTags(res, coll.SpecialTags.NativeDictionary);
                        if (!CurrentProfile.IgnoreNamedUserTags) CCUtils.SetNamedUserTags(res, coll.NamedUserTags.NativeDictionary);
                        if (!CurrentProfile.IgnoreUserTags) CCUtils.SetUserTags(res, coll.UserTags.NativeDictionary);



                        //-- Rename collection if necessary --\\
                        try { if (!String.IsNullOrEmpty(coll.Name) && String.Compare(coll.Name, res.Name, true) != 0) res.Rename(coll.Name); }
                        catch (Exception er)
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0010;
                            AddErrMsg(er.Message);
                            ILog.LogError(er);
                            if (CurrentProfile.ThrowAllExceptions) throw er;
                        }

                        //-- Set the collection priority --\\
                        if (coll.Priority != WorkflowPriorityLevel.Normal)
                        {
                            res.PriorityLevel = coll.Priority;
                        }

                        //-- Set the collection Absolute priority --\\
                        if (coll.AbsolutePriority != 0)
                        {
                            res.AbsolutePriority = coll.AbsolutePriority;
                        }
                        #endregion

                        //-- Move collection files to the work directory under collection name --\\
                        if (!LockOrMoveCollectionFiles(csm, coll, filesLock, ref pagesLock, ref errCode, res.Name))
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0223;
                            throw new Exception(String.Format("{0}, error code [{1}]", CCConstants.E0223, errCode));
                        }


                        //-- Add the created collection to the result --\\
                        result.Add(res.Name);


                        //-- Create the collection's forms --\\
                        CreateForms(res, coll.Forms);


                        #region handle create collection event
                        bool canPut = res != null;
                        if (CollectionCreator.OnCollectionCreated != null) CollectionCreator.OnCollectionCreated(csm, res);

                        if (!CollectionCreated(csm, res))
                        {
                            canFree = false;
                            res = null;
                            filesLock.Dispose();
                            continue;
                        }
                        #endregion

                        if (nextStation != String.Empty)
                            coll.TargetQueue = nextStation;

                        #region  //-- Move to target queue (if any specified) --\\
                        if (!String.IsNullOrEmpty(coll.TargetQueue))
                        {
                            //-- Make sure the name of the station is in the correct case --\\
                            ITisStationParams sta = csm.Setup.get_Station(coll.TargetQueue);
                            if (sta != null && csm.DynamicQuery.StationQueues.Contains(sta.Name))
                            {
                                try
                                {
                                    //-- Save collection attachemnst --\\
                                    try { res.AttachedFileManager.SaveAttachments(filesLock.Files, TIS_ATTACHMENT_EXISTS_ACTION.TIS_EXISTING_OVERRIDE); }
                                    catch (Exception ei) { ILog.LogError(ei); }

                                    csm.Dynamic.set_StampAsFinalByObject(res, true);
                                    csm.Dynamic.FreeSpecificCollection(res, true);
                                    csm.Dynamic.FreeCollections(true, false);
                                    filesLock.DeleteFiles();
                                    canFree = false;

                                    //-- Move the collection --\\
                                    csm.DynamicManage.MoveCollectionToQueueByName(res.Name, sta.Name);
                                    continue;
                                }
                                catch (Exception eg)
                                {
                                    AddErrMsg(eg.Message);
                                    ILog.LogError(eg);
                                    if (CurrentProfile.ThrowAllExceptions) throw eg;
                                }
                            }
                        }
                        #endregion
                    }
                    catch (Exception ep)
                    {
                        AddErrMsg(ep.Message);
                        ILog.LogError(ep);

                        //-- Move all involved files to error folder if such is specified --\\
                        if (!String.IsNullOrEmpty(CurrentProfile.ErrorFolderPath))
                        {
                            CCFileList.MoveToFolder(CurrentProfile.ErrorFolderPath, coll.Files);
                        }

                        if (CurrentProfile.ThrowAllExceptions) throw ep;
                    }
                    finally
                    {
                        if (csm != null && res != null && canFree)
                        {
                            //res.AttachedFileManager.
                            //-- Save collection attachemnst --\\
                            try { res.AttachedFileManager.SaveAttachments(filesLock.Files, TIS_ATTACHMENT_EXISTS_ACTION.TIS_EXISTING_OVERRIDE); }
                            catch (Exception ei) { ILog.LogError(ei); }

                            //-- free the collection\\s --\\  
                            csm.Dynamic.set_StampAsFinalByObject(res, true);
                            csm.Dynamic.FreeSpecificCollection(res, true);
                            csm.Dynamic.FreeCollections(true, false);

                            filesLock.DeleteFiles();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrMsg(ex.Message);
                ILog.LogError(ex);
                if (CurrentProfile.ThrowAllExceptions) throw ex;
            }
            finally
            {
                CurrentCollection = null;
            }
            return result.ToArray();
        }
        #endregion

        #region "CreateForms" function
        /// <summary>
        /// Add\create forms to a collection.
        /// </summary>
        /// <param name="collection">The ITisCollectionData to create forms in</param>
        /// <param name="form_info">The forms creation information array.</param>
        /// <returns>An array of created ITisFormData in the specified collection</returns>
        public ITisFormData[] CreateForms(ITisCollectionData collection, params CCCollection.CCForm[] form_info)
        {
            List<ITisFormData> result = new List<ITisFormData>();
            try
            {
                int formCount = 0;
                int exitingForms = collection.NumberOfForms;

                foreach (CCCollection.CCForm form in form_info)
                {
                    formCount++;
                    try
                    {
                        //-- Get a form for update --\\
                        ITisFormData eflowForm = UpdateState != CCEnums.CCUpdateState.CreateNew && !String.IsNullOrEmpty(form.Name) ? collection.get_Form(form.Name) : null;

                        if (eflowForm == null && UpdateState != CCEnums.CCUpdateState.UpdateOnly)
                        {
                            //-- Create a form --\\
                            String nextFormName = String.Format("FP{0}", (formCount + exitingForms).ToString("X").PadLeft(4, '0'));
                            while (collection.get_Form(nextFormName) != null)
                            {
                                formCount++;
                                nextFormName = String.Format("FP{0}", (formCount + exitingForms).ToString("X").PadLeft(4, '0'));
                            }

                            eflowForm = collection.CreateForm(nextFormName);
                        }

                        //-- if a form exists use it --\\
                        if (eflowForm != null)
                        {
                            //-- Save original intended fro m name --\\
                            if (CurrentProfile.UseSourceNamedUserTags) eflowForm.set_NamedUserTags(this.GetType().Name + "." + CCEnums.CCNames.SourceName.ToString(), form.Name);

                            result.Add(eflowForm);

                            //-- Set SpecialTags (SpecialTags),  NamedUserTags and UserTags --\\
                            if (!CurrentProfile.IgnoreExceptions) CCUtils.SetSpecialTags(eflowForm, form.SpecialTags.NativeDictionary);
                            if (!CurrentProfile.IgnoreNamedUserTags) CCUtils.SetNamedUserTags(eflowForm, form.NamedUserTags.NativeDictionary);
                            if (!CurrentProfile.IgnoreUserTags) CCUtils.SetUserTags(eflowForm, form.UserTags.NativeDictionary);

                            //-- Define FormType --\\
                            eflowForm.FormType = form.FormType;

                            //-- Try to rename the form if other named specified --\\
                            if (!String.IsNullOrEmpty(form.Name) && String.Compare(form.Name, eflowForm.Name, true) != 0) eflowForm.Rename(form.Name);

                            //-- Add page\\s --\\
                            if (form.Pages != null && form.Pages.Length > 0) CreatePages(eflowForm, form.Pages);
                        }
                    }
                    catch (Exception ep)
                    {
                        AddErrMsg(ep.Message);
                        ILog.LogError(ep);
                        if (CurrentProfile.ThrowAllExceptions) throw ep;
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrMsg(ex.Message);
                ILog.LogError(ex);
                if (CurrentProfile.ThrowAllExceptions) throw ex;
            }
            return result.ToArray();
        }
        #endregion

        #region "CreatePages" function
        /// <summary>
        /// Add\create form's page\s.
        /// </summary>
        /// <param name="form">The ITisFormData to add the page\s to.</param>
        /// <param name="page_info">The pages settings to create.</param>
        /// <returns>An arry of the ITisPageData that were created linked to the specified eFlow form.</returns>
        public ITisPageData[] CreatePages(ITisFormData form, params CCCollection.CCPage[] page_info)
        {
            List<ITisPageData> result = new List<ITisPageData>();
            try
            {
                int pageCount = 0;
                int existingPages = form.ParentCollection.NumberOfPages;

                foreach (CCCollection.CCPage page in page_info)
                {
                    pageCount++;
                    try
                    {
                        //-- Get a page for update --\\
                        ITisPageData eflowPage = UpdateState != CCEnums.CCUpdateState.CreateNew && !String.IsNullOrEmpty(page.Name) ? form.ParentCollection.get_Page(page.Name) : null;

                        if (eflowPage == null && UpdateState != CCEnums.CCUpdateState.UpdateOnly)
                        {
                            //-- Create a page --\\
                            String nextPageName = GetPageName(pageCount + existingPages);
                            while (form.ParentCollection.get_Page(nextPageName) != null)
                            {
                                pageCount++;
                                nextPageName = GetPageName(pageCount + existingPages);
                            }
                            eflowPage = form.ParentCollection.CreatePage(GetPageName(pageCount + existingPages));
                            form.AddPageLink(eflowPage);
                        }

                        //-- if a page exists use it --\\
                        if (eflowPage != null)
                        {
                            result.Add(eflowPage);

                            //-- Set page ID (does nothing) --\\
                           eflowPage.PageId = page.PageID;
                            
                            //-- Set NameduserTags and UserTags (no specialTags on page level) --\\
                           if (!CurrentProfile.IgnoreNamedUserTags) CCUtils.SetNamedUserTags(eflowPage, page.NamedUserTags.NativeDictionary);
                           if (!CurrentProfile.IgnoreNamedUserTags) CCUtils.SetUserTags(eflowPage, page.UserTags.NativeDictionary);

                            //-- Save original intended page name --\\
                           if (CurrentProfile.UseSourceNamedUserTags) eflowPage.set_NamedUserTags(this.GetType().Name + "." + CCEnums.CCNames.SourceName.ToString(), page.Name);
                           if (CurrentProfile.UseSourceNamedUserTags) eflowPage.set_NamedUserTags(this.GetType().Name + "." + CCEnums.CCNames.SourcePageID.ToString(), page.PageID);

                            //-- Create linked field groups (and fields...) --\\
                            if (page.LinkedGroups != null && page.LinkedGroups.Length > 0) CreateGroups(eflowPage, page.LinkedGroups);


                            //-- Rename the page to the required name --\\
                            if (!String.IsNullOrEmpty(page.Name) && form.ParentCollection.get_Page(page.Name) == null) eflowPage.Rename(page.Name);
                        }
                    }
                    catch (Exception ep)
                    {
                        AddErrMsg(ep.Message);
                        ILog.LogError(ep);
                        if (CurrentProfile.ThrowAllExceptions) throw ep;
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrMsg(ex.Message);
                ILog.LogError(ex);
                if (CurrentProfile.ThrowAllExceptions) throw ex;
            }
            return result.ToArray();
        }
        #endregion

        #region "CreateGroups" function
        /// <summary>
        /// Create the field groups of a page
        /// </summary>
        /// <param name="page">The page to add the group\\s to.</param>
        /// <param name="group_info">The groups settings to create eflow field-groups with.</param>
        /// <returns>And array of the created FiledGroups</returns>
        public ITisFieldGroupData[] CreateGroups(ITisPageData page, params CCCollection.CCGroup[] group_info)
        {
            List<ITisFieldGroupData> result = new List<ITisFieldGroupData>();
            try
            {
                foreach (CCCollection.CCGroup group in group_info)
                {
                    try
                    {
                        //-- Get a field group for update --\\
                        ITisFieldGroupData eflowGroup = UpdateState != CCEnums.CCUpdateState.CreateNew && !String.IsNullOrEmpty(group.Name) ? page.ParentForm.get_LinkedFieldGroup(group.Name) : null;

                        //-- Create a field group --\\
                        if (eflowGroup == null && UpdateState != CCEnums.CCUpdateState.UpdateOnly)
                        {
                            eflowGroup = page.ParentCollection.CreateFieldGroup(group.Name);
                            page.ParentForm.AddFieldGroupLink(eflowGroup);
                        }

                        //-- if a field group exists use it --\\
                        if (eflowGroup != null)
                        {
                            result.Add(eflowGroup);

                            //-- Set SpecialTags, NamedUserTags and UserTags --\\
                            if (!CurrentProfile.IgnoreExceptions) CCUtils.SetSpecialTags(eflowGroup, group.SpecialTags.NativeDictionary);
                            if (!CurrentProfile.IgnoreNamedUserTags) CCUtils.SetNamedUserTags(eflowGroup, group.NamedUserTags.NativeDictionary);
                            if (!CurrentProfile.IgnoreUserTags) CCUtils.SetNamedUserTags(eflowGroup, group.UserTags.NativeDictionary);

                            //--Create Group linked fields --\\
                            if (group.LinkedFields != null && group.LinkedFields.Length > 0) CreateFields(page, eflowGroup, group.LinkedFields);

                            //-- Create group linked tables --\\
                            if (group.LinkedTables != null && group.LinkedTables.Length > 0) CreateTables(page, eflowGroup, group.LinkedTables);
                        }
                    }
                    catch (Exception ep)
                    {
                        AddErrMsg(ep.Message);
                        ILog.LogError(ep);
                        if (CurrentProfile.ThrowAllExceptions) throw ep;
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrMsg(ex.Message);
                ILog.LogError(ex);
                if (CurrentProfile.ThrowAllExceptions) throw ex;
            }
            return result.ToArray();
        }
        #endregion

        #region "CreateTables" function
        /// <summary>
        /// Create all the fields specified in a field group.
        /// </summary>
        /// <param name="page">The ITisPageData to create the table\s on.</param>
        /// <param name="group">The field-group to link the tabl eto.</param>
        /// <param name="fieldTable_info">The table\s setting to create with.</param>
        /// <returns>An array of created ITisFieldTableData\s.</returns>
        public ITisFieldTableData[] CreateTables(ITisPageData page, ITisFieldGroupData group, params CCCollection.CCTable[] fieldTable_info)
        {
            List<ITisFieldTableData> result = new List<ITisFieldTableData>();
            try
            {
                int tableCount = 0;
                foreach (CCCollection.CCTable fieldTbl in fieldTable_info)
                {
                    tableCount++;
                    try
                    {
                        //-- Get a field table for update --\\
                        ITisFieldTableData eflowFieldTable = UpdateState != CCEnums.CCUpdateState.CreateNew && !String.IsNullOrEmpty(fieldTbl.Name) ? group.get_LinkedFieldTable(fieldTbl.Name) : null;

                        //-- Create a field table --\\
                        if (eflowFieldTable == null && UpdateState != CCEnums.CCUpdateState.UpdateOnly)
                        {
                            eflowFieldTable = group.ParentForm.CreateFieldTable(fieldTbl.Name);
                            if (page != null) page.AddFieldTableLink(eflowFieldTable);
                            if (group != null) group.AddFieldTableLink(eflowFieldTable);
                        }

                        //-- if a field table exists use it --\\
                        if (eflowFieldTable != null)
                        {
                            result.Add(eflowFieldTable);

                            //-- Set SpecialTags, NamedUserTags and UserTags --\\
                            if (!CurrentProfile.IgnoreExceptions) CCUtils.SetSpecialTags(eflowFieldTable, fieldTbl.SpecialTags.NativeDictionary);
                            if (!CurrentProfile.IgnoreNamedUserTags) CCUtils.SetNamedUserTags(eflowFieldTable, fieldTbl.NamedUserTags.NativeDictionary);
                            if (!CurrentProfile.IgnoreUserTags) CCUtils.SetUserTags(eflowFieldTable, fieldTbl.UserTags.NativeDictionary);

                            //-- Create Field arrays and fields --\\
                            CreateFieldArrays(page, eflowFieldTable, fieldTbl.FieldArrays);
                        }
                    }
                    catch (Exception ep)
                    {
                        AddErrMsg(ep.Message);
                        ILog.LogError(ep);
                        if (CurrentProfile.ThrowAllExceptions) throw ep;
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrMsg(ex.Message);
                ILog.LogError(ex);
                if (CurrentProfile.ThrowAllExceptions) throw ex;
            }
            return result.ToArray();
        }
        #endregion

        #region "CreateFieldArrays" function
        /// <summary>
        /// Create all the fields specified in a field array.
        /// </summary>
        /// <param name="page">The eFlow page to link the field array and its sub componenets</param>
        /// <param name="table">The field table to link the field array\s to</param>
        /// <param name="fieldArray_info">The field arrays infor to create.</param>
        /// <returns>An array if the created fieldArrays.</returns>
        public ITisFieldArrayData[] CreateFieldArrays(ITisPageData page, ITisFieldTableData table, params CCCollection.CCFieldArray[] fieldArray_info)
        {
            List<ITisFieldArrayData> result = new List<ITisFieldArrayData>();
            try
            {
                foreach (CCCollection.CCFieldArray fieldArray in fieldArray_info)
                {
                    try
                    {
                        //-- Get a field array for update --\\
                        ITisFieldArrayData eflowFieldArray = UpdateState != CCEnums.CCUpdateState.CreateNew && !String.IsNullOrEmpty(fieldArray.Name) ? table.get_FieldArray(fieldArray.Name) : null;

                        //-- Create a field array --\\
                        if (eflowFieldArray == null && UpdateState != CCEnums.CCUpdateState.UpdateOnly) eflowFieldArray = table.CreateFieldArray(fieldArray.Name);

                        if (eflowFieldArray != null)
                        {
                            //-- if a field exists use it --\\
                            result.Add(eflowFieldArray);

                            //-- Set SpecialTags, NamedUserTags and UserTags --\\
                            if (!CurrentProfile.IgnoreExceptions) CCUtils.SetSpecialTags(eflowFieldArray, fieldArray.SpecialTags.NativeDictionary);
                            if (!CurrentProfile.IgnoreNamedUserTags) CCUtils.SetNamedUserTags(eflowFieldArray, fieldArray.NamedUserTags.NativeDictionary);
                            if (!CurrentProfile.IgnoreUserTags) CCUtils.SetUserTags(eflowFieldArray, fieldArray.UserTags.NativeDictionary);

                            //-- Create child fields --\\
                            CreateFields(page, eflowFieldArray, fieldArray.LinkedFields);
                        }
                    }
                    catch (Exception ep)
                    {
                        AddErrMsg(ep.Message);
                        ILog.LogError(ep);
                        if (CurrentProfile.ThrowAllExceptions) throw ep;
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrMsg(ex.Message);
                ILog.LogError(ex);
                if (CurrentProfile.ThrowAllExceptions) throw ex;
            }
            return result.ToArray();
        }
        #endregion

        #region "CreateFields" functions
        /// <summary>
        /// Create all the fields specified in a field group.
        /// </summary>
        /// <param name="page">The page to link the field to.</param>
        /// <param name="group">The field group to link the field to.</param>
        /// <param name="field_info">The field\s to create under the field group.</param>
        /// <returns>An array of the created fields.</returns>
        public ITisFieldData[] CreateFields(ITisPageData page, ITisFieldGroupData group, params CCCollection.CCField[] field_info)
        {
            List<ITisFieldData> result = new List<ITisFieldData>();
            try
            {
                int fieldCount = 0;
                foreach (CCCollection.CCField field in field_info)
                {
                    fieldCount++;
                    try
                    {
                        //-- Get a field for update --\\
                        ITisFieldData eflowField = UpdateState != CCEnums.CCUpdateState.CreateNew && !String.IsNullOrEmpty(field.Name) ? group.ParentForm.get_Field(field.Name) : null;
                        if (eflowField == null && UpdateState != CCEnums.CCUpdateState.UpdateOnly)
                        {
                            //-- Create a field --\\
                            eflowField = group.ParentForm.CreateField(field.Name);
                            if (page != null) page.AddFieldLink(eflowField);
                            if (group != null) group.AddFieldLink(eflowField);
                        }

                        //-- if a field exists use it --\\
                        if (eflowField != null)
                        {                          
                            result.Add(eflowField);

                            //-- Set SpecialTags, NamedUserTags and UserTags --\\
                            if (!CurrentProfile.IgnoreExceptions) CCUtils.SetSpecialTags(eflowField, field.SpecialTags.NativeDictionary);
                            if (!CurrentProfile.IgnoreNamedUserTags) CCUtils.SetNamedUserTags(eflowField, field.NamedUserTags.NativeDictionary);
                            if (!CurrentProfile.IgnoreUserTags) CCUtils.SetNamedUserTags(eflowField, field.UserTags.NativeDictionary);

                            //-- Set field properties --\\
                            eflowField.Confidence = (short)field.Confidence;
                            eflowField.Contents = field.Contents;
                            eflowField.FieldBoundingRect = field.Rect.TisRect;
                        }
                    }
                    catch (Exception ep)
                    {
                        AddErrMsg(ep.Message);
                        ILog.LogError(ep);
                        if (CurrentProfile.ThrowAllExceptions) throw ep;
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrMsg(ex.Message);
                ILog.LogError(ex);
                if (CurrentProfile.ThrowAllExceptions) throw ex;
            }
            return result.ToArray();
        }

        /// <summary>
        /// Create all the fields specified in a field array.
        /// </summary>
        /// <param name="page">The page to link the field to.</param>
        /// <param name="fieldArray">The field array to link the field to.</param>
        /// <param name="field_info">The field\s to create under the field array.</param>
        /// <returns>An array of the created fields.</returns>
        public ITisFieldData[] CreateFields(ITisPageData page, ITisFieldArrayData fieldArray, params CCCollection.CCField[] field_info)
        {
            List<ITisFieldData> result = new List<ITisFieldData>();
            try
            {
                int fieldCount = 0;
                foreach (CCCollection.CCField field in field_info)
                {
                    fieldCount++;
                    try
                    {
                        //-- Get a field for update --\\
                        ITisFieldData eflowField = UpdateState != CCEnums.CCUpdateState.CreateNew && !String.IsNullOrEmpty(field.Name) ? fieldArray.ParentForm.get_Field(field.Name) : null;
                        if (eflowField == null && UpdateState != CCEnums.CCUpdateState.UpdateOnly)
                        {
                            //-- Create a field --\\
                            eflowField = fieldArray.ParentForm.CreateField(field.Name);
                            if (page != null) page.AddFieldLink(eflowField);
                            if (fieldArray != null) fieldArray.AddFieldLink(eflowField);
                        }

                        //-- if a field exists use it --\\
                        if (eflowField != null)
                        {
                            result.Add(eflowField);

                            //-- Set SpecialTags, NamedUserTags and UserTags --\\
                            if (!CurrentProfile.IgnoreExceptions) CCUtils.SetSpecialTags(eflowField, field.SpecialTags.NativeDictionary);
                            if (!CurrentProfile.IgnoreNamedUserTags) CCUtils.SetNamedUserTags(eflowField, field.NamedUserTags.NativeDictionary);
                            if (!CurrentProfile.IgnoreUserTags) CCUtils.SetUserTags(eflowField, field.UserTags.NativeDictionary);

                            //-- Set field properties --\\
                            eflowField.Confidence = (short)field.Confidence;
                            eflowField.Contents = field.Contents;
                            eflowField.FieldBoundingRect = field.Rect.TisRect;
                        }
                    }
                    catch (Exception ep)
                    {
                        AddErrMsg(ep.Message);
                        ILog.LogError(ep);
                        if (CurrentProfile.ThrowAllExceptions) throw ep;
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrMsg(ex.Message);
                ILog.LogError(ex);
                if (CurrentProfile.ThrowAllExceptions) throw ex;
            }
            return result.ToArray();
        }
        #endregion

        #region "FromImages" function
        /// <summary>
        /// Create a CCCollection from image\s.
        /// </summary>
        /// <param name="workDirectory">The work directory to copy the collection image to.</param>
        /// <param name="saveImageName">Will us ethe first image in the specified filePaths to define the collection name when true.</param>
        /// <param name="multiPagePerForm">When true, all pages from an image in the list will be under a single form (i.e. a form with multipages per image).</param>
        /// <param name="deleteSource">Delete source images when true.</param>
        /// <param name="filePaths">The image\s to create the collection from.</param>
        /// <returns>CCCollection when successfull, null when not.</returns>
        public CCCollection FromImages(String workDirectory, bool saveImageName, bool multiPagePerForm, bool deleteSource, params String[] filePaths)
        {
            String createdImage = null;
            try
            {
                List<CCCollection.CCForm> frms = new List<CCCollection.CCForm>();

                #region //-- Prepare a temp file name --\\
                createdImage = Path.Combine(workDirectory, String.Format("{0:yyyyMMdd_HHmmss_ffff}.{1}", DateTime.Now, CCEnums.CCFilesExt.TIF));
                int counter = 0;
                while (File.Exists(createdImage))
                {
                    counter++;
                    createdImage = Path.Combine(workDirectory, String.Format("{0:yyyyMMdd_HHmmss_ffff{1}.{2}", DateTime.Now, counter, CCEnums.CCFilesExt.TIF));
                } 
                #endregion


                #region  //-- Create the form -> page structure --\\
                int pageCount = 0;
                int imageCount = 0;
                foreach (String sf in filePaths)
                {
                    imageCount++;
                    int pagesInImage = CCUtils.GetImagePageCount(sf, OnPageRead);
                    List<CCCollection.CCPage> pgs = new List<CCCollection.CCPage>();

                    for (int i = 1; i <= pagesInImage; i++)
                    {
                        pageCount++;
                        if (multiPagePerForm)
                        {
                            pgs.Add(new CCCollection.CCPage(this, null, null, null));
                        }
                        else
                        {
                            frms.Add(new CCCollection.CCForm(this, null, null, new CCCollection.CCPage(this, null, null, null)));
                        }
                    }


                    //-- in multi page per form, add the pages to a single form --\\
                    if (multiPagePerForm && pgs != null && pgs.Count > 0)
                    {
                        frms.Add(new CCCollection.CCForm(this, null, null, pgs.ToArray()));
                    }
                }
                #endregion

                //-- Merge all the images --\\
                if (CCUtils.MergeImages(createdImage, deleteSource, filePaths))
                {
                    //-- Create a collection --\\
                    CCCollection res = new CCCollection(this, null, frms.ToArray());
                   if (saveImageName) res.Name = Path.GetFileNameWithoutExtension(filePaths[0]);
                    res.ImagePath = createdImage;
                    return res;
                }
                else
                {
                    ILog.LogWarning("Failed processing collection image\\s: "+String.Join(Path.PathSeparator.ToString(), filePaths));
                }
            }
            catch (Exception ex)
            {
                AddErrMsg(ex.Message);
                ILog.LogError(ex);
                if (CurrentProfile.ThrowAllExceptions) throw ex;
            }
            return null;
        } 
        #endregion

        #region "FromFile" function
        /// <summary>
        /// Create a CCCollection from file, use the <see cref="OnCustomSourceFileRead"/> event to create the <see cref="CCCollection"/> result or use an XML file to desrialize the settings.
        /// </summary>
        /// <param name="csm">ITisClientServicesModule object.</param>
        /// <param name="deleteSource">Delete source images when true.</param>
        /// <param name="filePath">The file path to create the collection from.</param>
        /// <returns>A CCCollection when successfull, null when not.</returns>
        public CCCollection FromFile(ITisClientServicesModule csm, bool deleteSource, String filePath)
        {
            try
            {
                if (OnCustomSourceFileRead != null)
                {
                    return OnCustomSourceFileRead(this, deleteSource, filePath);
                }
                else
                {
                    return CCCollection.FromXml(filePath, deleteSource);
                }
            }
            catch (Exception ex)
            {
                AddErrMsg(ex.Message);
                ILog.LogError(ex);
                if (CurrentProfile.ThrowAllExceptions) throw ex;
            }
            return null;
        }
        #endregion
    }
}