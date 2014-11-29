using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TiS.Engineering.InputApi
{
    #region "CompareTypeEnm" enum
#if STAND_ALONE
    /// <summary>
    /// All the sorting possibilities enum.
    /// </summary>
    public enum CompareTypeEnm
    {
        None=0,
        FileCreationDate,
        FileModifiedDate,
        FileAccessedDate,
        FileExtension,
        FileSize,
        FileName,
        FilePath,
        Alpha,
        Numeric,
        NumericDecimal,
        Length
    };
#endif
    #endregion

    #region "FlexListComparer"
    /// <summary>
    /// Compare and sort file paths by size.
    /// </summary>
#if INTERNAL
    internal class FlexListComparer : System.Collections.Generic.IComparer<String>
#else
    public class FlexListComparer : System.Collections.Generic.IComparer<String> 
#endif
    {
        #region class variables
        private int compTyp;
        private bool descOrder;
        #endregion        

        #region "CompareType" property
        /// <summary>
        /// Compare type, defines the way which the class will sort the List items.
        /// </summary>
        public CCEnums.CompareTypeEnm CompareType
        {
            get
            {
                switch (compTyp)
                {
                    case (int)CCEnums.CompareTypeEnm.Alpha:
                        return CCEnums.CompareTypeEnm.Alpha;

                    case (int)CCEnums.CompareTypeEnm.FileSize:
                        return CCEnums.CompareTypeEnm.FileSize;

                    case (int)CCEnums.CompareTypeEnm.FileName:
                        return CCEnums.CompareTypeEnm.FileName;

                    case (int)CCEnums.CompareTypeEnm.FilePath:
                        return CCEnums.CompareTypeEnm.FilePath;

                    case (int)CCEnums.CompareTypeEnm.FileExtension:
                        return CCEnums.CompareTypeEnm.FileExtension;

                    case (int)CCEnums.CompareTypeEnm.FileAccessedDate:
                        return CCEnums.CompareTypeEnm.FileAccessedDate;

                    case (int)CCEnums.CompareTypeEnm.FileCreationDate:
                        return CCEnums.CompareTypeEnm.FileCreationDate;

                    case (int)CCEnums.CompareTypeEnm.FileModifiedDate:
                        return CCEnums.CompareTypeEnm.FileModifiedDate;

                    case (int)CCEnums.CompareTypeEnm.Numeric:
                        return CCEnums.CompareTypeEnm.Numeric;

                    case (int)CCEnums.CompareTypeEnm.NumericDecimal:
                        return CCEnums.CompareTypeEnm.NumericDecimal;

                    case (int)CCEnums.CompareTypeEnm.Length:
                        return CCEnums.CompareTypeEnm.Length;

                    default:
                        return CCEnums.CompareTypeEnm.None;
                }
            }
            set { compTyp = (int)value; }
        }
        #endregion

        #region "ComparerType" property
        /// <summary>
        /// Compare type property as int.
        /// </summary>
        public int ComparerType
        {
            get{ return compTyp;}
            set { compTyp = value; }
        }
        #endregion

        #region "DescendingOrder" property
        /// <summary>
        /// Get or set the descendinbg order property (descending when true, ascending when not).
        /// </summary>
        public bool DescendingOrder
        {
            get { return descOrder; }
            set { descOrder = value; }
        }
        #endregion

        #region class ctors
        public FlexListComparer(CCEnums.CompareTypeEnm compareType, bool descendingOrder)
            : this((int)compareType, descendingOrder)
        {
        }

        public FlexListComparer(int compareType, bool descendingOrder)
        {
            compTyp = compareType;
            descOrder = descendingOrder;
        }
        #endregion

        #region "Compare" method
        /// <summary>
        /// the actual compare method
        /// </summary>
        /// <param name="x">a file path object</param>
        /// <param name="y">a file path object</param>
        /// <returns>compare result</returns>
        public int Compare(String x, String y)
        {
            int result = 0;
            try
            {
                if (compTyp <= (int)CCEnums.CompareTypeEnm.None) return result;

                if (compTyp != (int)CCEnums.CompareTypeEnm.FileAccessedDate &&
                    compTyp != (int)CCEnums.CompareTypeEnm.FileCreationDate &&
                    compTyp != (int)CCEnums.CompareTypeEnm.FileModifiedDate &&
                    compTyp != (int)CCEnums.CompareTypeEnm.FileName &&
                    compTyp != (int)CCEnums.CompareTypeEnm.FileExtension &&
                    compTyp != (int)CCEnums.CompareTypeEnm.FilePath &&
                    compTyp != (int)CCEnums.CompareTypeEnm.Length &&
                    compTyp != (int)CCEnums.CompareTypeEnm.FileSize)
                {
                    bool validX = false;
                    bool validY = false;

                    if (compTyp == (int)CCEnums.CompareTypeEnm.Numeric)
                    {
                        //-- Compare numeric int --\\
                        int ix = 0;
                        int iy = 0;
                        validX = int.TryParse(x, out ix);
                        validY = int.TryParse(y, out iy);

                        if (!validX && !validY) return 0;
                        else if (validX && !validY) return 1;
                        else if (validY && !validX) return -1;
                        else
                        {
                            if (ix > iy) result = 1;
                            else if (ix < iy) result = -1;
                        }
                    }
                    else if (compTyp == (int)CCEnums.CompareTypeEnm.Length)
                    {
                        //-- Compare numeric decimal --\\
                        int lnX = (x ?? String.Empty).Length;
                        int lnY = (y ?? String.Empty).Length;

                        if (lnX > lnY) result = 1;
                        else if (lnX < lnY) result = -1;
                    }
                    else if (compTyp == (int)CCEnums.CompareTypeEnm.NumericDecimal)
                    {
                        //-- Compare numeric decimal --\\
                        decimal dx = 0;
                        decimal dy = 0;
                        validX = decimal.TryParse(x, out dx);
                        validY = decimal.TryParse(y, out dy);

                        if (!validX && !validY) return 0;
                        else if (validX && !validY) return 1;
                        else if (validY && !validX) return -1;
                        else
                        {
                            if (dx > dy) result = 1;
                            else if (dx < dy) result = -1;
                        }
                    }
                    else
                    {
                        //-- Compare alpha --\\
                        validX = !String.IsNullOrEmpty(x);
                        validY = !String.IsNullOrEmpty(y);

                        if (!validX && !validY) result = 0;
                        else if (validX && !validY) result = 1;
                        else if (validY && !validX) result = -1;
                        else
                        {
                            result = string.Compare(x, y);
                        }
                    }
                }
                else
                {
                    FileInfo fX = File.Exists(x) ? new FileInfo(x) : null;
                    FileInfo fY = File.Exists(y) ? new FileInfo(y) : null;

                    if (fX == null && fY == null) result = 0;
                    if (fY == null && fX != null) result = -1;
                    else if (fX == null && fY != null) result = 1;
                    else
                    {
                        if (compTyp == (int)CCEnums.CompareTypeEnm.FileSize)
                        {
                            //-- Compare File Size --\\
                            if (fX.Length > fY.Length) result = 1;
                            else if (fX.Length < fY.Length) result = -1;
                        }
                        else if (compTyp == (int)CCEnums.CompareTypeEnm.FileAccessedDate)
                        {
                            //-- Compare File last access time --\\
                            if (fX.LastAccessTime > fY.LastAccessTime) result = 1;
                            else if (fX.LastAccessTime < fY.LastAccessTime) result = -1;
                        }
                        else if (compTyp == (int)CCEnums.CompareTypeEnm.FileCreationDate)
                        {
                            //-- Compare File creation time --\\
                            if (fX.CreationTime > fY.CreationTime) result = 1;
                            else if (fX.CreationTime < fY.CreationTime) result = -1;
                        }
                        else if (compTyp == (int)CCEnums.CompareTypeEnm.FileModifiedDate)
                        {
                            //-- Compare File last modified time --\\
                            if (fX.LastWriteTime > fY.LastWriteTime) result = 1;
                            else if (fX.LastWriteTime < fY.LastWriteTime) result = -1;
                        }
                        else if (compTyp == (int)CCEnums.CompareTypeEnm.FileName)
                        {
                            //-- Compare File Name (without path) --\\
                            result = String.Compare(fX.Name, fY.Name);
                        }
                        else if (compTyp == (int)CCEnums.CompareTypeEnm.FilePath)
                        {
                            //-- Compare File Name (with full path) --\\
                            result = String.Compare(fX.FullName, fY.FullName);
                        }
                        else if (compTyp == (int)CCEnums.CompareTypeEnm.FileExtension)
                        {
                            //-- Compare File extesion --\\
                            result = String.Compare(fX.Extension, fY.Extension);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }

            return descOrder ? result *= -1 : result;
        } 
        #endregion
    }
    #endregion
}