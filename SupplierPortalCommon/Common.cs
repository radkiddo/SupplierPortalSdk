#region "about"

//
// eFLOW Supplier Portal SDK
// 2013 (c) - Top Image Systems (a project initiated by the UK branch)
//
// The purpose of this SDK is to communicate with the Supplier Portal for eFlow Invoices.
// Developed by: Eduardo Freitas
//

#endregion "about"

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using SupplierPortalCommon;
using System.Windows.Forms;
using eFlow.SupplierPortalCore;
using System.Data.SqlClient;
using eFlow.CollectionManagement;

using System.Reflection;
using System.Configuration;
using System.Globalization;
using System.Threading;

using Amazon.S3.Model;
using Amazon.S3;

using TiS.Core.Application;
using TiS.Core.Common;
using TiS.Core.Domain;
using TiS.Core.TisCommon;
using TiS.Core.Application.DataModel.Dynamic;
using TiS.Core.Application.Interfaces;

namespace SupplierPortalCommon
{
    public class Common
    {
        private static string[] settings = null;

        public static string GetSetting(string file, string settingname)
        {
            string result = String.Empty;

            try
            {
                if (settings == null)
                {
                    if (File.Exists(file))
                    {
                        settings = File.ReadAllLines(file);
                    }
                }

                if (settings != null && settings.Length > 0)
                {
                    foreach (string setting in settings)
                    {
                        if (setting.ToLower().Contains(settingname.ToLower()))
                        {
                            using (Sp p = new Sp())
                            {
                                result = p.ExtractStrInBetween(setting.Trim(), "<" + settingname + ">", "</" + settingname + ">");
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
        
        public static string GetSetting(string settingname)
        {
            string result = String.Empty;

            try
            {
                if (settings == null)
                {
                    if (File.Exists(Assembly.GetExecutingAssembly().Location + Constants.cStrDotConfig))
                    {
                        settings = File.ReadAllLines(Assembly.GetExecutingAssembly().Location + Constants.cStrDotConfig);
                    }
                }

                if (settings != null && settings.Length > 0)
                {
                    foreach (string setting in settings)
                    {
                        if (setting.ToLower().Contains(settingname.ToLower()))
                        {
                            using (Sp p = new Sp())
                            {
                                result = p.ExtractStrInBetween(setting.Trim(), "<" + settingname + ">", "</" + settingname + ">");
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

        public static void GetInterval(ref System.Timers.Timer t)
        {
            int interval = 2;

            if (File.Exists(Assembly.GetExecutingAssembly().Location + Constants.cStrDotConfig))
            {
                string tempInterval = GetSetting(Constants.cStrInterval);
                try
                {
                    if (tempInterval != String.Empty)
                        interval = Convert.ToInt32(tempInterval);
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.ToString());
                }
                finally
                {
                    interval *= 1000;

                    t.Interval = interval;
                    t.Start();
                }
            }
        }
        
        public static void GetInterval(ref System.Windows.Forms.Timer t)
        {
            int interval = 2;

            if (File.Exists(Assembly.GetExecutingAssembly().Location + Constants.cStrDotConfig))
            {
                string tempInterval = GetSetting(Constants.cStrInterval);
                try
                {
                    if (tempInterval != String.Empty)
                        interval = Convert.ToInt32(tempInterval);
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.ToString());
                }
                finally
                {
                    interval *= 1000;

                    t.Interval = interval;
                    t.Start();
                }
            }
        }

        public static void GetFromPortal()
        {
            try
            {
                if (File.Exists(Assembly.GetExecutingAssembly().Location + Constants.cStrDotConfig))
                {
                    string useS3 = GetSetting(CommonConst.useS3);

                    if (Convert.ToBoolean(useS3))
                        GetFromS3();
                    else
                        GetFromPortalFromFtp();
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        public static List<string> GetFilesFromS3Bucket(string bucket, ref AmazonS3 _Client)
        {
            List<string> files = new List<string>();

            try
            {
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = bucket;

                using(ListObjectsResponse response = _Client.ListObjects(request))
                {
                    foreach(S3Object entry in response.S3Objects)
                    {
                        files.Add(entry.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return files;
        }

        public static void GetFromS3()
        {
            try
            {
                List<string> apps = new List<string>();
                List<string> folders = new List<string>();
                
                if (File.Exists(Assembly.GetExecutingAssembly().Location + Constants.cStrDotConfig))
                {
                    if (GetSetting(CommonConst.app01ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app01ImportPath));

                    if (GetSetting(CommonConst.app02ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app02ImportPath));

                    if (GetSetting(CommonConst.app03ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app03ImportPath));

                    if (GetSetting(CommonConst.app04ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app04ImportPath));

                    if (GetSetting(CommonConst.app05ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app05ImportPath));

                    if (GetSetting(CommonConst.app06ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app06ImportPath));

                    if (GetSetting(CommonConst.app07ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app07ImportPath));

                    if (GetSetting(CommonConst.app08ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app08ImportPath));

                    if (GetSetting(CommonConst.app09ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app09ImportPath));

                    if (GetSetting(CommonConst.app10ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app10ImportPath));

                    if (GetSetting(CommonConst.app01WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app01WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app02WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app02WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app03WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app03WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app04WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app04WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app05WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app05WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app06WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app06WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app07WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app07WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app08WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app08WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app09WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app09WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app10WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app10WhoCanSeeInPortal));
                    
                    string accessKey = GetSetting(CommonConst.s3AccessKey);
                    string secretKey = GetSetting(CommonConst.s3SecretKey);
                    string s3Bucket = GetSetting(CommonConst.s3Bucket);

                    using (Sp p = new Sp())
                    {
                        if (accessKey != String.Empty && secretKey != String.Empty && s3Bucket != String.Empty)
                        {
                            AmazonS3 client = null;

                            DeleteObjectsRequest multiObjectDeleteRequest = new DeleteObjectsRequest();
                            multiObjectDeleteRequest.BucketName = s3Bucket;

                            using (client = Amazon.AWSClientFactory.CreateAmazonS3Client(accessKey, secretKey))
                            {
                                List<string> files = GetFilesFromS3Bucket(s3Bucket, ref client);

                                foreach (string keyName in files)
                                {
                                    string appName = p.ExtractStrInBetween(keyName, "_", "-");
                                    string domainName = keyName.Substring(0, keyName.IndexOf("_"));
                                    string supplierId = p.ExtractStrInBetween(keyName, "-", "__");
                                    string poNum = p.ExtractStrInBetween(keyName, "__", "___");
                                    string ext = Path.GetExtension(keyName);
                                    string fn = p.ExtractStrInBetween(keyName, "___", ext);

                                    string destFolder = GetDestFolder(folders.ToArray(), apps.ToArray(), appName, domainName);

                                    GetObjectRequest request = new GetObjectRequest().WithBucketName(s3Bucket).WithKey(keyName);

                                    using (GetObjectResponse response = client.GetObject(request))
                                    {
                                        string dest = Path.Combine(destFolder, fn + ext);
                                        string destXml = Path.Combine(destFolder, fn + ".xml");

                                        response.WriteResponseStreamToFile(dest, false);

                                        CreateSupplierXml(destXml, appName, domainName, supplierId, poNum, dest);
                                    }

                                    multiObjectDeleteRequest.AddKey(keyName);
                                    DeleteObjectsResponse rsp = client.DeleteObjects(multiObjectDeleteRequest);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        private static void CreateSupplierXml(string destXml, string appName, string domainName, string supplierId, string poNum, string dest)
        {
            try
            {
                //2013-06-18 09:12:59
                DateTime dt = new DateTime();
                string dtTime = dt.Year.ToString() + "-" + dt.Month.ToString().PadLeft(2, '0') + "-" + dt.Day.ToString().PadLeft(2, '0') + 
                    " " + dt.Hour.ToString().PadLeft(2, '0') + ":" + dt.Minute.ToString().PadLeft(2, '0') + ":" +
                    dt.Second.ToString().PadLeft(2, '0');
                
                List<string> lines = new List<string>();

                lines.Add(CommonConst.xmlHeader);
                lines.Add(CommonConst.XmlSTag);
                lines.Add(CommonConst.SupplierInfoSXmlTag);
                lines.Add(CommonConst.eFlowAppNameSXmlTag + appName + CommonConst.eFlowAppNameEXmlTag);
                lines.Add(CommonConst.DomainNameSXmlTag + domainName + CommonConst.DomainNameEXmlTag);
                lines.Add(CommonConst.SupplierIdSXmlTag + supplierId + CommonConst.SupplierIdEXmlTag);
                lines.Add(CommonConst.PONumberSXmlTag + poNum + CommonConst.PONumberEXmlTag);
                lines.Add(CommonConst.FileSXmlTag + dest + CommonConst.FileEXmlTag);
                lines.Add(CommonConst.DateTimeSXmlTag + dtTime + CommonConst.DateTimeEXmlTag);
                lines.Add(CommonConst.SupplierInfoEXmlTag);
                lines.Add(CommonConst.XmlETag);

                File.WriteAllLines(destXml, lines.ToArray());
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        private static string GetDestFolder(string[] folders, string[] apps, string appName, string domainName)
        {
            string result = String.Empty;
            bool found = false;
            
            try
            {
                int index = 0;
                foreach (string app in apps)
                {
                    if (app.ToLower().Contains(appName.ToLower()))
                    {
                        string dmn = app.Substring(app.IndexOf("=") + 1);
                        string[] dmns = dmn.Split('|');

                        if (dmns.Length > 0)
                        {
                            foreach (string dn in dmns)
                            {
                                if (dn.ToLower() == domainName.ToLower())
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (found)
                        {
                            result = folders[index];
                            break;
                        }
                    }

                    index++;
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        public static void GetFromPortalFromFtp()
        {
            try
            {
                List<string> folders = new List<string>();
                List<string> apps = new List<string>();

                if (File.Exists(Assembly.GetExecutingAssembly().Location + Constants.cStrDotConfig))
                {
                    if (GetSetting(CommonConst.app01ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app01ImportPath));

                    if (GetSetting(CommonConst.app02ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app02ImportPath));

                    if (GetSetting(CommonConst.app03ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app03ImportPath));

                    if (GetSetting(CommonConst.app04ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app04ImportPath));

                    if (GetSetting(CommonConst.app05ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app05ImportPath));

                    if (GetSetting(CommonConst.app06ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app06ImportPath));

                    if (GetSetting(CommonConst.app07ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app07ImportPath));

                    if (GetSetting(CommonConst.app08ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app08ImportPath));

                    if (GetSetting(CommonConst.app09ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app09ImportPath));

                    if (GetSetting(CommonConst.app10ImportPath) != String.Empty)
                        folders.Add(GetSetting(CommonConst.app10ImportPath));

                    if (GetSetting(CommonConst.app01WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app01WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app02WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app02WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app03WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app03WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app04WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app04WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app05WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app05WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app06WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app06WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app07WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app07WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app08WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app08WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app09WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app09WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.app10WhoCanSeeInPortal) != String.Empty)
                        apps.Add(GetSetting(CommonConst.app10WhoCanSeeInPortal));

                    if (GetSetting(CommonConst.imgReceive) != String.Empty &&
                        folders.Count > 0 && apps.Count > 0 && folders.Count == apps.Count)
                    {
                        using (Sp p = new Sp())
                        {
                            p.FtpDownload(GetSetting(CommonConst.imgReceive), folders.ToArray(), apps.ToArray(),
                                GetSetting(CommonConst.portalFilesRouteKey),
                                GetSetting(CommonConst.imgUserName),
                                GetSetting(CommonConst.imgPwd));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        public static bool CleanImagesWithNoCollections(string cust, string app, string fn, string fnUp)
        {
            bool result = false;

            try
            {
                using (Sp p = new Sp())
                {
                    result = p.FtpCleanupImageWithNoCollections(cust, app, fn, fnUp, GetSetting(CommonConst.imgFolder),
                        GetSetting(CommonConst.portalFilesRouteKey),
                        GetSetting(CommonConst.imgUserName),
                        GetSetting(CommonConst.imgPwd));
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        public static bool SendFileWebRepository(string cust, string app, string fn, string fnUp)
        {
            bool result = false;

            try
            {
                using (Sp p = new Sp())
                {
                    result = p.FtpSendFile(cust, app, fn, fnUp, GetSetting(CommonConst.imgFolder),
                        GetSetting(CommonConst.portalFilesRouteKey),
                        GetSetting(CommonConst.imgUserName),
                        GetSetting(CommonConst.imgPwd));
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        public static void GetFlexibleDbParams(out string domain, out int port, out bool https)
        {
            domain = String.Empty;
            port = 80;
            https = false;

            if (File.Exists(Assembly.GetExecutingAssembly().Location + Constants.cStrDotConfig))
            {
                string dm = GetSetting(CommonConst.flexibleDb);
                int prt = Convert.ToInt32(GetSetting(CommonConst.flexibleDbPort));
                bool sec = Convert.ToBoolean(GetSetting(CommonConst.flexibleDbHttps));

                if (prt > -1) port = prt;
                https = sec;

                if (dm != String.Empty) domain = dm;
            }
        }

        public static string SendToPortal(string customer, string appName, string batchName, string cd)
        {
            string res = String.Empty;

            try
            {
                using (Sp p = new Sp())
                {
                    string domain = String.Empty;
                    int port = 80;
                    bool https = false;

                    GetFlexibleDbParams(out domain, out port, out https);

                    if (domain != String.Empty) p.SetDomain(domain);

                    if (!p.HttpPostCollectionQry(customer, appName, Constants.cStrHttpPostCollectionDatacollectionNameQryCln,
                        batchName, port, https).ToLower().Contains(Constants.cStrEmptyJsonResponse))
                    {
                        p.HttpPostCollectionDeleteAll(customer, appName, batchName, port, https);
                        p.HttpPostCollectionDeleteAll(customer, appName, batchName, port, https);
                    }

                    res = p.HttpPostSimple(customer, appName, batchName, cd, port, https);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return res;
        }

        private const string cStrDateFormat = "dd-MMM-yyyy";
        private const string cStrTimeFormat = "HH:mm:ss";

        private static void LoadCulture()
        {
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            ci.DateTimeFormat.ShortDatePattern = cStrDateFormat;
            ci.DateTimeFormat.ShortTimePattern = cStrTimeFormat;
            ci.DateTimeFormat.FullDateTimePattern = cStrDateFormat;
            ci.DateTimeFormat.LongDatePattern = cStrDateFormat;
            ci.DateTimeFormat.LongTimePattern = cStrTimeFormat;
        }

        private static void GetBasicBatchInfo(SqlDataReader rdr, out string creationTime, out string batchName, out string queueName, out string status)
        {
            LoadCulture();
            
            DateTime ct = (DateTime)rdr.GetValue(0);
            creationTime = ct.DayOfWeek.ToString() + " " + ct.ToString(); 

            batchName = (rdr.GetValue(1) as string).Trim();
            queueName = (rdr.GetValue(2) as string).Trim();
            int st = (int)rdr.GetValue(3);
            status = st.ToString();
        }

        public static string GetCreationDate(DateTime cd)
        {
            LoadCulture();
            return cd.DayOfWeek.ToString() + " " + cd.ToString(); 
        }

        public static void ServerCollectionsCleanUp(string selectString)
        {
            try
            {
                List<string> dbConn = new List<string>();
                List<string> allowed = new List<string>();

                if (File.Exists(Assembly.GetExecutingAssembly().Location + Constants.cStrDotConfig))
                {
                    dbConn.Add(GetSetting(CommonConst.connectionStr01));
                    dbConn.Add(GetSetting(CommonConst.connectionStr02));
                    dbConn.Add(GetSetting(CommonConst.connectionStr03));
                    dbConn.Add(GetSetting(CommonConst.connectionStr04));
                    dbConn.Add(GetSetting(CommonConst.connectionStr05));
                    dbConn.Add(GetSetting(CommonConst.connectionStr06));
                    dbConn.Add(GetSetting(CommonConst.connectionStr07));
                    dbConn.Add(GetSetting(CommonConst.connectionStr08));
                    dbConn.Add(GetSetting(CommonConst.connectionStr09));
                    dbConn.Add(GetSetting(CommonConst.connectionStr10));

                    allowed.Add(GetSetting(CommonConst.app01WhoCanSeeInPortal));
                    allowed.Add(GetSetting(CommonConst.app02WhoCanSeeInPortal));
                    allowed.Add(GetSetting(CommonConst.app03WhoCanSeeInPortal));
                    allowed.Add(GetSetting(CommonConst.app04WhoCanSeeInPortal));
                    allowed.Add(GetSetting(CommonConst.app05WhoCanSeeInPortal));
                    allowed.Add(GetSetting(CommonConst.app06WhoCanSeeInPortal));
                    allowed.Add(GetSetting(CommonConst.app07WhoCanSeeInPortal));
                    allowed.Add(GetSetting(CommonConst.app08WhoCanSeeInPortal));
                    allowed.Add(GetSetting(CommonConst.app09WhoCanSeeInPortal));
                    allowed.Add(GetSetting(CommonConst.app10WhoCanSeeInPortal));

                    int index = -1;
                    string[] allw = allowed.ToArray();

                    foreach (string itm in dbConn)
                    {
                        index++;

                        if (itm != String.Empty && allw[index] != String.Empty)
                        {
                            string DB_CONN_STRING = itm;

                            using (SqlConnection mySqlConnection = new SqlConnection(DB_CONN_STRING))
                            {
                                List<string> existingBatches = new List<string>();

                                using (SqlCommand mySqlCommand = new SqlCommand(selectString))
                                {
                                    mySqlCommand.Connection = mySqlConnection;

                                    mySqlConnection.Open();
                                    using (SqlDataReader rdr = mySqlCommand.ExecuteReader())
                                    {
                                        if (rdr.HasRows)
                                        {
                                            while (rdr.Read())
                                            {
                                                string str = (rdr.GetValue(1) as string).Trim();
                                                existingBatches.Add(str);
                                            }

                                            rdr.Close();
                                        }
                                        else
                                            rdr.Close();
                                    }
                                }

                                mySqlConnection.Close();

                                if (existingBatches.Count >= 0)
                                {
                                    if (allw[index].Contains("_") && allw[index].Contains("="))
                                    {
                                        string appName = allw[index].Substring(0, allw[index].IndexOf("_"));
                                        string domainStr = allw[index].Substring(allw[index].IndexOf("=") + 1);

                                        string[] domains = (domainStr.Contains("|")) ? domainStr.Split('|') : new string[] { domainStr };

                                        if (domains.Length > 0)
                                        {
                                            foreach (string dm in domains)
                                            {
                                                DoCollectionCleanUp(dm, appName, existingBatches.ToArray());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        private static void DoCollectionCleanUp(string customer, string appName, string[] existingBatches)
        {
            try
            {
                using (Sp p = new Sp())
                {
                    string domain = String.Empty;
                    int port = 80;
                    bool https = false;

                    GetFlexibleDbParams(out domain, out port, out https);

                    if (domain != String.Empty) p.SetDomain(domain);

                    string res = p.HttpGetCollectionsData(customer, appName, port, https);

                    if (!res.ToLower().Contains(Constants.cStrEmptyJsonResponse))
                    {
                        SpClasses.SpTableResponse tbls = SpClasses.SpClassParser.FromJson<SpClasses.SpTableResponse>(res);

                        try
                        {
                            if (tbls != null && tbls.result != null && tbls.result.Length > 0)
                            {
                                foreach (SpClasses.SpTable tbl in tbls.result)
                                {
                                    if (!Array.Exists<string>(existingBatches, delegate(string s) { return s == tbl.eFlowCollectionName; }))
                                    {
                                        p.HttpPostCollectionDeleteAll(customer, appName, tbl.eFlowCollectionName, port, https);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.WriteLog(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        public static bool ExistsInExcludedStations(string station, string excluded)
        {
            bool result = false;

            try
            {
                string[] arr = (excluded.Contains("|")) ? excluded.Split('|') : new string[] { excluded };

                if (arr != null && arr.Length > 0)
                {
                    foreach (string item in arr)
                    {
                        string nItem = item.Replace("]", "").Replace("[", "").Replace(CommonConst.SPortalImport, "").
                            Replace(CommonConst.SPortalAfter, "");
                        
                        if (station.ToLower() == nItem.ToLower())
                        {
                            result = true;
                            break;
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

        private static string SendSupplierPOsInfoToPortal(string[] items)
        {
            string res = String.Empty;
            
            try
            {
                string[] allowedApps = SpClasses.SpUtils.FromPipe(GetSetting(CommonConst.refDb_AllowedApps));
                string[] allowedDomains = SpClasses.SpUtils.FromPipe(GetSetting(CommonConst.refDb_AllowedDomains));

                using (Sp p = new Sp())
                {
                    string domain = String.Empty;
                    int port = 80;
                    bool https = false;

                    GetFlexibleDbParams(out domain, out port, out https);

                    if (domain != String.Empty) p.SetDomain(domain);

                    foreach (string item in items)
                    {
                        for (int i = 0; i <= allowedApps.Length - 1; i++)
                        {
                            string app = allowedApps[i] + Constants.POsTableSuffix;
                            string customer = allowedDomains[i];
                            string itemId = item.Substring(0, item.IndexOf("="));
                            string cd = item;

                            string exists = p.HttpPostCollectionQry(customer, app,
                                Constants.cStrHttpPostCollectionDatacollectionNameQryCln,
                                itemId, port, https);

                            string strData = p.ExtractStrInBetween(exists, "\"strData\": \"", "cln").
                                Replace("\"", "").Replace("\"", "").Replace(",", "").Trim();

                            if (!exists.ToLower().Contains(Constants.cStrEmptyJsonResponse))
                            {
                                if (strData != String.Empty)
                                {
                                    if (strData.ToLower() != cd.ToLower())
                                    {
                                        p.HttpPostCollectionDeleteAll(customer, app, itemId, port, https);

                                        res = p.HttpPostSimple(customer, app, itemId, cd, port, https);
                                    }
                                }
                            }
                            else
                                res = p.HttpPostSimple(customer, app, itemId, cd, port, https);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return res;
        }

        private static string SendSupplierInfoToPortal(string[] items)
        {
            string res = String.Empty;
            
            try
            {
                string[] allowedApps = SpClasses.SpUtils.FromPipe(GetSetting(CommonConst.refDb_AllowedApps));
                string[] allowedDomains = SpClasses.SpUtils.FromPipe(GetSetting(CommonConst.refDb_AllowedDomains));

                using (Sp p = new Sp())
                {
                    string domain = String.Empty;
                    int port = 80;
                    bool https = false;

                    GetFlexibleDbParams(out domain, out port, out https);

                    if (domain != String.Empty) p.SetDomain(domain);

                    foreach (string item in items)
                    {
                        for (int i = 0; i <= allowedApps.Length - 1; i++)
                        {
                            string app = allowedApps[i] + Constants.SupplierDataTableSuffix;
                            string customer = allowedDomains[i];
                            string itemId = p.ExtractStrInBetween(item, "id=", "|");
                            string cd = item;

                            string exists = p.HttpPostCollectionQry(customer, app,
                                Constants.cStrHttpPostCollectionDatacollectionNameQryCln,
                                itemId, port, https);

                            string strData = p.ExtractStrInBetween(exists, "\"strData\": \"", "cln").
                                Replace("\"", "").Replace("\"", "").Replace(",", "").Trim();

                            if (!exists.ToLower().Contains(Constants.cStrEmptyJsonResponse))
                            {
                                if (strData != String.Empty)
                                {
                                    if (strData.ToLower() != cd.ToLower())
                                    {
                                        p.HttpPostCollectionDeleteAll(customer, app, itemId, port, https);

                                        res = p.HttpPostSimple(customer, app, itemId, cd, port, https);
                                    }
                                }
                            }
                            else
                                res = p.HttpPostSimple(customer, app, itemId, cd, port, https);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return res;
        }

        private static void GetCustomerApp(string tmpCust, out string customer, out string app)
        {
            app = String.Empty;
            customer = String.Empty;
            
            try
            {
                List<string> apps = new List<string>();

                apps.Add(GetSetting(CommonConst.app01WhoCanSeeInPortal));
                apps.Add(GetSetting(CommonConst.app02WhoCanSeeInPortal));
                apps.Add(GetSetting(CommonConst.app03WhoCanSeeInPortal));
                apps.Add(GetSetting(CommonConst.app04WhoCanSeeInPortal));
                apps.Add(GetSetting(CommonConst.app05WhoCanSeeInPortal));
                apps.Add(GetSetting(CommonConst.app06WhoCanSeeInPortal));
                apps.Add(GetSetting(CommonConst.app07WhoCanSeeInPortal));
                apps.Add(GetSetting(CommonConst.app08WhoCanSeeInPortal));
                apps.Add(GetSetting(CommonConst.app09WhoCanSeeInPortal));
                apps.Add(GetSetting(CommonConst.app10WhoCanSeeInPortal));

                if (apps.Count > 0)
                {
                    foreach (string ap in apps)
                    {
                        if (ap.Contains(tmpCust) && ap.Contains("=") && ap.Contains("_"))
                        {
                            customer = tmpCust;
                            app = ap.Substring(0, ap.IndexOf("_"));

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        //<refDb_AllowedApps>IRSupplierPortal|IRSupplierPortal</refDb_AllowedApps>
        //<refDb_AllowedDomains>topimagesystems.com|doksend.com</refDb_AllowedDomains>
        public static void ClearSupplierUser2SupplierIds()
        {
            try
            {
                string appStr = GetSetting(CommonConst.refDb_AllowedApps);
                string dmnStr = GetSetting(CommonConst.refDb_AllowedDomains);

                string[] apps = (appStr.Contains("|")) ? appStr.Split('|') : new string[] { appStr };
                string[] dms = (dmnStr.Contains("|")) ? dmnStr.Split('|') : new string[] { dmnStr };

                if (apps != null && apps.Length > 0 && dms != null && dms.Length > 0)
                {
                    using (Sp p = new Sp())
                    {
                        string domain = String.Empty;
                        int port = 80;
                        bool https = false;

                        GetFlexibleDbParams(out domain, out port, out https);

                        if (domain != String.Empty) p.SetDomain(domain);

                        int index = 0;
                        foreach (string app in apps)
                        {
                            // Example: doksend.com_IRSupplierPortal__SupplierIdsPerUser
                            string tbl = dms[index] + "_" + app + CommonConst.__SupplierIdsPerUser;

                            p.HttpDeleteAllRowsFromTable(tbl, port, https);

                            index++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        private static string SendSupplierUser2SupplierIds(string[] items)
        {
            string res = String.Empty;
            
            try
            {
                if (items != null && items.Length > 0)
                {
                    using (Sp p = new Sp())
                    {
                        string customer = String.Empty;
                        string app = String.Empty;

                        string domain = String.Empty;
                        int port = 80;
                        bool https = false;

                        GetFlexibleDbParams(out domain, out port, out https);

                        if (domain != String.Empty) p.SetDomain(domain);

                        foreach (string itm in items)
                        {
                            if (itm.Contains("=") && itm.Contains("@"))
                            {
                                string itemId = itm.Substring(0, itm.IndexOf("="));
                                string tmpCust = itemId.Substring(itemId.IndexOf("@") + 1);

                                GetCustomerApp(tmpCust, out customer, out app);

                                string cd = itm;
                                app += Constants.supplierUser2supplierIdsSuffix;

                                res = p.HttpPostSimple(customer, app, itemId, cd, port, https);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return res;
        }

        public static void SupplierUser2SupplierIds()
        {
            try
            {
                List<string> s = new List<string>();
                List<string> items = new List<string>();

                s.Add(GetSetting(CommonConst.supplierUser2supplierIds1));
                s.Add(GetSetting(CommonConst.supplierUser2supplierIds2));
                s.Add(GetSetting(CommonConst.supplierUser2supplierIds3));
                s.Add(GetSetting(CommonConst.supplierUser2supplierIds4));
                s.Add(GetSetting(CommonConst.supplierUser2supplierIds5));
                s.Add(GetSetting(CommonConst.supplierUser2supplierIds6));
                s.Add(GetSetting(CommonConst.supplierUser2supplierIds7));
                s.Add(GetSetting(CommonConst.supplierUser2supplierIds8));
                s.Add(GetSetting(CommonConst.supplierUser2supplierIds9));
                s.Add(GetSetting(CommonConst.supplierUser2supplierIds10));

                foreach (string str in s)
                {
                    if (str != String.Empty)
                    {
                        if (str.Contains("=") && str.Contains("@"))
                        {
                            string[] temp = SpClasses.SpUtils.FromPipe(str);

                            foreach (string t in temp)
                            {
                                items.Add(t);
                            }
                        }
                    }
                }

                if (items != null && items.Count > 0)
                {
                    SendSupplierUser2SupplierIds(items.ToArray());
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        public static void SyncValidationsFiles()
        {
            try
            {
                List<string> s = new List<string>();

                s.Add(GetSetting(CommonConst.offlineFieldValidationFile1));
                s.Add(GetSetting(CommonConst.offlineFieldValidationFile2));
                s.Add(GetSetting(CommonConst.offlineFieldValidationFile3));
                s.Add(GetSetting(CommonConst.offlineFieldValidationFile4));
                s.Add(GetSetting(CommonConst.offlineFieldValidationFile5));
                s.Add(GetSetting(CommonConst.offlineFieldValidationFile6));
                s.Add(GetSetting(CommonConst.offlineFieldValidationFile7));
                s.Add(GetSetting(CommonConst.offlineFieldValidationFile8));
                s.Add(GetSetting(CommonConst.offlineFieldValidationFile9));
                s.Add(GetSetting(CommonConst.offlineFieldValidationFile10));

                string folder = GetSetting(CommonConst.onlineFieldValidationFolder);
                string hostname = GetSetting(CommonConst.portalFilesRouteKey);
                string username = GetSetting(CommonConst.imgUserName);
                string pwd = GetSetting(CommonConst.imgPwd);

                using (Sp p = new Sp())
                {
                    foreach (string file in s.ToArray())
                    {
                        if (file != String.Empty)
                        {
                            p.FtpSendSimpleFile(file, Path.GetFileName(file), folder, hostname, username, pwd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        public static void SyncValidations()
        {
            try
            {
                SyncValidationsFiles();
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        public static bool IsAdmin(string cust, string[] customers)
        {
            bool result = false;

            try
            {
                int i = 0;
                foreach (string c in customers)
                {
                    if (c.ToLower() == cust.ToLower() && i == 0)
                    {
                        result = true;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        public static void RefDbFetch()
        {
            try
            {
                List<string> supList = new List<string>();
                List<string> sList = new List<string>();

                string refDbStr = GetSetting(CommonConst.refDb);

                using (SqlConnection mySqlConnection = new SqlConnection(refDbStr))
                {
                    string supplierSelect = "SELECT " + GetSetting(CommonConst.refDb_SupplierId) + ", " + 
                        GetSetting(CommonConst.refDb_SupplierName) + ", " + 
                        GetSetting(CommonConst.refDb_SupplierStreet) + ", " + 
                        GetSetting(CommonConst.refDb_SupplierZIP) + ", " + 
                        GetSetting(CommonConst.refDb_SupplierCity) + ", " +
                        GetSetting(CommonConst.refDb_SupplierLand) + ", " + 

                        GetSetting(CommonConst.refDb_SupplierVATID) + ", " + 
                        GetSetting(CommonConst.refDb_SupplierTelephone) + ", " +
                        GetSetting(CommonConst.refDb_SupplierFaxnumber) + ", " + 
                        GetSetting(CommonConst.refDb_SupplierEmail) + ", " + 
                        GetSetting(CommonConst.refDb_SupplierBranchcode) + ", " +
                        GetSetting(CommonConst.refDb_SupplierAccountNo) + ", " +
                        GetSetting(CommonConst.refDb_SupplierIBAN) + ", " + 
                        GetSetting(CommonConst.refDb_SupplierSWIFT) + ", " + 
                        GetSetting(CommonConst.refDb_SupplierBUKRS) + 

                        " FROM " + GetSetting(CommonConst.refDb_SupplierTable);

                    using (SqlCommand mySqlCommand = new SqlCommand(supplierSelect))
                    {
                        mySqlCommand.Connection = mySqlConnection;

                        mySqlConnection.Open();
                        
                        using (SqlDataReader rdr = mySqlCommand.ExecuteReader())
                        {
                            if (rdr.HasRows)
                            {
                                while (rdr.Read())
                                {
                                    string supplierId = (rdr.GetValue(0) as string).Trim();
                                    string supplierName = (rdr.GetValue(1) as string).Trim();
                                    string supplierStreet = (rdr.GetValue(2) as string).Trim();
                                    string supplierZip = (rdr.GetValue(3) as string).Trim();
                                    string supplierCity = (rdr.GetValue(4) as string).Trim();
                                    string supplierLand = (rdr.GetValue(5) as string).Trim();

                                    string supplierVATID = (rdr.GetValue(6) as string).Trim();
                                    string supplierTelephone = (rdr.GetValue(7) as string).Trim();
                                    string supplierFaxnumber = (rdr.GetValue(8) as string).Trim();
                                    string supplierEmail = (rdr.GetValue(9) as string).Trim();
                                    string supplierBranchcode = (rdr.GetValue(10) as string).Trim();
                                    string supplierAccountNo = (rdr.GetValue(11) as string).Trim();
                                    string supplierIBAN = (rdr.GetValue(12) as string).Trim();
                                    string supplierSWIFT = (rdr.GetValue(13) as string).Trim();
                                    string supplierBURKS = (rdr.GetValue(14) as string).Trim();

                                    supList.Add("id=" + supplierId + "|" + "name=" + supplierName + "|" + "street=" + supplierStreet + "|" +
                                        "zip=" + supplierZip + "|" + "city=" + supplierCity + "|" + "land=" + supplierLand + "|" + 
                                        "vatid=" + supplierVATID + "|" + "phone=" + supplierTelephone + "|" + 
                                        "fax=" + supplierFaxnumber + "|" + "email=" + supplierEmail + "|" + 
                                        "branch=" + supplierBranchcode + "|" + "account=" + supplierAccountNo + "|" + 
                                        "iban=" + supplierIBAN + "|" + "swift=" + supplierSWIFT + "|" + 
                                        "burks=" + supplierBURKS);

                                    sList.Add(supplierId);
                                }

                                rdr.Close();
                            }
                            else
                                rdr.Close();
                        }

                        mySqlConnection.Close();
                    }

                    if (supList.Count > 0)
                    {
                        SendSupplierInfoToPortal(supList.ToArray());

                        List<string> supPOs = new List<string>();

                        foreach (string sup in sList)
                        {
                            string poSelect = "SELECT " + GetSetting(CommonConst.refDb_SupplierPOsPONumber) + " FROM " +
                                GetSetting(CommonConst.refDb_SupplierPOsTable) + " WHERE (" +
                                GetSetting(CommonConst.refDb_SupplierPOsSupplierId) + " = '" + sup + "')";

                            using (SqlCommand mySqlCommand = new SqlCommand(poSelect))
                            {
                                mySqlCommand.Connection = mySqlConnection;

                                mySqlConnection.Open();

                                using (SqlDataReader rdr = mySqlCommand.ExecuteReader())
                                {
                                    if (rdr.HasRows)
                                    {
                                        string poStr = String.Empty;
                                        
                                        while (rdr.Read())
                                        {
                                            string poNum = (rdr.GetValue(0) as string).Trim();

                                            poStr += poNum + "|";
                                        }

                                        if (poStr != String.Empty)
                                        {
                                            if (poStr[poStr.Length - 1] == '|') 
                                                poStr = poStr.Substring(0, poStr.Length - 1);

                                            supPOs.Add(sup + "=" + poStr);
                                        }

                                        rdr.Close();
                                    }
                                    else
                                        rdr.Close();
                                }

                                mySqlConnection.Close();
                            }

                        }

                        if (supPOs.Count > 0)
                        {
                            SendSupplierPOsInfoToPortal(supPOs.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        public static void CleanUnusedImages()
        {
            try
            {
                List<string> dbConn = new List<string>();
                List<string> appNames = new List<string>();
                List<string> allowed = new List<string>();

                if (File.Exists(Assembly.GetExecutingAssembly().Location + Constants.cStrDotConfig))
                {
                    dbConn.Add(GetSetting(CommonConst.connectionStr01));
                    dbConn.Add(GetSetting(CommonConst.connectionStr02));
                    dbConn.Add(GetSetting(CommonConst.connectionStr03));
                    dbConn.Add(GetSetting(CommonConst.connectionStr04));
                    dbConn.Add(GetSetting(CommonConst.connectionStr05));
                    dbConn.Add(GetSetting(CommonConst.connectionStr06));
                    dbConn.Add(GetSetting(CommonConst.connectionStr07));
                    dbConn.Add(GetSetting(CommonConst.connectionStr08));
                    dbConn.Add(GetSetting(CommonConst.connectionStr09));
                    dbConn.Add(GetSetting(CommonConst.connectionStr10));

                    allowed = GetCustomers(ref appNames);
                }

                int index = -1;
                string[] apps = appNames.ToArray(), allw = allowed.ToArray();

                foreach (string itm in dbConn)
                {
                    index++;

                    if (itm != String.Empty && apps[index] != String.Empty && allw[index] != String.Empty)
                    {
                        string[] customers = SpClasses.SpUtils.FromPipe(allw[index]);

                        foreach (string cust in customers)
                        {
                            if (IsAdmin(cust, customers))
                            {
                                CleanImagesWithNoCollections(cust, apps[index], String.Empty, String.Empty);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        public static void ClaimGarbage()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.WaitForFullGCComplete();
                GC.Collect();
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        public static void ExecuteMonitor(string selectString)
        {
            try
            {
                List<string> dbConn = new List<string>();
                List<string> appNames = new List<string>();
                List<string> allowed = new List<string>();

                if (File.Exists(Assembly.GetExecutingAssembly().Location + Constants.cStrDotConfig))
                {
                    dbConn.Add(GetSetting(CommonConst.connectionStr01));
                    dbConn.Add(GetSetting(CommonConst.connectionStr02));
                    dbConn.Add(GetSetting(CommonConst.connectionStr03));
                    dbConn.Add(GetSetting(CommonConst.connectionStr04));
                    dbConn.Add(GetSetting(CommonConst.connectionStr05));
                    dbConn.Add(GetSetting(CommonConst.connectionStr06));
                    dbConn.Add(GetSetting(CommonConst.connectionStr07));
                    dbConn.Add(GetSetting(CommonConst.connectionStr08));
                    dbConn.Add(GetSetting(CommonConst.connectionStr09));
                    dbConn.Add(GetSetting(CommonConst.connectionStr10));

                    allowed = GetCustomers(ref appNames);
                }

                int index = -1;
                string[] apps = appNames.ToArray(), allw = allowed.ToArray();

                foreach (string itm in dbConn)
                {
                    index++;

                    string excluded = GetSetting(CommonConst.excluded + (index + 1).ToString());

                    if (itm != String.Empty && apps[index] != String.Empty && allw[index] != String.Empty)
                    {
                        string DB_CONN_STRING = itm;

                        using (SqlConnection mySqlConnection = new SqlConnection(DB_CONN_STRING))
                        {
                            using (SqlCommand mySqlCommand = new SqlCommand(selectString))
                            {
                                mySqlCommand.Connection = mySqlConnection;

                                mySqlConnection.Open();
                                using (SqlDataReader rdr = mySqlCommand.ExecuteReader())
                                {
                                    if (rdr.HasRows)
                                    {
                                        while (rdr.Read())
                                        {
                                            string origin = String.Empty;
                                            string ct = String.Empty;
                                            string fn = String.Empty;
                                            string[] fieldInfo = null;
                                            string batchName = String.Empty, queueName = String.Empty, status = String.Empty;

                                            GetBasicBatchInfo(rdr, out ct, out batchName, out queueName, out status);

                                            if (!ExistsInExcludedStations(queueName, excluded))
                                            {
                                                string setting = GetSetting(CommonConst.stationsToCheckForFields + (index + 1).ToString());

                                                if (SpClasses.SpUtils.ExistsInPipeStr(false, queueName, setting))
                                                {
                                                    //if (status == "1") // Can only get Collection data if the batch is 'available' - eFlow limitation :(
                                                        fieldInfo = SpClasses.SpUtils.GetFieldBatchInfo(
                                                            GetSetting(CommonConst.tempFileUploadFolder),
                                                            apps[index], queueName, batchName,
                                                            GetSetting(CommonConst.fieldsToCheck + (index + 1).ToString()), out fn, out origin);
                                                }

                                                string[] customers = SpClasses.SpUtils.FromPipe(allw[index]);

                                                foreach (string cust in customers)
                                                {
                                                    string currentApp = apps[index];

                                                    string bInfo = String.Empty;
                                                    string fnUp = String.Empty;

                                                    string fDomain = GetSetting(CommonConst.fileDomain);
                                                    SpClasses.SpUtils.GetExtendedFieldBatchInfo(origin, fn, ct, fDomain, fieldInfo, batchName, queueName, status, cust, currentApp, out bInfo, out fnUp);

                                                    string nfn = fnUp.Substring(fnUp.LastIndexOf("/") + 1);

                                                    if (fn != String.Empty)
                                                    {
                                                        if ((IsAdmin(cust, customers)) || ((cust.ToLower() == origin.ToLower() ||
                                                            origin == eFlow.SupplierPortalCore.Constants.eFlowOrigin)))
                                                        {
                                                            SendFileWebRepository(cust, apps[index], fn, nfn);
                                                            Thread.Sleep(50);
                                                        }
                                                    }

                                                    string alias = String.Empty;
                                                    if (IsAliasStation(setting, queueName, out alias))
                                                    {
                                                        if (alias != String.Empty)
                                                            bInfo = bInfo.Replace(queueName, alias);
                                                    }

                                                    // Only sync back collection data to the portal that is from the same origin as the customer
                                                    // (which means that is was submitted by the customer through the portal itself)
                                                    // or collection data which is originally from eFlow (not submitted through the portal)
                                                    // and is related to the main domain (admin domain = when firstTime is equal to 0).

                                                    if ((IsAdmin(cust, customers) || ((cust.ToLower() == origin.ToLower()) ||
                                                        (origin == eFlow.SupplierPortalCore.Constants.eFlowOrigin))))
                                                    {
                                                        SendToPortal(cust, apps[index], batchName, bInfo);
                                                        Thread.Sleep(50);
                                                    }
                                                }

                                                if (File.Exists(fn))
                                                    File.Delete(fn);
                                            }
                                        }

                                        rdr.Close();
                                    }
                                    else
                                        rdr.Close();
                                }
                            }

                            mySqlConnection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }
        }

        public static bool IsAliasStation(string setting, string queueName, out string alias) 
        {
            bool valor = false;
            alias = String.Empty;

            try
            {
                string qn = "(" + queueName + ")";

                if (setting.ToLower().Contains(qn.ToLower()))
                {
                    valor = true;

                    string tmp = String.Empty;
                    bool start = false;

                    for (int i = setting.IndexOf(qn); i >= 0; i--)
                    {
                        if (setting[i] != '=' && !start)
                            continue;
                        else
                            if (setting[i] == '=')
                                start = true;
                            else
                                if (start)
                                    if (Char.IsLetterOrDigit(setting[i]))
                                    {
                                        tmp += setting[i].ToString();
                                    }
                                    else
                                        break;
                    }

                    if (tmp != String.Empty)
                        alias = SpClasses.SpUtils.Reverse(tmp);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return valor;
        }

        public static string[] GetCustomersForApp(string eFlowAppName)
        {
            string[] customers = null;
            List<string> appNames = new List<string>();

            string custnames = String.Empty;
 
            try
            {
                string tmp = GetSetting(CommonConst.app01WhoCanSeeInPortal);
                if (tmp.Contains("_") && tmp.ToLower().Contains(eFlowAppName.ToLower()))
                {
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));
                    if (tmp.Contains("=")) custnames = tmp.Substring(tmp.IndexOf("=") + 1);
                }

                tmp = GetSetting(CommonConst.app02WhoCanSeeInPortal);
                if (tmp.Contains("_") && tmp.ToLower().Contains(eFlowAppName.ToLower()))
                {
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));
                    if (tmp.Contains("=")) custnames = tmp.Substring(tmp.IndexOf("=") + 1);
                }

                tmp = GetSetting(CommonConst.app03WhoCanSeeInPortal);
                if (tmp.Contains("_") && tmp.ToLower().Contains(eFlowAppName.ToLower()))
                {
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));
                    if (tmp.Contains("=")) custnames = tmp.Substring(tmp.IndexOf("=") + 1);
                }

                tmp = GetSetting(CommonConst.app04WhoCanSeeInPortal);
                if (tmp.Contains("_") && tmp.ToLower().Contains(eFlowAppName.ToLower()))
                {
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));
                    if (tmp.Contains("=")) custnames = tmp.Substring(tmp.IndexOf("=") + 1);
                }

                tmp = GetSetting(CommonConst.app05WhoCanSeeInPortal);
                if (tmp.Contains("_") && tmp.ToLower().Contains(eFlowAppName.ToLower()))
                {
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));
                    if (tmp.Contains("=")) custnames = tmp.Substring(tmp.IndexOf("=") + 1);
                }

                tmp = GetSetting(CommonConst.app06WhoCanSeeInPortal);
                if (tmp.Contains("_") && tmp.ToLower().Contains(eFlowAppName.ToLower()))
                {
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));
                    if (tmp.Contains("=")) custnames = tmp.Substring(tmp.IndexOf("=") + 1);
                }

                tmp = GetSetting(CommonConst.app07WhoCanSeeInPortal);
                if (tmp.Contains("_") && tmp.ToLower().Contains(eFlowAppName.ToLower()))
                {
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));
                    if (tmp.Contains("=")) custnames = tmp.Substring(tmp.IndexOf("=") + 1);
                }

                tmp = GetSetting(CommonConst.app08WhoCanSeeInPortal);
                if (tmp.Contains("_") && tmp.ToLower().Contains(eFlowAppName.ToLower()))
                {
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));
                    if (tmp.Contains("=")) custnames = tmp.Substring(tmp.IndexOf("=") + 1);
                }

                tmp = GetSetting(CommonConst.app09WhoCanSeeInPortal);
                if (tmp.Contains("_") && tmp.ToLower().Contains(eFlowAppName.ToLower()))
                {
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));
                    if (tmp.Contains("=")) custnames = tmp.Substring(tmp.IndexOf("=") + 1);
                }

                tmp = GetSetting(CommonConst.app10WhoCanSeeInPortal);
                if (tmp.Contains("_") && tmp.ToLower().Contains(eFlowAppName.ToLower()))
                {
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));
                    if (tmp.Contains("=")) custnames = tmp.Substring(tmp.IndexOf("=") + 1);
                }

                if (appNames.Count > 0)
                {
                    if (custnames != String.Empty)
                    {
                        customers = custnames.Split('|');
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return customers;
        }

        public static List<string> GetCustomers(ref List<string> appNames)
        {
            List<string> allowed = new List<string>();

            try
            {
                string tmp = GetSetting(CommonConst.app01WhoCanSeeInPortal);
                if (tmp.Contains("_"))
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));

                if (tmp.Contains("="))
                    allowed.Add(tmp.Substring(tmp.IndexOf("=") + 1));

                tmp = GetSetting(CommonConst.app02WhoCanSeeInPortal);
                if (tmp.Contains("_"))
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));

                if (tmp.Contains("="))
                    allowed.Add(tmp.Substring(tmp.IndexOf("=") + 1));

                tmp = GetSetting(CommonConst.app03WhoCanSeeInPortal);
                if (tmp.Contains("_"))
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));

                if (tmp.Contains("="))
                    allowed.Add(tmp.Substring(tmp.IndexOf("=") + 1));

                tmp = GetSetting(CommonConst.app04WhoCanSeeInPortal);
                if (tmp.Contains("_"))
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));

                if (tmp.Contains("="))
                    allowed.Add(tmp.Substring(tmp.IndexOf("=") + 1));

                tmp = GetSetting(CommonConst.app05WhoCanSeeInPortal);
                if (tmp.Contains("_"))
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));

                if (tmp.Contains("="))
                    allowed.Add(tmp.Substring(tmp.IndexOf("=") + 1));

                tmp = GetSetting(CommonConst.app06WhoCanSeeInPortal);
                if (tmp.Contains("_"))
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));

                if (tmp.Contains("="))
                    allowed.Add(tmp.Substring(tmp.IndexOf("=") + 1));

                tmp = GetSetting(CommonConst.app07WhoCanSeeInPortal);
                if (tmp.Contains("_"))
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));

                if (tmp.Contains("="))
                    allowed.Add(tmp.Substring(tmp.IndexOf("=") + 1));

                tmp = GetSetting(CommonConst.app08WhoCanSeeInPortal);
                if (tmp.Contains("_"))
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));

                if (tmp.Contains("="))
                    allowed.Add(tmp.Substring(tmp.IndexOf("=") + 1));

                tmp = GetSetting(CommonConst.app09WhoCanSeeInPortal);
                if (tmp.Contains("_"))
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));

                if (tmp.Contains("="))
                    allowed.Add(tmp.Substring(tmp.IndexOf("=") + 1));

                tmp = GetSetting(CommonConst.app10WhoCanSeeInPortal);
                if (tmp.Contains("_"))
                    appNames.Add(tmp.Substring(0, tmp.IndexOf("_")));

                if (tmp.Contains("="))
                    allowed.Add(tmp.Substring(tmp.IndexOf("=") + 1));
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return allowed;
        }
    }
}
