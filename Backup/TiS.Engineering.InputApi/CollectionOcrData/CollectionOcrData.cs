using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using TiS.Core.eFlowAPI;
using TiS.Recognition;
using TiS.Recognition.Common;

namespace TiS.Engineering.InputApi
{
    #region "CollectionOcrData" class
    /// <summary>
    /// CollectionOcrData class, contsians all OcrPOage data for all pages in the collection.
    /// </summary>
    public partial class CollectionOcrData : CCGenericSerializer, ICollectionOcrData
    {
        #region class variables
        /// <summary>
        /// The pages from whcih the collection is consisted.
        /// </summary>
        private PageOcrData[] pages;
        #endregion

        #region class properties
        #region "CollectionID" property
        private String collectionID;
        /// <summary>
        /// Get or set  the name of the collection that was used to initialize the class.
        /// </summary>
        public String CollectionID { get { return collectionID; } set { collectionID = value; } }
        #endregion

        #region "FlowType" property
        private String flowType;
        /// <summary>
        /// Get or set the flow name of the collection that was used to initialize the class.
        /// </summary>
        [XmlIgnore]
        public String FlowType { get { return flowType; } set { flowType = value; } }
        #endregion

        #region "Pages" property
        /// <summary>
        /// Get or set  the collection PRD data (a <see cref="PageOcrData"/> class for each <see cref="ITisPageData"/>).
        /// </summary>
        public PageOcrData[] Pages
        {
            get { return pages; }
            set { pages = value; }
        } 
        #endregion
        #endregion

        #region "FromCollection" function
        /// <summary>
        /// Get CollectionOcrData from ITisCollectionData object.
        /// </summary>
        /// <param name="collection">The collection to get it's page OCR data.</param>
        /// <returns>The CollectionOcrData as deserialized from ITisCollectionData.</returns>
#if INTERNAL
        internal static CollectionOcrData FromCollection(ITisCollectionData collection)
#else
        public static CollectionOcrData FromCollection(ITisCollectionData collection)
#endif
        {
            try
            {
                if (collection != null)
                {
                    CollectionOcrData res = new CollectionOcrData();
                    res.CollectionID = collection.Name;
                    res.FlowType = collection.FlowType;
                    List<PageOcrData> pgs = new List<PageOcrData>();

                    //-- Iterate collection pages and add them to the class Pages property. --\\
                    foreach (ITisPageData page in collection.Pages)
                    {
                        pgs.Add(PageOcrData.FromPRD(page));
                    }

                    if (pgs != null && pgs.Count > 0)
                    {
                        res.pages = pgs.ToArray();
                        return res;
                    }
                }
                else
                {
                    throw new Exception(String.Format("ITisCollectionData specified in: [{0}] is not valid", MethodBase.GetCurrentMethod().Name));
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return null;
        }
        #endregion

        #region "FromXml" function
        /// <summary>
        /// Get CollectionOcrData from XML file
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to deserialize the data from.</param>
        /// <returns>The CollectionOcrData as deserialized from XML.</returns>
#if INTERNAL
        internal static new CollectionOcrData FromXml(String xmlFilePath)
#else
        public static new CollectionOcrData FromXml(String xmlFilePath)
#endif
        {
            try
            {
                return FromXml(xmlFilePath, typeof(CollectionOcrData)) as CollectionOcrData;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return null;
        }
        #endregion

        #region "XMLToPRD" event and function
        /// <summary>
        /// Convert XML file\s to PRD .
        /// </summary>
        /// <param name="targetPRD">The target PRD file to create</param>
        /// <param name="sourceXMLs">Source XML files.</param>
        /// <returns>true when successfull.</returns>
        public static bool XMLToPRD(string targetPRD, params string[] sourceXMLs)
        {
            bool result = false;
            try
            {
                foreach (string sourceXML in sourceXMLs)
                {
                    if (!File.Exists(sourceXML)) continue;

                    //-- Try loading as a single page --\\
                    PageOcrData pod = PageOcrData.FromXml(sourceXML);
                    if (pod != null && pod.ToPRD(targetPRD) != null)
                    {
                        if (File.Exists(targetPRD)) File.Delete(targetPRD);

                        ILog.LogDebug(string.Format("Done converting XML file [{0}] to PRD file [{1}]", sourceXML, targetPRD));
                        continue;
                    }

                    //-- Try as a whole collection in a single XML --\\
                    if (pod == null)
                    {
                        CollectionOcrData cod = CollectionOcrData.FromXml(sourceXML);
                        if (cod != null)
                        {
                            if (cod.Pages != null)
                            {
                                int lastIndex = -1;
                                int maxIndex = 0;

                                foreach (PageOcrData pd in cod.Pages)
                                {
                                    lastIndex = pd.Index;

                                    while (lastIndex < 0 || lastIndex == pd.Index || lastIndex <= maxIndex) lastIndex++;
                                    maxIndex = lastIndex;

                                    //-- Create a PRD file per page --\\
                                    string newPageName = Path.Combine(Path.GetDirectoryName(targetPRD), string.Format("{0}_P{1}.{2}", Path.GetFileNameWithoutExtension(targetPRD), (lastIndex).ToString("X").PadLeft(3, '0'), Path.GetExtension(targetPRD).TrimStart('.')));
                                    if (File.Exists(newPageName)) File.Delete(newPageName);
                                    pd.ToPRD(newPageName);
                                }
                            }

                            ILog.LogDebug(string.Format("Done converting XML file [{0}] to PRD file [{1}]", sourceXML, targetPRD));
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result;
        }

        /// <summary>
        /// Show an open file dialog for source XML and save File dialog for atrget PRD, then convert source XML to PRX.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
#if INTERNAL
        internal static void XMLToPRD(object sender, EventArgs e)
#else
        public static void XMLToPRD(object sender, EventArgs e)
#endif
        {
            try
            {
                using (OpenFileDialog opn = new OpenFileDialog())
                {
                    opn.Filter = "XML files (*.xml)|*.xml|Any file (*.*)|*.*";
                    opn.Title = "Open source XML...";
                    if (opn.ShowDialog() == DialogResult.OK)
                    {
                        using (SaveFileDialog sve = new SaveFileDialog())
                        {
                            sve.Filter = "PRD files (*.prd)|*.prd|Any file (*.*)|*.*";
                            sve.Title = "Save target PRD...";
                            sve.InitialDirectory = Path.GetDirectoryName(opn.FileName);
                            sve.FileName = Path.GetFileNameWithoutExtension(opn.FileName);
                            sve.DefaultExt = "prd";

                            if (sve.ShowDialog() == DialogResult.OK)
                            {
                                if (XMLToPRD(sve.FileName, opn.FileName))
                                {
                                    ILog.LogDebug(string.Format("Done converting XML file [{0}] to PRD file [{1}].", opn.FileName, sve.FileName));
                                    return;
                                }
                            }
                        }
                    }
                }

                ILog.LogWarning("Failed converting XML file to PRD file.");
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        }
        #endregion

        #region "PRDToXml" event and function
        /// <summary>
        /// Show an open file dialog for source PRD and save File dialog for atrget XML, then convert source PRD to XML.
        /// when multiple are selected they will be inserted into one XML file representing a collection
        /// </summary>>
        public static bool PRDToXml(string targetXML, params string[] sourcePRDs)
        {
            try
            {
                if (File.Exists(targetXML)) File.Delete(targetXML);

                if (sourcePRDs != null && sourcePRDs.Length == 1)
                {
                    if (PageOcrData.FromPRD(sourcePRDs[0]).ToXml(targetXML) && File.Exists(targetXML))
                    {
                        ILog.LogDebug(string.Format("Done converting PRD file [{0}] to XML file [{1}]", sourcePRDs[0], targetXML));
                        return true;
                    }
                }
                else if (sourcePRDs != null && sourcePRDs.Length > 1)
                {
                    CollectionOcrData collOcr = new CollectionOcrData();
                    List<PageOcrData> pages = new List<PageOcrData>();
                    foreach (string srcPrd in sourcePRDs)
                    {
                        if (File.Exists(srcPrd))
                        {
                            pages.Add(PageOcrData.FromPRD(srcPrd));
                        }
                    }
                    collOcr.pages = pages.ToArray();
                    collOcr.CollectionID = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fffff");
                    return collOcr.ToXml(targetXML);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return false;
        }

        /// <summary>
        /// Show an open file dialog for source PRD and save File dialog for atrget XML, then convert source PRD to XML.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
#if INTERNAL
        internal static void PRDToXml(object sender, EventArgs e)
#else
        public static void PRDToXml(object sender, EventArgs e)
#endif
        {
            try
            {
                using (OpenFileDialog opn = new OpenFileDialog())
                {
                    opn.Filter = "PRD files (*.prd)|*.prd|Any file (*.*)|*.*";
                    opn.Title = "Open source PRD\\s...";
                    opn.Multiselect = true;
                    opn.DefaultExt = "prd";

                    if (opn.ShowDialog() == DialogResult.OK)
                    {
                        using (SaveFileDialog sve = new SaveFileDialog())
                        {
                            sve.Filter = "XML files (*.xml)|*.xml|Any file (*.*)|*.*";
                            sve.Title = "Save target XML...";
                            sve.DefaultExt = "xml";
                            sve.InitialDirectory = Path.GetDirectoryName(opn.FileName);
                            sve.FileName = Path.GetFileNameWithoutExtension(opn.FileName);

                            if (sve.ShowDialog() == DialogResult.OK)
                            {
                                PRDToXml(sve.FileName, opn.FileNames);
                                return;
                            }
                        }
                    }
                }

                ILog.LogWarning("Failed converting PRD file to XML file.");
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        }
        #endregion
    }
    #endregion
}
