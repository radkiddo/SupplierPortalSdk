using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using TiS.Core.eFlowAPI;

namespace TiS.Engineering.InputApi
{
    #region "CCConfiguration" class
    /// <summary>
    /// The class to use to configure file search and all other global settings.
    /// </summary>
    /// <remarks>Can't be internal for deserialization reasons</remarks>
    public partial class CCConfiguration:CCGenericSerializer, ICCConfiguration
    {
        #region class variables
        private CCConfigurationData[] configurations;
        #endregion

        #region "XmlPath" property
        private String xmlPath;
        /// <summary>
        /// stores the source XMl file path.
        /// </summary>
        [XmlIgnore]
        public String XmlPath { get { return xmlPath; } set { xmlPath = value; } }
        #endregion

        #region "FromCSM" function
        /// <summary>
        /// Get CCConfiguration from CSM work directory XML file
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to deserialize the data from.</param>
        /// <returns>The CCConfiguration as deserialized from XML.</returns>
        public static CCConfiguration FromCSM(ITisClientServicesModule csm)
        {
            return FromXml(Path.Combine(csm.PathLocator.get_Path(CCEnums.CCFilesExt.TIF.ToString()),
                String.Format("{0}-{1}.{2}", Application.ProductName, CCEnums.CCNames.Settings, CCEnums.CCFilesExt.XML)));
        }
        #endregion

        #region "FromXml" function
        /// <summary>
        /// Get CCConfiguration from XML file
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to deserialize the data from.</param>
        /// <returns>The CCConfiguration as deserialized from XML.</returns>
        public static new CCConfiguration FromXml(String xmlFilePath)
        {
            return FromXml(xmlFilePath, typeof(CCConfiguration)) as CCConfiguration;
        }

        /// <summary>
        /// Get CCConfiguration from XML file, using the default path.
        /// </summary>
        /// <returns>The CCConfiguration as deserialized from XML.</returns>
        public static CCConfiguration FromXml()
        {
            return FromXml(CCUtils.GetSettingsFilePath(), typeof(CCConfiguration)) as CCConfiguration;
        }
        #endregion

        #region "GetConfiguration" function
        /// <summary>
        /// Get a profile by name.
        /// </summary>
        /// <param name="configName">The profile name to get</param>
        /// <returns>A CCConfigurationData when successfull.</returns>
        public CCConfigurationData GetConfiguration(String configName)
        {
            try
            {
                if (configurations != null)
                {
                    foreach (CCConfiguration.CCConfigurationData ccd in configurations)
                    {
                        if (String.Compare(ccd.Name, configName, true) == 0)
                        {
                            ccd.ParentConfiguration = this;
                            return ccd;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex, false);
                throw ex;
            }
            return null;
        }
        #endregion

        #region "ProfileNames" property
        /// <summary>
        /// Get all profile names.
        /// </summary>
        /// <returns>An array of profile names when successfull.</returns>
        [XmlIgnore]
        public String[] ProfileNames
        {
            get
            {
                List<String> res = new List<String>();
                try
                {
                    if (configurations != null)
                    {
                        foreach (CCConfigurationData ccd in configurations)
                        {
                            res.Add(ccd.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
                return res.ToArray();
            }
        }
        #endregion

        #region class properties
        /// <summary>
        /// The class configurations
        /// </summary>
        public CCConfigurationData[] Configurations
        {
            get { return this.configurations; }
            set
            {
                this.configurations = value;
                if (this.configurations != null)
                {
                    foreach (CCConfigurationData cd in this.configurations)
                    {
                        cd.XmlFilePath = this.XmlPath;
                        cd.ParentConfiguration = this;
                    }
                }
            }
        }
        #endregion
    }
    #endregion
}
