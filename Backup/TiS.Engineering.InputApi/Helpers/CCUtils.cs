using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiS.Core.eFlowAPI;
using TiS.Imaging.FileOP;

namespace TiS.Engineering.InputApi
{
    /// <summary>
    /// Utility methods and functions.
    /// </summary>
    internal  static class CCUtils
    {
        private const string DLL_NAME = "TiS.Engineering.InputApi";

        #region "AddSetDictionaryItem" method
        /// <summary>
        /// Add a List item safely, when key exists, change value.
        /// </summary>
        /// <param name="key">The dictionary item key.</param>
        /// <param name="val">The dictionary item value.</param>
        /// <param name="excpetionOnDictDuplicates">When true; throw an exception when duplicate keys found, when false, update the value of the key.</param>
        /// <param name="lstDct">The dictionary to add or set the key value pair to.</param>
        public static void AddSetDictionaryItem(String key, String val, bool excpetionOnDictDuplicates, ref   Dictionary<String, String> lstDct)
        {
            try
            {
                if (lstDct == null) lstDct = new Dictionary<String, String>();

                if (!excpetionOnDictDuplicates && lstDct.ContainsKey(key)) lstDct[key] = val ?? String.Empty;
                else lstDct.Add(key, val ?? String.Empty);
            }
            catch (Exception ex)
            {
                ILog.LogWarning("Key: [{0}], Value [{1}], caused an error [{2}] in method [{3}]", key ?? String.Empty, val ?? String.Empty, ex.Message, MethodBase.GetCurrentMethod().Name);
                ILog.LogError(ex,false);
                throw ex;
            }
        }
        #endregion

        #region "GetAttachments" functions
        /// <summary>
        /// Get all the file attachments defined for a page.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static String[] GetAttachments(ITisPageData page)
        {
            List<String> result = new List<String>();
            try
            {
                foreach (String sa in page.ParentCollection.AttachedFileManager.QueryAttachedTypes())
                {
                    String fullPath = page.GetAttachmentFileName(sa.Trim('.'));
                    if (File.Exists(fullPath) && !result.Contains(fullPath)) result.Add(fullPath);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get all (collection level) attachments.
        /// </summary>
        /// <param name="csm"></param>
        /// <param name="fm"></param>
        /// <returns></returns>
        public static String[] GetAttachments(ITisClientServicesModule csm, ITisAttachedFileManager fm)
        {
            List<String> result = new List<String>();
            try
            {
                IStringVectorReadOnly rvc = fm.QueryAllAttachments();
                if (rvc != null && rvc.Count > 0)
                {
                    foreach (String s in rvc)
                    {
                        String fullPath = csm == null ? s : Path.Combine(csm.PathLocator.get_Path(CCEnums.CCFilesExt.TIF.ToString()), s);
                        if (csm == null || File.Exists(fullPath)) result.Add(fullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get all attachments as specified per station.
        /// </summary>
        /// <param name="csm"></param>
        /// <param name="stationName"></param>
        /// <param name="eflowObject"></param>
        /// <returns></returns>
        public static String[] GetAttachments(ITisClientServicesModule csm, String stationName, object eflowObject)
        {
            List<String> result = new List<String>();
            try
            {
                ITisStationParams sta = csm.Setup.get_Station(stationName);
                if (sta != null && sta.StationDeclaration != null)
                {
                    List<String> filesExt = new List<String>();
                    List<String> filesList = new List<String>();
                    foreach (String s in sta.StationDeclaration.ReceiveAttachments)
                    {
                        if (!filesExt.Contains(s.ToLower())) filesExt.Add(s.ToLower());
                    }

                    foreach (String s in sta.StationDeclaration.SaveAttachments)
                    {
                        if (!filesExt.Contains(s.ToLower())) filesExt.Add(s.ToLower());
                    }


                    foreach (String s in filesExt)
                    {
                        foreach (String sf in Directory.GetFiles(csm.PathLocator.get_Path(CCEnums.CCFilesExt.TIF.ToString()), String.Format("*.{0}", s.Trim(' ', '*', '.'))))
                        {
                            filesList.Add(sf);
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

        #region "GetImagePageCount" function
        /// <summary>
        /// Get the page count of the specified image
        /// </summary>
        /// <param name="sourceImage">The image  path to get size from.</param>
        /// <param name="evt">Process page event</param>
        public static int GetImagePageCount(String sourceImage, object evt)
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

                        if (result>0 && evt != null && evt is CCDelegates.OnPageReadEvt)
                        {
                            for (int i = 0; i < result; i++)
                            {
                                img.SelectActiveFrame(FrameDimension.Page, i);
                                try
                                {
                                    (evt as CCDelegates.OnPageReadEvt)(evt, sourceImage, i, img as Bitmap);
                                }
                                catch (Exception ep)
                                {
                                    ILog.LogError(ep);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //-- not interested in error, if image was ooened it has one page probably.
                ILog.LogInfo(ex.ToString());
            }
            return result;
        }
        #endregion


        #region "ListComparerByRoiOrder"
        /// <summary>
        /// Compare and sort roi list items by FieldOrder (left to right).
        /// </summary>
        internal class ListComparerByRoiOrder : System.Collections.Generic.IComparer<ITisROIParams>
        {
            /// <summary>
            /// the actual compare method
            /// </summary>
            /// <param name="x">an ITisROIParams object</param>
            /// <param name="y">an ITisROIParams object</param>
            /// <returns>compare result</returns>
            public int Compare(ITisROIParams x, ITisROIParams y)
            {
                if (x.Miscellaneous.OrderInField > y.Miscellaneous.OrderInField)
                {
                    return 1;
                }
                else if (x.Miscellaneous.OrderInField < y.Miscellaneous.OrderInField)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion

        #region "GetLinkedRois" function
        /// <summary>
        /// Get all the ROIs that are linke to a field.
        /// </summary>
        /// <param name="efi"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static ITisROIParams[] GetLinkedRois(ITisEFIParams efi, String fieldName)
        {
            List<ITisROIParams> rois = new List<ITisROIParams>();
            try
            {
                foreach (ITisROIParams roi in efi.ROIs)
                {
                    if (String.Compare(roi.Miscellaneous.FieldName, fieldName, true) == 0)
                    {
                        rois.Add(roi);
                    }
                }

                rois.Sort(new ListComparerByRoiOrder());
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return rois.ToArray();
        } 
        #endregion

        #region "MergerRoisRect" function
        /// <summary>
        /// Merge all the specified ROIs to a single rect.
        /// </summary>
        /// <param name="rois">The rois to merge</param>
        /// <returns>A rectangle of the merged rois</returns>
        public static TIS_RECT MergerRoisRect(params ITisROIParams[] rois)
        {
            TIS_RECT result = new TIS_RECT();
            
            try
            {
                Rectangle rct = Rectangle.Empty;
                
                foreach (ITisROIParams roi in rois)
                {
                    if (rct.IsEmpty ) rct = new Rectangle(roi.Miscellaneous.RegionLeft, roi.Miscellaneous.RegionTop, roi.Miscellaneous.RegionRight - roi.Miscellaneous.RegionLeft, roi.Miscellaneous.RegionBottom - roi.Miscellaneous.RegionTop);
                    else rct.Intersect(new Rectangle(roi.Miscellaneous.RegionLeft, roi.Miscellaneous.RegionTop, roi.Miscellaneous.RegionRight - roi.Miscellaneous.RegionLeft, roi.Miscellaneous.RegionBottom - roi.Miscellaneous.RegionTop));
                }

                result.Left = rct.Left;
                result.Top = rct.Top;
                result.Right = rct.Right;
                result.Bottom = rct.Bottom;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result;
        }
        #endregion

        #region "GetSettingsFilePath function
        /// <summary>
        /// Create the path to the default settings XML file path.
        /// </summary>
        /// <returns></returns>
        public static String GetSettingsFilePath()
        {
            try
            {
                string baseDllName = Path.Combine(DLL_NAME, String.Format("{0}-{1}.{2}", DLL_NAME, CCEnums.CCNames.Settings, CCEnums.CCFilesExt.XML));
                string baseName = baseDllName;

                try
                {
                    baseName = Path.Combine(Application.ProductName, String.Format("{0}-{1}.{2}", Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().ManifestModule.Name), CCEnums.CCNames.Settings, CCEnums.CCFilesExt.XML));
                }
                catch (Exception ea)
                {
                    ILog.LogError(ea);
                }

                string pathTest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), baseName);

                if (!File.Exists(pathTest)) pathTest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), baseDllName);
                if (!File.Exists(pathTest)) pathTest = Path.Combine(TisClientServicesModule.GetSingletoneInstance().BasicConfiguration.eFlowBinPath, Path.GetFileName( baseName));
                if (!File.Exists(pathTest)) pathTest = Path.Combine(TisClientServicesModule.GetSingletoneInstance().BasicConfiguration.eFlowBinPath, Path.GetFileName( baseDllName));
                if (!File.Exists(pathTest)) pathTest = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Path.GetFileName( baseName));
                if (!File.Exists(pathTest)) pathTest = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Path.GetFileName( baseDllName));

                return pathTest;

            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return null;
        }
        #endregion       

        #region "DataTableFromString" function
        /// <summary>
        /// Parse a string into a DataTable.
        /// </summary>
        /// <param name="tableDef">The string to read as a table data</param>
        /// <param name="rowDelimiters">The data row delimiter\s</param>
        /// <param name="colDelimiters">The data columns delimiter\s</param>
        /// <returns>a DataTable.</returns>
        public static DataTable DataTableFromString(String tableDef, String rowDelimiters, String colDelimiters)
        {
            DataTable result = new DataTable();
            try
            {
                //-- Break the data to rows, ignoring empty rows --\\
                String[] rows = tableDef.Split(rowDelimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int rowCounter = 0;

                foreach (String columns in rows)
                {
                    //-- Break each row into it's columns contents --\\
                    String[] cols = columns.Split(colDelimiters.ToCharArray());
                    List<String> colVals = new List<String>();

                    foreach (String col in cols)
                    {
                        if (rowCounter == 0)
                        {
                            //-- if this is the first row, add a Table.Column for each column String value --\\
                            result.Columns.Add(col);
                        }
                        else
                        {
                            //-- Collect the row data --\\
                            colVals.Add(col);
                        }
                    }

                    //-- Add the data row to DataTable (Unless this is the first row that should have column names) --\\
                    if (rowCounter > 0) result.Rows.Add(colVals.ToArray());
                    rowCounter++;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result;
        }
        #endregion 
        
        #region "HoldCollection" method
        /// <summary>
        /// This mehtod put active collection on hold (in Custom or SimpleAuto stations)
        /// If collection is given in parmeter then given collection is put on hold.
        /// </summary>
        /// <param name="csm">eFlows Client Service Module</param>
        /// <param name="collection">Collection to hold.</param>
        public static void HoldCollection(ITisClientServicesModule csm, ITisCollectionData collection)
        {
            if (collection != null && csm != null)
            {
                try
                {
                    WriteToStationWindow(csm, String.Format("Collection [{0}] will be put on   -> HOLD <-", collection.Name));

                    List<String> collections = new List<String>(csm.Dynamic.AvailableCollectionNames);

                    if (collections.Contains(collection.Name))
                    {
                        csm.Dynamic.FreeSpecificCollection(collection, false);
                        csm.DynamicManage.SetCollectionHoldStateByName(collection.Name, true);
                    }
                    else
                    {
                        ILog.LogInfo("Collection {0} is not available in the [{1}] station collections", collection.Name, csm.Session.StationName);
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
            }
        }
        #endregion "HoldCollection" method
 
        #region "KeyExists" function
        /// <summary>
        /// Check if a key in a List with ICCName implemeted exists.
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static bool KeyExists(List<ICCName> lst, String key)
        {//people.ConvertAll(item => (IName)item);
            try
            {
                foreach (ICCName nm in lst)
                {
                    if (String.Compare(key, nm.Name, true) == 0) return true;
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

        #region "SetNamedUserTags" function
        /// <summary>
        /// Set namedUserTags too the eFlow object.
        /// </summary>
        /// <param name="eflowObject">The eFlow object to set its NamedUserTags.</param>
        /// <param name="setNamedTags">The dictionary of NamedUserTags to set.</param>
        /// <returns>true when successfull.</returns>
        public static bool SetNamedUserTags(ITisDataLayerTreeNode eflowObject, Dictionary<String,String> setNamedTags)
        {
            try
            {
                if (eflowObject == null || setNamedTags == null || setNamedTags.Count <= 0) return false;

                foreach (KeyValuePair<String, String> kv in setNamedTags)
                {
                    if (eflowObject.UserTagsMap.Contains(kv.Key))
                    {
                        eflowObject.UserTagsMap[kv.Key] = kv.Value;
                    }
                    else
                    {
                        eflowObject.UserTagsMap.Add(kv.Key, kv.Value);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return false;
        }
        #endregion

        #region "SetUserTags" function
        /// <summary>
        /// Set specified UserTags to the eFlow object.
        /// </summary>
        /// <param name="eflowObject">The eFlow object to set its UserTags.</param>
        /// <param name="setTags">The dictionary of UserTags to set.</param>
        /// <returns>true whne successfull.</returns>
        public static bool SetUserTags(ITisDataLayerTreeNode eflowObject, Dictionary<String,String> setTags)
        {
            try
            {
                if (eflowObject == null || setTags == null || setTags.Count <= 0) return false;
                int count = 0;
                foreach (KeyValuePair<String,String> kv in setTags)
                {
                    eflowObject.set_UserTags((short)count, kv.Value);
                    count++;
                }
                return true;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return false;
        }
        #endregion

        #region "SetSpecialTags" function
        /// <summary>
        /// Set the specified specialTags\Specialtags to the eFlow object.
        /// </summary>
        /// <param name="eflowObject">The eFlow object to set its UserTags.</param>
        /// <param name="setSpecialTags">The dictionary of UserTags to set.</param>
        /// <returns>true whne successfull.</returns>
        public static bool SetSpecialTags(ISupportsSpecialTags eflowObject, Dictionary<String, String> setSpecialTags)
        {
            try
            {
                if (eflowObject == null || setSpecialTags == null || setSpecialTags.Count <= 0) return false;

                foreach (KeyValuePair<String,String> kv in setSpecialTags)
                {
                    if (eflowObject.AttachedSpecialTags.Contains(kv.Key))
                    {
                        String tor = eflowObject.AttachedSpecialTags[kv.Key].TagOrigin ?? String.Empty;
                        if (tor.CompareTo(kv.Value) == 0) continue;

                        eflowObject.RemoveSpecialTag(kv.Key);
                    }

                    eflowObject.AddSpecialTag(kv.Key, kv.Value);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return false;
        }

        /// <summary>
        /// Set the specified specialTags\Specialtags to the eFlow object.
        /// </summary>
        /// <param name="eflowObject"></param>
        /// <param name="setExceptions"></param>
        /// <returns></returns>
        public static bool SetExceptions(ISupportsSpecialTags eflowObject, Dictionary<String,String> setExceptions)
        {
            try
            {
                if (eflowObject == null || setExceptions == null || setExceptions.Count <= 0) return false;

                foreach (KeyValuePair<String, String> kv in setExceptions)
                {
                    if (eflowObject.AttachedSpecialTags.Contains(kv.Key))
                    {
                        String tor = eflowObject.AttachedSpecialTags[kv.Key].TagOrigin ?? String.Empty;
                        if (tor.CompareTo(kv.Value) == 0) continue;

                        eflowObject.RemoveSpecialTag(kv.Key);
                    }

                    eflowObject.AddSpecialTag(kv.Key, kv.Value);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return false;
        }
        #endregion

        #region "GetNamedUserTags" function
        /// <summary>
        /// Get the namedUserTags of the specified eflow object.
        /// </summary>
        /// <param name="eflowObject">The eFlow object to get its NamedUserTags.</param>
        /// <param name="skipEmpty">Skip empty NamedUserTags (value)</param>
        /// <returns>A dictionary of Key values when successfull.</returns>
        public static Dictionary<String, String> GetNamedUserTags(ITisDataLayerTreeNode eflowObject, bool skipEmpty)
        {
            Dictionary<String, String> result = new Dictionary<String, String>();
            try
            {
                System.Collections.IDictionaryEnumerator enm = eflowObject.UserTagsMap.GetEnumerator();
                while (enm.MoveNext())
                {
                    if (skipEmpty && String.IsNullOrEmpty(enm.Value as String)) continue;
                    result.Add(enm.Key as String, enm.Value as String);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result;
        }
        #endregion

        #region "GetUserTags" function
        /// <summary>
        /// Get the UserTags of the specified eflow object.
        /// </summary>
        /// <param name="eflowObject">the eFlow object to get it's UserTags</param>
        /// <param name="skipEmpty">Skip empty UserTags</param>
        /// <returns>A dictionary of Key (number) values when successfull.</returns>
        public static Dictionary<String, String> GetUserTags(ITisDataLayerTreeNode eflowObject, bool skipEmpty)
        {
            Dictionary<String, String> result = new Dictionary<String, String>();
            try
            {
                int sc = 0;
                for (int i = 0; i < eflowObject.NumberOfUserTags; i++)
                {
                    if (skipEmpty && String.IsNullOrEmpty(eflowObject.get_UserTags((short)i))) continue;

                    sc++;
                    result.Add(sc.ToString("000"), eflowObject.get_UserTags((short)i));
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result;
        }
        #endregion

        #region "GetSpecialTags" function
        /// <summary>
        /// Get the specialTags of the specified eFlow object.
        /// </summary>
        /// <param name="eflowObject">The eFlow object to get it's SpecialTags</param>
        /// <returns>A dictionary of Key values when successfull.</returns>
        public static Dictionary<String,String> GetSpecialTags(ISupportsSpecialTags eflowObject)
        {
            Dictionary<String, String> result = new Dictionary<String, String>();
            try
            {
                foreach (ITisSpecialTagData sp in eflowObject.AttachedSpecialTags)
                {
                    result.Add(sp.Name, sp.TagOrigin ?? String.Empty);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result;
        }
        #endregion

        #region "MergeImages" function
        /// <summary>
        /// Merge all tiff images (multi-page or not) to a multi-page tif.
        /// </summary>
        /// <param name="targetPath">The target path to save the merged images to.</param>
        /// <param name="deleteSource">Delete source files when flag is true.</param>
        /// <param name="filePaths">The image\s to merge.</param>
        /// <returns>true when successfull.</returns>
        public static bool MergeImages(String targetPath, bool deleteSource, params String[] filePaths)
        {
            try
            {
                if (filePaths.Length <= 0) throw new Exception("No files specified in method: " + MethodBase.GetCurrentMethod().Name);
                else if (filePaths.Length == 1)
                {
                    // TODO: use 3rd pary software to make the image compatiable (dpi bw etc)
                    if (deleteSource)
                    {
                        File.SetAttributes(filePaths[0], FileAttributes.Normal);
                        File.Move(filePaths[0], targetPath);
                    }
                    else
                    {
                        File.Copy(filePaths[0], targetPath);
                        File.SetAttributes(targetPath, FileAttributes.Normal);
                    }
                }
                else
                {
                    FileOP fop = new FileOP();
                    fop.MergeImageFile(targetPath, filePaths);
                    // TODO: use 3rd pary software to make the image compatiable (dpi bw etc)

                    if (deleteSource)
                    {
                        foreach (String s in filePaths)
                        {
                            try
                            {
                                if (File.Exists(s))
                                {
                                    File.SetAttributes(s, FileAttributes.Normal);
                                    File.Delete(s);
                                }
                            }
                            catch (Exception ep)
                            {
                                ILog.LogError(ep);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
                return false;
            }
        }
        #endregion

        #region "StrToIntDef" function
		/// <summary>
        /// Convert a string to int.
        /// </summary>
        /// <param name="str">The string to converto to int.</param>
        /// <param name="def">The default value to set when failed.</param>
        /// <returns>Parsed int of default value if failed.</returns>
        internal static int StrToIntDef(String str, int def)
        {
            try
            {
                def = int.Parse(str);
            }
            catch { }
            return def;
        }
        #endregion

        #region "WriteToStationWindow" procedure code
        /// <summary>
        /// Write a message to a station's window.
        /// </summary>
        /// <param name="oCSM">ITisClientServicesModule object.</param>
        /// <param name="sFormat">The format of the message.</param>
        /// <param name="Params">The message to send.</param>
        /// <example><code>WriteToStationWindow(oCSM,"G","YourMessage");</code></example>
        public static void WriteToStationWindow(ITisClientServicesModule oCSM, String sFormat, params object[] Params)
        {
            if (oCSM != null)
            {
                String sMessage = String.Format(sFormat, Params);
                oCSM.ModuleAccess.DoAction(CCEnums.CCNames.ReportMessage.ToString(), DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " - " + sMessage);
            }
        }

        /// <summary>
        /// Write a message to a station's window.
        /// </summary>
        /// <param name="oCSM">ITisClientServicesModule object.</param>
        /// <param name="sMessage">The message to send.</param>
        /// <example><code>WriteToStationWindow(oCSM,"YourMessage");</code> </example>
        public static void WriteToStationWindow(ITisClientServicesModule oCSM, String sMessage)
        {
            if (oCSM != null)
            {
                oCSM.ModuleAccess.DoAction(CCEnums.CCNames.ReportMessage.ToString(), DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " - " + sMessage);
            }
        }

        /// <summary>
        /// Write a message to a station's window.
        /// </summary>
        /// <param name="oCSM">ITisClientServicesModule object.</param>
        /// <param name="sMessage">The message to send.</param>
        /// <param name="simpleString">A dummy variable to differentiate this function (write a simple string message)</param>
        /// <example><code>WriteToStationWindow(oCSM,"YourMessage",true);</code></example>
        public static void WriteToStationWindow(ITisClientServicesModule oCSM, String sMessage, bool simpleString)
        {
            if (oCSM != null)
            {
                oCSM.ModuleAccess.DoAction(CCEnums.CCNames.ReportMessage.ToString(), sMessage);
            }
        }

        /// <summary>
        /// Write a message to a station's window.
        /// </summary>
        /// <param name="oCSM">ITisClientServicesModule object.</param>
        /// <param name="scMessages">The messages to send.</param>        
        /// <example><code>
        /// StringCollection messages = new StringCollection();
        /// messages.Add("YourMessage_1");
        /// messages.Add("YourMessage_2");
        /// messages.Add("YourMessage_3");
        /// WriteToStationWindow(oCSM, messages);
        /// </code> </example>
        public static void WriteToStationWindow(ITisClientServicesModule oCSM, params String[] scMessages)
        {
            if (oCSM != null && scMessages != null)
            {
                foreach (String msg in scMessages)
                {
                    if (msg != null)
                    {
                        oCSM.ModuleAccess.DoAction(CCEnums.CCNames.ReportMessage.ToString(), msg);
                    }
                }
            }
        }
        #endregion "WriteToStationWindow" method code
    }
}