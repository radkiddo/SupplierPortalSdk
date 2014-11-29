using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TiS.Core.eFlowAPI;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace TiS.Engineering.InputApi
{
    /// <summary>
    /// DataTable to CCCOllection parser and validator.
    /// </summary>
#if INTERNAL
    internal class CCDataTable
#else
    public class CCDataTable
#endif
    {
        #region class variables
        /// <summary>
        /// A list containing all the possible DataTable columns.
        /// </summary>
        private readonly static List<String> chkLst = new List<String>(new String[] { 
                    CCEnums.CCTableColumns.Level.ToString().ToUpper(),
                    CCEnums.CCTableColumns.DataType.ToString().ToUpper(),
                    CCEnums.CCTableColumns.Key.ToString().ToUpper(),
                    CCEnums.CCTableColumns.Data.ToString().ToUpper()});

        //private static readonly bool LogCollectionDataTable = Regex.IsMatch(Environment.CommandLine,@"(?i)[\-/]LogCollectionDataTable" );

        /// <summary>
        /// Avoid an exception on list duplicates.
        /// </summary>
        private static bool excpetionOnDictDuplicates = !Regex.IsMatch(Environment.CommandLine, "(?i)AllowNamedUserTagsDuplicates");
        #endregion

        #region "FromDataTable" function
        /// <summary>
        /// Read collection definitions from DataTable and load into a CCCollection definition
        /// </summary>
        /// <param name="cfg">The configuration to use.</param>
        /// <param name="errCode">Returns the erroro code for this function.</param>
        /// <param name="errMsg">Will contain all the errors that this function has encountered</param>
        /// <param name="copySourceFiles">Copy source files</param>
        /// <param name="createdFiles">A list of files created durning process. (like XML to PRD)</param>
        /// <param name="dbCollectionData">The data table containing the data for the collection.</param>
        /// <returns>Return a CCCollection from the specified collection data.</returns>
        public static CCCollection FromDataTable(CCConfiguration.CCConfigurationData cfg, out int errCode, out String errMsg, bool copySourceFiles, out String[] createdFiles, DataTable dbCollectionData)
        {
            errCode = -1;
            errMsg = null;
            createdFiles = null;

            try
            {
                //-- Log DataTable --\\
                if (cfg!= null && cfg.LogCollectionDataTable)
                {
                    try
                    {
                        ILog.LogInfo("{0} received DataTable [{1}], rows data (see next line\\s):\r\n[{2}], [{3}], [{4}], [{5}]\r\n{6}", "InputAPI", dbCollectionData.TableName ?? string.Empty,
                        CCEnums.CCTableColumns.Level, CCEnums.CCTableColumns.DataType, CCEnums.CCTableColumns.Key, CCEnums.CCTableColumns.Data, 
                        Helpers.CCTDataRows.FromDataRows(dbCollectionData.Select()).ToString("\r\n", ", ")); }
                    catch { }
                }

                #region //-- Name table columns (if not named) --\\
                if (!chkLst.Contains(dbCollectionData.Columns[0].ColumnName.ToUpper()))
                {
                    dbCollectionData.Columns[0].ColumnName = CCEnums.CCTableColumns.Level.ToString();
                }

                if (!chkLst.Contains(dbCollectionData.Columns[1].ColumnName.ToUpper()))
                {
                    dbCollectionData.Columns[1].ColumnName = CCEnums.CCTableColumns.DataType.ToString();
                }

                if (!chkLst.Contains(dbCollectionData.Columns[2].ColumnName.ToUpper()))
                {
                    dbCollectionData.Columns[2].ColumnName = CCEnums.CCTableColumns.Key.ToString();
                }

                if (!chkLst.Contains(dbCollectionData.Columns[3].ColumnName.ToUpper()))
                {
                    dbCollectionData.Columns[3].ColumnName = CCEnums.CCTableColumns.Data.ToString();
                }
                #endregion

                //-- Process header data --\\
                int pgCnt = 0;
                CCCollection res = ReadHeaderData(cfg, out errCode, out errMsg, out pgCnt, dbCollectionData);

                if (res != null && errCode == (int)CCEnums.CCErrorCodes.E0001)
                {
                    //-- Process pages data --\\
                    createdFiles = ReadPagesData(cfg, out errCode, out errMsg, pgCnt, ref res, dbCollectionData);
                    if (errCode == (int)CCEnums.CCErrorCodes.E0001)
                    {
                        return res;
                    }
                }
                else
                {
                    if  (String.IsNullOrEmpty( errMsg)) errMsg = CCConstants.E0000;
                    throw new Exception(errMsg);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex, false);
                throw ex;
            }
            return null;
        } 
        #endregion

        #region "ReadHeaderData" function
        /// <summary>
        /// Read header data.
        /// </summary>
        /// <param name="cfg">The configuration profile to use.</param>
        /// <param name="errCode">The return error code, 1  is OK, anything else not.</param>
        /// <param name="errMsg">Will contain the error data if any error occures.</param>
        /// <param name="expectedNumberOfPages">Returns the expected number of pages.</param>
        /// <param name="dbCollectionData">The data table with rows to get Hedaer info.</param>
        /// <returns>A CCCollection when successfull, null when failed,</returns>
        public static CCCollection ReadHeaderData(CCConfiguration.CCConfigurationData cfg, out int errCode, out String errMsg, out int expectedNumberOfPages, DataTable dbCollectionData)
        {
            errCode = -1;
            errMsg = null;
            expectedNumberOfPages = 0;

            try
            {
                if (dbCollectionData == null || dbCollectionData.Rows.Count <= 0 || dbCollectionData.Columns.Count < 4)
                {
                    errCode = (int)CCEnums.CCErrorCodes.E0041;
                    errMsg = String.Format("{0}, error code [{1}]", CCConstants.E0041, errCode);
                    throw new Exception(errMsg);
                }

                //-- Get header rows --\\
                DataRow[] headerRows = dbCollectionData.Select(String.Format("{0}={1}",  CCEnums.CCTableColumns.Level, IsStringData(dbCollectionData.Columns[ CCEnums.CCTableColumns.Level.ToString()]) ? "'0'":"0"));
                if (headerRows == null || headerRows.Length <= 0) headerRows = dbCollectionData.Select(String.Format("{0}='0'", CCEnums.CCTableColumns.Level)); 


                //-- Process header data --\\
                if (headerRows != null && headerRows.Length > 0)
                {
                    CCCollection res = new CCCollection();
                    Dictionary<String, String> lstDct = new Dictionary<String, String>();
                    List<String> attachments = new List<String>();
                    String startDate = null;
                    String endDate = null;
                    bool hasFlow = false;


                    foreach (DataRow dr in headerRows)
                    {
                        //-- Extract columns data --\\
                        if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.Name.ToString(), true) == 0)
                        {
                            //-- Name --\\
                            res.Name = dr[CCEnums.CCTableColumns.Data.ToString()].ToString();
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.FlowType.ToString(), true) == 0)
                        {
                            //-- FlowType --\\
                            hasFlow = true;
                            res.FlowType = dr[CCEnums.CCTableColumns.Data.ToString()].ToString();
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.AbsolutePriority.ToString(), true) == 0)
                        {
                            //-- AbsolutePriority --\\
                            double absPrio =0d;
                            if (double.TryParse(dr[CCEnums.CCTableColumns.Data.ToString()].ToString(), out absPrio)) res.AbsolutePriority = absPrio;
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.StartDate.ToString(), true) == 0)
                        {
                            //-- StartDate --\\
                            startDate = dr[CCEnums.CCTableColumns.Data.ToString()].ToString();
                            CCUtils.AddSetDictionaryItem(CCEnums.CCHedaerDataType.StartDate.ToString(), startDate, excpetionOnDictDuplicates, ref lstDct);
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.EndDate.ToString(), true) == 0)
                        {
                            //-- EndDate --\\
                            endDate = dr[CCEnums.CCTableColumns.Data.ToString()].ToString();
                            CCUtils.AddSetDictionaryItem(CCEnums.CCHedaerDataType.EndDate.ToString(), endDate, excpetionOnDictDuplicates, ref lstDct);
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.DeviceID.ToString(), true) == 0)
                        {
                            //-- DeviceID --\\
                            CCUtils.AddSetDictionaryItem(CCEnums.CCHedaerDataType.DeviceID.ToString(), dr[CCEnums.CCTableColumns.Data.ToString()].ToString(),excpetionOnDictDuplicates, ref lstDct);
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.UserID.ToString(), true) == 0)
                        {
                            //-- UserID --\\
                            CCUtils.AddSetDictionaryItem(CCEnums.CCHedaerDataType.UserID.ToString(), dr[CCEnums.CCTableColumns.Data.ToString()].ToString(), excpetionOnDictDuplicates, ref lstDct);
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.MachineID.ToString(),  true) == 0)
                        {
                            //-- MachineID --\\
                            CCUtils.AddSetDictionaryItem(CCEnums.CCHedaerDataType.MachineID.ToString(), dr[CCEnums.CCTableColumns.Data.ToString()].ToString(), excpetionOnDictDuplicates, ref lstDct);
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.TotalPagesInBatch.ToString(), true) == 0)
                        {
                            //-- TotalPagesInBatch --\\                           
                            expectedNumberOfPages = CCUtils.StrToIntDef(dr[CCEnums.CCTableColumns.Data.ToString()].ToString(), 0);
                            CCUtils.AddSetDictionaryItem(CCEnums.CCHedaerDataType.TotalPagesInBatch.ToString(), expectedNumberOfPages.ToString(),excpetionOnDictDuplicates, ref lstDct);
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.ImagePath.ToString(), true) == 0)
                        {
                            //-- ImagePath --\\
                            res.ImagePath = dr[CCEnums.CCTableColumns.Data.ToString()].ToString();
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.Attachments.ToString(), true) == 0)
                        {
                            //-- Attachments --\\
                            String val = dr[CCEnums.CCTableColumns.Data.ToString()].ToString();
                            if (!String.IsNullOrEmpty(val) && !File.Exists(val))
                            {
                                errCode = (int)CCEnums.CCErrorCodes.E0065;
                                errMsg = String.Format("{0},  file path [{1}], error code [{2}]", CCConstants.E0065, val ?? String.Empty, errCode);
                                throw new Exception(errMsg);
                            }
                            else
                            {
                                if (!attachments.Contains(val)) attachments.Add(val);
                            }
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.BatchType.ToString(), true) == 0)
                        {
                            //-- BatchType --\\
                            CCUtils.AddSetDictionaryItem(CCEnums.CCHedaerDataType.BatchType.ToString(), dr[CCEnums.CCTableColumns.Data.ToString()].ToString(),excpetionOnDictDuplicates, ref lstDct);
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.Priority.ToString(), true) == 0)
                        {
                            //-- Priority --\\
                            String pr = dr[CCEnums.CCTableColumns.Data.ToString()].ToString().ToUpper();
                            if (!String.IsNullOrEmpty(pr) && !Regex.IsMatch(pr, CCConstants.RX_PRIORITY))
                            {
                                errCode = (int)CCEnums.CCErrorCodes.E0070;
                                errMsg = String.Format("{0}, specified value [{1}], error code [{2}]", CCConstants.E0070, pr, errCode);
                                throw new Exception(errMsg);
                            }
                            else
                            {
                                res.Priority = pr == WorkflowPriorityLevel.AboveNormal.ToString().Substring(0, 1) || pr.Replace(" ",String.Empty) == WorkflowPriorityLevel.AboveNormal.ToString().ToUpper()  ? WorkflowPriorityLevel.AboveNormal :
                                                    pr == WorkflowPriorityLevel.High.ToString().Substring(0, 1) || pr == WorkflowPriorityLevel.High.ToString().ToUpper() ? WorkflowPriorityLevel.High :
                                                    pr == WorkflowPriorityLevel.Low.ToString().Substring(0, 1) || pr == WorkflowPriorityLevel.Low.ToString().ToUpper() ? WorkflowPriorityLevel.Low : WorkflowPriorityLevel.Normal;
                            }
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.ApplicationName.ToString(), true) == 0)
                        {
                            //-- ApplicationName --\\
                            res.LoginApplication = dr[CCEnums.CCTableColumns.Data.ToString()].ToString();
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.StationName.ToString(), true) == 0)
                        {
                            //-- StationName --\\
                            res.LoginStation = dr[CCEnums.CCTableColumns.Data.ToString()].ToString();
                        }
                        else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCHedaerDataType.MetaData.ToString(), true) == 0)
                        {
                            //-- Additional MetaData --\\
                            String tagName = dr[CCEnums.CCTableColumns.Key.ToString()] is DBNull || String.IsNullOrEmpty(dr[CCEnums.CCTableColumns.Key.ToString()].ToString()) ? null : dr[CCEnums.CCTableColumns.Key.ToString()].ToString();
                            if (!String.IsNullOrEmpty(tagName))
                            {
                                CCUtils.AddSetDictionaryItem(tagName, dr[CCEnums.CCTableColumns.Data.ToString()].ToString(), excpetionOnDictDuplicates, ref lstDct);
                            }
                        }
                    }

                    if (!hasFlow && !String.IsNullOrEmpty(cfg.FlowType))
                    {
                        //-- Take flow type from configuration, if defined --\\
                        res.FlowType = cfg.FlowType;
                    }

                    #region //-- Do some validations --\\
                    if (expectedNumberOfPages <= 0)
                    {
                        //-- Validate expected number of pages value --\\
                        errCode = (int)CCEnums.CCErrorCodes.E0051;
                        errMsg = String.Format("{0}, error code [{1}]", CCConstants.E0051, errCode);
                        throw new Exception(errMsg);
                    }
                    else if (!File.Exists(res.ImagePath))
                    {
                        //-- Validate image path --\\
                        errCode = (int)CCEnums.CCErrorCodes.E0060;
                        errMsg = String.Format("{0}, file path [{1}], error code [{2}]", CCConstants.E0060, res.ImagePath ?? String.Empty, errCode);
                        throw new Exception(errMsg);
                    }
                    else if (!String.IsNullOrEmpty(res.Name) && !Regex.IsMatch(res.Name, CCConstants.RX_COLLECTION_NAME))
                    {
                        //-- Validate collection name --\\
                        errCode = (int)CCEnums.CCErrorCodes.E0010;
                        errMsg = String.Format("{0}, error code [{1}]", CCConstants.E0010, errCode);
                        throw new Exception(errMsg);
                    }
                    else if (!ValidateDates(startDate, endDate, out errCode, out errMsg))
                    {
                        //-- Validate start and end time\date --\\
                        errCode = (int)CCEnums.CCErrorCodes.E0020;
                        errMsg = String.Format("{0}, {1} [{2}],{3} [{4}], error code [{5}]", CCConstants.E0020, CCEnums.CCHedaerDataType.StartDate, startDate, CCEnums.CCHedaerDataType.EndDate, endDate, errCode);
                        throw new Exception(errMsg);
                    } 
                    #endregion


                    //-- Finish setting data --\\
                    res.Attachments = attachments.ToArray();
                    res.NamedUserTags.NativeDictionary = lstDct;

                    //-- Done gathering data from all header rows --\\
                    errCode = (int)CCEnums.CCErrorCodes.E0001;
                    return res;
                }
                else
                {
                    errCode = 999;
                    errMsg = String.Format("{0}, error code [{1}]", CCConstants.E0040, errCode);
                    throw new Exception(errMsg);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex,false);
                throw (ex);
            }
        } 
        #endregion

        #region "IsStringData" functions
        /// <summary>
        /// Check a column data type to see if it is a string or not (requires '' or not...)
        /// </summary>
        /// <param name="dataCol">The data column to check it's type.</param>
        /// <returns>true if the column is a string</returns>
        internal static bool IsStringData(DataColumn dataCol)
        {
            try
            {
                return dataCol.DataType == typeof(string);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return false;
        }

        /// <summary>
        /// Check a column data type to see if it is a string or not (requires '' or not...)
        /// </summary>
        /// <param name="dataTbl">The data table to check it's column.</param>
        /// <param name="columnName">The name of the column to check.</param>
        /// <returns>true if the column is a string</returns>
        internal static bool IsStringData(DataTable dataTbl, String columnName)
        {
            try
            {
                return IsStringData(dataTbl.Columns[columnName]);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return false;
        }

        /// <summary>
        /// Check a column data type to see if it is a string or not (requires '' or not...)
        /// </summary>
        /// <param name="dataTbl">The data table to check it's column.</param>
        /// <param name="columnIndex">The index of the column to check.</param>
        /// <returns>true if the column is a string</returns>
        internal static bool IsStringData(DataTable dataTbl, int columnIndex)
        {
            try
            {
                return IsStringData(dataTbl.Columns[columnIndex]);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return false;
        }
        #endregion

        #region "ReadPagesData" function
        /// <summary>
        /// Read and load pages data into the collection definition.
        /// </summary>
        /// <param name="cfg">The configuration profile to use.</param>
        /// <param name="errCode">Returns the error code of this operation.</param>
        /// <param name="errMsg">Returns the error string of this operation</param>
        /// <param name="expectedNumberOfPages">The expected nuumber of pages in the collection.</param>
        /// <param name="coll">The collection definition to add the data to.</param>
        /// <param name="colData">The data table containing the data for the collection.</param>
        /// <returns>An array oif file paths for files that were created in this method</returns>
        private static String[] ReadPagesData(CCConfiguration.CCConfigurationData cfg, out int errCode, out String errMsg, int expectedNumberOfPages, ref CCCollection coll, DataTable colData)
        {
            errCode = (int)CCEnums.CCErrorCodes.E0000;
            errMsg = null;
            List<String> result = new List<String>();

            try
            {
                //-- Validate expected page count --\\
                if (expectedNumberOfPages <= 0)
                {
                    errCode = (int)CCEnums.CCErrorCodes.E0051;
                    errMsg = String.Format("{0}, error code [{1}]", CCConstants.E0051, errCode);
                    throw new Exception(errMsg);
                }

                int numberOfImagePages = CCUtils.GetImagePageCount(coll.ImagePath, null);

                //-- Validate page count --\\
                if (numberOfImagePages <= 0)
                {
                    errCode = (int)CCEnums.CCErrorCodes.E0061;
                    errMsg = String.Format("{0}, file path [{1}], error code [{2}]", CCConstants.E0061, coll.ImagePath, errCode);
                    throw new Exception(errMsg);
                }

                //-- Validate page count against expected page count --\\
                if (numberOfImagePages != expectedNumberOfPages)
                {
                    errCode = (int)CCEnums.CCErrorCodes.E0050;
                    errMsg = String.Format("{4}. expected number of pages [{1}], actual image count [{2}], file path [{3}], error code [{0}]", errCode, expectedNumberOfPages, numberOfImagePages, coll.ImagePath, CCConstants.E0050);
                    throw new Exception(errMsg);
                }

                List<CCCollection.CCForm> forms = new List<CCCollection.CCForm>();

                for (int pageCount = 1; pageCount <= expectedNumberOfPages; pageCount++)
                {
                    Dictionary<String, String> lstDct = new Dictionary<String, String>();
                    List<String> attachments = new List<String>();
                    DataRow[] pageRows = colData.Select(String.Format("{0}={1}", CCEnums.CCTableColumns.Level, IsStringData( colData.Columns[CCEnums.CCTableColumns.Level.ToString()]) ? "'"+pageCount.ToString()+"'" :  pageCount.ToString()));
                    if (pageRows == null || pageRows.Length <= 0) pageRows = colData.Select(String.Format("{0}='{1}'", CCEnums.CCTableColumns.Level, pageCount));

                    //-- Process pages data --\\
                    if (pageRows != null && pageRows.Length > 0)
                    {
                        List<int> pageNumbers = new List<int>();
                        String startDate = null;
                        String endDate = null;

                        //-- Iterate pages rows --\\
                        foreach (DataRow dr in pageRows)
                        {
                            //-- Get the page number the data is itntended to --\\
                            int pageNumber = CCUtils.StrToIntDef(dr[CCEnums.CCTableColumns.Level.ToString()].ToString(), -1);
                            if (!pageNumbers.Contains(pageNumber)) pageNumbers.Add(pageNumber);

                            //-- << Read pages data >>--\\
                            if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCPagesDataType.PageType.ToString(), true) == 0)
                            {
                                //-- PageType --\\
                                CCUtils.AddSetDictionaryItem(CCEnums.CCPagesDataType.PageType.ToString(), dr[CCEnums.CCTableColumns.Data.ToString()].ToString(), excpetionOnDictDuplicates, ref  lstDct);
                            }
                            else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCPagesDataType.StartDate.ToString(), true) == 0)
                            {
                                //-- StartDate --\\
                                startDate = dr[CCEnums.CCTableColumns.Data.ToString()].ToString();
                                CCUtils.AddSetDictionaryItem(CCEnums.CCPagesDataType.StartDate.ToString(), startDate, excpetionOnDictDuplicates, ref  lstDct);
                            }
                            else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCPagesDataType.EndDate.ToString(), true) == 0)
                            {
                                //-- EndDate --\\
                                endDate = dr[CCEnums.CCTableColumns.Data.ToString()].ToString();
                                CCUtils.AddSetDictionaryItem(CCEnums.CCPagesDataType.EndDate.ToString(), endDate, excpetionOnDictDuplicates, ref  lstDct);
                            }
                            else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCPagesDataType.Attachments.ToString(), true) == 0)
                            {
                                //-- Attachments --\\
                                String val = dr[CCEnums.CCTableColumns.Data.ToString()].ToString();
                                if (!String.IsNullOrEmpty(val) && !File.Exists(val))
                                {
                                    errCode = (int)CCEnums.CCErrorCodes.E0065;
                                    errMsg = String.Format("{1}, file path[{2}], error code [{0}]", errCode, CCConstants.E0065, val ?? String.Empty);
                                    throw new Exception(errMsg);
                                }
                                else
                                {
                                    if (!attachments.Contains(val)) attachments.Add(val);
                                }
                            }
                            else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCPagesDataType.XmlPrdPath.ToString(), true) == 0)
                            {
                                //-- XmlPrdPath --\\
                                int errC = 0;
                                String errS = null;
                                String sourceXmlPath = dr[CCEnums.CCTableColumns.Data.ToString()].ToString();

                                //-- Convert XML to PRD  and add as page attachment --\\
                                String prdPath = DeserializePRD(out errC, out errS, pageNumber, false, sourceXmlPath);
                                if (!String.IsNullOrEmpty(prdPath))
                                {
                                    if (File.Exists(sourceXmlPath))
                                    {
                                        //-- Lock PRD file --\\
                                        String lockPrd = CCFileList.GetLockedFilePath(prdPath, cfg.LockExtension);
                                        if (String.Compare(lockPrd, prdPath, true) != 0)
                                        {
                                            if (File.Exists(lockPrd))
                                            {
                                                File.SetAttributes(lockPrd, FileAttributes.Normal);
                                                File.Delete(lockPrd);
                                            }

                                            File.SetAttributes(prdPath, FileAttributes.Normal);
                                            File.Move(prdPath, lockPrd);
                                        }

                                        File.Delete(sourceXmlPath);
                                        if (!attachments.Contains(lockPrd)) attachments.Add(lockPrd);
                                        if (!result.Contains(lockPrd)) result.Add(lockPrd);
                                    }
                                }
                            }
                            else if (String.Compare(dr[CCEnums.CCTableColumns.DataType.ToString()].ToString(), CCEnums.CCPagesDataType.MetaData.ToString(), true) == 0)
                            {
                                //-- Additional MetaData--\\
                                String tagName = dr[CCEnums.CCTableColumns.Key.ToString()] is DBNull || String.IsNullOrEmpty(dr[CCEnums.CCTableColumns.Key.ToString()].ToString()) ? null : dr[CCEnums.CCTableColumns.Key.ToString()].ToString();
                                if (!String.IsNullOrEmpty(tagName))
                                {
                                    CCUtils.AddSetDictionaryItem(tagName, dr[CCEnums.CCTableColumns.Data.ToString()].ToString(), excpetionOnDictDuplicates, ref lstDct);
                                }
                            }
                        }

                        //-- Validate page dates --\\
                        if (!ValidateDates(startDate, endDate, out errCode, out errMsg))
                        {
                            //-- Validate start and end time\date --\\
                            errCode = (int)CCEnums.CCErrorCodes.E0020;
                            errMsg = String.Format("{0}, {1} [{2}],{3} [{4}], page number [{5}], error code [{6}]", CCConstants.E0020, CCEnums.CCPagesDataType.StartDate, startDate, CCEnums.CCPagesDataType.EndDate, endDate, pageCount, errCode);
                            throw new Exception(errMsg);
                        }
                    }

                    //-- Create a form and page, NF 2011-11-07 added support for multipage per form --\\
                    CCCollection.CCForm frm = cfg.MultiPagePerForm ? forms.Count > 0 ? forms[0] : new CCCollection.CCForm(null, null, null) : new CCCollection.CCForm(null, null, null);
                    

                     if (!cfg.MultiPagePerForm || (cfg.MultiPagePerForm  && forms.Count<=0))   frm.NamedUserTags.NativeDictionary = lstDct;//-- Add page Named user tags to Form user tags --\\
                    CCCollection.CCPage pg = new CCCollection.CCPage();
                    pg.Attachments = attachments.ToArray();
                    pg.NamedUserTags.NativeDictionary = lstDct;
                    if (!cfg.MultiPagePerForm || forms.Count <= 0)
                    {
                        frm.Pages = new CCCollection.CCPage[] { pg };
                    }
                    else
                    {
                        List<CCCollection.CCPage> pgs = new List<CCCollection.CCPage>(frm.Pages);
                        pgs.Add(pg);
                        frm.Pages = pgs.ToArray();
                    }

                   if (!cfg.MultiPagePerForm || forms.Count<=0) forms.Add(frm);
                }

                //-- Do some validations --\\
                if (expectedNumberOfPages != forms.Count)
                {
                    if (!cfg.MultiPagePerForm || forms.Count<=0)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0090;
                        errMsg = String.Format("Error code [{0}], {1},expected number of pages [{2}], eflow form count [{3}]", errCode, CCConstants.E0090, expectedNumberOfPages, forms.Count);
                        throw new Exception(errMsg);
                    }
                    else
                    {
                        if (forms[0].Pages.Length != expectedNumberOfPages)
                        {
                            errCode = (int)CCEnums.CCErrorCodes.E0090;
                            errMsg = String.Format("Error code [{0}], {1},expected number of pages [{2}] in one form, eflow form count [{3}]", errCode, CCConstants.E0090, expectedNumberOfPages, forms.Count);
                            throw new Exception(errMsg);
                        }
                    }
                }


                errCode = (int)CCEnums.CCErrorCodes.E0001;
                coll.Forms = forms.ToArray();
            }
            catch (Exception ex)
            {
                ILog.LogError(ex,false);

                if (String.IsNullOrEmpty(errMsg)) errMsg = CCConstants.E0000 + "," + ex.Message;
                throw (ex);
            }
            return result.ToArray();
        } 
        #endregion

        #region "ValidateDates" function
        /// <summary>
        /// Validate the date\\s specified.
        /// </summary>
        /// <param name="startDate">Start date to check</param>
        /// <param name="endDate">End date to check</param>
        /// <param name="errCode">The return error code</param>
        /// <param name="errMsg">The return error string.</param>
        /// <returns>true when successfull, false whennot.</returns>
        public static bool ValidateDates(String startDate, String endDate, out int errCode, out String errMsg)
        {
            errCode = (int)CCEnums.CCErrorCodes.E0000;
            errMsg = null;

            try
            {
                //-- Both strings are empty, thus valid --\\
                if (String.IsNullOrEmpty(startDate) && String.IsNullOrEmpty(endDate))
                {
                    return true;
                }

                CultureInfo countryDateFormat = new CultureInfo("en-GB");
                DateTime stDate;
                bool stDateOk = DateTime.TryParse(startDate, countryDateFormat, DateTimeStyles.None, out stDate);

                DateTime enDate;
                bool enDateOk = DateTime.TryParse(endDate, countryDateFormat, DateTimeStyles.None, out enDate);

                if (stDateOk && enDateOk)
                {
                    //-- Validate start time before or equal to end time --\\
                    if (stDate > enDate)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0030;
                        errMsg = String.Format("{0}, {1} [{2}],{3} [{4}], error code [{5}]", CCConstants.E0030, CCEnums.CCHedaerDataType.StartDate, stDate.ToString(CCConstants.DATE_FORMAT), CCEnums.CCHedaerDataType.EndDate, enDate.ToString(CCConstants.DATE_FORMAT), errCode);
                        throw new Exception(errMsg);
                    }
                    else return true;
                }
                else if (!stDateOk && !String.IsNullOrEmpty(startDate))
                {
                    //-- Validate start time to be a valid date if a String is present --\\
                    errCode = (int)CCEnums.CCErrorCodes.E0020;
                    errMsg = String.Format("{0}, {1} [{2}], error code [{3}]", CCConstants.E0020, CCEnums.CCHedaerDataType.StartDate, stDate.ToString(CCConstants.DATE_FORMAT), errCode);
                    throw new Exception(errMsg);
                }
                else if (!enDateOk && !String.IsNullOrEmpty(endDate))
                {
                    //-- Validate end time to be a valid date if a String is present --\\
                    errCode = (int)CCEnums.CCErrorCodes.E0020;
                    errMsg = String.Format("{0}, {1} [{2}], error code [{3}]", CCConstants.E0020, CCEnums.CCHedaerDataType.EndDate, enDate.ToString(CCConstants.DATE_FORMAT), errCode);
                    throw new Exception(errMsg);
                }
                else
                {
                    //-- A date String is present and it is a valid date --\\
                    return true;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex,false);
                throw (ex);
            }
        } 
        #endregion

        #region "DeserializePRDs" function
        /// <summary>
        /// Deserialize any PRD file specified as Xml-Prd and return a path to the converted PRD file.
        /// </summary>
        /// <param name="errCode">Returns the error code from the function.</param>
        /// <param name="errMsg">Returns the error message\\s if any occured.</param>
        /// <param name="pageNumber">The page number to relate this PRD file to.</param>
        /// <param name="throwAllErrors">Throw all erros when true, log them when false.</param>        
        /// <param name="xmlToPrdFilePath">The path of an XML file to deserialize as PRD.</param>
        /// <returns>a path to a prd file when successfull.</returns>
        public static String DeserializePRD(out int errCode, out String errMsg, int pageNumber, bool throwAllErrors, String xmlToPrdFilePath)
        {
            errCode = -1;
            errMsg = null;

            try
            {
                //-- if XmlPRD files were defined convert them --\\
                if (!String.IsNullOrEmpty(xmlToPrdFilePath) && File.Exists(xmlToPrdFilePath))
                {
                    try
                    {
                        //-- Deserialize the XML file to a Prd memory object --\\
                        CollectionOcrData.PageOcrData poc = CollectionOcrData.PageOcrData.FromXml(xmlToPrdFilePath);
                        if (poc != null)
                        {
                            //-- Prepare target PRD file path --\\
                            String targetPRD =  Path.Combine(Path.GetDirectoryName(xmlToPrdFilePath), String.Format("{0}_P{1}.{2}", Path.GetFileNameWithoutExtension(xmlToPrdFilePath), pageNumber.ToString("X").PadLeft(4, '0'), CCEnums.CCFilesExt.PRD));
                            poc.ToPRD(targetPRD);
                            if (File.Exists(targetPRD))
                            {
                                errCode = (int)CCEnums.CCErrorCodes.E0001;
                                return targetPRD;
                            }
                        }
                    }
                    catch (Exception ec)
                    {
                        errCode = (int)CCEnums.CCErrorCodes.E0000;
                        errMsg = String.Format("{0}, error code [{1}], error data: {2}", CCConstants.E0000, errCode, ec.Message);
                        ILog.LogError(ec);
                        if (throwAllErrors) throw ec;
                    }
                }
            }
            catch (Exception ex)
            {
                errCode = (int)CCEnums.CCErrorCodes.E0000;
                errMsg = String.Format("{0}, error code [{1}], error data: {2}", CCConstants.E0000, errCode, ex.Message);
                ILog.LogError(ex);
                if (throwAllErrors) throw ex;
            }
            return null;
        } 
        #endregion
    }
}
