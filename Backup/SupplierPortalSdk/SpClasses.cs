#region "about"

//
// eFLOW Supplier Portal SDK
// 2013 (c) - Top Image Systems (a project initiated by the UK branch)
//
// The purpose of this SDK is to communicate with the Supplier Portal for eFlow Invoices.
// Developed by: Eduardo Freitas
//

#endregion "about"

#region "Using"

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using TiS.Core.eFlowAPI;
using System.IO;
using eFlow.CollectionManagement;
using Microsoft.Win32;

#endregion "Using"

namespace eFlow.SupplierPortalCore
{
    /// <summary>
    /// "SpClasses" --> Classes that respresent the Table and Collection JSON objects on the Supplier Portal DB (Flexible Cloud DB). 
    /// </summary>
    public class SpClasses
    {
        /// <summary>
        /// "SpTName" --> This class represents an instance the Supplier Portal 'Tables' definitions list. 
        /// Each instance is an item on the 'tbls' table on the Flexible Cloud DB.
        /// </summary>
        public class SpTName
        {
            private string tname; // table name of item on the 'tbls' list
            private string id; // automatic generated id string by the Flexible Cloud DB.

            /// <summary>
            /// "tName" --> Table name of item on the 'tbls' list. 
            /// </summary>
            public string tName
            {
                get { return tname; }
                set { tname = value; }
            }

            /// <summary>
            /// "_id" --> Automatic generated id string by the Flexible Cloud DB. 
            /// </summary>
            public string _id
            {
                get { return id; }
                set { id = value; }
            }
        }

        /// <summary>
        /// "SpTable" --> This class represents the Supplier Portal 'Table' itself which contains collection data from eFlow. 
        /// </summary>
        public class SpTable
        {
            private string _cid; // collection name from eFlow preceeded by a "c_" char, i.e. "c_00000323"
            private string strdata; // collection data from eFlow.
            private string _cln; // Collection name from eFlow preceeded by a "c_" char, i.e. "c_00000323" 
            private string id; // automatic generated id string by the Flexible Cloud DB.

            /// <summary>
            /// "cid" --> Collection name from eFlow preceeded by a "c_" char, i.e. "c_00000323"
            /// used internally by the Flexible Cloud DB.
            /// </summary>
            public string cid
            {
                get { return _cid; }
                set { _cid = value; }
            }

            /// <summary>
            /// "strData" --> Collection data from eFlow, i.e.
            /// "TEST1|SYSTEM|CLS|00000323|FreeProcess|1|Invoice_Date=|Invoice_Number=|Net_Amount1=|PO_Number=963645|Supplier_Name=|Total_Amount=0.00|VAT_Amount1=|VAT1=|speedyservices.com_CLS-00000323.tif"
            /// "machine_name|user_name|eFlowAppName|CollectionName|StationName|CollectionStatus|[FieldName1=[FieldValue1]...]"
            /// </summary>
            public string strData
            {
                get { return strdata; }
                set { strdata = value; }
            }

            /// <summary>
            /// "cln" --> Collection name from eFlow preceeded by a "c_" char, i.e. "c_00000323" 
            /// used by the Flexible Cloud DB and Supplier Portal SDK for querying
            /// </summary>
            public string cln
            {
                get { return _cln; }
                set { _cln = value; }
            }

            /// <summary>
            /// "_id" --> Automatic generated id string by the Flexible Cloud DB.
            /// </summary>
            public string _id
            {
                get { return id; }
                set { id = value; }
            }

            /// <summary>
            /// "eFlowCollectionName" --> Collection name from eFlow without the "c_" char required by the Flexible Cloud DB, 
            /// i.e. "00000323" 
            /// </summary>
            public string eFlowCollectionName
            {
                get { return cln.Replace("c_", ""); }
            }
        }

        /// <summary>
        /// "SpTableResponse" --> .NET representation of json response containing SpTable items. 
        /// </summary>
        public class SpTableResponse
        {
            private SpTable[] _result;

            /// <summary>
            /// "result" --> .NET representation of json response containing SpTable items. 
            /// </summary>
            public SpTable[] result
            {
                get { return _result; }
                set { _result = value; }
            }
        }

        /// <summary>
        /// "SpTNameResponse" --> .NET representation of json response containing SpTName items. 
        /// </summary>
        public class SpTNameResponse
        {
            private SpTName[] _result;

            /// <summary>
            /// "result" --> .NET representation of json response containing SpTName items. 
            /// </summary>
            public SpTName[] result
            {
                get { return _result; }
                set { _result = value; }
            }
        }

        /// <summary>
        /// "SpClassParser" --> Wrapper around the JSON.NET parser. 
        /// </summary>
        public class SpClassParser
        {
            /// <summary>
            /// "FromJson" --> Converts a json response to a .NET class instance.
            /// </summary>
            public static T FromJson<T>(string jsonResult)
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(jsonResult);
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.ToString());
                    return default(T);
                }
            }
        }

        /// <summary>
        /// "SpUtils" --> Utility class for the Supplier Portal SDK. 
        /// </summary>
        public class SpUtils
        {
            /// <summary>
            /// "GetFieldBatchInfo" --> Returns the Field Data of particular eFlow Field as a single piped string, which is the format required by the Supplier Portal and Flexible Cloud DB. 
            /// </summary>
            /// <param name="cd">ITisCollectionData instance with the collection's data</param>
            /// <param name="tmpUpFolder">Indicates the name of the temporary upload folder</param>
            /// <param name="appName">Indicates the eFlow App Name</param>
            /// <param name="stName">Indicates the eFlow station from where to get the field data from</param>
            /// <param name="bName">Indicates the eFlow Collection Name from where to get the field data from</param>
            /// <param name="fToCheck">Indicates which fields name to get data from</param>
            /// <param name="fn">Indicates the name of the TIFF file associated with the collection</param>
            /// <param name="origin">Indicates the location from where the collection originated from</param>
            /// <example><code>s.GetFieldBatchInfo(collectionData, "CLS", "FreeMatch", "00000323", "Invoice_Number|PO_Number|Total_Amount|Supplier_Name|Net_Amount1|VAT_Amount1|VAT1|Invoice_Date", out fn, out origin);</code></example>
            public static string[] GetFieldBatchInfo(ITisCollectionData cd, string tmpUpFolder, string appName, string stName, string bName, string fToCheck, out string fn, out string origin)
            {
                List<string> result = new List<string>();
                fn = String.Empty;
                origin = String.Empty;

                try
                {
                    fn = GetFieldBatchInfoCore(tmpUpFolder, appName, fToCheck, fn, result, cd, out origin);
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.ToString());
                }

                return result.ToArray();
            }
            
            /// <summary>
            /// "GetFieldBatchInfo" --> Returns the Field Data of particular eFlow Field as a single piped string, which is the format required by the Supplier Portal and Flexible Cloud DB. 
            /// </summary>
            /// <param name="tmpUpFolder">Indicates the name of the temporary upload folder</param>
            /// <param name="appName">Indicates the eFlow App Name</param>
            /// <param name="stName">Indicates the eFlow station from where to get the field data from</param>
            /// <param name="bName">Indicates the eFlow Collection Name from where to get the field data from</param>
            /// <param name="fToCheck">Indicates which fields name to get data from</param>
            /// <param name="fn">Indicates the name of the TIFF file associated with the collection</param>
            /// <param name="origin">Indicates the location from where the collection originated from</param>
            /// <example><code>s.GetFieldBatchInfo("CLS", "FreeMatch", "00000323", "Invoice_Number|PO_Number|Total_Amount|Supplier_Name|Net_Amount1|VAT_Amount1|VAT1|Invoice_Date", out fn, out origin);</code></example>
            public static string[] GetFieldBatchInfo(string tmpUpFolder, string appName, string stName, string bName, string fToCheck, out string fn, out string origin)
            {
                List<string> result = new List<string>();
                fn = String.Empty;
                origin = String.Empty;

                try
                {
                    using (Batch b = new Batch(appName, stName))
                    {
                        ITisCollectionData cd = b.Get(bName);

                        fn = GetFieldBatchInfoCore(tmpUpFolder, appName, fToCheck, fn, result, cd, out origin);

                        b.Free(cd);
                    }
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.ToString());
                }

                return result.ToArray();
            }

            private static string GetFieldBatchInfoCore(string tmpUpFolder, string appName, string fToCheck, string fn, List<string> result, 
                ITisCollectionData cd, out string origin)
            {
                string res = String.Empty;
                origin = String.Empty;

                try
                {
                    try
                    {
                        origin = cd.get_NamedUserTags(Constants.SupplierPortalDomainTag);
                    }
                    catch { }

                    if (origin == String.Empty)
                        origin = Constants.eFlowOrigin;
                    
                    foreach (ITisFormData fd in cd.Forms)
                    {
                        foreach (ITisFieldData fld in fd.Fields)
                        {
                            if (ExistsInPipeStr(true, fld.Name, fToCheck))
                            {
                                result.Add(fd.Name + "_" + fld.Name + "=" + fld.Contents.ToString());
                            }
                        }

                        foreach (ITisPageData pd in fd.LinkedPages)
                        {
                            string tfn = pd.GetAttachmentFileName(".tif");

                            fn = tfn.Substring(0, tfn.IndexOf("_")) + ".tif";

                            if (!Directory.Exists(tmpUpFolder))
                                Directory.CreateDirectory(tmpUpFolder);

                            string tmpFn = String.Empty;

                            if (File.Exists(fn))
                                tmpFn = Path.Combine(tmpUpFolder, Path.GetFileName(fn));
                            else
                            {
                                string origFn = Path.GetFileName(fn);
                                fn = Path.Combine(tmpUpFolder, getFileInDynamic(appName, origFn));
                                tmpFn = Path.Combine(tmpUpFolder, Path.GetFileName(origFn));
                            }

                            if (!File.Exists(tmpFn)) 
                                File.Copy(fn, tmpFn);

                            fn = tmpFn;

                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.ToString());
                }

                res = fn;

                return res;
            }

            public static string Reverse(string str)
            {
                char[] array = str.ToCharArray();
                Array.Reverse(array);
                return new string(array);
            }

            public static string[] ParseStrDataToFields(string strData)
            {
                List<string> res = new List<string>();

                try
                {
                    string[] tmp = FromPipe(strData);

                    if (tmp != null && tmp.Length > 0)
                    {
                        foreach (string tp in tmp)
                        {
                            if (tp.Contains("=") && !tp.Contains(Constants.cStrRc) && !tp.Contains(Constants.cStrDc))
                            {
                                res.Add(tp);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.ToString());
                }

                return res.ToArray();
            }

            public static string getFileInDynamic(string appName, string fn)
            {
                string result = String.Empty;
                bool breakAll = false;

                try
                {
                    RegistryKey regKey = Registry.LocalMachine.OpenSubKey(Constants.cStrDynamicReg);

                    if (regKey != null)
                    {
                        string loc = (string)regKey.GetValue(Constants.cStrConfigFilesLocation);

                        loc = loc.Replace(Constants.cStrConfiguration, "");

                        if (Directory.Exists(loc))
                        {
                            string[] dirs = Directory.GetDirectories(loc);

                            if (dirs != null && dirs.Length > 0)
                            {
                                foreach (string dir in dirs)
                                {
                                    if (dir.ToLower().Contains(Environment.MachineName.ToLower()) ||
                                        dir.ToLower() == Environment.MachineName.ToLower())
                                    {
                                        string dr = Path.Combine(Path.Combine(dir, appName), Constants.cStrDynamic);

                                        if (Directory.Exists(dr))
                                        {
                                            foreach (string f in Directory.GetFiles(dr, "*" + fn, SearchOption.AllDirectories))
                                            {
                                                result = f;
                                                breakAll = true;

                                                break;
                                            }
                                        }
                                    }

                                    if (breakAll)
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.ToString());
                }

                return result;
            }

            public static string EndsWith(string str, string end)
            {
                return (str[str.Length - 1].ToString() == end) ? str : str + end;
            }

            /// <summary>
            /// "GetExtendedFieldBatchInfo" --> Returns appended extended details for a given collection, such as: MachineName, UserName and TIFF file name. 
            /// </summary>
            /// <param name="origin">Indicate if the collection is originally from eFlow or it was sbumitted via the web (which domain)</param>
            /// <param name="fn">Out param from GetFieldBatchInfo - which indicates the name  of the TIFF file associated to the collection</param>
            /// <param name="creationTime">Indicates the collection's creationTime</param>
            /// <param name="fDomain">Indicates the HTTP path for the file to upload</param>
            /// <param name="fieldInfo">Basic Field info returned from GetFieldBatchInfo</param>
            /// <param name="batchName">Indicates the eFlow Collection Name related to this data</param>
            /// <param name="queueName">Indicates the eFlow station related to this data</param>
            /// <param name="status">Indicates the status of the eFlow Collection</param>
            /// <param name="cust">Indicates the name of the customer associated to this collection, i.e. topimagesystems.com</param>
            /// <param name="currentApp">Indicates the name of the eFlow App related to this data, i.e. CLS</param>
            /// <param name="bInfo">Result of the execution of this function, which contains the extended batch info</param>
            /// <param name="fnUp">Indicates the name of the TIFF file associated to this collection that's going to be given when uploaded to the Suppiler Portal</param>
            public static void GetExtendedFieldBatchInfo(string origin, string fn, string creationTime, string fDomain, string[] fieldInfo, string batchName, string queueName, string status, string cust, string currentApp, out string bInfo, out string fnUp)
            {
                string fInfo = ToPipe(fieldInfo);

                string basicInfo = Environment.MachineName + "|" + Environment.UserName + "|" +
                       currentApp + "|" + batchName + "|" + queueName + "|" + status;

                bInfo = (fInfo != String.Empty) ? basicInfo + "|" + fInfo : basicInfo;
                fnUp = (fn != String.Empty) ? EndsWith(fDomain, "/") + cust + "_" + currentApp + "-" + Path.GetFileName(fn) : "";

                if (origin == Constants.eFlowOrigin)
                    fnUp = fnUp.Replace(cust, Constants.allCustomers);

                if (queueName.ToUpper() == Constants.SPortalCompl.ToUpper())
                    bInfo += "|sb=" + origin + "|dc=" + creationTime + "|rc=1"; // requires online indexing on the portal
                else
                    bInfo += "|sb=" + origin + "|dc=" + creationTime + "|rc=0"; // does not require indexing on the portal

                bInfo = (fn != String.Empty) ? bInfo + "|" + fnUp : bInfo;
            }

            /// <summary>
            /// "FromPipe" --> Converts a piped string to a string array. 
            /// </summary>
            public static string[] FromPipe(string str)
            {
                return (str.Contains("|")) ? str.Split('|') : new string[] { str };
            }

            /// <summary>
            /// "ToPipe" --> Converts a string array to a piped string. 
            /// </summary>
            public static string ToPipe(string[] arr)
            {
                return (arr != null && arr.Length > 0) ? string.Join("|", arr) : "";
            }

            /// <summary>
            /// "ToSemiColon" --> Converts a string array to a semi-coloned string. 
            /// </summary>
            public static string ToSemiColon(string[] arr)
            {
                return (arr != null && arr.Length > 0) ? string.Join(";", arr) : "";
            }

            /// <summary>
            /// "ExistsInPipeStr" --> Checks if a string value is cointained within a piped string. 
            /// </summary>
            public static bool ExistsInPipeStr(bool contained, string str, string pipestr)
            {
                bool result = false;
                string[] arr = (pipestr.Contains("|")) ? pipestr.Split('|') : new string[] { pipestr };

                foreach (string item in arr)
                {
                    string nItem = item.Replace("]", "").Replace("[", "").Replace(Constants.SPortalImport, "").
                        Replace(Constants.SPortalAfter, "").Replace("(", "").Replace(")", "").Replace("=", "");
                    
                    if (contained)
                    {
                        if (str.ToLower().Contains(nItem.ToLower()))
                        {
                            result = true;
                            break;
                        }
                    }
                    else
                        if (nItem.ToLower() == str.ToLower())
                        {
                            result = true;
                            break;
                        }
                }

                return result;
            }
        }
    }
}
