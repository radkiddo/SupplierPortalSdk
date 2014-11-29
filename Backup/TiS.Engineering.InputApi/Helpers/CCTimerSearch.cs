using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiS.Core.eFlowAPI;
using TiS.Core.PlatformRuntime;

namespace TiS.Engineering.InputApi
{
    /// <summary>
    /// A calsss that can be used s an exe or instanced to operate as folder monitor, polling station.
    /// </summary>
#if INTERNAL
    internal class CCTimerSearch : ApplicationContext, ICCTimerSearch
#else
    public class CCTimerSearch : ApplicationContext, ICCTimerSearch
#endif
    {
        #region Class variables
        private CCCollection currentCollection;
        private Timer pollingTimer;        
        private CCConfiguration config;
        private CCConfiguration.CCConfigurationData currentProfile;//-- required a s a variable to be able to use this property as a ref variable (using the internal variable).        
        private bool processing;
        private String workDir;
        private bool stopTimerSearch;
        #endregion

        #region Events
        /// <summary>
        /// This event will contain the referenced CCCollection data object that will be used to create an eFlow collection, setting it to null will eliminate collection creation.
        /// </summary>
        public virtual event CCDelegates.OnPreCreateCollectionEvt OnPreCreateCollection;

        /// <summary>
        /// This event will contain the name of the eFlow collection that was created using a CCCOllection data object.
        /// </summary>
        public virtual event CCDelegates.OnPostCreateCollectionEvt OnPostCreateCollection;

        /// <summary>
        /// Conatins a referenced file list to review , edit or set to null to cancel this search.
        /// </summary>
        public virtual event CCDelegates.OnSearchedFilesEvt OnSearchedFiles;

        /// <summary>
        /// Fires every time that the search timer has finished an interval.
        /// </summary>
        public virtual event EventHandler OnTimer;

        /// <summary>
        /// Fired when this object is created.
        /// </summary>
        public virtual event CCDelegates.OnBaseActionEvt OnCreate;

        /// <summary>
        /// Fired when this object is being disposed.
        /// </summary>
        public virtual event CCDelegates.OnBaseActionEvt OnDispose;

        /// <summary>
        /// This event will contain the ITisCollectionData object that was created using a CCCOllection data object.
        /// </summary>
        public virtual event CCDelegates.OnCollectionCreatedEvt OnCollectionCreated;

        /// <summary>
        /// This event is fire for every key\trigger file that is being locked, eferencing the file name, to review, edit or set to null to eliminate lock.
        /// </summary>
        public virtual event CCDelegates.OnPreFileLockEvt OnPreFileLock;

        /// <summary>
        /// This event is fire for every key\trigger file that was successfully locked, containing the file name.
        /// </summary>
        public virtual event CCDelegates.OnPostFileLockEvt OnPostFileLock;

        #region "OnPageRead" event
        /// <summary>
        /// When this event is assinged every page in the key image will fire this event, passing a Bitmap object of the each page, this event can possibly slow things down so use with care.
        /// </summary>
        public virtual event CCDelegates.OnPageReadEvt OnPageRead
        {
            add { if (CollectionsCreator != null) CollectionsCreator.OnPageRead += value; }
            remove { if (CollectionsCreator != null) CollectionsCreator.OnPageRead -= value; }
        }
        #endregion

        #region "PreCreateCollection" method
        /// <summary>
        /// The method that fires the 'OnPreCreateCollection' event
        /// </summary>
        protected virtual void PreCreateCollection()
        {
            try
            {
                if (OnPreCreateCollection != null) OnPreCreateCollection(this, ref currentCollection);
            }
            catch (Exception ep)
            {
                ILog.LogError(ep);
            }
        }
        #endregion

        #region "PostFileLock" method
        /// <summary>
        /// The method that fires the 'OnPostFileLock' event
        /// </summary>
        /// <param name="cApi"></param>
        /// <param name="fileHandler"></param>
        /// <param name="fileName"></param>
        protected virtual void PostFileLock(CCTimerSearch cApi, object fileHandler, String fileName)
        {
            try
            {
                if (OnPostFileLock != null) OnPostFileLock(this, fileHandler, fileName);
            }
            catch (Exception ep)
            {
                ILog.LogError(ep);
            }
        }
        #endregion

        #region "PreFileLock" method\event
        /// <summary>
        /// The method that fires the 'OnPreFileLock' event.
        /// </summary>
        /// <param name="cApi"></param>
        /// <param name="filesHandler"></param>
        /// <param name="fileName"></param>
        protected virtual void PreFileLock(CCTimerSearch cApi, object filesHandler, ref String fileName)
        {
            try
            {
                if (OnPreFileLock != null) OnPreFileLock(this, filesHandler, ref fileName);
            }
            catch (Exception ep)
            {
                ILog.LogError(ep);
            }
        }
        #endregion

        #region "CollectionCreated" method
        /// <summary>
        /// The method that fires the 'OnCollectionCreated' event
        /// </summary>
        /// <param name="cApi"></param>
        /// <param name="creator"></param>
        /// <param name="csm"></param>
        /// <param name="collection"></param>
        /// <param name="canPut"></param>
        protected virtual void CollectionCreated(CCTimerSearch cApi, CCreator creator, ITisClientServicesModule csm, ITisCollectionData collection, ref bool canPut)
        {
            try
            {
                if (OnCollectionCreated != null) OnCollectionCreated(this, creator, csm, collection, ref canPut);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        }
        #endregion

        #region "PostCreateCollection" method
        /// <summary>
        /// The method that fires the 'OnPostCreateCollection' event
        /// </summary>
        /// <param name="collectionName"></param>
        protected virtual void PostCreateCollection(String collectionName)
        {
            try
            {
                if (OnPostCreateCollection != null) OnPostCreateCollection(this, collectionName);
            }
            catch (Exception ep)
            {
                ILog.LogError(ep);
            }
        }
        #endregion

        #region "SearchedFiles" method
        /// <summary>
        /// Th emethod that fires the 'OnSearchedFiles' event.
        /// </summary>
        /// <param name="collectedFiles"></param>
        protected virtual void SearchedFiles(ref CCFileList[] collectedFiles)
        {
            try
            {
                if (OnSearchedFiles != null) OnSearchedFiles(this, ref collectedFiles);
            }
            catch (Exception ep)
            {
                ILog.LogError(ep);
            }
        }
        #endregion

        #region "TimerEvent" method
        /// <summary>
        /// The method that fired the 'OnTimer' event.
        /// </summary>
        protected virtual void TimerEvent()
        {
            try
            {
                if (OnTimer != null) OnTimer(this, new EventArgs());
            }
            catch (Exception ep)
            {
                ILog.LogError(ep);
            }
        }
        #endregion

        #region "CreateEvent" method
        /// <summary>
        /// The method that fires the 'OnCreate' event.
        /// </summary>
        protected virtual void CreateEvent()
        {
            try
            {
                if (OnCreate != null) OnCreate(this);
            }
            catch (Exception ep)
            {
                ILog.LogError(ep);
            }
        }
        #endregion

        #region "DisposeEvent" method
        /// <summary>
        /// The method that fires the 'OnDispose' event.
        /// </summary>
        protected virtual void DisposeEvent()
        {
            try
            {
                if (OnDispose != null) OnDispose(this);
            }
            catch (Exception ep)
            {
                ILog.LogError(ep);
            }
        }
        #endregion
        #endregion

        #region class constructors
        public CCTimerSearch(ITisClientServicesModule oCSM)
            : this(oCSM, null, null)
        {
        }

        public CCTimerSearch(ITisClientServicesModule oCSM, CCConfiguration.CCConfigurationData dataCfg)
        {

            try
            {
                CSM = oCSM;
                CollectionsCreator = new CCreator();
                if (CSM != null)
                {
                    CSM.Session.OnMessage += StationMessage;
                    workDir = CSM.PathLocator.get_Path(CCEnums.CCFilesExt.TIF.ToString());
                }

                //-- Initialize profile --\\
                config = dataCfg.ParentConfiguration;

                //-- Initialize profile --\\
                currentProfile = dataCfg;

                //-- Define collections creator settings --\\
                CollectionsCreator.CurrentProfile = currentProfile;                
                CollectionsCreator.OnCollectionCreated += CollectionCreated;

                if (SearchHandler == null) SearchHandler = new CCSearchFiles(currentProfile);
                SearchHandler.OnPostFileLock += PostFileLock;
                SearchHandler.OnPreFileLock += PreFileLock;

                PollingTimer.Enabled = currentProfile.SearchPaths != null && currentProfile.SearchPaths.Length > 0 && currentProfile.SearchExtensions != null && currentProfile.SearchExtensions.Length > 0;
                CreateEvent();//-- fire OnCreate  event --\
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        }

        public CCTimerSearch(ITisClientServicesModule oCSM, String configPath, String profileName)
        {
            try
            {
                CSM = oCSM;
                CollectionsCreator = new CCreator();
                if (CSM != null)
                {
                    CSM.Session.OnMessage += StationMessage;
                    workDir = CSM.PathLocator.get_Path(CCEnums.CCFilesExt.TIF.ToString());
                }

                //-- Initialize profile --\\
                if (File.Exists(configPath ?? String.Empty)) config = CCConfiguration.FromXml(configPath);
                else config = CCConfiguration.FromCSM(CSM);

                //-- Initialize profile --\\
                if (String.IsNullOrEmpty(profileName)) profileName = CSM.Application.AppName;
                currentProfile = config != null ? config.GetConfiguration(profileName) : null;
                if (currentProfile == null) currentProfile = new CCConfiguration.CCConfigurationData();//-- create a default profile --\\

                //-- Define collections creator settings --\\
                CollectionsCreator.CurrentProfile = currentProfile;

                //collectionsCreator.OnPostFileLock += PostFileLock;
                CollectionsCreator.OnCollectionCreated += CollectionCreated;

                if (SearchHandler == null) SearchHandler = new CCSearchFiles(currentProfile);
                SearchHandler.OnPostFileLock += PostFileLock;
                SearchHandler.OnPreFileLock += PreFileLock;

                PollingTimer.Enabled = currentProfile.SearchPaths != null && currentProfile.SearchPaths.Length > 0 && currentProfile.SearchExtensions != null && currentProfile.SearchExtensions.Length > 0;
                CreateEvent();//-- fire OnCreate  event --\
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        }
        #endregion

        #region "StationMessage" event
        /// <summary>
        /// Receive and process station messages.
        /// </summary>
        /// <param name="message">The message sent by an eFlow application.</param>
        private void StationMessage(String message)
        {
            try
            {
                ILog.LogInfo("Received eflow system MESSAGE: [{0}]", message);


                if (Regex.IsMatch(message, @"(?i)Show\s?Config|Show\s?setting?s|Show\s?definitions?"))
                {
                    CCConfigDialog dlg = new CCConfigDialog();
                    dlg.ShowDialog(config, CurrentProfile.Name, false);

                    Match mtc = Regex.Match(message, @"(?i)(?<=(.+)lprofile:)[^\s]+");
                    if (mtc.Success && mtc.Length > 0)
                    {
                        CurrentProfile = config.GetConfiguration(mtc.Value);
                    }
                }

                if (Regex.IsMatch(message, "(?i)Stop|Pause|Disable|End"))
                {
                    Enabled = false;
                    ILog.LogInfo("Received eflow system message: [{0}], file polling timer is: enabled", message);
                }

                if (Regex.IsMatch(message, "(?i)Continue|Play|Resume|Enabled|Start|Begin"))
                {
                    Enabled = true;
                    ILog.LogInfo("Received eflow system message: [{0}], file polling timer is: disbaled", message);
                }

                if (Regex.IsMatch(message, "(?i)Exit|Terminate|Close"))
                {
                    Enabled = false;
                    processing = true;
                    Dispose();
                    ILog.LogInfo("Received eflow system message: [{0}], Colisng application down", message);
                    this.ExitThread();
                }

                if (Regex.IsMatch(message, "(?i)search|find"))
                {
                    StartFileSearch();
                    ILog.LogInfo("Received eflow system message: [{0}], starting a search", message);
                }

                if (Regex.IsMatch(message, "(?i)((.+)?load:|config(uration):)") || Regex.IsMatch(message, @"(?i)(?<=(.+)lprofile:)[^\s]+"))
                {
                    Match mtc = Regex.Match(message, @"(?i)(?<=(.+)load:|refresh:)[^\s]+");
                    if (mtc.Success && mtc.Length > 0 && File.Exists(mtc.Value))
                    {
                        config = CCConfiguration.FromXml(message);
                    }

                    mtc = Regex.Match(message, @"(?i)(?<=(.+)lprofile:)[^\s]+");
                    if (mtc.Success && mtc.Length > 0)
                    {
                        CurrentProfile = config.GetConfiguration(mtc.Value);
                    }
                }

                if (Regex.IsMatch(message, "(?i)((.+)?timer?:|interval:)"))
                {
                    Match mtc = Regex.Match(message, @"(?i)(?<=(.+)timer:|interval:)[^\s]+");
                    if (mtc.Success && mtc.Length > 0)
                    {
                        int prs = -1;
                        if (int.TryParse(mtc.Value, out prs))
                        {
                            PollingTimer.Interval = Math.Max(int.MaxValue, prs * 1000);
                        }
                    }
                }

                if (Regex.IsMatch(message, "(?i)((.+)?max.?count:)"))
                {
                    Match mtc = Regex.Match(message, @"(?i)(?<=(.+)max.?count)[^\s]+");
                    if (mtc.Success && mtc.Length > 0)
                    {
                        int prs = -1;
                        if (int.TryParse(mtc.Value, out prs))
                        {
                            CurrentProfile.MaxFilesLock = prs;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        }
        #endregion

        #region "Dispose"method
        /// <summary>
        /// Dispose used resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    try
                    {
                        DisposeEvent();//-- Fire OnDispose event --\\
                        processing = true;

                        if (pollingTimer != null)
                        {
                            pollingTimer.Enabled = false;
                            pollingTimer.Dispose();
                            pollingTimer = null;
                        }

                        CSM = null;
                    }
                    catch (Exception ex)
                    {
                        ILog.LogError(ex);
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion

        #region "StartFileSearch" method
        /// <summary>
        /// The internal search files methodevent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnStartFileSearch(object sender, EventArgs e)
        {
            TimerEvent();
            StartFileSearch();
        }
        #endregion

        #region "StartFileSearch" method
        /// <summary>
        /// The search files method.
        /// </summary>
        public virtual CCFileList[] StartFileSearch()
        {
            if (!processing)
                try
                {
                    stopTimerSearch = false;
                    CCFileList[] fileCollections = SearchHandler.SearchFiles(CurrentProfile);
                    SearchedFiles(ref fileCollections);

                    if (fileCollections != null)
                    {
                        foreach (CCFileList fc in fileCollections)
                        {
                            Application.DoEvents();
                            if (stopTimerSearch)
                            {
                                SearchHandler.StopSearch();
                                return null;
                            }

                            currentCollection = null;
                            if (fc != null)
                            {
                                //-- Create a collection definition --\\
                                currentCollection = CollectionsCreator.FromImages(workDir, CurrentProfile.KeepFileName, CurrentProfile.MultiPagePerForm, !CurrentProfile.CopySourceFiles, fc.KeyFile);
                                currentCollection.Attachments = fc.Files;


                                //-- fire 'PreCreateCollection' event --\\
                                PreCreateCollection();
                                Application.DoEvents();
                                if (stopTimerSearch)
                                {
                                    SearchHandler.StopSearch();
                                    return null;
                                }


                                if (currentCollection != null)
                                {
                                    int errCode = 0;
                                    //--<< if all is set, create the collection >>--\\
                                    String[] res = CollectionsCreator.CreateCollections(CSM, out errCode, currentCollection);
                                    PostCreateCollection(res != null && res.Length > 0 ? res[0] : String.Empty);//-- Fire post create collection event --\\
                                }
                            }
                        }
                    }
                    return fileCollections;
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                    if (this.CurrentProfile.ThrowAllExceptions) throw (ex);
                }
                finally
                {
                    processing = false;
                }
            return null;
        }
        #endregion

        #region class properties
        #region "Config" property
        /// <summary>
        /// The current mian profile.
        /// </summary>
        public virtual CCConfiguration Config
        {
            get { return config; }
            set { config = value; }
        }
        #endregion

        #region "CSM" property
        /// <summary>
        /// The current CSM object.
        /// </summary>
        private ITisClientServicesModule cSM;
        public virtual ITisClientServicesModule CSM { get { return cSM; } set { cSM = value; } }
        #endregion

        #region "CollectionsCreator" property
        /// <summary>
        /// The coollection creator class that is use to create the collection in eFlow.
        /// </summary>
        private CCreator collectionsCreator;
        public virtual CCreator CollectionsCreator { get { return collectionsCreator; } private set { collectionsCreator = value; } }
        #endregion

        #region "CurrentProfile" property
        /// <summary>
        /// The current profile profile in use.
        /// </summary>
        public virtual CCConfiguration.CCConfigurationData CurrentProfile
        {
            get { return currentProfile; }
            set
            {
                currentProfile = value;
                try
                {
                    if (currentProfile == null) currentProfile = new CCConfiguration.CCConfigurationData();
                    PollingTimer.Enabled = currentProfile.SearchPaths != null && currentProfile.SearchPaths.Length > 0 && currentProfile.SearchExtensions != null && currentProfile.SearchExtensions.Length > 0;
                    PollingTimer.Interval = currentProfile.TimerIntervals * 1000;
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
            }
        }
        #endregion

        #region "Enabled" property
        /// <summary>
        /// Enable disable the Search timer.
        /// </summary>
        public bool Enabled
        {
            get { return PollingTimer.Enabled; }
            set
            {
                PollingTimer.Enabled = value;

                //-- Flag stop all searches --\\
                stopTimerSearch = !value;
                if (stopTimerSearch && this.SearchHandler != null) SearchHandler.StopSearch();
            }
        }
        #endregion

        #region "CurrentCollection" property
        /// <summary>
        /// Holds the current collection (in process).
        /// </summary>
        public CCCollection CurrentCollection
        {
            get { return currentCollection; }
            set { currentCollection = value; }
        }
        #endregion

        #region "SearchHandler" property
        private CCSearchFiles searchHandler;
        /// <summary>
        /// Get or set the files search handler object that is used to search, rename and prepare the collection files.
        /// </summary>
        public virtual CCSearchFiles SearchHandler { get { return searchHandler; } set { searchHandler = value; } }
        #endregion

        #region "PollingTimer" property
        /// <summary>
        /// The timer object that calls the file search every timer interval
        /// </summary>
        public virtual Timer PollingTimer
        {
            get
            {
                if (pollingTimer == null)
                {
                    pollingTimer = new Timer();
                    pollingTimer.Tick += OnStartFileSearch;
                    pollingTimer.Enabled = false;
                    pollingTimer.Interval = Math.Min(int.MaxValue, currentProfile == null ? 10000 : currentProfile.TimerIntervals * 1000);
                }
                return pollingTimer;
            }
            set { pollingTimer = value; }
        }
        #endregion
        #endregion
    }
}