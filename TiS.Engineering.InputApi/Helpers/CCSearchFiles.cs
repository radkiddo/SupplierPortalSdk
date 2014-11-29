using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace TiS.Engineering.InputApi
{
    #region "CCSearchFiles" class
    /// <summary>
    /// File handler class, search rename and prepare files for collection creation.
    /// </summary>
#if INTERNAL
    internal class CCSearchFiles : ICCSearchFiles
#else
    public class CCSearchFiles : ICCSearchFiles
#endif
    {
        #region class variables
        private CCConfiguration.CCConfigurationData currentProfile;
        private bool processing;
        private bool stopSearch;
        #endregion

        #region Events
        /// <summary>
        /// This event is fire for every key\trigger file that is being locked, eferencing the file name, to review, edit or set to null to eliminate lock.
        /// </summary>
#if INTERNAL
        internal event CCDelegates.OnPreFileLockEvt OnPreFileLock;
#else
        public event CCDelegates.OnPreFileLockEvt OnPreFileLock;
#endif

        /// <summary>
        /// This event is fire for every key\trigger file that was successfully locked, containing the file name.
        /// </summary>
#if INTERNAL
        internal event CCDelegates.OnPostFileLockEvt OnPostFileLock;
#else
        public event CCDelegates.OnPostFileLockEvt OnPostFileLock;
#endif

        #region "PreFileLock" function
        /// <summary>
        /// The function that fires the 'OnPreFileLock' event if is assigned.
        /// </summary>
        /// <param name="fileName">The name of the file being locked.</param>
        /// <returns></returns>
        protected bool PreFileLock(ref String fileName)
        {
            bool res = true;
            try
            {
                if (OnPreFileLock != null) OnPreFileLock(null, this, ref fileName);
                res = File.Exists(fileName);
            }
            catch (Exception ep)
            {
                ILog.LogError(ep);
            }
            return res;
        } 
        #endregion

        #region "PostFileLock" method
        /// <summary>
        /// The method that fires the 'OnPostFileLock' event if is assigned.
        /// </summary>
        /// <param name="fileName">the name of the file taht was locked.</param>
        protected void PostFileLock(String fileName)
        {
            try
            {
                if (OnPostFileLock != null) OnPostFileLock(null, this, fileName);
            }
            catch (Exception ep)
            {
                ILog.LogError(ep);
            }
        }  
        #endregion
        #endregion

        #region class constructors
        public CCSearchFiles()
        {
        }

        public CCSearchFiles(CCConfiguration.CCConfigurationData config)
        {
            currentProfile = config;
        }
        #endregion

        #region "Dispose" method
        /// <summary>
        /// Dispose used resources.
        /// </summary>
        public void Dispose()
        {
        } 
        #endregion

        #region class properties.
        /// <summary>
        /// The profile to use
        /// </summary>
        [XmlIgnore]
        public virtual CCConfiguration.CCConfigurationData CurrentProfile
        {
            get
            {
                if (currentProfile == null) currentProfile = new CCConfiguration.CCConfigurationData();
                return currentProfile;
            }
            set { currentProfile = value; }
        }
        #endregion

        #region "StopSearch" method
        /// <summary>
        /// Stop any running search, break all loops and return null, this will be reset in the next call for SearchFiles.
        /// </summary>
        public void StopSearch()
        {
            stopSearch = true;
        } 
        #endregion

        #region "SearchFiles" functions
        /// <summary>
        /// Search an input folders for files of certain extension as key\trigger files then search and collect all other related attachments.
        /// </summary>
        /// <param name="rootPaths">The root paths to start the search from</param>
        /// <param name="subFolders">Inckude subfolders in te search when true.</param>
        /// <param name="dateSorted">Sort the files by date when true (alpha when false)</param>
        /// <param name="maxLock">The maximum key\trigger files to lock.</param>
        /// <param name="keyExtensions">The key\trigger extensions to look for.</param>
        /// <param name="subExtensions">The attachments extensions to look for once a key extension is found.</param>
        /// <returns>An array of FileCollection objects with all files of a collection, devided by extension</returns>
        public virtual CCFileList[] SearchFiles()
        {
            return SearchFiles(CurrentProfile);
        }

        /// <summary>
        /// Search an input folders for files of certain extension as key\trigger files then search and collect all other related attachments.
        /// </summary>
        /// <param name="rootPaths">The root paths to start the search from</param>
        /// <param name="subFolders">Inckude subfolders in te search when true.</param>
        /// <param name="dateSorted">Sort the files by date when true (alpha when false)</param>
        /// <param name="maxLock">The maximum key\trigger files to lock.</param>
        /// <param name="keyExtensions">The key\trigger extensions to look for.</param>
        /// <param name="subExtensions">The attachments extensions to look for once a key extension is found.</param>
        /// <returns>An array of FileCollection objects with all files of a collection, devided by extension</returns>
        public virtual CCFileList[] SearchFiles(CCConfiguration.CCConfigurationData cfg)
        {
            return SearchFiles(cfg.SearchPaths, cfg.CopySourceFiles, cfg.SearchSubFolders, cfg.FilesSort, cfg.MaxFilesLock, cfg.SearchExtensions, cfg.AttachmentsExtensions);
        }

        /// <summary>
        /// Search an input folders for files of certain extension as key\trigger files then search and collect all other related attachments.
        /// </summary>
        /// <param name="rootPaths">The root paths to start the search from</param>
        /// <param name="subFolders">Inckude subfolders in te search when true.</param>
        /// <param name="int">Sort the files by date when equals 1 (no sort when 0 or lower,<see cref="CompareTypeEnm"/> for a full list of possibilities).</param>
        /// <param name="maxLock">The maximum key\trigger files to lock.</param>
        /// <param name="keyExtensions">The key\trigger extensions to look for.</param>
        /// <param name="subExtensions">The attachments extensions to look for once a key extension is found.</param>
        /// <returns>An array of FileCollection objects with all files of a collection, devided by extension</returns>
#if INTERNAL
        internal virtual CCFileList[] SearchFiles(String[] rootPaths, bool copySrcFiles, bool subFolders, int sortType, int maxLock, String[] keyExtensions, params String[] subExtensions)
#else
        public virtual CCFileList[] SearchFiles(String[] rootPaths, bool copySrcFiles, bool subFolders, int sortType, int maxLock, String[] keyExtensions, params String[] subExtensions)
#endif
        {
            if (!processing)
                try
                {
                    stopSearch = false;
                    processing = true;
                    maxLock = Math.Max(maxLock, 1);
                    List<CCFileList> res = new List<CCFileList>();
                    int counter = 0;
                    
                    CCFileList filesCol = new CCFileList();

                    //-- Get all files (to sort as specified) --\\
                    foreach (String rootPath in rootPaths)
                    {
                        if (stopSearch) return null;

                        filesCol.AddFiles(GetFileList(rootPath, sortType, subFolders, keyExtensions).ToArray());
                    }


                    //-- Lock all possible files --\\
                    if (filesCol != null  &&  filesCol.FileCount > 0)
                    {
                        List<String> files = new List<String>(filesCol.Files);
                        if (sortType>1) files.Sort(new FlexListComparer(sortType, false));

                        //-- Iterate all possible files, break after locking the specified amount --\\
                        for (int i = 0; i < files.Count; i++)
                        {
                            try
                            {
                                Application.DoEvents();
                                if (stopSearch) return null;

                                //-- Prepare to fire  'OnPreFileLock'  event --\\
                                 String lstFile = files[i];
                                 if (!PreFileLock( ref lstFile)) continue;                                 

                                 CCFileList fileLck = new CCFileList(lstFile);
                                 fileLck.OnPreFileLock -= OnPreFileLock;
                                fileLck.OnPreFileLock += OnPreFileLock;
                                fileLck.OnPostFileLock -= OnPostFileLock;
                                 fileLck.OnPostFileLock += OnPostFileLock;
                                
                                //-- Try to lock file name --\\
                                if (fileLck.LockFiles(CurrentProfile.LockExtension, false, copySrcFiles, false))
                                {
                                    counter++;
                                    PostFileLock(fileLck.KeyFile);//-- fire 'OnPostFileLock' event --\\

                                    #region   //-- Get other attachment files --\\
                                    if (subExtensions != null && subExtensions.Length > 0)
                                    {
                                        //-- NF: 2012-07-06, Don't search same extension as key --\\
                                        List<String> subExts = new List<string>(subExtensions);
                                        for (int iSub = subExts.Count - 1;iSub>=0 ; iSub--)
                                        {
                                            foreach (String sKy in keyExtensions)
                                            {
                                                if (String.Compare(subExts[iSub], sKy, true) == 0)
                                                {
                                                    subExts.RemoveAt(iSub);
                                                    break;
                                                }
                                            }
                                        }

                                        //-- Get attachments --\\
                                        CCFileList lckList = new CCFileList(GetFileList(Path.GetDirectoryName(fileLck.KeyFile), Path.GetFileNameWithoutExtension(fileLck.KeyFile), subFolders, subExts.ToArray()));
                                        if (lckList.FileCount > 0)
                                        {
                                            try
                                            {
                                                lckList.AddFiles(CurrentProfile.LockExtension, fileLck.KeyFile);
                                                lckList.SetKeyFile(fileLck.KeyFile);
                                                fileLck = lckList;

                                                if (! lckList.LockFiles(CurrentProfile.LockExtension, false, copySrcFiles, false))
                                                {
                                                    throw new Exception(String.Format("Failed locking collection additional \\attachment files: " + String.Join(Path.PathSeparator.ToString(), lckList.Files)));
                                                }
                                            }
                                            catch (Exception et)
                                            {
                                                ILog.LogError(et);
                                                lckList.RollbackLock(CurrentProfile.LockExtension); //-- reverse extension state \ rollback file ext. --\\
                                            }
                                        }
                                    }

                                    res.Add(fileLck);
                                    #endregion
                                }
                            }
                            catch (Exception ep)
                            {
                                ILog.LogError(ep);
                            }

                            //-- Break after max count, to lock as many files specified --\\
                            if (counter >= maxLock) break;
                        }
                    }

                    return res.ToArray();
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                finally
                {
                    processing = false;
                }
            return null;
        }
        #endregion

        #region "GetFileList" function
        /// <summary>
        /// Search the specified folder path for source files
        /// </summary>
        /// <param name="folderPath">The root path to search in.</param>
        /// <param name="sortType">Sort by creation date (FiFo) when equals 1, no sort when 0 or lower.</param>
        /// <param name="subFolders">Include sub folders in search when true.</param>
        /// <param name="search_Extensions">The extensions to search</param>
        /// <returns>A list of files, sorted by date if specified.</returns>
        protected virtual List<String> GetFileList(String folderPath, int sortType, bool subFolders, params String[] search_Extensions)
        {
            List<String> result = new List<String>();

            try
            {
                //-- Stop searching files if main class was asked to stop --\\
                if (stopSearch) return result;

                //-- Validate search path --\\
                if (String.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                            if (!Directory.Exists(folderPath))
                            {
                                ILog.LogError("Search folder [{0}] does not exists, search aborted, in method [{1}]", folderPath, MethodBase.GetCurrentMethod().Name);
                                return result;
                            }
                        }
                        else
                        {
                            ILog.LogError("Search folder [{0}] does not exists, search aborted, in method [{1}]", folderPath, MethodBase.GetCurrentMethod().Name);
                            return result;
                        }
                    }
                    catch (Exception ei)
                    {
                        ILog.LogError(ei);
                    }
                }
                
                 //-- Search for new incoming batches --\\
                result.AddRange(GetFileList(folderPath, null, subFolders, search_Extensions));

                if (sortType > 0 && result.Count > 1) result.Sort(new FlexListComparer(sortType, false));
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result;
        }

        /// <summary>
        /// Get a file list filtered by the specified extension\s.
        /// </summary>
        /// <param name="folderPath">The root path to search in.</param>
        /// <param name="namePrefix">the name prefix to search by.</param>
        /// <param name="subFolders">Include sub folders in search when true.</param>
        /// <param name="search_Extensions">The extensions to search</param>
        /// <returns>An array of file paths when successfull.</returns>
#if INTERNAL
        internal String[] GetFileList(String folderPath, String namePrefix, bool subFolders, params String[] search_Extensions)
#else
        public String[] GetFileList(String folderPath, String namePrefix, bool subFolders, params String[] search_Extensions)
#endif
        {
            List<String> result = new List<String>();

            try
            {
                //-- Stop searching files if main class was asked to stop --\\
                if (stopSearch) return new String[0];

                //-- Search for new incoming batches --\\
                String[] fileList = Directory.GetFiles(folderPath, String.Format( "{0}*.*",namePrefix??String.Empty), subFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                foreach (String filePath in fileList)
                {
                    //-- Stop searching files if main class was asked to stop --\\
                    if (stopSearch) return new String[0];

                    String sFileExt = Path.GetExtension(filePath).Trim('.', '*', ' ');
                    foreach (String sExt in search_Extensions)
                    {
                        //-- Stop searching files if main class was asked to stop --\\
                        if (stopSearch) return new String[0];

                        if (String.Compare(sExt.Trim('.', '*', ' '), sFileExt, true) == 0)
                        {
                            result.Add(filePath);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result.ToArray();
        }
        #endregion
    }
    #endregion
}
