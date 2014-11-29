using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Drawing.Design;

namespace TiS.Engineering.InputApi
{
    public partial class CCConfiguration
    {
        #region "CCBaseConfigurationData" class
        /// <summary>
        /// Profile data class, contains settings per profile.
        /// </summary>
#if INTERNAL
        public class CCBaseConfigurationData : CCGenericSerializer, ICCBaseConfigurationData
#else
        public class CCBaseConfigurationData : CCGenericSerializer, ICCBaseConfigurationData
#endif
        {
            #region class variables
            private String lockExtension;
            private String errorFolder;               
            #endregion

            #region class properties
            #region "Name" property
            private String name;
            /// <summary>
            /// The profile profile name.
            /// </summary>
            [Description("The profile profile name.")]
            public String Name { get { return name; } set { name = value; } }
            #endregion

            #region "FlowType" property
            private String flowType;
            /// <summary>
            /// The profile collections flow type (leave empty to define when creating the collection).
            /// </summary>
            [Description("The profile collections flow type (leave empty to define when creating the collection).")]
            public String FlowType { get { return flowType; } set { flowType = value; } }
            #endregion

            #region "CopySourceFiles" property
            private bool copySourceFiles;
            /// <summary>
            /// Get or set copy source files when true, move when false.
            /// </summary>
            [Description("Copy source files when true, move when false.")]
            public bool CopySourceFiles { get { return copySourceFiles; } set { copySourceFiles = value; } }
            #endregion

            #region "ErrorFolderPath" property
            /// <summary>
            /// Get or set the ierror folder path, where all collection files would be moved to when an error occures (nothing would happen if this value is not specifiec).
            /// </summary>
            [Editor(typeof(FolderBrowseProp), typeof(UITypeEditor)), Category("Path"), Description("Get or set the error folder path, where all collection files would be moved to when an error occures (nothing would happen if this value is not specifiec).")]
            public String ErrorFolderPath
            {
                get
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(errorFolder) && !String.IsNullOrEmpty(ErrorFolderDateFormat))
                        {
                            return Path.Combine(errorFolder, String.Format(ErrorFolderDateFormat, DateTime.Now));
                        }
                    }
                    catch (Exception ex)
                    {
                        ILog.LogError(ex);
                    }
                    return errorFolder ?? String.Empty;
                }

                set { errorFolder = value; }
            }
            #endregion

            #region "ErrorFolderDateFormat" property
            private String errorFolderDateFormat;
            /// <summary>
            /// Get or set the error folder path dynamic additional folder path, (i.e. sub folder\\s per collection),  position {0} is current DateTime.
            /// </summary>
            /// <example>ErrorFolderDynamicFormat="{0:ddMMyyy_HHMMss}"; //-- this will create an additional folder named 13052011_120723 for example --\\</example>
            [Description("Get or set the error folder path date additional folder path, (i.e. sub folder\\s per collection), position {0} is current DateTime.")]
            public String ErrorFolderDateFormat { get { return errorFolderDateFormat; } set { errorFolderDateFormat = value; } }
            #endregion

            #region "IgnoreExceptions" property
            private bool ignoreExceptions;
            /// <summary>
            /// Get or set the ignore specialTags state, avoid get or set specialTags when deserializing\creating a collection from file.
            /// </summary>
            [Description("Get or set the ignore specialTags state, avoid get or set specialTags when deserializing\\creating a collection from file.")]
            public bool IgnoreExceptions { get { return ignoreExceptions; } set { ignoreExceptions = value; } }
            #endregion

            #region "IgnoreNamedUserTags" property
            private bool ignoreNamedUserTags;
            /// <summary>
            /// Get or set the ignore NamedUserTags state, avoid get or set NamedUserTags when deserializing\creating a collection from file.
            /// </summary>
            [Description("Get or set the ignore NamedUserTags state, avoid get or set NamedUserTags when deserializing\\creating a collection from file.")]
            public bool IgnoreNamedUserTags { get { return ignoreNamedUserTags; } set { ignoreNamedUserTags = value; } }
            #endregion

            #region "IgnoreUserTags" property
            private bool ignoreUserTags;
            /// <summary>
            /// Get or set the ignore UserTags state, avoid get or set UserTags when deserializing\creating a collection from file.
            /// </summary>
            [Description("Get or set the ignore UserTags state, avoid get or set UserTags when deserializing\\creating a collection from file.")]
            public bool IgnoreUserTags { get { return ignoreUserTags; } set { ignoreUserTags = value; } }
            #endregion

            #region "LockExtension" property
            /// <summary>
            /// Get or set the extension to use for file lock.
            /// </summary>
            [Description("Get or set the extension to use for file lock.")]
            public String LockExtension
            {
                get { return (lockExtension ?? CCEnums.CCNames.ExtProc.ToString()).Trim('*', '.', ' '); }
                set { lockExtension = value ?? CCEnums.CCNames.ExtProc.ToString(); }
            }
            #endregion

            #region "LoginStation" property
            private String loginStation;
            /// <summary>
            /// Get or set the station name to login in the defined application.
            /// </summary>
            [Description("Get or set the station name to login in the defined application.")]
            public String LoginStation { get { return loginStation; } set { loginStation = value; } }
            #endregion

            #region "LoginApplication" property
            private string loginApplication;
            /// <summary>
            /// Get or set the application name to login in to.
            /// </summary>
            [Description("Get or set the application name to login in to.")]
            public String LoginApplication { get { return loginApplication; } set { loginApplication = value; } }
            #endregion

            #region "MaxCsmCount" property
            private int maxCsmCount;
            /// <summary>
            /// Get or set the Maximum amount of files to lock per timer interval.
            /// </summary>
            [Description("Get or set the Maximum CSM count to use in the csm manager.")]
            public int MaxCsmCount { get { return maxCsmCount; } set { maxCsmCount = value; } }
            #endregion

            #region "ParentConfiguration" property
            private CCConfiguration parentConfiguration;
            /// <summary>
            /// The parent configuration runtime access.
            /// </summary>
            [XmlIgnore, ReadOnly(true)]
            public CCConfiguration ParentConfiguration { get { return parentConfiguration; } set { parentConfiguration = value; } }
            #endregion

            #region "ThrowAllExceptions" property
            private bool throwAllExceptions;
            /// <summary>
            /// Throw all errors so they will be caught by the caller when true.
            /// </summary>
            [Description("When true throw all specialTags so they will be caught by the API caller.")]
            public bool ThrowAllExceptions { get { return throwAllExceptions; } set { throwAllExceptions = value; } }
            #endregion

            #region "UseSourceNamedUserTags" property
            private bool useSourceNamedUserTags;
            /// <summary>
            /// Get or set the use named user tags for storing collection info.
            /// </summary>
            [Description("Get or set the use named user tags for storing collection info.")]
            public bool UseSourceNamedUserTags { get { return useSourceNamedUserTags; } set { useSourceNamedUserTags = value; } }
            #endregion

            #region "XmlFilePath" property
            private String xmlFilePath;
            /// <summary>
            /// Get or set the source XML file path.
            /// </summary>
            [XmlIgnore, Description("Get the source XML file path."), ReadOnly(true)]
            public String XmlFilePath { get { return xmlFilePath; } set { xmlFilePath = value; } }
            #endregion
            #endregion
        }
        #endregion
    }
}