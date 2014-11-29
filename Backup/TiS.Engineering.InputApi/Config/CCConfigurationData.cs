using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace TiS.Engineering.InputApi
{
    public partial class CCConfiguration
    {
        #region "CCConfigurationData" class
        /// <summary>
        /// Search extended profile data class, contains settings per profile.
        /// </summary>
#if INTERNAL
        public class CCConfigurationData : CCBaseConfigurationData, ICCConfigurationData
#else
        public class CCConfigurationData : CCBaseConfigurationData, ICCConfigurationData
#endif
        {
            #region class variables
            private String[] searchPaths;
            private String[] searchExt;
            private String[] attachmentsExtensions;            
            #endregion

            #region class contsructor
            public CCConfigurationData()
                : base()
            {
                FilesSort = 1;
            } 
            #endregion

            #region "FromXml" function
            /// <summary>
            /// From XMl function, deserializes a profile from XML
            /// </summary>
            /// <param name="xmlPath">The XML file path</param>
            /// <param name="profileName">the profile name to load.</param>
            /// <returns>CCConfigurationData</returns>
            public static CCConfigurationData FromXml(String xmlPath, String profileName)
            {
                CCConfiguration.CCConfigurationData result = null;

                try
                {
                    CCConfiguration cfg = CCConfiguration.FromXml(xmlPath);
                    return cfg.GetConfiguration(profileName);
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                    if (result != null && result.ThrowAllExceptions) throw ex;
                }
                return result;
            }

            /// <summary>
            /// From XML function, deserializes a profile from XML, using the default path.
            /// </summary>
            /// <param name="profileName">the profile name to load.</param>
            /// <returns>CCConfigurationData</returns>
            public static new CCConfigurationData FromXml(String profileName)
            {
                return FromXml(CCUtils.GetSettingsFilePath(), profileName);
            }
            #endregion

            #region class properties
            private int filesSort;
            /// <summary>
            /// Sort searched files depending on sort value.
            /// </summary>
            [Description("Sort searched files by date when set to 1,no sort when 0 or lower")]
            public int FilesSort { get { return filesSort; } set { filesSort = value; } }

            /// <summary>
            /// The attachments extensions (files)  to copy with a collection.
            /// </summary>
            [Description("The attachments extensions (files) to copy with a collection")]
            public String[] AttachmentsExtensions
            {
                get
                {
                    if (attachmentsExtensions == null) attachmentsExtensions = new String[0];
                    return attachmentsExtensions;
                }
                set { attachmentsExtensions = value; }
            }

            /// <summary>
            /// Get or set the keep file name state; use the original image name as the collection name.
            /// </summary>
            private bool keepFileName;
            [Description("Get or set the keep file name state; use the original image name as the collection name.")]
            public bool KeepFileName { get { return keepFileName; } set { keepFileName = value; } }

            /// <summary>
            /// Log the collection definition DataTable contents when true.
            /// </summary>
            private bool logCollectionDataTable;
            [Description("Log the collection definition DataTable contents when true.")]
            public bool LogCollectionDataTable { get { return logCollectionDataTable; } set { logCollectionDataTable = value; } }            

            /// <summary>
            /// Get or set the Maximum amount of files to lock per timer interval.
            /// </summary>
            private int maxFilesLock;
            [Description("Get or set the Maximum amount of files to lock per timer interval.")]
            public int MaxFilesLock { get { return maxFilesLock; } set { maxFilesLock = value; } }

            /// <summary>
            /// Get or set the Multi page per form status (create all pages from a single file to a single form).
            /// </summary>
            private bool multiPagePerForm;
            [Description("Get or set the Multi page per form status (create all pages from a single file to a single form).")]
            public bool MultiPagePerForm { get { return multiPagePerForm; } set { multiPagePerForm = value; } }

            /// <summary>
            /// The search paths to look for trigger files
            /// </summary>
            [Description("The search path\\s to look for trigger files.")]
            public String[] SearchPaths
            {
                get
                {
                    if (searchPaths == null) searchPaths = new String[0];
                    return searchPaths;
                }
                set { searchPaths = value; }
            }

            /// <summary>
            /// The extension\s of the trigger files to look for.
            /// </summary>
            [Description("The extension\\s of the trigger files to look for.")]
            public String[] SearchExtensions
            {
                get
                {
                    if (searchExt == null) searchExt = new String[0];
                    return searchExt;
                }
                set { searchExt = value; }
            }

            /// <summary>
            /// Search sub folders when true.
            /// </summary>
            private bool searchSubFolders;
            [Description("Search sub folders when true.")]
            public bool SearchSubFolders { get { return searchSubFolders; } set { searchSubFolders = value; } }

            /// <summary>
            /// The polling timer intervals
            /// </summary>
            private int timerIntervals;
            [Description("The polling timer intervals.")]
            public int TimerIntervals { get { return timerIntervals; } set { timerIntervals = value; } }
            #endregion
        }
        #endregion
    }
}