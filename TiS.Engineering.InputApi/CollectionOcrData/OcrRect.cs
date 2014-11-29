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
{    /// <summary>
    /// CollectionOcrData class, contsians all OcrPOage data for all pages in the collection.
    /// </summary>
    public partial class CollectionOcrData 
    {
        #region "OcrRect" class
        /// <summary>
        /// OcrRect class, contsins definitions for a custom rect.
        /// to reduce serialization size 
        /// (System.Drwaing.Rectangle has too many parameters that we don't need when serializing and deserializing.))
        /// </summary>
        public class OcrRect : ICCFieldRect
        {
            #region class ctors
            /// <summary>
            /// Base cosntructor.
            /// </summary>
            public OcrRect()
            {
            }

            /// <summary>
            /// Rectangle extended cosntructor.
            /// </summary>
            /// <param name="rct">The rectangle to initialize the class with.</param>
            internal OcrRect(Rectangle rct)
            {
                this.Rectangle = rct;
            }

            /// <summary>
            /// Extended cosntructor.
            /// </summary>
            /// <param name="leftRct"></param>
            /// <param name="topRct"></param>
            /// <param name="widthRct"></param>
            /// <param name="heightRct"></param>
            public OcrRect(int leftRct, int topRct, int widthRct, int heightRct)
            {
                this.Left = leftRct;
                this.Top = topRct;
                this.Width = widthRct;
                this.Height = heightRct;
            }
            #endregion

            #region class properties
            #region "Bottom" proeprty
            /// <summary>
            /// Get or set the rectangle Bottom
            /// </summary>
            [XmlIgnore]
            public virtual int Bottom
            {
                get { return Top + Height; }
                set { Height = value - Height; }
            } 
            #endregion

            #region "Left" property
            private int left;
            /// <summary>
            /// Get or set the rectangle Left
            /// </summary>
            public virtual int Left { get { return left; } set { left = value; } }
            #endregion

            #region "Height" property
            private int height;
            /// <summary>
            /// Get or set the rectangle Height
            /// </summary>
            public virtual int Height { get { return height; } set { height = value; } }
            #endregion

            #region "Top" property
            private int top;
            /// <summary>
            /// Get or set the rectangle Top
            /// </summary>
            public virtual int Top { get { return top; } set { top = value; } }
            #endregion

            #region "Right" property
            /// <summary>
            /// Get or set the rectangle Right
            /// </summary>
            [XmlIgnore]
            public virtual int Right
            {
                get { return Left + Width; }
                set { Width = value - Width; }
            } 
            #endregion

            #region "Width" property
            private int width;
            /// <summary>
            /// Get or set the rectangle Width
            /// </summary>
            public virtual int Width { get { return width; } set { width = value; } }
            #endregion

            #region "Rectangle" proeprty
            /// <summary>
            /// /// Get or set the OCR rectangle Rectangle
            /// </summary>
            [XmlIgnore]
            public virtual Rectangle Rectangle
            {
                get { return new Rectangle(Left, Top, Width, Height); }
                set
                {
                    this.Left = value.Left;
                    this.Top = value.Top;
                    this.Width = value.Width;
                    this.Height = value.Height;
                }
            } 
            #endregion

            #region "TisRect" property
            /// <summary>
            /// Returns a TIS_RECT (field rect).
            /// </summary>
            [XmlIgnore]
            public TIS_RECT TisRect
            {
                get
                {
                    TIS_RECT res = new TIS_RECT();
                    res.Left = Left;
                    res.Right = Left + Width;
                    res.Top = Top;
                    res.Bottom = Top + Height;
                    return res;
                }
            }
            #endregion
            #endregion            
        }
        #endregion
    }
}