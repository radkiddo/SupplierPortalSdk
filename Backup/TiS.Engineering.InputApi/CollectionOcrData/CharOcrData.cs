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
        #region "CharOcrData" class
        /// <summary>
        /// CharOcrData class, the basic class to contain one OCRed char.
        /// </summary>
        public class CharOcrData : CCGenericSerializer, ICharOcrData
        {
            #region class properties
            #region "Confidence" property
            private int confidence;
            /// <summary>
            /// The char confidence.
            /// </summary>
            public virtual int Confidence { get { return confidence; } set { confidence = value; } }
            #endregion

            #region "Index" property
            /// <summary>
            /// The item index in the list\collection.
            /// </summary>
            private int index;
            public int Index { get { return index; } set { index = value; } }
            #endregion

            #region "Rect" property
            /// <summary>
            /// The char Rectangle.
            /// </summary>
            private Rectangle rect;
            [XmlIgnore]
            public virtual Rectangle Rect { get { return rect; } set { rect = value; } }
            #endregion

            #region "Rectangle" property
            /// <summary>
            /// The char OCR Rectangle.
            /// </summary>
            public virtual OcrRect Rectangle
            {
                get { return new OcrRect(Rect); }
                set { Rect = value.Rectangle; }
            } 
            #endregion

            #region "Value" property
            /// <summary>
            /// The char that this class contains.
            /// </summary>
            private char vale;
            public virtual Char Value { get { return vale; } set { vale = value; } }
            #endregion
            #endregion
        }
        #endregion
    }
}