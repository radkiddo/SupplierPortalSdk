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
    public partial class CollectionOcrData
    {
        #region "PageOcrData" class
        /// <summary>
        /// PageOcrData class with static and instance methods.
        /// </summary>
        public class PageOcrData :OcrData, IPageOcrData
        {
            #region class variables
            private LineOcrData[] lines;
            private String linesDelimiter = "\r\n";
            #endregion

            #region class properties
            #region "Index" property
            /// <summary>
            /// Get or set the line index.
            /// </summary>
            public override int Index
            {
                get
                {
                    return base.Index;
                }
                set
                {
                    base.Index = value;
                }
            } 
            #endregion

            #region "Lines" property
            /// <summary>
            /// Get or set the item's LINES.
            /// </summary>
            public LineOcrData[] Lines
            {
                get { return lines; }
                set { lines = value; }
            } 
            #endregion

            #region "LinesSeparator" property
            /// <summary>
            /// Get or set the item's line separator\delimiter.
            /// </summary>
            [XmlIgnore]
            public String LinesSeparator
            {
                get { return linesDelimiter ?? "\r\n"; }
                set { linesDelimiter = value; }
            } 
            #endregion

            #region "PrdFilePath" proeprty
            /// <summary>
            /// Get or set the item's PRD file path.
            /// </summary>
            private String prdFilePath;
            [XmlIgnore]
            public String PrdFilePath { get { return prdFilePath; } set { prdFilePath = value; } }
            #endregion

            #region "Value" property
            /// <summary>
            /// Get the item's lines as text..
            /// </summary>
            [XmlIgnore]
            public virtual String Value
            {
                get
                {
                    if (lines != null || lines.Length > 0)
                    {
                        List<String> linesStr = new List<String>();
                        foreach (LineOcrData lod in lines)
                        {
                            linesStr.Add(lod.Value);
                        }
                        return String.Join(LinesSeparator, linesStr.ToArray());
                    }
                    else return String.Empty;
                }
            } 
            #endregion
            #endregion

            #region "FromPRD" functions
            /// <summary>
            /// Create <see cref="PageOcrData"/> from a prd file
            /// </summary>
            /// <param name="prdPath">The PRD file path to desrialize as <see cref="PageOcrData"/>.</param>
            /// <param name="doNotParseName">Do not parse the file name to retrieve it's index</param>
            /// <returns>PageOcrData when successfull, null when failed.</returns>
#if INTERNAL
            internal static PageOcrData FromPRD(String prdPath, bool doNotParseName)
#else
            public static PageOcrData FromPRD(String prdPath, bool doNotParseName)
#endif
            {
                try
                {
                    if (File.Exists(prdPath))
                    {
                        //-- Load from file -\\
                        PageOcrData res = FromPRD(TPage.LoadFromPRD(prdPath));
                        res.PrdFilePath = prdPath;

                        #region  //-- Try and get the page index from the file name --\\
                        if (!doNotParseName) try
                            {
                                int prs = -1;
                                String[] splt = Path.GetFileNameWithoutExtension(prdPath).Split('_');
                                prs = Convert.ToInt32(splt[splt.Length - 1].Trim('p', 'P'), 16);
                            }
                            catch { }
                        #endregion

                        return res;
                    }
                    else
                    {
                        throw new Exception(String.Format("File [{0}] could not be found, in: [{1}]", prdPath ?? String.Empty, MethodBase.GetCurrentMethod().Name));
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                return null;
            }

            /// <summary>
            /// Create <see cref="PageOcrData"/> from a prd file (get the path from the specified tis page.
            /// </summary>
            /// <param name="pageData">The ITisPageData object to desrialize as <see cref="PageOcrData"/>.</param>
            /// <returns>PageOcrData when successfull, null when failed.</returns>
#if INTERNAL
            internal static PageOcrData FromPRD(ITisPageData pageData)
#else
            public static PageOcrData FromPRD(ITisPageData pageData)
#endif
            {
                try
                {
                    if (pageData != null)
                    {
                        PageOcrData res = FromPRD(pageData.GetAttachmentFileName("PRD"), true);
                        res.Index = pageData.OrderInCollection - 1;
                        return res;
                    }
                    else
                    {
                        throw new Exception(String.Format("ITisPageData specified in: [{0}] is not valid", MethodBase.GetCurrentMethod().Name));
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                return null;
            }

            /// <summary>
            /// Create <see cref="PageOcrData"/> from a prd file
            /// </summary>
            /// <param name="pagePRD">The PRD page data to desrialize as <see cref="PageOcrData"/>.</param>
            /// <returns>PageOcrData when successfull, null when failed.</returns>
#if INTERNAL
            internal static PageOcrData FromPRD(TPage pagePRD)
#else
            public static PageOcrData FromPRD(TPage pagePRD)
#endif
            {
                try
                {
                    if (pagePRD != null)
                    {
                        PageOcrData res = new PageOcrData();
                        res.Index = -1;
                        res.Rect = pagePRD.Rectangle;
                        int li = 0;
                        List<LineOcrData> linesData = new List<LineOcrData>();

                        //-- Iterate PRD data lines --\\
                        foreach (TLine linePRD in pagePRD.Lines)
                        {
                            linesData.Add(new LineOcrData());
                            List<WordOcrData> wordsData = new List<WordOcrData>();
                            int wi = 0;

                            //-- Iterate each line's words --\\
                            foreach (TWord wordPRD in linePRD.Words)
                            {
                                wordsData.Add(new WordOcrData());

                                if (wordPRD.Chars != null && wordPRD.Chars.Count > 0)
                                {
                                    List<CharOcrData> charsData = new List<CharOcrData>();
                                    int ci = 0;

                                    //-- Iterate each word's chars --\\
                                    foreach (TChar charPRD in wordPRD.Chars)
                                    {
                                        //-- Add and define char data --\\
                                        charsData.Add(new CharOcrData());
                                        charsData[charsData.Count - 1].Index = ci;
                                        charsData[charsData.Count - 1].Confidence = charPRD.Confidance;
                                        charsData[charsData.Count - 1].Rect = charPRD.Rectangle;
                                        charsData[charsData.Count - 1].Value = charPRD.CharData;
                                        ci++;
                                    }
                                    if (charsData != null && charsData.Count > 0) wordsData[wordsData.Count - 1].Chars = charsData.ToArray();
                                }

                                //-- Define word data --\\
                                wordsData[wordsData.Count - 1].Confidence = wordPRD.Confidance;
                                wordsData[wordsData.Count - 1].Index = wi;
                                wordsData[wordsData.Count - 1].Rect = wordPRD.Rectangle;
                                wordsData[wordsData.Count - 1].Style = wordPRD.Style;
                                wordsData[wordsData.Count - 1].Value = wordPRD.WordData;
                                wi++;
                            }

                            if (wordsData != null && wordsData.Count > 0)
                            {
                                //-- Define line data --\\
                                linesData[linesData.Count - 1].Confidence = linePRD.Confidance;
                                linesData[linesData.Count - 1].Index = li;
                                linesData[linesData.Count - 1].Rect = linePRD.Rectangle;
                                linesData[linesData.Count - 1].Words = wordsData.ToArray();
                            }
                            li++;
                        }

                        //-- Add the recognized lines to the result --\\
                        if (linesData != null) res.Lines = linesData.ToArray();
                        if (res.Lines != null) return res;
                    }
                    else
                    {
                        throw new Exception(String.Format("TPage specified in: [{0}] is not valid", MethodBase.GetCurrentMethod().Name));
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                return null;
            }

            /// <summary>
            /// Create <see cref="PageOcrData"/> from a prd file
            /// </summary>
            /// <param name="pagePRDPath">The PRD file path to desrialize as <see cref="PageOcrData"/>.</param>
            /// <returns>PageOcrData when successfull, null when failed.</returns>
#if INTERNAL
            internal static PageOcrData FromPRD(String pagePRDPath)
#else
            public static PageOcrData FromPRD(String pagePRDPath)
#endif
            {
                return FromPRD(TPage.LoadFromPRD(pagePRDPath));
            }
            #endregion

            #region "ToPRD" functions
            /// <summary>
            /// Convert a <see cref="PageOcrData"/> to <see cref="TPage"/> object.
            /// </summary>
            /// <returns>TPage when successfull, null when not.</returns>
            internal TPage ToPRD()
            {
                return ToPRD(this, null);
            }

            /// <summary>
            /// Convert a <see cref="PageOcrData"/> to <see cref="TPage"/> object.
            /// </summary>
            /// <param name="saveToPrdPath">A path (optionla) to save the PRD file to.</param>
            /// <returns>TPage when successfull, null when not.</returns>
#if INTERNAL
            internal TPage ToPRD(String saveToPrdPath)
#else
            public TPage ToPRD(String saveToPrdPath)
#endif
            {
                return ToPRD(this, saveToPrdPath);
            }

            /// <summary>
            /// Convert a <see cref="PageOcrData"/> to <see cref="TPage"/> object.
            /// </summary>
            /// <param name="ocrData">The <see cref="PageOcrData"/> to convert to PRD.</param>
            /// <returns>TPage when successfull, null when not.</returns>
#if INTERNAL
            internal static TPage ToPRD(PageOcrData ocrData)
#else
            public static TPage ToPRD(PageOcrData ocrData)
#endif
            {
                return ToPRD(ocrData, null);
            }

            /// <summary>
            /// Convert a <see cref="PageOcrData"/> to <see cref="TPage"/> object.
            /// </summary>
            /// <param name="ocrData">The <see cref="PageOcrData"/> to convert to PRD.</param>
            /// <param name="saveToPrdPath">A path (optionla) to save the PRD file to.</param>
            /// <returns>TPage when successfull, null when not.</returns>
#if INTERNAL
            internal static TPage ToPRD(PageOcrData ocrData, String saveToPrdPath)
#else
            public static TPage ToPRD(PageOcrData ocrData, String saveToPrdPath)
#endif
            {
                try
                {
                    if (ocrData != null)
                    {
                        TPage res = new TPage();

                        //-- iterate data lines --\\
                        foreach (LineOcrData lineOcr in ocrData.Lines)
                        {
                            TLine currentLine = new TLine();
                            //-- Itarate the line's words --\\
                            foreach (WordOcrData wordOcr in lineOcr.Words)
                            {
                                //-- Iterate the word's chars --\\
                                TWord wordPrd = new TWord();
                                foreach (CharOcrData charOcr in wordOcr.GetAsChars(true))
                                {
                                    wordPrd.AddChar(new TChar(charOcr.Value, (short)charOcr.Confidence, new TOCRRect(charOcr.Rect)));
                                }
                                currentLine.AddWord(wordPrd);
                            }

                            if (currentLine != null && currentLine.Words != null && currentLine.Words.Count > 0) res.AddLine(currentLine);
                        }

                        //-- Save the contents to a PRD file if 'saveToPrdPath' is specified --\\
                        if (!String.IsNullOrEmpty(saveToPrdPath))
                        {
                            if (!Directory.Exists(Path.GetDirectoryName(saveToPrdPath))) Directory.CreateDirectory(Path.GetDirectoryName(saveToPrdPath));
                            TPage.SaveToPRD(res, saveToPrdPath);
                        }
                        return res;
                    }
                    else
                    {
                        throw new Exception(String.Format("Invalid input PRD object in: [{0}]", MethodBase.GetCurrentMethod().Name));
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
            /// Get PageOcrData from XML file
            /// </summary>
            /// <param name="xmlFilePath">The path to the XML file.</param>
            /// <returns>The PageOcrData as deserialized from XML.</returns>
#if INTERNAL
            public static new PageOcrData FromXml(String xmlFilePath)
#else
            public static new PageOcrData FromXml(String xmlFilePath)
#endif
            {
                try
                {
                    try
                    {
                        PageOcrData res = FromXml(xmlFilePath, typeof(PageOcrData)) as PageOcrData;
                        if (res != null) return res;
                    }
                    catch { }

                    //-- Try a collection ocr data --\\
                    CollectionOcrData coll = FromXml(xmlFilePath, typeof(CollectionOcrData)) as CollectionOcrData;
                    if (coll != null && coll.Pages.Length > 0) return coll.Pages[0];
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                return null;
            }
            #endregion

            #region "PRDToXml" event
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
                CollectionOcrData.PRDToXml(sender, e);
            }
            #endregion

            #region "XMLToPRD" event
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
                CollectionOcrData.XMLToPRD(sender, e);
            }
            #endregion
        }
        #endregion
    }
    #endregion
}