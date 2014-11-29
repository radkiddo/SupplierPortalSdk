using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using TiffDLL90;

namespace TiS.Engineering.TiffDll90
{
    /// <summary>
    /// A set of TIFF functions to: Join, Split, Annotate, Change resolution and get image info.
    /// </summary>
    public class TiffDll_Methods
    {
        public delegate void OnMessage(String format, params object[] args);      

        public static event OnMessage AllMsg
        {
            add
            {
                ILog.DbgMsg += value;
                ILog.InfoMsg += value;
                ILog.ErrorMsg += value;
                ILog.WarningMsg += value;
            }

            remove
            {
                ILog.DbgMsg -= value;
                ILog.InfoMsg -= value;
                ILog.ErrorMsg -= value;
                ILog.WarningMsg -= value;
            }
        }

        #region class constants and variables
        /// <summary>
        /// Info string to be used with tiffDll90.
        /// </summary>
        public const string INFO = "INFO";

        /// <summary>
        /// TifDll90 license key p1.
        /// </summary>
        private const string TD90LIC_1 = "ISC";

        /// <summary>
        /// TifDll90 license key p2.
        /// </summary>
        private const string TD90LIC_2 = "ZAT";

        /// <summary>
        /// Lic part 3
        /// </summary>
        private const long TD90LIC_3 = 534311130091139;

        /// <summary>
        /// Lic var 1
        /// </summary>
        private static readonly int multiPL = Convert.ToInt32('(') / 8;

        /// <summary>
        /// Lic var 2
        /// </summary>
        private static readonly int multiPI = Convert.ToInt32('*') - (multiPL*8);

        public static List<string> errorList = new List<string>();
        #endregion

        #region "AnnotateTiff" functions
        /// <summary>
        /// Annotate a tiff file with text.
        /// </summary>
        /// <param name="imagePath">the image path to annotate on.</param>
        /// <param name="targetPath">the target path for the annotated image (ommit to save as source).</param>
        /// <param name="printText">The string to print.</param>
        /// <param name="smallFont">Use smaller font.</param>
        /// <param name="position">The position to print on the image.</param>
        /// <returns>true when successfull, false when failed.</returns>
        /// <remarks>other properties as text roatation are available via further coding.</remarks>
        private static bool AnnotateTiff(string imagePath, string targetPath, string printText, bool smallFont, PlacementHorizontal position)
        {
            TiffDLL obj = new TiffDLL();

            bool result = false;
            try
            {
                obj._RegistrationCode = GetRegCode(multiPL);
                obj._OpenFile.Filename = imagePath;
                obj._SaveFile.OverwriteFile = OverwriteFile.Overwrite;
                obj._SaveFile.Format = Format.TIFF_CCITT4_1bit;
                obj._SaveFile.Filename = string.IsNullOrEmpty(targetPath) ? imagePath : targetPath;
                if (!Directory.Exists(Path.GetDirectoryName(obj._SaveFile.Filename))) Directory.CreateDirectory(Path.GetDirectoryName(obj._SaveFile.Filename));

                obj._AnnotationSimple.PlacementHorizontal = position;
                obj._AnnotationSimple.SmallerFont = smallFont;
                obj._AnnotationSimple.Text = printText;
                obj._AnnotationSimple.Opaque = true;

                //-- Perform the annotation --\\
                int res = obj.Run();
                if (res != 0)
                {
                    ILog.LogWarning("Tiff annotate file [{0}] as file [{1}] with text [{2}] at position [{5}], error code [{3}], method [{4}]", imagePath, targetPath, printText, res, MethodBase.GetCurrentMethod().Name, position);
                    return false;
                }

                result = File.Exists(targetPath);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                if (obj != null)
                {
                    obj._SaveFile = new Save();
                    obj._OpenFile = new Open();
                    obj = null;
                }
            }
            return result;
        }

        /// <summary>
        /// Annotate a tiff file with text.
        /// </summary>
        /// <param name="imagePath">the image path to annotate on.</param>
        /// <param name="targetPath">the target path for the annotated image (ommit to save as source).</param>
        /// <param name="printText">The string to print.</param>
        /// <param name="smallFont">Use smaller font.</param>
        /// <param name="position">The position to print on the image (do not use Middle, as this will be changed to Top, as middle alignment does not exist in this fast mode).</param>
        /// <returns>true when successfull, false when failed.</returns>
        /// <remarks>other properties as text roatation are available via further coding.</remarks>
        public static bool AnnotateTiff(string imagePath, string targetPath, string printText, bool smallFont, ContentAlignment position)
        {
            bool result = false;
            try
            {
                //-- Define position --\\
                PlacementHorizontal pos = PlacementHorizontal.BottomCenter;

                switch (position)
                {
                    //-- Default, not required --\\
                    //case ContentAlignment.BottomCenter:
                    //    pos = PlacementHorizontal.BottomCenter;
                    //    break;

                    case ContentAlignment.BottomLeft:
                        pos = PlacementHorizontal.BottomLeft;
                        break;

                    case ContentAlignment.BottomRight:
                        pos = PlacementHorizontal.BottomRight;
                        break;

                    case ContentAlignment.MiddleCenter:
                        pos = pos = PlacementHorizontal.TopCenter;
                        break;

                    case ContentAlignment.MiddleLeft:
                        pos = PlacementHorizontal.TopLeft;
                        break;

                    case ContentAlignment.MiddleRight:
                        pos = PlacementHorizontal.TopRight;
                        break;

                    case ContentAlignment.TopCenter:
                        pos = PlacementHorizontal.TopCenter;
                        break;

                    case ContentAlignment.TopLeft:
                        pos = PlacementHorizontal.TopLeft;
                        break;

                    case ContentAlignment.TopRight:
                        pos = PlacementHorizontal.TopRight;
                        break;
                }

                //-- Prepare annotation info --\\
                return AnnotateTiff(imagePath, targetPath, printText, smallFont, pos);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result;
        }
        #endregion

        #region "ChangeTiffResolution" function
        /// <summary>
        /// Change the resolution of a given tiff single page image.
        /// </summary>
        /// <param name="imagePath">the source file to change it's resolution.</param>
        /// <param name="targetPath">the target path for the changed resolution file (ommit to save as original).</param>
        /// <param name="resolution">the required resolution.</param>
        /// <returns>true when successfull, false when failed.</returns>
        public static bool ChangeTiffResolution(string imagePath, string targetPath, int resolution)
        {
            TiffDLL obj = new TiffDLL();

            bool result = false;
            try
            {
                obj._RegistrationCode = GetRegCode(multiPL);
                obj._SaveFile.Filename = INFO;
                obj._OpenFile.Filename = imagePath;
                obj.Run();

                obj._SaveFile.Filename = string.IsNullOrEmpty(targetPath) ? imagePath : targetPath;
                obj._SaveFile.Format = Format.TIFF_CCITT4_1bit;
                obj._SaveFile.OverwriteFile = OverwriteFile.Overwrite;

                //-- Check whether an image resolution can be corrected --\\
                bool resample = obj._Information.HorizontalResolution != obj._Information.VerticalResolution;

                if (resample && obj._Information.HorizontalResolution / 2 == obj._Information.VerticalResolution)
                {
                    obj._ResizeImage.ResizeMode = ResizeMode.Percent;
                    obj._ResizeImage.Width = 100;
                    obj._ResizeImage.Height = 200;
                }
                else if (resample && obj._Information.VerticalResolution / 2 == obj._Information.HorizontalResolution)
                {
                    obj._ResizeImage.ResizeMode = ResizeMode.Percent;
                    obj._ResizeImage.Width = 200;
                    obj._ResizeImage.Height = 100;
                }
                else if (resample)
                {
                    obj._Resolution.Resample = true;
                }

                if (!Directory.Exists(Path.GetDirectoryName(obj._SaveFile.Filename))) Directory.CreateDirectory(Path.GetDirectoryName(obj._SaveFile.Filename));

                if (resolution > 0 && resolution <= 95)
                {
                    obj._Resolution.ResolutionHorizontal = Resolution.Res072;
                    obj._Resolution.ResolutionVertical = Resolution.Res072;
                }
                else if (resolution >= 96 && resolution < 100)
                {
                    obj._Resolution.ResolutionHorizontal = Resolution.Res096;
                    obj._Resolution.ResolutionVertical = Resolution.Res096;
                }
                else if (resolution >= 100 && resolution < 144)
                {
                    obj._Resolution.ResolutionHorizontal = Resolution.Res100;
                    obj._Resolution.ResolutionVertical = Resolution.Res100;
                }
                else if (resolution >= 144 && resolution < 150)
                {
                    obj._Resolution.ResolutionHorizontal = Resolution.Res144;
                    obj._Resolution.ResolutionVertical = Resolution.Res144;
                }
                else if (resolution >= 150 && resolution < 192)
                {
                    obj._Resolution.ResolutionHorizontal = Resolution.Res150;
                    obj._Resolution.ResolutionVertical = Resolution.Res150;
                }
                else if (resolution >= 192 && resolution < 200)
                {
                    obj._Resolution.ResolutionHorizontal = Resolution.Res192;
                    obj._Resolution.ResolutionVertical = Resolution.Res192;
                }
                else if (resolution >= 200 && resolution < 300)
                {
                    obj._Resolution.ResolutionHorizontal = Resolution.Res200;
                    obj._Resolution.ResolutionVertical = Resolution.Res200;
                }
                else if (resolution >= 300 && resolution < 400)
                {
                    obj._Resolution.ResolutionHorizontal = Resolution.Res300;
                    obj._Resolution.ResolutionVertical = Resolution.Res300;
                }
                else if (resolution >= 400 && resolution < 500)
                {
                    obj._Resolution.ResolutionHorizontal = Resolution.Res400;
                    obj._Resolution.ResolutionVertical = Resolution.Res400;
                }
                else if (resolution >= 500 && resolution < 600)
                {
                    obj._Resolution.ResolutionHorizontal = Resolution.Res500;
                    obj._Resolution.ResolutionVertical = Resolution.Res500;
                }
                else if (resolution >= 600)
                {
                    obj._Resolution.ResolutionHorizontal = Resolution.Res600;
                    obj._Resolution.ResolutionVertical = Resolution.Res600;
                }

                int res = obj.Run();
                if (res != 0)
                {
                    ILog.LogWarning("Tiff change file [{0}] resolution to [{1}]  as target file[{2}], error code [{3}], method [{4}]", imagePath, resolution, targetPath, res, MethodBase.GetCurrentMethod().Name);
                    return false;
                }
                result = true;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                if (obj != null)
                {
                    obj._SaveFile = new Save();
                    obj._OpenFile = new Open();
                    obj = null;
                }
            }
            return result;
        }
        #endregion

        #region "GetRegCode" function
        private static String GetRegCode(int multiple)
        {
            return String.Format("{1}{2}{3}{2}{0}", (TD90LIC_3 *multiPI) * multiple, TD90LIC_1, (char)(69 + multiple), TD90LIC_2);
        } 
        #endregion       

        #region "JoinTiffPages" function
        /// <summary>
        /// join tif images to a single multi-tiff.
        /// </summary>
        /// <param name="targetPath">the path to the joned tiff file.</param>
        /// <param name="tiffPges">the paths of the tiff pages to join as a single muoti-page tiff.</param>
        /// <returns>true when successfull, false when failed.</returns>
        public static bool JoinTiffPages(string targetPath, params string[] tiffPges)
        {
            TiffDLL obj = new TiffDLL();
            bool result = false;

            try
            {
                obj._RegistrationCode = GetRegCode(5);
                if (File.Exists(targetPath)) File.Delete(targetPath);

                foreach (string s in tiffPges)
                {
                    obj._OpenFile.Filename = s;
                    obj._SaveFile.Filename = targetPath;
                    obj._SaveFile.Format = Format.TIFF_CCITT4_1bit;
                    if (!Directory.Exists(Path.GetDirectoryName(obj._SaveFile.Filename))) Directory.CreateDirectory(Path.GetDirectoryName(obj._SaveFile.Filename));
                    obj._SaveFile.OverwriteFile = TiffDLL90.OverwriteFile.AppendTIFFonly;
                    int res = obj.Run();
                    if (res != 0)
                    {
                        ILog.LogWarning("Tiff append file [{0}] as page, error code [{1}], method [{2}]", s, res, MethodBase.GetCurrentMethod().Name);
                        return false;
                    }
                }

                result = File.Exists(targetPath);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                if (obj != null)
                {
                    obj._SaveFile = new Save();
                    obj._OpenFile = new Open();
                    obj = null;
                }
            }
            return result;
        }
        #endregion        

        #region "GetImageInfo" functions
        /// <summary>
        /// Get all relevant image data of an image
        /// </summary>
        /// <param name="imagePath">the image path to get it's information</param>
        /// <param name="imageSize">returns the image size.</param>
        /// <param name="imageResolution">returns the image resolution.</param>
        /// <param name="pageCount">returns the page count</param>
        /// <param name="tiffCompression">returns the tiff compression</param>
        /// <param name="colorBits">returns the color bits</param>
        /// <returns>true when successfull, false when failed.</returns>
        public static bool GetImageInfo(string imagePath, ref Size imageSize, ref Size imageResolution, ref int pageCount, ref int tiffCompression, ref int colorBits)
        {
            TiffDLL obj = new TiffDLL();
            try
            {
                obj._RegistrationCode = GetRegCode(multiPL);
                obj._OpenFile.Filename = imagePath;
                return GetImageInfo(obj, ref imageSize, ref imageResolution, ref pageCount, ref tiffCompression, ref colorBits);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                if (obj != null)
                {
                    obj._SaveFile = new Save();
                    obj._OpenFile = new Open();
                    obj = null;
                }
            }
            return false;
        }

        /// <summary>
        /// Get all relevant image data of an image
        /// </summary>
        /// <param name="obj">the image object to get it's information</param>
        /// <param name="imageSize">returns the image size.</param>
        /// <param name="imageResolution">returns the image resolution.</param>
        /// <param name="pageCount">returns the page count</param>
        /// <param name="tiffCompression">returns the tiff compression</param>
        /// <param name="colorBits">returns the color bits</param>
        /// <returns>true when successfull, false when failed.</returns>
        public static bool GetImageInfo(TiffDLL obj, ref Size imageSize, ref Size imageResolution, ref int pageCount, ref int tiffCompression, ref int colorBits)
        {
            bool result = false;
            try
            {
                obj._SaveFile.Filename = INFO;
                obj._RegistrationCode = GetRegCode(multiPL);
                int res = obj.Run();

                if (res != 0)
                {
                    ILog.LogWarning("Can't get information of tiff object, error code [{0}], method [{1}]", res, MethodBase.GetCurrentMethod().Name);
                    return result;
                }

                imageResolution = new Size(obj._Information.HorizontalResolution, obj._Information.VerticalResolution);
                imageSize = new Size(obj._Information.Width, obj._Information.Height);
                pageCount = obj._Information.Pages;
                tiffCompression = obj._Information.TIFFcompression;
                colorBits = obj._Information.ColorDepth;
                result = true;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return result;
        }
        #endregion

        #region "GetImageNumberOfPages" function
        /// <summary>
        /// Get image number of pages
        /// </summary>
        /// <param name="imagePath">the image path to get it's information</param>
        /// <returns>The image num of pages when successfull, 0 when failed.</returns>
        public static int GetImageNumberOfPages(string imagePath)
        {
            int result = 0;
            TiffDLL obj = new TiffDLL();

            try
            {
                obj._RegistrationCode = GetRegCode(multiPL);
                obj._OpenFile.Filename = imagePath;
                obj._SaveFile.Filename = INFO;
                int res = obj.Run();

                if (res != 0)
                {
                    ILog.LogWarning("Can't get page count of file [{0}], error code [{1}], method [{2}]", imagePath, res, MethodBase.GetCurrentMethod().Name);
                    return result;
                }

                result = obj._Information.Pages;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                if (obj != null)
                {
                    obj._SaveFile = new Save();
                    obj._OpenFile = new Open();
                    obj = null;
                }
            }
            return result;
        }
        #endregion

        #region "GetImageResolution" function
        /// <summary>
        /// Get image resolution.
        /// </summary>
        /// <param name="imagePath">the image path to get it's information</param>
        /// <returns>The image resolution when successfull, size.Empty when failed.</returns>
        public static Size GetImageResolution(string imagePath)
        {
            TiffDLL obj = new TiffDLL();
            Size result = Size.Empty;
            try
            {
                obj._RegistrationCode = GetRegCode(multiPL);
                obj._OpenFile.Filename = imagePath;
                obj._SaveFile.Filename = INFO;
                int res = obj.Run();

                if (res != 0)
                {
                    ILog.LogWarning("Can't get resolution of file [{0}], error code [{1}], method [{2}]", imagePath, res, MethodBase.GetCurrentMethod().Name);
                    return result;
                }

                result = new Size(obj._Information.HorizontalResolution, obj._Information.VerticalResolution);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                if (obj != null)
                {
                    obj._SaveFile = new Save();
                    obj._OpenFile = new Open();
                    obj = null;
                }
            }
            return result;
        }
        #endregion        

        #region "GetImageSize" function
        /// <summary>
        /// Get image size
        /// </summary>
        /// <param name="imagePath">the image path to get it's information</param>
        /// <returns>The image size when successfull, size.Empty when failed.</returns>
        public static Size GetImageSize(string imagePath)
        {
            Size result = Size.Empty;
            TiffDLL obj = new TiffDLL();

            try
            {
                obj._RegistrationCode = GetRegCode(multiPL);
                obj._OpenFile.Filename = imagePath;
                obj._SaveFile.Filename = INFO;
                int res = obj.Run();

                if (res != 0)
                {
                    ILog.LogWarning("Can't get size of file [{0}], error code [{1}], method [{2}]", imagePath, res, MethodBase.GetCurrentMethod().Name);
                    return result;
                }

                result = new Size(obj._Information.Width, obj._Information.Height);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                if (obj != null)
                {
                    obj._SaveFile = new Save();
                    obj._OpenFile = new Open();
                    obj = null;
                }
            }
            return result;
        }
        #endregion

        #region "SplitTiffPages" functions
        /// <summary>
        /// Split a multi-page tiff to single pages.
        /// </summary>
        /// <param name="sourceFile">The multi-tiff image to split.</param>
        /// <param name="outputFileNameFormat">ommit to use the source file name as the base for the created pages or specify folder path + format for the output path, i.e. @"C:\myfile_{0}.tif"</param>
        /// <param name="pageNumbers">Ommit to split all pages, specify page numbers to split those pages only.</param>
        /// <returns>The list of page names created by splitting when successfull, false when failed.</returns>
        public static string[] SplitTiffPages(string sourceFile, string outputFileNameFormat, params int[] pageNumbers)
        {
            List<string> result = new List<string>();
            TiffDLL obj = new TiffDLL();

            try
            {
                if (string.IsNullOrEmpty(outputFileNameFormat) || string.Compare(sourceFile, outputFileNameFormat, true) == 0)
                {
                    outputFileNameFormat = Path.Combine(Path.GetDirectoryName(sourceFile), Path.GetFileNameWithoutExtension(sourceFile) + "_{0}." + Path.GetExtension(sourceFile).Trim('.'));
                }

                //-- Split to individual pages --\\               
                obj._RegistrationCode = GetRegCode(multiPL);
                obj._OpenFile.Filename = sourceFile;

                //-- Get page count --\\
                obj._SaveFile.Filename = INFO;
                obj._SaveFile.Format = Format.TIFF_CCITT4_1bit;
                obj._SaveFile.OverwriteFile = OverwriteFile.Overwrite;
                int res = obj.Run();

                if (res != 0)
                {
                    ILog.LogWarning("Tiff file [{0}] cant be split, get page count error code [{1}], method [{2}]", sourceFile, res, MethodBase.GetCurrentMethod().Name);
                    return result.ToArray();
                }

                int pageCount = obj._Information.Pages;
                obj._SaveFile.Filename = outputFileNameFormat;

                List<int> pageNumbersList = new List<int>();
                if (pageNumbers != null && pageNumbers.Length > 0) pageNumbersList.AddRange(pageNumbers);


                for (int i = 1; i <= pageCount; i++)
                {
                    obj._SaveFile.Filename = Path.Combine(Path.GetDirectoryName(sourceFile), string.Format(outputFileNameFormat, i) + "." + Path.GetExtension(sourceFile).Trim('.'));
                    if (!Directory.Exists(Path.GetDirectoryName(obj._SaveFile.Filename))) Directory.CreateDirectory(Path.GetDirectoryName(obj._SaveFile.Filename));
                    obj._OpenFile.PageSelection = i;
                    obj._SaveFile.Format = TiffDLL90.Format.Auto;

                    //-- Perform page split --\\
                    if (pageNumbersList == null || pageNumbersList.Count <= 0 || pageNumbersList.Contains(i))
                    {
                        res = obj.Run();
                        result.Add(obj._SaveFile.Filename);

                        if (res != 0)
                        {
                            ILog.LogWarning("Tiff file [{0}] split to file [{1}], error code [{2}], method [{3}]", sourceFile, string.Format(outputFileNameFormat, i), res, MethodBase.GetCurrentMethod().Name);
                            return result.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                if (obj != null)
                {
                    obj._SaveFile = new Save();
                    obj._OpenFile = new Open();
                    obj = null;
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Split a multi-page tiff to single pages.
        /// </summary>
        /// <param name="sourceFile">The multi-tiff image to split.</param>
        /// <param name="outputFileNames">list of target file names</param>
        /// <returns>true when successfull, false when failed.</returns>
        public static bool SplitTiffPages(string sourceFile, string[] outputFileNames)
        {
            bool result = false;
            TiffDLL obj = new TiffDLL();

            try
            {
                //-- Split to individual pages --\\

                obj._RegistrationCode = GetRegCode(multiPL);
                obj._OpenFile.Filename = sourceFile;

                //-- Set save data --\\
                obj._SaveFile.Filename = INFO;
                obj._SaveFile.Format = Format.TIFF_CCITT4_1bit;
                obj._SaveFile.OverwriteFile = OverwriteFile.Overwrite;

                int res = obj.Run();

                if (res != 0)
                {
                    ILog.LogWarning("Tiff file [{0}] cant be split, get page count, error code [{1}], method [{2}]", sourceFile, res, MethodBase.GetCurrentMethod().Name);
                    return false;
                }

                //-- Get page count --\\
                int pageCount = obj._Information.Pages;

                if (pageCount == 1)
                { //nothing to split
                    File.Copy(sourceFile, outputFileNames[0]);
                    return true;
                }


                for (int i = 1; i <= pageCount; i++)
                {
                    obj._SaveFile.Filename = outputFileNames[i - 1];
                    if (!Directory.Exists(Path.GetDirectoryName(obj._SaveFile.Filename))) Directory.CreateDirectory(Path.GetDirectoryName(obj._SaveFile.Filename));
                    obj._OpenFile.PageSelection = i;
                    obj._SaveFile.Format = Format.TIFF_CCITT4_1bit;

                    //-- Perform page split --\\
                    res = obj.Run();
                    if (res != 0)
                    {
                        ILog.LogWarning("Tiff file [{0}] split to file [{1}], error code [{2}], method [{3}]", sourceFile, outputFileNames[i], res, MethodBase.GetCurrentMethod().Name);
                        return false;
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                if (obj != null)
                {
                    obj._SaveFile = new Save();
                    obj._OpenFile = new Open();
                    obj = null;
                }
            }
            return result;
        }
        #endregion        
    }
}