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
    /// <summary>
    /// CollectionOcrData class, contsians all OcrPOage data for all pages in the collection.
    /// </summary>
    public partial class CollectionOcrData
    {
        #region "LineOcrData" class
        /// <summary>
        /// LineOcrData class
        /// </summary>
        public class LineOcrData : OcrData, ILineOcrData
        {
            #region class variables
            private WordOcrData[] words;
            private String wordsDelimiter;
            #endregion

            #region class properties
            #region "Value" property
            /// <summary>
            /// Get the item's words as text (sentence).
            /// </summary>
            [XmlIgnore]
            public virtual String Value
            {
                get
                {
                    if (words != null || words.Length > 0)
                    {
                        List<String> wordsStr = new List<String>();
                        foreach (WordOcrData wod in words)
                        {
                            wordsStr.Add(wod.Value);
                        }
                        return String.Join(" ", wordsStr.ToArray());
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            } 
            #endregion

            #region "Words" property
            /// <summary>
            /// Get or set the item's words.
            /// </summary>
            public WordOcrData[] Words
            {
                get { return words; }
                set { words = value; }
            } 
            #endregion

            #region "WordsSeparator" property
            /// <summary>
            /// Get or set the item's word seprator.
            /// </summary>
            [XmlIgnore]
            public String WordsSeparator
            {
                get { return wordsDelimiter ?? " "; }
                set { wordsDelimiter = value ?? String.Empty; }
            } 
            #endregion

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
            #endregion
        }
        #endregion
    }
}