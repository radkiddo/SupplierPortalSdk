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
using System.ComponentModel;

#endregion "using"

#region "SupplierPortalSdk NS"

namespace eFlow.SupplierPortalCore
{
    #region "class Constants"

    /// <summary>
    /// "Constants" Class --> Constants used by the SpInternals & Sp classes. 
    /// </summary>
    public class Constants
    {
        // Chilkat
        public const string cStrChilkatHttpLic = "EDUARDOFREHttp_dhvxfzmc1X1O";
        public const string cStrChilkatFtpLic = "FTP!TEAMCRD!_rAy8OuCQ9R4o";

        // Flexible Cloud API Db
        public const string cStrFlexibleDb = "sp";

        // parameters
        public const string cStrHttpPostCollectionDataCustomerParam = "customer";
        public const string cStrHttpPostCollectionDataeFlowAppNameParam = "eFlowAppName";
        public const string cStrHttpPostCollectionDatatName = "tName";
        public const string cStrHttpPostCollectionDatacollectionName = "cid";
        public const string cStrHttpPostCollectionDatacollectionNameQryCln = "cln";
        public const string cStrHttpPostCollectionDatadomainUsers = "usersDomains";
        public const string cStrHttpPostCollectionDatastrData = "strData";

        public const string cStrHttpPostCollectionDataFn = "fn";
        public const string cStrHttpPostCollectionDataFv = "fv";

        #region "Deprecated / Obsolote"
        // Supplier Portal allows email domain name (this is used to know which users (email addresses) can view collection data
        public const string cStrHttpPostSpAllowedEmailDomains = "spd";

        // Indicates the collection version tag (each different version makes the collection data unique)
        public const string cStrHttpPostCollectionDataVersionTag = "spv";

        // Indicates the station name where the collection data is bound to
        public const string cStrHttpPostStationTag = "sps";
        #endregion "Deprecated / Obsolete"

        // Collection name prefix on the Supplier Portal backend
        public const string cStrCollectionDataNamePrefix = "c_";

        // Collection data field separators
        public const string cStrPipe = "|";
        public const string cStrFieldValueToken = "=";

        // Extensions
        public const string cStrAllAll = "*.*";
        public const string cStrAllTif = "*.tif";
        public const string cStrTif = ".tif";
        public const string cStrPdf = ".pdf";
        public const string cStrXml = ".xml";

        // Json results
        public const string cStrEmptyJsonResponse = "\"result\": []";

        // urls
        public static string cStrDomain = "supplierportal.aws.af.cm";
        public const string cStrHttpGetBaseCollectionDataUrl = "/k4e2550htyjrtyj4345435/radkiddo/eafa/sp/efl/timgsys/api/";
        public const string cStrHttpPostCollectionDataTableUrl = "/k4e2550htyjrtyj4345435/radkiddo/eafa/sp/efl/timgsys/api/cdt/p";
        public const string cStrHttpPostCollectionDataUrl = "/k4e2550htyjrtyj4345435/radkiddo/eafa/sp/efl/timgsys/api/cd/p";
        public const string cStrHttpPostCollectionDataQuery = "/k4e2550htyjrtyj4345435/radkiddo/eafa/sp/efl/timgsys/api/cd/p/query";
        public const string cStrHttpPostCollectionDataDeleteFirstFound = "/k4e2550htyjrtyj4345435/radkiddo/eafa/sp/efl/timgsys/api/cd/p/delete";
        public const string cStrHttpPostCollectionDataDeleteAll = "/k4e2550htyjrtyj4345435/radkiddo/eafa/sp/efl/timgsys/api/cd/p/deleteAll";
        public const string cStrHttpDeleteAllRowsInTable = "/k4e2550htDtKgXLG4455435/radkiddo/eafa/sp/efl/timgsys/api/p/table";

        // eFlow Dynamic
        public const string cStrDynamicReg = @"Software\TopImageSystems\eFLOW 4.5";
        public const string cStrConfigFilesLocation = "ConfigFilesLocation";
        public const string cStrEFLOW = "EFLOW";
        public const string cStrConfiguration = "Configuration";
        public const string cStrDynamic = "Dynamic";
        public const string cStrBIN = "Bin";

        // Logger name
        public const string cStrLoggerName = "Supplier Portal Logger";

        // Config file names
        public const string cStrInterval = "interval";
        public const string cStrDotConfig = ".config";
        public const string cStrFullDotConfig = "SupplierPortalCommon.dll.config";

        // Requires or Not Online Input
        public const string cStrOnlineInputNotRequired = "rc=0";
        public const string cStrOnlineInputRequired = "rc=1";

        // Require Completion / Date Created tags
        public const string cStrRc = "rc";
        public const string cStrDc = "dc";

        public const string POsTableSuffix = "__POs";
        public const string SupplierDataTableSuffix = "__SupplierData";
        public const string supplierUser2supplierIdsSuffix = "__SupplierIdsPerUser";

        // SPortalImport, SPortalAfter, SPortalCompl
        public const string SPortalImport = "SPortalImport";
        public const string SPortalAfter = "SPortalAfter";
        public const string SPortalCompl = "SPortalCompl";

        // tag to indicates if a collection originates from eFlow directly
        public const string eFlowOrigin = "eFlow";

        // Named Tags
        public const string SupplierPortalTag = "SupplierPortal";
        public const string SupplierPortalDomainTag = "SupplierPortalDomain";

        // all customer web tag, which is set to indicate that an image on the portal can be seen by all customers
        public const string allCustomers = "all";

        public const string efInternal = "efInternal";
    }

    #endregion "class Constants"
}

#endregion "SupplierPortalSdk NS"
