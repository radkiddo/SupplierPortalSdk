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

using TiS.Core.Application;
using TiS.Core.Common;
using TiS.Core.Domain;
using TiS.Core.TisCommon;
using TiS.Core.Application.DataModel.Dynamic;
using TiS.Core.Application.Interfaces;

#endregion "using"

#region "SupplierPortalSdk NS"

namespace eFlow.SupplierPortalCore
{
    #region "Class"

    /// <summary>
    /// "Sp" Class --> Is a wrapper class that performs HTTP operations around a ITisCollectionData object. 
    /// </summary>
    public class Sp : SpInternals
    {
        #region "Constructor-Finalizer-Dispose"

        #region "Constructor"
        /// <summary>
        /// [Constructor] Sp() --> Initializes an Sp object instance.
        /// </summary>
        /// <example><code>Sp s = new Sp();</code></example>
        public Sp() : base()
        {
        }
        #endregion "Constructor"

        #region "Destructor"
        /// <summary>
        /// [Destructor] Sp() --> Releases unmanaged resources and performs other cleanup operations before the is reclaimed by garbage collection.
        /// </summary>
        ~Sp()
        {
            // Our finalizer should call our Dispose(bool) method with false
            base.Dispose(false);
        }
        #endregion "Destructor"

        #endregion "Constructor-Finalizer-Dispose"

        /// <summary>
        /// GetCollectionData() --> Gets the collection data available within a ITisCollectionData object.
        /// </summary>
        /// <param name="collData">The ITisCollectionData object from where the data will be retrieved</param>
        /// <param name="filterForms">Filter by forms, i.e. just data from form 1, 2 and 3. If null, all forms will be taken into account</param>
        /// <param name="filterFields">Just use the field names indicated here for each form. All fields are not allowed, due to string length on HTPP requests, exceeding limits</param>
        /// <param name="useFormName">Use the form name for each field (if true), otherwise use 'f1', 'f2', etc...</param>
        /// <example><code>s.GetCollectionData(collData);</code></example>
        public string GetCollectionData(ITisCollectionData collData, string[] filterForms, string[] filterFields, bool useFormName)
        {
            string result = String.Empty;
            
            try
            {
                if (collData != null)
                {
                    int frm_idx = 1;
                    bool allForms = (filterForms == null) ? true : false;

                    foreach (ITisFormData fd in collData.Forms)
                    {
                        string frmName = fd.Name;
                        int fld_idx = 1;

                        bool formOk = (allForms ||
                            Array.IndexOf(filterForms, frm_idx.ToString()) >= 0) ? true : false;

                        if (formOk)
                        {
                            foreach (ITisFieldData fldData in fd.Fields)
                            {
                                if (fldData.Contents != String.Empty)
                                {
                                    if (Array.IndexOf(filterFields, fldData.Name) >= 0)
                                    {
                                        string fldStr = (useFormName) ?
                                            frmName + "." + fldData.Name + "=" + fldData.Contents :
                                                "f" + frm_idx.ToString() + "." + fldData.Name + "=" + fldData.Contents;

                                        result += (fd.Fields.Count == fld_idx) ? fldStr : fldStr + "|";
                                    }
                                }

                                fld_idx++;
                            }
                        }

                        frm_idx++;
                    }
                }
            }
            catch { }

            if (result[result.Length - 1] == '|')
                result = result.Substring(0, result.Length - 1);

            return result;
        }

        #region "Exposed/Public methods"

        /// <summary>
        /// HttpGetCollectionsData() --> Gets all the collections for the table 'customer' for eFlowAppName.
        /// </summary>
        /// <param name="customer">Indicates the name of the customer to which the data is bound to</param>
        /// <param name="eFlowAppName">Indicates the name of the eFlow App to which the data is bound to</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// </summary>
        /// <example><code>s.HttpGetCollectionsData("topimagesystems.com", "CLS", 80, false);</code></example>
        public string HttpGetCollectionsData(string customer, string eFlowAppName, int port, bool ssl)
        {
            string tn = customer + "_" + eFlowAppName;
            return HttpInternalGetCollectionsData(tn, port, ssl);
        }

        /// <summary>
        /// FtpDownload() --> Performs an FTP Download.
        /// </summary>
        /// <param name="folder">Indicates the folder where the file will be uploaded</param>
        /// <param name="localfolders">Indicates the local folders where the file will be downloaded to</param>
        /// <param name="apps">Indicates the eFlow apps for which the download are going to be placed</param>
        /// <param name="hostname">Indicates the host name where the file will be uploaded</param>
        /// <param name="username">Indicates the name of the user allowed to login into the hostname</param>
        /// <param name="pwd">Indicates the password of the user allowed to login into the hostname</param>
        /// <example><code>s.FtpDownload("/images_receive", localfolders, apps, "ftp.myserver.com", "supplierportaluser", "password");</code></example>
        public bool FtpDownload(string folder, string[] localfolders, string[] apps, string hostname, string username, string pwd)
        {
            return FtpInternalDownload(folder, localfolders, apps, hostname, username, pwd);
        }

        /// <summary>
        /// FtpSendSimpleFile() --> Performs an FTP Upload, without indicating the customer or eFlow App. Should be used for upload of validations files only.
        /// </summary>
        /// <param name="fn">Indicates the name of the file to upload.</param>
        /// <param name="fnUp">Indicates the name of the file on the server (how it will be called once uploaded).</param>
        /// <param name="folder">Indicates the folder where the file will be uploaded</param>
        /// <param name="hostname">Indicates the host name where the file will be uploaded</param>
        /// <param name="username">Indicates the name of the user allowed to login into the hostname</param>
        /// <param name="pwd">Indicates the password of the user allowed to login into the hostname</param>
        /// <example><code>s.FtpSendSimpleFile("cls.js", "/validations", "ftp.doksend.com", "supplierportaluser", "pwd");</code></example>
        public bool FtpSendSimpleFile(string fn, string fnUp, string folder, string hostname, string username, string pwd)
        {
            return FtpInternalSendSimpleFile(fn, fnUp, folder, hostname, username, pwd);
        }

        /// <summary>
        /// FtpSendFile() --> Performs an FTP Upload.
        /// </summary>
        /// <param name="cust">Indicates the name of supplier portal customer.</param>
        /// <param name="app">Indicates the name of eFlow application.</param>
        /// <param name="fn">Indicates the name of the file to upload.</param>
        /// <param name="fnUp">Indicates the name of the file on the server (how it will be called once uploaded).</param>
        /// <param name="folder">Indicates the folder where the file will be uploaded</param>
        /// <param name="hostname">Indicates the host name where the file will be uploaded</param>
        /// <param name="username">Indicates the name of the user allowed to login into the hostname</param>
        /// <param name="pwd">Indicates the password of the user allowed to login into the hostname</param>
        /// <example><code>s.FtpSendFile("topimagesystems.com", "CLS", "00000323.tif", "/images", "ftp.doksend.com", "supplierportaluser", "pwd");</code></example>
        public bool FtpSendFile(string cust, string app, string fn, string fnUp, string folder, string hostname, string username, string pwd)
        {
            return FtpInternalSendFile(cust, app, fn, fnUp, folder, hostname, username, pwd);
        }

        /// <summary>
        /// FtpCleanupImageWithNoCollections() --> Cleanup images which do not have a corresponding collection on the portal (FTP).
        /// </summary>
        /// <param name="cust">Indicates the name of supplier portal customer.</param>
        /// <param name="app">Indicates the name of eFlow application.</param>
        /// <param name="fn">Indicates the name of the file to upload.</param>
        /// <param name="fnUp">Indicates the name of the file on the server (how it will be called once uploaded).</param>
        /// <param name="folder">Indicates the folder where the file will be uploaded</param>
        /// <param name="hostname">Indicates the host name where the file will be uploaded</param>
        /// <param name="username">Indicates the name of the user allowed to login into the hostname</param>
        /// <param name="pwd">Indicates the password of the user allowed to login into the hostname</param>
        /// <example><code>s.FtpCleanupImageWithNoCollections("topimagesystems.com", "CLS", "00000323.tif", "/images", "ftp.doksend.com", "supplierportaluser", "pwd");</code></example>
        public bool FtpCleanupImageWithNoCollections(string cust, string app, string fn, string fnUp, string folder, string hostname, string username, string pwd)
        {
            return FtpInternalCleanupImageWithNoCollections(cust, app, fn, fnUp, folder, hostname, username, pwd);
        }

        /// <summary>
        /// HttpPostSimple() --> Performs a 'simple' HTPP POST (for CollectionData) request using the Chilkat engine. Returns an HTTP string response.
        /// </summary>
        /// <param name="customer">Indicates the name of the customer to which the data is bound to</param>
        /// <param name="eFlowAppName">Indicates the name of the eFlow App to which the data is bound to</param>
        /// <param name="station">Indicates the name of the eFlow station to which the data is bound to</param>
        /// <param name="collName">Indicates the name of the collection for which the request is valid.</param>
        /// <param name="strData">The data to POST in string format.</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// <example><code>s.HttpPostSimple("topimagesystems.com", "CLS", "00000323", "TEST1|SYSTEM|CLS|00000323|FreeProcess|1|Invoice_Date=|Invoice_Number=|Net_Amount1=|PO_Number=963645|Supplier_Name=|Total_Amount=0.00|VAT_Amount1=|VAT1=|speedyservices.com_CLS-00000323.tif", 80, false);</code></example>
        public string HttpPostSimple(string customer, string eFlowAppName, string collName, string strData, int port, bool ssl)
        {
            string response = String.Empty;
            string tn = customer + "_" + eFlowAppName;

            HttpPostCollectionDataTable(customer, eFlowAppName, port, ssl);

            return HttpPostCollectionDataSimple(tn, collName, 1, strData, port, ssl);
        }

        //public string HttpSubmitCollectionData(string collData)

        /// <summary>
        /// HttpPostCollectionQry() --> Performs an HTPP POST (for a CollectionData query) request using the Chilkat engine. Returns an HTTP string response. 
        /// </summary>
        /// <param name="customer">Indicates the name of the customer to which the data is bound to</param>
        /// <param name="eFlowAppName">Indicates the name of the eFlow App to which the data is bound to</param>
        /// <param name="fn">Indicates the name of the field to query.</param>
        /// <param name="fv">Indicates the value of the field to query..</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// <example><code>s.HttpPostCollectionQry("topimagesystems.com", "CLS", "cln", "00000323", 80, false);</code></example>
        public string HttpPostCollectionQry(string customer, string eFlowAppName, string fn, string fv, int port, bool ssl)
        {
            string tn = customer + "_" + eFlowAppName;
            return HttpPostCollectionDataQry(tn, fn, fv, port, ssl);
        }

        /// <summary>
        /// HttpPostCollectionTableQry() --> Performs an HTPP POST (for a CollectionData Table query) request using the Chilkat engine. Returns an HTTP string response. 
        /// </summary>
        /// <param name="customer">Indicates the name of the customer to which the data is bound to</param>
        /// <param name="eFlowAppName">Indicates the name of the eFlow App to which the data is bound to</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// <example><code>s.HttpPostCollectionTableQry("topimagesystems.com", "CLS", 80, false);</code></example>
        public string HttpPostCollectionTableQry(string customer, string eFlowAppName, int port, bool ssl)
        {
            string tn = customer + "_" + eFlowAppName;
            return HttpPostInternalCollectionTableQry(tn, port, ssl);
        }

        /// <summary>
        /// HttpPostCollectionDeleteFirstFound() --> Performs an HTPP POST (for a CollectionData delete) request using the Chilkat engine. 
        /// Returns an HTTP string response. ONLY the FIRST instance of collection data matching 'collectionName' WILL BE DELETED!
        /// </summary>
        /// <param name="tName">Indicates the name of the table from the Supplier Portal where the data will be DELETED from</param>
        /// <param name="collectionName">Indicates the name of the collection for which the request is valid.</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// <example><code>s.HttpPostCollectionDeleteFirstFound("topimagesystems.com", "CLS", "00000323", 80, false);</code></example>
        public string HttpPostCollectionDeleteFirstFound(string customer, string eFlowAppName, string collectionName, int port, bool ssl)
        {
            string tn = customer + "_" + eFlowAppName;
            return HttpPostCollectionDataDeleteFirstFound(tn, collectionName, port, ssl);
        }

        /// <summary>
        /// HttpPostCollectionDeleteAll() --> Performs an HTPP POST (for a CollectionData delete) request using the Chilkat engine. 
        /// Returns an HTTP string response. ALL instances of collection data matching 'collectionName' WILL BE DELETED!
        /// </summary>
        /// <param name="tName">Indicates the name of the table from the Supplier Portal where the data will be DELETED from</param>
        /// <param name="collectionName">Indicates the name of the collection for which the request is valid.</param>
        /// <param name="port">Indicates the port to make the HTTP request</param>
        /// <param name="ssl">If SSL is used or not (True or False)</param>
        /// <example><code>s.HttpPostCollectionDeleteAll("topimagesystems.com", "CLS", "00000323", 80, false);</code></example>
        public string HttpPostCollectionDeleteAll(string customer, string eFlowAppName, string collectionName, int port, bool ssl)
        {
            string tn = customer + "_" + eFlowAppName;
            return HttpPostCollectionDataDeleteAll(tn, collectionName, port, ssl);
        }

        public string HttpDeleteAllRowsFromTable(string tbl, int port, bool ssl)
        {
            return HttpInternalDeleteAllRowsFromTable(tbl, port, ssl);
        }

        #endregion "Exposed/Public methods"
    }

    #endregion "Class"
}

#endregion "SupplierPortalSdk NS"
