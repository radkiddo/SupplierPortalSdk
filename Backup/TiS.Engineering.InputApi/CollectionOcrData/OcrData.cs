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
        #region "OcrData" class
        /// <summary>
        /// OcrData class, base class to contsin OCR data.
        /// </summary>
        public abstract class OcrData : CCGenericSerializer, IOcrData
        {
            #region class properties
            #region "Confidence" property
            private int confidence;
            /// <summary>
            /// The item's confidence.
            /// </summary>
            [XmlIgnore]
            public virtual int Confidence { get { return confidence; } set { confidence = value; } }
            #endregion

            #region "Index" property
            private int index;
            /// <summary>
            /// The item index in the list\collection.
            /// </summary>
            [XmlIgnore]
            public virtual int Index { get { return index; } set { index = value; } }
            #endregion

            #region "Rect" property
            private Rectangle rect;
            /// <summary>
            /// The item's Rectangle.
            /// </summary>
            [XmlIgnore]
            public virtual Rectangle Rect { get { return rect; } set { rect = value; } }
            #endregion

            #region "Rectangle" property
            /// <summary>
            /// The item's OCR Rectangle.
            /// </summary>
            [XmlIgnore]
            public virtual OcrRect Rectangle
            {
                get { return new OcrRect(Rect); }
                set { Rect = value.Rectangle; }
            } 
            #endregion
            #endregion
        }
        #endregion
    }
}