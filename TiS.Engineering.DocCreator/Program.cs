using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace TiS.Engineering.DocCreator
{
    [XmlType(Namespace = Convert.DEF_NAMESPACE_DOCCREATOR)]
    internal static class Program
    {
        /// <summary>
        /// test harness for the docCreator functionality
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        public static void Main(params String[] args)
        {
            try
            {
                String srcFile = null;
                String trgtFile = null;
                String convertTo = null;
                int res = 300;

                foreach (String s in Environment.GetCommandLineArgs())
                {
                    Match mtc = Regex.Match(s, @"(?i)(?<=^[\-/]?Source:).+$");
                    if (mtc.Success) srcFile = mtc.Value.Trim(' ','"');
                    else
                    {
                        mtc = Regex.Match(s, @"(?i)(?<=^[\-/]?Target:).+$");
                        if (mtc.Success) trgtFile = mtc.Value.Trim(' ', '"');
                        else
                        {
                            mtc = Regex.Match(s, @"(?i)(?<=^[\-/]?(Convert(To)?)?type:)(tiff?|pdf)$");
                            if (mtc.Success) convertTo = mtc.Value.Trim(' ', '"');
                            else
                            {
                                mtc = Regex.Match(s, @"(?i)(?<=^[\-/]?Resolution:)\d+$");
                                if (mtc.Success && int.TryParse(mtc.Value.Trim(' ', '"'), out res))
                                {
                                    // do nothing \\
                                }
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(srcFile) && !String.IsNullOrEmpty(trgtFile) && !String.IsNullOrEmpty(convertTo))
                {
                    String errMsg = null;
                    if (String.Compare(convertTo, Convert.OutputExtEnm.PDF.ToString(), true) == 0)
                    {
                        Convert.TiffToPdf(srcFile, trgtFile, out errMsg);
                    }
                    else if (String.Compare(convertTo, Convert.OutputExtEnm.TIF.ToString(), true) == 0)
                    {
                        Convert.PdfToTiff(srcFile, trgtFile, res, out errMsg);
                    }
                    
                   if (!String.IsNullOrEmpty(errMsg)) ILog.LogInfo(errMsg);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        }
    }
}
