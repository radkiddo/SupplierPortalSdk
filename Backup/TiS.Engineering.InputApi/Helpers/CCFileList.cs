using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Reflection;
using System.Windows.Forms;

namespace TiS.Engineering.InputApi
{
    #region "CCFileList" class
    /// <summary>
    /// A class that conatins and manages the files defined for a collection
    /// </summary>
#if INTERNAL
    public class CCFileList : IDisposable
#else
    public class CCFileList : IDisposable
#endif
    {
        #region class variables
        Dictionary<String, List<String>> files;
#if INTERNAL
        internal event CCDelegates.OnPostFileLockEvt OnPostFileLock;
#else
        public event CCDelegates.OnPostFileLockEvt OnPostFileLock;
#endif

#if INTERNAL
        internal event CCDelegates.OnPreFileLockEvt OnPreFileLock;
#else
        public event CCDelegates.OnPreFileLockEvt OnPreFileLock;
#endif
        #endregion

        #region "ChangeKeyExtension" function
        /// <summary>
        /// Change key extension of existing items.
        /// </summary>
        /// <param name="oldKey">The old key name.</param>
        /// <param name="newKey">the new name to set.</param>
        /// <returns>true when successfull.</returns>
        public bool ChangeKeyExtension(String oldKey, String newKey)
        {
            try
            {
                if (!String.IsNullOrEmpty(oldKey) && !String.IsNullOrEmpty(newKey) && files.ContainsKey(oldKey.ToUpper()) && !files.ContainsKey(newKey.ToUpper()))
                {
                    List<String> tmp = new List<String>(files[oldKey].ToArray());
                    files.Remove(oldKey);
                    files.Add(newKey, tmp);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return false;
        } 
        #endregion

        #region "PreFileLock" event
        /// <summary>
        /// The internal method that fires the pre file lock event.
        /// </summary>
        /// <param name="fileName">The name of the file that is about to be locked.</param>
        /// <returns>true when successfull.</returns>
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

        #region "FileTypes" property
        /// <summary>
        /// The file types that this class contains
        /// </summary>
        [XmlIgnore]
        public virtual String[] FileTypes
        {
            get
            {
                List<String> fileTypes = new List<String>();
                if (files != null)
                {
                    foreach (KeyValuePair<String, List<String>> kvp in files)
                    {
                        fileTypes.Add(kvp.Key);
                    }
                }
                return fileTypes.ToArray();
            }
        }
        #endregion

        #region "GetFileTypes" property
        /// <summary>
        /// Get the file types that this class contains (as unlocked extension, i.e. no lock suffix).
        /// </summary>
        /// <param name="lockExt">The extension of the files to get.</param>
        public virtual String[] GetFileTypes(String lockExt)
        {
            try
            {
                List<String> fileTypes = new List<String>();
                if (files != null)
                {
                    foreach (KeyValuePair<String, List<String>> kvp in files)
                    {
                        fileTypes.Add( String.IsNullOrEmpty(lockExt) ? kvp.Key ?? String.Empty : (kvp.Key??String.Empty).ToUpper().Replace(lockExt.ToUpper(), String.Empty));
                    }
                }
                return fileTypes.ToArray();
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return null;
        }
        #endregion

        #region "Files" property
        /// <summary>
        /// All the files (from all types) that this class contains
        /// </summary>
        [XmlIgnore]
        public virtual String[] Files
        {
            get
            {
                List<String> fls = new List<String>();
                if (files != null)
                {
                    foreach (KeyValuePair<String, List<String>> kvp in files)
                    {
                        fls.AddRange(kvp.Value);
                    }
                }
                return fls.ToArray();
            }
        }
        #endregion

        #region "FileCount" property
        /// <summary>
        /// Get the total files in the class.
        /// </summary>
        [XmlIgnore]
        public virtual int FileCount
        {
            get
            {
                int res = 0;
                if (files != null)
                {
                    foreach (KeyValuePair<String, List<String>> kvp in files)
                    {
                      res+= kvp.Value.Count;
                    }
                }
                return res;
            }
        }
        #endregion

        #region "KeyFile" property
        /// <summary>
        /// Get KeyFile property, the first file that was added to the class.
        /// </summary>
        private String keyFile;
        public String KeyFile { get { return keyFile; } private set { keyFile = value; } }
        #endregion

        #region "SetKeyFile" method
        /// <summary>
        /// Set KeyFile property.
        /// </summary>
        internal void SetKeyFile(String ky)
        {
            KeyFile = ky ?? String.Empty;
            AddFiles(ky);
        }
        #endregion

        #region class constructors
        public CCFileList()
        {
        }

        public CCFileList(params String[] filesToAdd)
        {
            this.AddFiles(filesToAdd);
        }
        #endregion

        #region "AddFiles" methods
        /// <summary>
        /// Add a file\s to the list of files index by extension.
        /// </summary>
        /// <param name="filePaths">the file path\s to add</param>
        public virtual void AddFiles(params String[] filePaths)
        {
            AddFiles(null, filePaths);
        }

        /// <summary>
        /// Add a file\s to the list of files index by extension.
        /// </summary>
        /// <param name="unlockExtFiles">The (lock) extension to remove so the files will enter under an unlocked extension category</param>
        /// <param name="filePaths">the file path\s to add</param>
        public virtual void AddFiles(String unlockExtFiles, params String[] filePaths)
        {
            try
            {
                if (files == null) files = new Dictionary<String, List<String>>();
                foreach (String filePath in filePaths)
                {
                    if (String.IsNullOrEmpty(KeyFile)) KeyFile = filePath;

                    String sExt = Path.GetExtension(filePath).Trim('.').ToUpper();

                    if (!String.IsNullOrEmpty(unlockExtFiles)) sExt = sExt.ToUpper().Replace(unlockExtFiles.ToUpper(), String.Empty);

                    if (files.ContainsKey(sExt))
                    {
                        if (files[sExt] == null) files[sExt] = new List<String>(new String[] { filePath });
                        else if (!files[sExt].Contains(filePath)) files[sExt].Add(filePath);
                    }
                    else
                    {
                        files.Add(sExt, new List<String>(new String[] { filePath }));
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        }
        #endregion

        #region "SortList" method
        /// <summary>
        /// Sort an array by date.
        /// </summary>
        /// <param name="filePaths">the file path\s to add</param>
#if INTERNAL
        internal static String[] SortList(CCEnums.CompareTypeEnm sortType, params String[] filePaths)
#else
        public static String[] SortList(CCEnums.CompareTypeEnm sortType, params String[] filePaths)
#endif
        {
          
            try
            {
                List<String> files = new List<String>(filePaths);
                files.Sort(new FlexListComparer(sortType, false));
                return files.ToArray();
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return new String[0];
        }
        #endregion

        #region "GetFiles" function
        /// <summary>
        /// Get all files associated with the specified file type\s (extension).
        /// </summary>
        /// <param name="fileTypes">The file type\\s to get.</param>
        /// <returns>An array of files when successfull, empty array when failed.</returns>
        public virtual String[] GetFiles(params String[] fileTypes)
        {
            try
            {
                if (files != null)
                {
                    if (fileTypes == null || fileTypes.Length <= 0) return Files;
                    else
                    {
                        List<String> fls = new List<String>();
                        foreach (String fileType in fileTypes)
                        {
                            if (files.ContainsKey(fileType.ToUpper())) fls.AddRange(files[fileType].ToArray());
                        }
                        return fls.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return new String[0];
        }
        #endregion

        #region "RemoveFiles" function
        /// <summary>
        /// Remove the specified file\\s.
        /// </summary>
        /// <param name="fileExtensions">The file name\s to remove</param>
        public virtual void RemoveFiles(params String[] fileExtensions)
        {
            try
            {
                foreach (String fileExt in fileExtensions)
                {
                    String sExt = Path.GetExtension(fileExt).Trim(' ', '.').ToUpper();
                    if (String.IsNullOrEmpty(sExt)) sExt = fileExt.Trim(' ', '.').ToUpper();
                    
                    if (files != null && files.ContainsKey(sExt))
                    {
                        for (int i = files[sExt].Count - 1; i >= 0; i--)
                        {
                            files[sExt].RemoveAt(i);
                        }
                        files.Remove(sExt);
                    }

                    //-- Reset Key file --\\
                    if (files!=null&& ( files.Count <= 0 || files[sExt].Count<=0)) KeyFile = null;
                }              
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        }
        #endregion

        #region "Reset" method.
        /// <summary>
        /// Reset \ clear the class.
        /// </summary>
#if INTERNAL
        internal virtual void Reset()
#else
        public virtual void Reset()
#endif
        {
            if (files != null)
            {
                files.Clear();
                KeyFile = null;
            }
        }
        #endregion

        #region "Dispose" method
        /// <summary>
        /// dispose used objects
        /// </summary>
        public virtual void Dispose()
        {
            if (files != null)
            {
                files.Clear();
                files = null;
            }
        }
        #endregion

        #region "LockFiles" function
        /// <summary>
        /// Lock collection files
        /// </summary>
        /// <param name="extLock">The extension to use to lock the files.</param>
        /// <param name="updateExtensionKey">Update the extensions key with the new locked extesnion when true, levae the original extension when false.</param>
        /// <param name="copySourceFiles">Copy the source file instead of moving them (lock).</param>
        /// <returns>true if all files were locked.</returns>
        public virtual bool LockFiles(String extLock, bool updateExtensionKey, bool copySourceFiles, bool ignoreLockedFiles)
        {
            bool result = false;

            try
            {
                if (files != null && files.Count > 0)
                {
                    int lockCount = 0;
                    String fExt = null;
                    String fileName = null;
                    extLock = extLock.Trim('.', '*', ' ');


                    if (updateExtensionKey)
                    {
                        //-- Update the extension keys --\\
                        List<String> updateList = new List<String>();
                        foreach (String file in this.Files)
                        {
                            //-- Get file name and file extension --\\
                            fileName = file;
                            fExt = Path.GetExtension(file).Trim('.', ' ', '*');

                            int fLocked = fExt.ToUpper().IndexOf(extLock.ToUpper());
                            if (fLocked + extLock.Length != fExt.Length) fLocked = -1;

                            //-- if not lockecd --\\
                            if (fLocked < 0)
                            {
                                //-- Define the locked file name --\\
                                String newFileName = Path.Combine(Path.GetDirectoryName(fileName), String.Format("{0}.{1}{2}", Path.GetFileNameWithoutExtension(fileName), fExt, extLock));

                                //-- if previous locked files exists; exit --\\
                                if (File.Exists(newFileName))
                                {
                                    if (ignoreLockedFiles)
                                    {
                                        File.SetAttributes(newFileName, FileAttributes.Normal);
                                        File.Delete(newFileName);
                                    }
                                    else
                                    {
                                        result = false;
                                        return result;
                                    }
                                }

                                //-- OnPreFileLock event --\\
                                if (!PreFileLock(ref fileName)) continue; 

                                //-- Exit or remove previous locked files if exists --\\
                                if (copySourceFiles)
                                {
                                    File.Copy(fileName, newFileName);
                                    File.SetAttributes(newFileName, FileAttributes.Normal);
                                }
                                else
                                {
                                    File.SetAttributes(fileName, FileAttributes.Normal);
                                    File.Move(fileName, newFileName);
                                }

                                //-- Update the Key file --\\
                                if (String.Compare(file, KeyFile, true) == 0) KeyFile = newFileName;

                                if (OnPostFileLock != null) OnPostFileLock(null, this, newFileName);
                                
                                updateList.Add(newFileName);
                                lockCount++;
                            }
                            else
                            {
                                updateList.Add(file);
                                lockCount++;
                            }
                        }

                        //-- if all went well update the list--\\
                        if (lockCount == files.Count)
                        {
                            KeyFile = null;
                            files.Clear();
                            AddFiles(updateList.ToArray());
                        }
                    }
                    else
                    {
                        //-- Keep the previous\original extension key --\\
                        foreach (KeyValuePair<String, List<String>> kvp in files)
                        {
                            if (kvp.Value != null)
                            {
                                for (int i = 0; i < kvp.Value.Count; i++)
                                {
                                    //-- Get file name and extension --\\
                                    fileName = kvp.Value[i];
                                    fExt = Path.GetExtension(fileName).Trim('.', ' ', '*');
                                    int fLocked = fExt.ToUpper().IndexOf(extLock.ToUpper());
                                    if (fLocked + extLock.Length != fExt.Length) fLocked = -1;

                                    //-- if not lockecd --\\
                                    if (fLocked<0)
                                    {
                                        String newFileName = Path.Combine(Path.GetDirectoryName(fileName), String.Format("{0}.{1}{2}", Path.GetFileNameWithoutExtension(fileName), fExt, extLock));

                                        //-- if previous locked files esists exit --\\
                                        if (File.Exists(newFileName))
                                        {
                                            if (ignoreLockedFiles)
                                            {
                                                File.SetAttributes(newFileName, FileAttributes.Normal);
                                                File.Delete(newFileName);
                                            }
                                            else
                                            {
                                                //--NF 2012-07-06 --\\
                                                if (String.Compare(this.KeyFile, newFileName, true) != 0)
                                                {
                                                    result = false;
                                                    return result;
                                                }
                                            }
                                        }

                                        //-- OnPreFileLock event --\\
                                        if (!PreFileLock(ref fileName)) continue;

                                        //--NF 2012-07-06 --\\
                                        if (String.Compare(this.KeyFile, newFileName, true) != 0)
                                        {
                                            if (copySourceFiles)
                                            {
                                                File.Copy(fileName, newFileName);
                                                File.SetAttributes(newFileName, FileAttributes.Normal);
                                            }
                                            else
                                            {
                                                File.SetAttributes(fileName, FileAttributes.Normal);
                                                File.Move(fileName, newFileName);
                                            }
                                        }

                                        //-- Update the Key file --\\
                                        if (String.Compare(kvp.Value[i], KeyFile, true) == 0) KeyFile = newFileName;

                                        if (OnPostFileLock != null) OnPostFileLock(null, this, newFileName);

                                        kvp.Value[i] = newFileName;
                                        lockCount++;
                                    }
                                    else
                                    {
                                        lockCount++;
                                    }
                                }
                            }
                        }
                    }

                    result = lockCount == this.FileCount;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex, false);
                throw ex;
            }
            finally
            {
                //-- Rollback reverse extension state \ rollback file ext --\\
                if (!result && files.Count > 0 && !copySourceFiles) RollbackLock(extLock);
            }
            return result;
        } 
        #endregion

        #region "RollbackLock" method
        /// <summary>
        /// Rollback files lock.
        /// </summary>
        /// <param name="extLock">The extension to rename from the file list</param>
        public virtual void RollbackLock(String extLock)
        {
            try
            {
                //-- Rollback reverse extension state \ rollback file ext.
                if (files != null && files.Count > 0)
                {
                    extLock = extLock.Trim('.', '*', ' ').ToUpper();
                    String fExt = null;
                    String fileName = null;
                    String newFileName = null;

                    //-- Keep the previous\original extension key --\\
                    foreach (KeyValuePair<String, List<String>> kvp in files)
                    {
                        try
                        {
                            if (kvp.Value != null)
                            {
                                for (int i = 0; i < kvp.Value.Count; i++)
                                {
                                    //-- Get file name and extension --\\
                                    fileName = kvp.Value[i];
                                    fExt = Path.GetExtension(fileName).Trim('.', ' ', '*').ToUpper();

                                    //-- if it was locked --\\
                                    if (String.Compare(extLock, fExt, true) != 0)
                                    {
                                        fExt = fExt.Replace(extLock, String.Empty);
                                        newFileName = Path.Combine(Path.GetDirectoryName(fileName), String.Format("{0}.{1}", Path.GetFileNameWithoutExtension(fileName), fExt));

                                        //-- if previous locked files esists exit --\\
                                        File.SetAttributes(fileName, FileAttributes.Normal);
                                        File.Move(fileName, newFileName);
                                    }
                                }
                            }
                        }
                        catch (Exception ec)
                        {
                            ILog.LogError(ec, false);
                            throw ec;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex, false);
                throw ex;
            }
        } 
        #endregion

        #region "DeleteFiles" method
        /// <summary>
        /// Delete the files specified in the class.
        /// </summary>
        public virtual bool DeleteFiles()
        {
            try
            {
                int delCount = 0;
                if (files == null || files.Count <= 0) return false;

                foreach (String sf in this.Files)
                {
                    try
                    {
                        if (File.Exists(sf ?? String.Empty))
                        {
                            File.SetAttributes(sf, FileAttributes.Normal);
                            File.Delete(sf);
                            delCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        ILog.LogError(ex, false);
                        throw ex;
                    }
                }

                if (files.Count <= 0) KeyFile = null;
                return delCount == this.files.Count;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex, false);
                throw ex;
            }
        }
        #endregion

        #region "GetLockedFilePath" functions
        /// <summary>
        /// Get locked file path
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="collectionName"></param>
        /// <param name="lockExtension"></param>
        /// <param name="workDirectoryPath"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public static String GetLockedFilePath(String sourceFile, String collectionName, String lockExtension, String workDirectoryPath, int pageNumber)
        {
            String result = sourceFile ?? String.Empty;
            try
            {
                if (String.IsNullOrEmpty(result) || String.IsNullOrEmpty(lockExtension)) return result;

                if (String.IsNullOrEmpty(workDirectoryPath) || !Directory.Exists(workDirectoryPath)) workDirectoryPath = Path.GetDirectoryName(sourceFile);

                if (String.IsNullOrEmpty(collectionName)) collectionName = Path.GetFileNameWithoutExtension(sourceFile);

                lockExtension = (lockExtension ?? String.Empty).ToUpper().Trim('.', '*');

                String fExt = Path.GetExtension(sourceFile).Trim('.', ' ', '*').ToUpper();

                int fLocked = fExt.ToUpper().IndexOf(lockExtension.ToUpper());
                if (fLocked + lockExtension.Length != fExt.Length) fLocked = -1;

                //-- if not lockecd --\\
                if (fLocked < 0)
                {
                    result = Path.Combine(workDirectoryPath, String.Format("{0}{1}.{2}{3}", collectionName, pageNumber > 0 ? "_P" + pageNumber.ToString("X").PadLeft(4, '0') : String.Empty, fExt, lockExtension));
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result;
        }      
        
        /// <summary>
        /// Get locked file path
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="lockExtension"></param>
        /// <returns></returns>
        public static String GetLockedFilePath(String sourceFile, String lockExtension)
        {
            return GetLockedFilePath(sourceFile, null, lockExtension, null, 0);
        }
        #endregion

        #region "MoveToFolder" function
        /// <summary>
        /// Move the files specified to a folder (dsigned for moving stuff to the error folder).
        /// </summary>
        /// <param name="targetFolder">The target folder path</param>
        /// <param name="files">The file\s and or folder\s to move.</param>
        /// <returns>A list of errors when failed along the way, empty list when successfull.</returns>
        public static String[] MoveToFolder(String targetFolder, params String[] files)
        {
            List<String> result = new List<String>();
            try
            {
                //-- Check target path --\\
                if (String.IsNullOrEmpty(targetFolder))
                {
                    result.Add("Target folder path is empty, in: " + MethodBase.GetCurrentMethod().Name);
                }
                else if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                //-- check file\s array --\\
                if (files == null || files.Length <= 0)
                {
                    result.Add("No files to copy specified, in: " + MethodBase.GetCurrentMethod().Name);
                }

                if (result != null && result.Count > 0) return result.ToArray();

                foreach (String s in files)
                {
                    try
                    {
                        Application.DoEvents();
                        if (!String.IsNullOrEmpty(s))
                        {
                            String tempName = null;
                            int count = 0;
                            if (File.Exists(s))
                            {
                                tempName = Path.Combine(targetFolder, Path.GetFileName(s));
                                while (File.Exists(tempName))
                                {
                                    Application.DoEvents();
                                    count++;
                                    tempName = Path.Combine(targetFolder, String.Format("{0}_{1:0000}.{2}", Path.GetFileNameWithoutExtension(s), count, Path.GetExtension(s).Trim(' ', '.')));
                                }

                                File.SetAttributes(s, FileAttributes.Normal);
                                File.Move(s, tempName);
                            }
                            else if (Directory.Exists(s))
                            {
                                tempName = Path.Combine(targetFolder, Path.GetFileNameWithoutExtension(s));
                                while (Directory.Exists(tempName))
                                {
                                    Application.DoEvents();
                                    count++;
                                    tempName = Path.Combine(targetFolder, String.Format("{0}_{1:0000}", Path.GetFileNameWithoutExtension(s), count));
                                }

                                Directory.Move(s, tempName);
                            }
                            else
                            {
                                result.Add(String.Format("File or folder [{0}] does not exist, item is ignored in method: [{1}]", s, MethodBase.GetCurrentMethod().Name));
                                ILog.LogInfo(result[result.Count - 1]);
                            }
                        }
                    }
                    catch (Exception ep)
                    {
                        result.Add(String.Format("File or folder [{0}] failed move to [{1}], item is ignored in method: [{2}], error: [{3}]", s ?? String.Empty, targetFolder ?? String.Empty, MethodBase.GetCurrentMethod().Name, ep.ToString()));
                        ILog.LogError(ep);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Add(String.Format("Error in method: [{0}], target folder [{1}], error: [{2}]", MethodBase.GetCurrentMethod().Name, targetFolder ?? String.Empty, ex.ToString()));
                ILog.LogError(ex);
            }
            return result.ToArray();
        }
        #endregion

        #region "MoveFilesToWorkDirectory" function
        /// <summary>
        /// Move collection files to the WorkDirectory (to be attached to the collection).
        /// </summary>
        /// <param name="workDirectoryPath">The path to the work directory.</param>
        /// <param name="collectionName">the collection name to use for the file\\s</param>
        /// <param name="updateExtensionKey">Update the extensions key with the new locked extesnion when true, levae the original extension when false.</param>
        /// <param name="pageNumber">Specify a page number to flag an extension that is enumerable (page name), use 0 or lower for collection type name</param>
        /// <param name="extLock">The extension to use to lock the files.</param>
        /// <returns>true if all files were locked.</returns>
        public virtual bool MoveFilesToWorkDirectory(String workDirectoryPath, String collectionName, bool updateExtensionKey, int pageNumber, String extLock)
        {
            try
            {
                if (files != null && files.Count > 0)
                {
                    int moveCount = 0;
                    String fExt = null;
                    String newFileName = null;
                    extLock = extLock.Trim('.', '*', ' ').ToUpper();

                    if (updateExtensionKey)
                    {
                        //-- Update the extension keys --\\
                        List<String> updateList = new List<String>();
                        foreach (String file in this.Files)
                        {
                            fExt = Path.GetExtension(file).Trim('.', ' ', '*').ToUpper().Replace(extLock, String.Empty);
                            newFileName = Path.Combine(workDirectoryPath, String.Format("{0}{2}.{1}", collectionName, fExt, pageNumber>0 ? "_P"+pageNumber.ToString("X").PadLeft(4,'0'): String.Empty));

                            //-- Prepare to move the file --\\
                            if (File.Exists(newFileName)) File.Delete(newFileName);
                            File.SetAttributes(file, FileAttributes.Normal);
                            File.Move(file, newFileName);
                            updateList.Add(newFileName);

                            //-- Update the Key file --\\
                            if (String.Compare(file, KeyFile, true) == 0) KeyFile = newFileName;
                            moveCount++;
                        }

                        //-- if all went well update the list--\\
                        if (moveCount == files.Count)
                        {
                            KeyFile = null;
                            files.Clear();
                            AddFiles(updateList.ToArray());
                        }
                    }
                    else
                    {
                        //-- Keep the previous\original extension key --\\
                        foreach (KeyValuePair<String, List<String>> kvp in files)
                        {
                            if (kvp.Value != null)
                            {
                                for (int i = 0; i < kvp.Value.Count; i++)
                                {
                                    //-- Get the file name  & extension--\\
                                    String fileName = kvp.Value[i];
                                    fExt = Path.GetExtension(fileName).Trim('.', ' ', '*').ToUpper().Replace(extLock, String.Empty);
                                    newFileName = Path.Combine(workDirectoryPath, String.Format("{0}{2}.{1}", collectionName, fExt, pageNumber > 0 ? "_P" + pageNumber.ToString("X").PadLeft(4, '0') : String.Empty));

                                    //-- Prepare to move the file --\\
                                    if (File.Exists(newFileName)) File.Delete(newFileName);
                                    File.SetAttributes(fileName, FileAttributes.Normal);
                                    File.Move(fileName, newFileName);

                                    //-- Update the Key file --\\
                                    if (String.Compare(kvp.Value[i], KeyFile, true) == 0) KeyFile = newFileName;

                                    kvp.Value[i] = newFileName;
                                    moveCount++;
                                }
                            }
                        }
                    }

                    return moveCount == files.Count;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex,false);
                throw ex;
            }
            return false;
        }
        #endregion

        #region "SearchFiles" functions
        /// <summary>
        /// Search for files using the configuration specified as settings.
        /// </summary>
        /// <param name="profileName">The application\profile name to load from the configuration</param>
        /// <returns>A list of 'CCFileList'  objects when successfull and the search was successfull.</returns>
        public static CCFileList[] SearchFiles(String profileName)
        {
            return SearchFiles(!String.IsNullOrEmpty(profileName) ?  CCConfiguration.CCConfigurationData.FromXml(CCUtils.GetSettingsFilePath(), profileName):null);
        }


        /// <summary>
        /// Search for files using the configuration specified as settings.
        /// </summary>
        /// <param name="cfgProfile">The application\profile to use it's settings for the search</param>
        /// <returns>A list of 'CCFileList'  objects when successfull and the search was successfull.</returns>
        public static CCFileList[] SearchFiles(CCConfiguration.CCConfigurationData cfgProfile)
        {
            try
            {
                if (cfgProfile != null)
                {
                    using (CCSearchFiles ccs = new CCSearchFiles(cfgProfile))
                    {
                        CCFileList[] result = ccs.SearchFiles();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return null;
        }
        #endregion

        #region "SearchAndLockFiles" functions DISABLED
        ///// <summary>
        ///// Search for files using the configuration specified as settings.
        ///// </summary>
        ///// <param name="profileName">The application\profile name to load from the configuration</param>
        ///// <returns>A list of 'CCFileList'  objects when successfull and the search was successfull.</returns>
        //public CCFileList[] SearchAndLockFiles(String profileName)
        //{
        //    return SearchAndLockFiles(!String.IsNullOrEmpty(profileName) ? CCConfiguration.CCConfigurationData.FromXml(CCUtils.GetSettingsFilePath(), profileName) : null);
        //}


        ///// <summary>
        ///// Search for files using the configuration specified as settings.
        ///// </summary>
        ///// <param name="cfgProfile">The application\profile to use it's settings for the search</param>
        ///// <returns>A list of 'CCFileList'  objects when successfull and the search was successfull.</returns>
        //public CCFileList[] SearchAndLockFiles(CCConfiguration.CCConfigurationData cfgProfile)
        //{
        //    try
        //    {
        //        if (cfgProfile != null)
        //        {
        //            List<CCFileList> res = new List<CCFileList>();

        //            using (CCSearchFiles ccs = new CCSearchFiles(cfgProfile))
        //            {
        //                CCFileList[] result = ccs.SearchFiles();
        //                ccs.
        //                return result;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ILog.LogError(ex);
        //    }
        //    return null;
        //}
        #endregion
    }
    #endregion            
}
