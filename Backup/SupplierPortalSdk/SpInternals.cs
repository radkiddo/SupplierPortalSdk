#region "about"

//
// eFLOW Supplier Portal SDK
// 2013 (c) - Top Image Systems (a project initiated by the UK branch)
//
// The purpose of this SDK is to communicate with the Supplier Portal for eFlow Invoices.
// Developed by: Eduardo Freitas
//

#endregion "about"

#region "using"

using System;
using System.Collections.Generic;
using System.Text;

using TiS.Core.eFlowAPI;
using TiS.Core.PlatformRuntime;
using System.ComponentModel;
using System.IO;

#endregion "using"

#region "SupplierPortalSdk NS"

namespace eFlow.SupplierPortalCore
{
    #region "Class"

    /// <summary>
    /// "SpInternals" Class --> Is an internal wrapper class that performs HTTP operations around a ITisCollectionData object. 
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SpInternals : IDisposable
    {
        #region "Private declarations"
        
        /// <summary>
        /// When set to true indicates that the instance of the class has been disposed
        /// </summary>
        protected bool disposed;

        /// <summary>
        /// CSM object instance
        /// </summary>
        protected ITisClientServicesModule oCSM = null;

        #endregion "Private declarations"

        #region "Properties"
        #endregion "Properties"

        #region "Constructor-Finalizer-Dispose"

        #region "Constructor"
        /// <summary>
        /// [Constructor] SpInternals() --> Initializes a SpInternals object instance.
        /// </summary>
        protected SpInternals()
        {

        }
        #endregion "Constructor"

        #region "Destructor"
        /// <summary>
        /// [Destructor] SpInternals() --> Releases unmanaged resources and performs other cleanup operations before the is reclaimed by garbage collection.
        /// </summary>
        ~SpInternals()
        {
            // Our finalizer should call our Dispose(bool) method with false
            this.Dispose(false);
        }
        #endregion "Destructor"

        #region "protected Dispose"
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        /// <remarks>
        /// If the main class was marked as sealed, we could just make this a private void Dispose(bool).  Alternatively, we could (in this case) put
        /// all of our logic directly in Dispose().
        /// </remarks>
        public virtual void Dispose(bool disposing)
        {
            // Use our disposed flag to allow us to call this method multiple times safely.
            // This is a requirement when implementing IDisposable
            if (!this.disposed)
            {
                if (disposing)
                {
                    // If we have any managed, IDisposable resources, Dispose of them here.
                    // In this case, we don't, so this was unneeded.
                    // Later, we will subclass this class, and use this section.


                }

                // Always dispose of undisposed unmanaged resources in Dispose(bool)

            }
            // Mark us as disposed, to prevent multiple calls to dispose from having an effect, 
            // and to allow us to handle ObjectDisposedException
            this.disposed = true;
        }
        #endregion "protected Dispose"

        #region "Dispose"
        /// <summary>
        /// Dispose() --> Performs SpInternals defined tasks associated with freeing, releasing, or resetting managed and unmanaged resources.
        /// </summary>
        /// <example><code>s.Dispose();</code></example>
        public void Dispose()
        {
            // We start by calling Dispose(bool) with true
            this.Dispose(true);

            // Now suppress finalization for this object, since we've already handled our resource cleanup tasks
            GC.SuppressFinalize(this);
        }
        #endregion "Dispose"

        #endregion "Constructor-Finalizer-Dispose"

        #region "Internal Private/Protected methods"

        /// <summary>
        /// HttpPostCollectionDataTable() --> Performs an HTPP POST for the creation of the CollectionData table using the Chilkat engine. The table name will be the combination of the "customer"_"eFlowAppName". Returns an HTTP string response.
        /// </summary>
        /// <param name="customer">Indicates the name of the customer to which the data is bound to</param>
        /// <param name="eFlowAppName">Indicates the name of the eFlow App to which the data is bound to</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// <example><code>s.HttpPostCollectionDataTable("topimagesystems.com", "CLS");</code></example>
        protected string HttpPostCollectionDataTable(string customer, string eFlowAppName, int port, bool ssl)
        {
            string response = String.Empty;

            try
            {
                if (customer != String.Empty && eFlowAppName != String.Empty)
                {
                    Chilkat.HttpRequest req = new Chilkat.HttpRequest();

                    using (Chilkat.Http http = new Chilkat.Http())
                    {
                        http.UnlockComponent(Constants.cStrChilkatHttpLic);

                        req.UsePost();
                        req.Path = Constants.cStrHttpPostCollectionDataTableUrl;

                        req.AddParam(Constants.cStrHttpPostCollectionDataCustomerParam, customer);
                        req.AddParam(Constants.cStrHttpPostCollectionDataeFlowAppNameParam, eFlowAppName);

                        Chilkat.HttpResponse resp = http.SynchronousRequest(Constants.cStrDomain, port, ssl, req);
                        response = (resp == null) ? http.LastErrorText : resp.BodyStr;
                    }
                }
             }
            catch (Exception ex)
            {
                response = ex.ToString();
                Logging.WriteLog(response);
            }

            return response;
        }

        /// <summary>
        /// HttpPostCollectionDataSimple() --> Performs an HTPP POST (for CollectionData) request using the Chilkat engine. Returns an HTTP string response.
        /// </summary>
        /// <param name="tName">Indicates the name of the table from the Supplier Portal where the data will be written to</param>
        /// <param name="collectionName">Indicates the name of the collection for which the request is valid.</param>
        /// <param name="cVersion">Indicates the version of the collection for which the request is valid.</param>
        /// <param name="users">The users from the domains which are allowed to 'view' the data posted.</param>
        /// <param name="strData">The data to POST in string format.</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// <example><code>s.HttpPostCollectionDataSimple("topimagesystems.com_CLS", "00000323", 1, "TEST1|SYSTEM|CLS|00000323|FreeProcess|1|Invoice_Date=|Invoice_Number=|Net_Amount1=|PO_Number=963645|Supplier_Name=|Total_Amount=0.00|VAT_Amount1=|VAT1=|speedyservices.com_CLS-00000323.tif", 80, false);</code></example>
        protected string HttpPostCollectionDataSimple(string tName, string collectionName, int cVersion, string strData, int port, bool ssl)
        {
            string response = String.Empty;

            try
            {
                if (tName != String.Empty && collectionName != String.Empty && strData != String.Empty)
                {
                    Chilkat.HttpRequest req = new Chilkat.HttpRequest();

                    using (Chilkat.Http http = new Chilkat.Http())
                    {
                        http.UnlockComponent(Constants.cStrChilkatHttpLic);

                        req.UsePost();
                        req.Path = Constants.cStrHttpPostCollectionDataUrl;

                        req.AddParam(Constants.cStrHttpPostCollectionDatatName, tName);

                        req.AddParam(Constants.cStrHttpPostCollectionDatacollectionName,
                                Constants.cStrCollectionDataNamePrefix + collectionName);

                        req.AddParam(Constants.cStrHttpPostCollectionDatacollectionNameQryCln,
                                Constants.cStrCollectionDataNamePrefix + collectionName);

                        req.AddParam(Constants.cStrHttpPostCollectionDatastrData, strData);

                        Chilkat.HttpResponse resp = http.SynchronousRequest(Constants.cStrDomain, port, ssl, req);
                        response = (resp == null) ? http.LastErrorText : resp.BodyStr;
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.ToString();
                Logging.WriteLog(response);
            }

            return response;
        }

        /// <summary>
        /// HttpPostCollectionDataDeleteFirstFound() --> Performs an HTPP POST (for a CollectionData delete) request using the Chilkat engine. Returns an HTTP string response. ONLY the FIRST instance of collection data matching 'collectionName' WILL BE DELETED! (in case there are more than one instances)
        /// </summary>
        /// <param name="tName">Indicates the name of the table from the Supplier Portal where the data will be DELETED from</param>
        /// <param name="collectionName">Indicates the name of the collection for which the request is valid.</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// <example><code>s.HttpPostCollectionDataDeleteFirstFound("topimagesystems.com_CLS", "00000323", 80, false);</code></example>
        protected string HttpPostCollectionDataDeleteFirstFound(string tName, string collectionName, int port, bool ssl)
        {
            string response = String.Empty;

            try
            {
                if (tName != String.Empty && collectionName != String.Empty)
                {
                    Chilkat.HttpRequest req = new Chilkat.HttpRequest();

                    using (Chilkat.Http http = new Chilkat.Http())
                    {
                        http.UnlockComponent(Constants.cStrChilkatHttpLic);

                        req.UsePost();
                        req.Path = Constants.cStrHttpPostCollectionDataDeleteFirstFound;

                        req.AddParam(Constants.cStrHttpPostCollectionDatatName, tName);
                        req.AddParam(Constants.cStrHttpPostCollectionDatacollectionName,
                                Constants.cStrCollectionDataNamePrefix + collectionName);

                        Chilkat.HttpResponse resp = http.SynchronousRequest(Constants.cStrDomain, port, ssl, req);
                        response = (resp == null) ? http.LastErrorText : resp.BodyStr;
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.ToString();
                Logging.WriteLog(response);
            }

            return response;
        }

        /// <summary>
        /// HttpPostCollectionDataDeleteAll() --> Performs an HTPP POST (for a CollectionData delete) request using the Chilkat engine. Returns an HTTP string response. ALL instances of collection data matching 'collectionName' WILL BE DELETED!
        /// </summary>
        /// <param name="tName">Indicates the name of the table from the Supplier Portal where the data will be DELETED from</param>
        /// <param name="collectionName">Indicates the name of the collection for which the request is valid.</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// <example><code>s.HttpPostCollectionDataDeleteAll("topimagesystems.com_CLS", "00000323", 80, false);</code></example>
        protected string HttpPostCollectionDataDeleteAll(string tName, string collectionName, int port, bool ssl)
        {
            string response = String.Empty;

            try
            {
                if (tName != String.Empty && collectionName != String.Empty)
                {
                    Chilkat.HttpRequest req = new Chilkat.HttpRequest();

                    using (Chilkat.Http http = new Chilkat.Http())
                    {
                        http.UnlockComponent(Constants.cStrChilkatHttpLic);

                        req.UsePost();
                        req.Path = Constants.cStrHttpPostCollectionDataDeleteAll;

                        req.AddParam(Constants.cStrHttpPostCollectionDatatName, tName);
                        req.AddParam(Constants.cStrHttpPostCollectionDatacollectionName,
                                Constants.cStrCollectionDataNamePrefix + collectionName);

                        Chilkat.HttpResponse resp = http.SynchronousRequest(Constants.cStrDomain, port, ssl, req);
                        response = (resp == null) ? http.LastErrorText : resp.BodyStr;
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.ToString();
                Logging.WriteLog(response);
            }

            return response;
        }

        protected string HttpInternalDeleteAllRowsFromTable(string tbl, int port, bool ssl)
        {
            string response = String.Empty;
            
            try
            {
                Chilkat.HttpRequest req = new Chilkat.HttpRequest();

                using (Chilkat.Http http = new Chilkat.Http())
                {
                    http.UnlockComponent(Constants.cStrChilkatHttpLic);

                    req.UsePost();
                    req.Path = Constants.cStrHttpDeleteAllRowsInTable;

                    req.AddParam("db", Constants.cStrFlexibleDb);
                    req.AddParam("table", tbl);

                    Chilkat.HttpResponse resp = http.SynchronousRequest(Constants.cStrDomain, port, ssl, req);
                    response = (resp == null) ? http.LastErrorText : resp.BodyStr;
                }
            }
            catch (Exception ex)
            {
                response = ex.ToString();
                Logging.WriteLog(response);
            }

            return response;
        }

        /// <summary>
        /// HttpPostInternalCollectionTableQry() --> Performs an HTPP POST (for a CollectionData Table query) request using the Chilkat engine. Returns an HTTP string response. 
        /// Basically gets the table name and table data.
        /// </summary>
        /// <param name="tName">Indicates the name of the table from the Supplier Portal where the data will be read from</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// <example><code>s.HttpPostInternalCollectionTableQry("topimagesystems.com_CLS", 80, false);</code></example>
        protected string HttpPostInternalCollectionTableQry(string tName, int port, bool ssl)
        {
            string response = String.Empty;

            try
            {
                if (tName != String.Empty)
                {
                    if (tName != String.Empty)
                    {
                        Chilkat.HttpRequest req = new Chilkat.HttpRequest();

                        using (Chilkat.Http http = new Chilkat.Http())
                        {
                            http.UnlockComponent(Constants.cStrChilkatHttpLic);

                            req.UsePost();
                            req.Path = Constants.cStrHttpPostCollectionDataTableUrl;

                            req.AddParam(Constants.cStrHttpPostCollectionDatatName, tName);

                            Chilkat.HttpResponse resp = http.SynchronousRequest(Constants.cStrDomain, port, ssl, req);
                            response = (resp == null) ? http.LastErrorText : resp.BodyStr;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.ToString();
                Logging.WriteLog(response);
            }

            return response;
        }

        /// <summary>
        /// HttpInternalGetCollectionsData() --> Gets all the collections for the table 'customer' for eFlowAppName.
        /// </summary>
        /// <param name="tName">Indicates the name of the table from the Supplier Portal where the data will be read from</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// </summary>
        /// <example><code>s.HttpInternalGetCollectionsData("topimagesystems.com_CLS", 80, false);</code></example>
        protected string HttpInternalGetCollectionsData(string tName, int port, bool ssl)
        {
            string response = String.Empty;

            try
            {
                if (tName != String.Empty)
                {
                    Chilkat.HttpRequest req = new Chilkat.HttpRequest();

                    using (Chilkat.Http http = new Chilkat.Http())
                    {
                        http.UnlockComponent(Constants.cStrChilkatHttpLic);
                        req.UseGet();

                        req.Path = Constants.cStrHttpGetBaseCollectionDataUrl + Constants.cStrFlexibleDb + "/"
                            + tName;

                        Chilkat.HttpResponse resp = http.SynchronousRequest(Constants.cStrDomain, port, ssl, req);
                        response = (resp == null) ? http.LastErrorText : resp.BodyStr;
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.ToString();
                Logging.WriteLog(response);
            }

            return response;
        }

        /// <summary>
        /// HttpPostCollectionDataQry() --> Performs an HTPP POST (for a CollectionData query) request using the Chilkat engine. 
        /// Returns an HTTP string response. 
        /// Basically gets a collection data from a table.
        /// </summary>
        /// <param name="tName">Indicates the name of the table from the Supplier Portal where the data will be read from</param>
        /// <param name="fn">Indicates the name of the field to query.</param>
        /// <param name="fv">Indicates the value of the field to query.</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// <example><code>s.HttpPostCollectionDataQry("topimagesystems.com_CLS", "cln", "00000002", 80, false);</code></example>
        protected string HttpPostCollectionDataQry(string tName, string fn, string fv, int port, bool ssl)
        {
            string response = String.Empty;

            try
            {
                if (tName != String.Empty && fn != String.Empty && fv != String.Empty)
                {
                    Chilkat.HttpRequest req = new Chilkat.HttpRequest();

                    using (Chilkat.Http http = new Chilkat.Http())
                    {
                        http.UnlockComponent(Constants.cStrChilkatHttpLic);

                        req.UsePost();
                        req.Path = Constants.cStrHttpPostCollectionDataQuery;

                        req.AddParam(Constants.cStrHttpPostCollectionDatatName, tName);
                        
                        req.AddParam(Constants.cStrHttpPostCollectionDataFn, fn);

                        if (fn == Constants.cStrHttpPostCollectionDatacollectionNameQryCln)
                            req.AddParam(Constants.cStrHttpPostCollectionDataFv, Constants.cStrCollectionDataNamePrefix + fv);
                        else
                            req.AddParam(Constants.cStrHttpPostCollectionDataFv, fv);

                        Chilkat.HttpResponse resp = http.SynchronousRequest(Constants.cStrDomain, port, ssl, req);
                        response = (resp == null) ? http.LastErrorText : resp.BodyStr;
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.ToString();
                Logging.WriteLog(response);
            }

            return response;
        }

        /// <summary>
        /// ExtractStrInBetween() --> Extracts a substring between two string delimiters.
        /// </summary>
        /// <param name="contents">Indicates the contents string from where to extract the information.</param>
        /// <param name="InitDelimiter">Indicates the starter delimiter string.</param>
        /// <param name="EndDelimiter">Indicates the end delimiter string.</param>
        /// <example><code>string res = s.ExtractStrInBetween("topimagesystems.com_CLS-tiff", "_", "-");</code></example>
        /// <example><code>res will have the value of "CLS"</code></example>
        public string ExtractStrInBetween(string contents, string InitDelimiter, string EndDelimiter)
        {
            string extractedStr = String.Empty;
            try
            {
                int InitPos = contents.IndexOf(InitDelimiter);
                if (InitPos >= 0)
                {
                    int StartPos = InitPos + InitDelimiter.Length;
                    int EndPos = contents.IndexOf(EndDelimiter) - 1;
                    if (EndPos < 0)
                        EndPos = contents.LastIndexOf(EndDelimiter) - 1;
                    if ((EndPos >= 0) && (EndPos >= StartPos))
                        extractedStr = ExtractCharsBetweenPositions(contents, StartPos, EndPos);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return extractedStr;
        }

        /// <summary>
        /// ExtractStrInBetweenClean() --> Extracts a substring between two string delimiters and removes unwanted chars.
        /// </summary>
        /// <param name="contents">Indicates the contents string from where to extract the information.</param>
        /// <param name="InitDelimiter">Indicates the starter delimiter string.</param>
        /// <param name="EndDelimiter">Indicates the end delimiter string.</param>
        /// <example><code>string res = s.ExtractStrInBetweenClean("topimagesystems.com_CLS-tiff", "_", "-");</code></example>
        /// <example><code>res will have the value of "CLS"</code></example>
        public string ExtractStrInBetweenClean(string contents, string InitDelimiter, string EndDelimiter)
        {
            string extractedStr = String.Empty;
            try
            {
                int InitPos = contents.IndexOf(InitDelimiter);
                if (InitPos >= 0)
                {
                    int StartPos = InitPos + InitDelimiter.Length;
                    int EndPos = contents.IndexOf(EndDelimiter) - 1;
                    if (EndPos < 0)
                        EndPos = contents.LastIndexOf(EndDelimiter) - 1;

                    if ((EndPos >= 0) && (EndPos >= StartPos))
                    {
                        extractedStr = ExtractCharsBetweenPositions(contents, StartPos, EndPos);

                        string tmp = String.Empty;
                        for (int i = 0; i <= extractedStr.Length - 1; i++)
                        {
                            if (Char.IsLetterOrDigit(extractedStr[i]) ||
                                extractedStr[i] == '.' || extractedStr[i] == '_' ||
                                extractedStr[i] == '-' || extractedStr[i] == ':' ||
                                extractedStr[i] == ' ')
                            {
                                tmp += extractedStr[i]; 
                            }
                        }

                        if (tmp != String.Empty)
                            extractedStr = tmp;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return extractedStr;
        }

        private string ExtractCharsBetweenPositions(string str, int start, int end)
        {
            string extracted = "";
            for (int i = start; i <= end; i++)
                extracted += str[i];

            return extracted;
        }

        /// <summary>
        /// FtpInternalDownload() --> Performs an FTP Download.
        /// </summary>
        /// <param name="folder">Indicates the folder where the file will be uploaded</param>
        /// <param name="localfolders">Indicates the local folders where the file will be downloaded to</param>
        /// <param name="apps">Indicates the eFlow apps for which the download are going to be placed</param>
        /// <param name="hostname">Indicates the host name where the file will be uploaded</param>
        /// <param name="username">Indicates the name of the user allowed to login into the hostname</param>
        /// <param name="pwd">Indicates the password of the user allowed to login into the hostname</param>
        /// <example><code>s.FtpInternalDownload("/images_receive", localfolders, apps, "ftp.doksend.com", "supplierportaluser", "e7low5!!");</code></example>
        protected bool FtpInternalDownload(string folder, string[] localfolders, string[] apps, string hostname, string username, string pwd)
        {
            bool result = false;

            try
            {
                using (Chilkat.Ftp2 ftp = new Chilkat.Ftp2())
                {
                    if (ftp.UnlockComponent(Constants.cStrChilkatFtpLic))
                    {
                        ftp.Hostname = hostname;
                        ftp.Username = username;
                        ftp.Password = pwd;

                        ftp.Passive = true;

                        if (ftp.Connect())
                        {
                            if (ftp.ChangeRemoteDir(folder))
                            {
                                ftp.ListPattern = Constants.cStrAllAll;

                                for (int i = 0; i <= ftp.NumFilesAndDirs - 1; i++)
                                {
                                    if (localfolders.Length > 0 && apps.Length > 0)
                                    {
                                        string fl = ftp.GetFilename(i);
                                        string lfn = GetLocalFolderFileName(fl, localfolders, apps);

                                        if (File.Exists(lfn))
                                            File.Delete(lfn);

                                        result = ftp.GetFile(fl, lfn);
                                        ftp.DeleteRemoteFile(fl);
                                    }
                                }
                            }
                        }
                        else
                            Logging.WriteLog(ftp.LastErrorText);

                        ftp.Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        private string GetLocalFolderFileName(string remoteFileName, string[] localFolders, string[] apps)
        {
            string result = String.Empty;

            try
            {
                int index = -1;

                foreach (string fld in localFolders)
                {
                    index++;

                    string customer = remoteFileName.Substring(0, remoteFileName.IndexOf("_"));
                    string appName = ExtractStrInBetween(remoteFileName, "_", "-");

                    if (apps[index].ToLower().Contains(appName.ToLower()))
                    {
                        string batchName = String.Empty;
                        string extName = String.Empty;

                        if (remoteFileName.ToLower().Contains(Constants.cStrTif.ToLower()))
                        {
                            batchName = ExtractStrInBetween(remoteFileName, "-", Constants.cStrTif);
                            extName = Constants.cStrTif;
                        }
                        else if (remoteFileName.ToLower().Contains(Constants.cStrPdf.ToLower()))
                        {
                            batchName = ExtractStrInBetween(remoteFileName, "-", Constants.cStrPdf);
                            extName = Constants.cStrPdf;
                        }
                        else if (remoteFileName.ToLower().Contains(Constants.cStrXml.ToLower()))
                        {
                            batchName = ExtractStrInBetween(remoteFileName, "-", Constants.cStrXml);
                            extName = Constants.cStrXml;
                        }

                        if (!Directory.Exists(fld))
                            Directory.CreateDirectory(fld);

                        if (batchName != String.Empty && extName != String.Empty)
                            result = Path.Combine(fld, batchName + extName);

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// FtpInternalSendSimpleFile() --> Performs an FTP Upload, without indicating the customer or eFlow App. Should be used for upload of validations files only.
        /// </summary>
        /// <param name="fn">Indicates the name of the file to upload.</param>
        /// <param name="fnUp">Indicates the name of the file on the server (how it will be called once uploaded).</param>
        /// <param name="folder">Indicates the folder where the file will be uploaded</param>
        /// <param name="hostname">Indicates the host name where the file will be uploaded</param>
        /// <param name="username">Indicates the name of the user allowed to login into the hostname</param>
        /// <param name="pwd">Indicates the password of the user allowed to login into the hostname</param>
        /// <example><code>s.FtpInternalSendSimpleFile("cls.js", "/validations", "ftp.doksend.com", "supplierportaluser", "e7low5!!");</code></example>
        protected bool FtpInternalSendSimpleFile(string fn, string fnUp, string folder, string hostname, string username, string pwd)
        {
            bool result = false;

            try
            {
                using (Chilkat.Ftp2 ftp = new Chilkat.Ftp2())
                {
                    if (ftp.UnlockComponent(Constants.cStrChilkatFtpLic))
                    {
                        ftp.Hostname = hostname;
                        ftp.Username = username;
                        ftp.Password = pwd;

                        ftp.Passive = true;

                        if (ftp.Connect())
                        {
                            if (ftp.ChangeRemoteDir(folder))
                            {
                                result = ftp.PutFile(fn, fnUp);
                            }
                        }
                        else
                            Logging.WriteLog(ftp.LastErrorText);

                        ftp.Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// FtpInternalSendFile() --> Performs an FTP Upload.
        /// </summary>
        /// <param name="cust">Indicates the name of supplier portal customer.</param>
        /// <param name="app">Indicates the name of eFlow application.</param>
        /// <param name="fn">Indicates the name of the file to upload.</param>
        /// <param name="fnUp">Indicates the name of the file on the server (how it will be called once uploaded).</param>
        /// <param name="folder">Indicates the folder where the file will be uploaded</param>
        /// <param name="hostname">Indicates the host name where the file will be uploaded</param>
        /// <param name="username">Indicates the name of the user allowed to login into the hostname</param>
        /// <param name="pwd">Indicates the password of the user allowed to login into the hostname</param>
        /// <example><code>s.FtpInternalSendFile("topimagesystems.com", "CLS", "00000323.tif", "/images", "ftp.doksend.com", "supplierportaluser", "e7low5!!");</code></example>
        protected bool FtpInternalSendFile(string cust, string app, string fn, string fnUp, string folder, string hostname, string username, string pwd)
        {
            bool result = false;

            try
            {
                using (Chilkat.Ftp2 ftp = new Chilkat.Ftp2())
                {
                    if (ftp.UnlockComponent(Constants.cStrChilkatFtpLic))
                    {
                        ftp.Hostname = hostname;
                        ftp.Username = username;
                        ftp.Password = pwd;

                        ftp.Passive = true;

                        if (ftp.Connect())
                        {
                            if (ftp.ChangeRemoteDir(folder))
                            {
                                if (ftp.GetSizeByName(fnUp) < 0)
                                {
                                    if (File.Exists(fn))
                                    {
                                        result = ftp.PutFile(fn, fnUp);
                                    }
                                }
                            }
                        }
                        else
                            Logging.WriteLog(ftp.LastErrorText);

                        ftp.Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// FtpInternalCleanupImageWithNoCollections() --> Cleanup images which do not have a corresponding collection on the portal (FTP).
        /// </summary>
        /// <param name="cust">Indicates the name of supplier portal customer.</param>
        /// <param name="app">Indicates the name of eFlow application.</param>
        /// <param name="fn">Indicates the name of the file to upload.</param>
        /// <param name="fnUp">Indicates the name of the file on the server (how it will be called once uploaded).</param>
        /// <param name="folder">Indicates the folder where the file will be uploaded</param>
        /// <param name="hostname">Indicates the host name where the file will be uploaded</param>
        /// <param name="username">Indicates the name of the user allowed to login into the hostname</param>
        /// <param name="pwd">Indicates the password of the user allowed to login into the hostname</param>
        /// <example><code>s.FtpInternalCleanupImageWithNoCollections("topimagesystems.com", "CLS", "00000323.tif", "/images", "ftp.doksend.com", "supplierportaluser", "e7low5!!");</code></example>
        protected bool FtpInternalCleanupImageWithNoCollections(string cust, string app, string fn, string fnUp, string folder, string hostname, string username, string pwd)
        {
            bool result = false;

            try
            {
                using (Chilkat.Ftp2 ftp = new Chilkat.Ftp2())
                {
                    if (ftp.UnlockComponent(Constants.cStrChilkatFtpLic))
                    {
                        ftp.Hostname = hostname;
                        ftp.Username = username;
                        ftp.Password = pwd;

                        ftp.Passive = true;

                        if (ftp.Connect())
                        {
                            if (ftp.ChangeRemoteDir(folder))
                            {
                                // Cleanup images which do not have a corresponding collection on the portal
                                ftp.ListPattern = Constants.cStrAllTif;
                                for (int i = 0; i <= ftp.NumFilesAndDirs - 1; i++)
                                {
                                    string fl = ftp.GetFilename(i);
                                    string customer = fl.Substring(0, fl.IndexOf("_"));
                                    string appName = ExtractStrInBetween(fl, "_", "-");
                                    string batchName = ExtractStrInBetween(fl, "-", Constants.cStrTif);

                                    //if (HttpPostCollectionDataQry(customer + "_" + appName,
                                    if (HttpPostCollectionDataQry(cust + "_" + appName,
                                            Constants.cStrHttpPostCollectionDatacollectionNameQryCln,
                                            batchName, 80, false).ToLower().Contains(Constants.cStrEmptyJsonResponse))
                                    {
                                        ftp.DeleteRemoteFile(fl);
                                    }
                                }
                            }
                        }
                        else
                            Logging.WriteLog(ftp.LastErrorText);

                        ftp.Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.ToString());
            }

            return result;
        }

        #endregion "Internal Private/Protected methods"

        #region "Public methods"

        /// <summary>
        /// SetDomain() --> Sets the Flexible Cloud Domain (Supplier Portal Backend Web Host Domain Name).
        /// </summary>
        /// <param name="domain">Indicates the Supplier Portal Backend Web Host Domain Name.</param>
        /// <example><code>s.SetDomain("supplierportal.aws.af.cm");</code></example>
        public void SetDomain(string domain)
        {
            Constants.cStrDomain = domain;
        }

        #endregion "Public methods"
    }

    #endregion "Class"
}

#endregion "SupplierPortalSdk NS"
