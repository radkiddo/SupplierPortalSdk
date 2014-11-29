using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace TiS.Engineering.DocCreator
{
    [XmlType(Namespace = Convert.DEF_NAMESPACE_DOCCREATOR)]
    public class Convert
    {
        public const String DEF_NAMESPACE_DOCCREATOR = "TiS.Engineering.DocCreator";

        private static bool throwAllExceptions;
        public static bool ThrowAllExceptions { get { return throwAllExceptions; } set { throwAllExceptions = value; } }
        public delegate void OnMessage(String format, params object[] args);

        #region  //-- To register docCreator --\\
        // User name: Top Image Systems UK Ltd
        // Company: Top Image Systems UK Ltd
        // Serial number: 3Q6TB-M448D-CF5NG-PKHW3-FQ8NZ
        // Run: "c:\program files\neevia.com\docCreator\dcreg.exe"
        // And copy paste the above license details. 
        #endregion

        public static event OnMessage AllMsg
        {
            add { ILog.AllMsg += value; }
            remove { ILog.AllMsg -= value; }
        }

        public static event OnMessage ErrorMsg
        {
            add { ILog.ErrorMsg += value; }
            remove { ILog.ErrorMsg -= value; }
        }

        [XmlType(Namespace = Convert.DEF_NAMESPACE_DOCCREATOR)]
        public enum OutputExtEnm
        {
            PDF,
            TIF
        };

        public static bool PdfToTiff(String sFile, String targetFile, int resolution, out String errMsg)
        {
            bool iret = false;
            errMsg = null;

            try
            {
                /*
                 * // TODO: Test:
 Example - set the output document paper size to 3x5 centimeters
NVDC.setParameter("DocumentPaperSize", "3cmx5cm")
 * */

                if (resolution <= 0) resolution = 300;
                string filename = Path.GetFileNameWithoutExtension(targetFile);
                docCreator.docCreatorClass DC = new docCreator.docCreatorClass();
                DC.DocumentOutputFolder = Path.GetDirectoryName(targetFile);
                DC.DocumentOutputName = filename;
                DC.DocumentOutputFormat = OutputExtEnm.TIF.ToString(); 
                DC.TIFFType = "tiffg4";
                DC.DocumentResolution = resolution;
                DC.MPTIFF = true;
                //DC.Scaling = -1;

                DC.SetInputDocument(sFile,string.Empty);
                long RVal = DC.Create();
                if (RVal == 0)
                {
                    iret = true;
                }
                else
                {
                    errMsg = String.Format("Error code [{0}] returned from docCreator.Create(TIFF), calld in method: [{1}], Source file [{2}], target file [{3}], resolution [{4}]", RVal, MethodBase.GetCurrentMethod().Name, sFile ?? String.Empty, targetFile ?? String.Empty, resolution);
                    ILog.InfoMsg(errMsg);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
                if (ThrowAllExceptions) throw ex;
            }

            return iret;
        }

        public static bool TiffToPdf(String sFile, String targetFile, out String errMsg)
        {
            bool iret = false;
            errMsg = null;
            try
            {
                string filename = Path.GetFileNameWithoutExtension(targetFile);
                docCreator.docCreatorClass DC = new docCreator.docCreatorClass();
                DC.DocumentOutputFormat = OutputExtEnm.PDF.ToString();
 
                long RVal = DC.ConvertImage(sFile, targetFile);
                
                if (RVal == 0)
                {                  
                    iret = true;
                }
                else
                { 
                    errMsg = String.Format("Error code [{0}] returned from docCreator.ConvertImage, calld in method: [{1}], Source file [{2}], target file [{3}]", RVal, MethodBase.GetCurrentMethod().Name, sFile ?? String.Empty, targetFile ?? String.Empty);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
                if (ThrowAllExceptions) throw ex;
            }

            return iret;
        }
    }
}
