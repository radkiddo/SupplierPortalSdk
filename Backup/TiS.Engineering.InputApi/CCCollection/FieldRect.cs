using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using TiS.Core.eFlowAPI;
using System.ComponentModel;

namespace TiS.Engineering.InputApi
{
    /// <summary>
    /// Field rectangle class.
    /// </summary>
    partial class CCCollection
    {
        #region "FieldRect" class
        /// <summary>
        /// FieldRect class, contsins definitions for a custom rect.
        /// to reduce serialization size 
        /// (System.Drwaing.Rectangle has too many parameters that we don't need when serializing and deserializing.))
        /// </summary> 
        public class FieldRect : ICCFieldRect
        {
            #region class ctors
            /// <summary>
            /// Base cosntructor.
            /// </summary>
            public FieldRect()
            {
            }

            /// <summary>
            /// Rectangle extended cosntructor.
            /// </summary>
            /// <param name="rct">The rectangle to initialize the class with.</param>
            public FieldRect(Rectangle rct):this()
            {
                this.Rectangle = rct;
            }

            /// <summary>
            /// Rectangle extended cosntructor.
            /// </summary>
            /// <param name="rct">The rectangle to initialize the class with.</param>
            public FieldRect(TIS_RECT rct)
                : this()
            {
                this.Left = rct.Left;
                this.Top = rct.Top;
                this.Width = rct.Right - rct.Left;
                this.Height = rct.Bottom - rct.Top;
            }

            /// <summary>
            /// Extended cosntructor.
            /// </summary>
            /// <param name="leftRct">Left position (X).</param>
            /// <param name="topRct">Right position (Y).</param>
            /// <param name="widthRct">Rectangle width.</param>
            /// <param name="heightRct">Rectabgle height.</param>
            public FieldRect(int leftRct, int topRct, int widthRct, int heightRct)
                : this()
            {
                this.Left = leftRct;
                this.Top = topRct;
                this.Width = widthRct;
                this.Height = heightRct;
            }

            /// <summary>
            /// Extended cosntructor.
            /// </summary>
            /// <param name="location">Rectangle location \ position (X and Y).</param>
            /// <param name="dimensions">Rectangle dimensionss (Width and Height).</param>
            public FieldRect(Point location, Size dimensions)
                : this()
            {
                this.Left = location.X;
                this.Top = location.Y;
                this.Width = dimensions.Width;
                this.Height = dimensions.Height;
            }
            #endregion

            #region class properties
            #region "Bottom" property
            /// <summary>
            /// Get or set the rectangle Bottom
            /// </summary>
            [XmlIgnore, Description("Get or set the rectangle Bottom.")]
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
            [Description("Get or set the rectangle Left.")]
            public virtual int Left { get { return left; } set { left = value; } }
            #endregion

            #region "Height" property
            private int height;
            /// <summary>
            /// Get or set the rectangle Height
            /// </summary>
            [Description("Get or set the rectangle Height.")]
            public virtual int Height { get { return height; } set { height = value; } }
            #endregion

            #region "Top" property
            private int top;
            /// <summary>
            /// Get or set the rectangle Top
            /// </summary>
            [Description("Get or set the rectangle Top.")]
            public virtual int Top { get { return top; } set { top = value; } }
            #endregion

            #region "Right" property
            /// <summary>
            /// Get or set the rectangle Right
            /// </summary>
            [XmlIgnore, Description("Get or set the rectangle Right.")]
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
            [Description("Get or set the rectangle Width.")]
            public virtual int Width { get { return width; } set { width = value; } }
            #endregion

            #region "Rectangle" property
            /// <summary>
            /// Get or set the OCR rect via System.Drawing.Rectangle
            /// </summary>
            [XmlIgnore, Description("Get or set the OCR rect via System.Drawing.Rectangle.")]
            public virtual Rectangle Rectangle
            {
                get { return new Rectangle(Top, Left, Width, Height); }
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
            /// Get or set the rect via TIS_RECT
            /// </summary>
            [XmlIgnore, Description("Get or set the rect via TIS_RECT.")]
            public virtual TIS_RECT TisRect
            {
                get
                {
                    TIS_RECT res = new TIS_RECT();
                    res.Left = Left;
                    res.Top = Top;
                    res.Right = Left + Width;
                    res.Bottom = Top + Height;
                    return res;
                }
                set
                {
                    this.Left = value.Left;
                    this.Top = value.Top;
                    this.Width = value.Right - value.Left;
                    this.Height = value.Bottom - value.Top;
                }
            }
            #endregion
            #endregion
        }
        #endregion
    }
}