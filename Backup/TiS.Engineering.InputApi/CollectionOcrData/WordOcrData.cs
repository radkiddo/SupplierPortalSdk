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
        #region "WordOcrData" class
        /// <summary>
        /// WordOcrData class, contains OCRed word data.
        /// </summary>
        public class WordOcrData : OcrData, IWordOcrData
        {
            #region class properties
            #region "Chars"property
            /// <summary>
            /// The characters that make the worde (not serializable to save on the serialized XML file size).
            /// </summary>
            private CharOcrData[] chars;
            [XmlIgnore]
            public CharOcrData[] Chars { get { return chars; } set { chars = value; } }
            #endregion

            #region "Style" property
            /// <summary>
            /// The recognized word style (if any).
            /// </summary>
            private TWord.TStyle style;
            [XmlIgnore]
            public virtual TWord.TStyle Style { get { return style; } set { style = value; } }
            #endregion

            #region "Value" property
            /// <summary>
            /// The recognized word value (String).
            /// </summary>
            private String vale;
            public virtual String Value { get { return vale; } set { vale = value; } }
            #endregion

            #region "Rectangle" property
            /// <summary>
            /// Rectangle property (override base to expose for XML serialization).
            /// </summary>
            public override OcrRect Rectangle
            {
                get
                {
                    return base.Rectangle;
                }
                set
                {
                    base.Rectangle = value;
                }
            }
            #endregion

            #region "Confidence" property
            /// <summary>
            /// Confidence property (override base to expose for XML serialization).
            /// </summary>
            public override int Confidence
            {
                get
                {
                    return base.Confidence;
                }
                set
                {
                    base.Confidence = value;
                }
            }
            #endregion

            #region "Index" property
            /// <summary>
            /// Index property (override base to expose for XML serialization).
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

            #region "GetAsChars" functions
            /// <summary>
            /// Get the chars a word is consisted of.
            /// </summary>
            /// <returns>A CharOcrData array of the characters the word is consisted of.</returns>
#if INTERNAL
            internal CharOcrData[] GetAsChars()
#else
            public CharOcrData[] GetAsChars()
#endif
            {
                return GetAsChars(this, true);
            }

            /// <summary>
            /// Get the chars a word is consisted of.
            /// </summary>
            /// <param name="autoSelectSource">when true will use TChar class extracted from the TWord data,
            /// when false will get the data by calculating the word rectangle and String and breaking it to the chars it is made of</param>
            /// <returns>A CharOcrData array of the characters the word is consisted of.</returns>
#if INTERNAL
            internal CharOcrData[] GetAsChars(bool autoSelectSource)
#else
            public CharOcrData[] GetAsChars(bool autoSelectSource)
#endif
            {
                return GetAsChars(this, autoSelectSource);
            }

            /// <summary>
            /// Get the chars a word is consisted of.
            /// </summary>
            /// <param name="sourceWord">The <see cref="WordOcrData"/> to get it's chars.</param>
            /// <returns>A CharOcrData array of the characters the word is consisted of.</returns>
#if INTERNAL
            internal static CharOcrData[] GetAsChars(WordOcrData sourceWord)
#else
            public static CharOcrData[] GetAsChars(WordOcrData sourceWord)
#endif
            {
                return GetAsChars(sourceWord, true);
            }

            /// <summary>
            /// Get the chars a word is consisted of.
            /// </summary>
            /// <param name="sourceWord">The <see cref="WordOcrData"/> to get it's chars.</param>
            /// <param name="autoSelectSource">when true will use TChar class extracted from the TWord data,
            /// when false will get the data by calculating the word rectangle and String and breaking it to the chars it is made of</param>
            /// <returns>A CharOcrData array of the characters the word is consisted of.</returns>
#if INTERNAL
            internal static CharOcrData[] GetAsChars(WordOcrData sourceWord, bool autoSelectSource)
#else
            public static CharOcrData[] GetAsChars(WordOcrData sourceWord, bool autoSelectSource)
#endif
            {
                try
                {
                    if (autoSelectSource && sourceWord.Chars != null && sourceWord.Chars.Length > 0) return sourceWord.Chars;

                    List<CharOcrData> res = new List<CharOcrData>();
                    int pos = 0;
                    foreach (Char ch in sourceWord.Value)
                    {
                        pos++;
                        res.Add(new CharOcrData());
                        res[res.Count - 1].Confidence = sourceWord.Confidence;
                        res[res.Count - 1].Rect = new Rectangle(sourceWord.Rect.X + (pos * (sourceWord.Rect.Width / sourceWord.Value.Length)), sourceWord.Rect.Y, sourceWord.Rect.Width / sourceWord.Value.Length, sourceWord.Rect.Height);
                        res[res.Count - 1].Value = ch;
                    }
                    return res.ToArray();
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                return null;
            }
            #endregion
        }
        #endregion
    }
}