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

namespace SupplierPortalCommon
{
    public class CommonConst
    {
        public const string app01ImportPath = "app01ImportPath";
        public const string app02ImportPath = "app02ImportPath";
        public const string app03ImportPath = "app03ImportPath";
        public const string app04ImportPath = "app04ImportPath";
        public const string app05ImportPath = "app05ImportPath";
        public const string app06ImportPath = "app06ImportPath";
        public const string app07ImportPath = "app07ImportPath";
        public const string app08ImportPath = "app08ImportPath";
        public const string app09ImportPath = "app09ImportPath";
        public const string app10ImportPath = "app10ImportPath";

        public const string app01WhoCanSeeInPortal = "app01WhoCanSeeInPortal";
        public const string app02WhoCanSeeInPortal = "app02WhoCanSeeInPortal";
        public const string app03WhoCanSeeInPortal = "app03WhoCanSeeInPortal";
        public const string app04WhoCanSeeInPortal = "app04WhoCanSeeInPortal";
        public const string app05WhoCanSeeInPortal = "app05WhoCanSeeInPortal";
        public const string app06WhoCanSeeInPortal = "app06WhoCanSeeInPortal";
        public const string app07WhoCanSeeInPortal = "app07WhoCanSeeInPortal";
        public const string app08WhoCanSeeInPortal = "app08WhoCanSeeInPortal";
        public const string app09WhoCanSeeInPortal = "app09WhoCanSeeInPortal";
        public const string app10WhoCanSeeInPortal = "app10WhoCanSeeInPortal";

        public const string imgReceive = "imgReceive";
        public const string portalFilesRouteKey = "portalFilesRouteKey";
        public const string imgUserName = "imgUserName";
        public const string imgPwd = "imgPwd";
        public const string imgFolder = "imgFolder";

        public const string fieldsToCheck = "fieldsToCheck";

        public const string connectionStr01 = "connectionStr01";
        public const string connectionStr02 = "connectionStr02";
        public const string connectionStr03 = "connectionStr03";
        public const string connectionStr04 = "connectionStr04";
        public const string connectionStr05 = "connectionStr05";
        public const string connectionStr06 = "connectionStr06";
        public const string connectionStr07 = "connectionStr07";
        public const string connectionStr08 = "connectionStr08";
        public const string connectionStr09 = "connectionStr09";
        public const string connectionStr10 = "connectionStr10";

        public const string stationsToCheckForFields = "stationsToCheckForFields";

        public const string flexibleDb = "flexibleDb";
        public const string flexibleDbHttps = "flexibleDbHttps";
        public const string flexibleDbPort = "flexibleDbPort";
        public const string fileDomain = "fileDomain";

        public const string excluded = "excludeStations";
        public const string tempFileUploadFolder = "tempFileUploadFolder";
        public const string supplierPortalStationName = "supplierPortalStationName";

        // ref db consts
        public const string refDb = "refDb";
        public const string refDb_SupplierTable = "refDb_SupplierTable";
        public const string refDb_SupplierId = "refDb_SupplierId";
        public const string refDb_SupplierName = "refDb_SupplierName";
        public const string refDb_SupplierStreet = "refDb_SupplierStreet";
        public const string refDb_SupplierZIP = "refDb_SupplierZIP";
        public const string refDb_SupplierCity = "refDb_SupplierCity";
        public const string refDb_SupplierLand = "refDb_SupplierLand";
        public const string refDb_SupplierVATID = "refDb_SupplierVATID";
        public const string refDb_SupplierTelephone = "refDb_SupplierTelephone";
        public const string refDb_SupplierFaxnumber = "refDb_SupplierFaxnumber";
        public const string refDb_SupplierEmail = "refDb_SupplierEmail";
        public const string refDb_SupplierBranchcode = "refDb_SupplierBranchcode";
        public const string refDb_SupplierAccountNo = "refDb_SupplierAccountNo";
        public const string refDb_SupplierIBAN = "refDb_SupplierIBAN";
        public const string refDb_SupplierSWIFT = "refDb_SupplierSWIFT";
        public const string refDb_SupplierBUKRS = "refDb_SupplierBUKRS";
       
        public const string refDb_SupplierPOsTable = "refDb_SupplierPOsTable";
        public const string refDb_SupplierPOsSupplierId = "refDb_SupplierPOsSupplierId";
        public const string refDb_SupplierPOsPONumber = "refDb_SupplierPOsPONumber";

        public const string refDb_AllowedApps = "refDb_AllowedApps";
        public const string refDb_AllowedDomains = "refDb_AllowedDomains";

        // Amazon S3
        public const string useS3 = "useS3";
        public const string s3AccessKey = "s3AccessKey";
        public const string s3SecretKey = "s3SecretKey";
        public const string s3Bucket = "s3Bucket";

        // supplierUser2supplierIds
        public const string supplierUser2supplierIds1 = "supplierUser2supplierIds1";
        public const string supplierUser2supplierIds2 = "supplierUser2supplierIds2";
        public const string supplierUser2supplierIds3 = "supplierUser2supplierIds3";
        public const string supplierUser2supplierIds4 = "supplierUser2supplierIds4";
        public const string supplierUser2supplierIds5 = "supplierUser2supplierIds5";
        public const string supplierUser2supplierIds6 = "supplierUser2supplierIds6";
        public const string supplierUser2supplierIds7 = "supplierUser2supplierIds7";
        public const string supplierUser2supplierIds8 = "supplierUser2supplierIds8";
        public const string supplierUser2supplierIds9 = "supplierUser2supplierIds9";
        public const string supplierUser2supplierIds10 = "supplierUser2supplierIds10";

        // Create Supplier Xml file tags
        public const string xmlHeader = "<?xml version=\"1.0\"?>";
        public const string XmlSTag = "<xml>";
        public const string XmlETag = "</xml>";
        public const string SupplierInfoSXmlTag = "<SupplierInfo>";
        public const string eFlowAppNameSXmlTag = "<eFlowAppName>";
        public const string eFlowAppNameEXmlTag = "</eFlowAppName>";
        public const string DomainNameSXmlTag = "<DomainName>";
        public const string DomainNameEXmlTag = "</DomainName>";
        public const string SupplierIdSXmlTag = "<SupplierId>";
        public const string SupplierIdEXmlTag = "</SupplierId>";
        public const string PONumberSXmlTag = "<PONumber>";
        public const string PONumberEXmlTag = "</PONumber>";
        public const string FileSXmlTag = "<File>";
        public const string FileEXmlTag = "</File>";
        public const string DateTimeSXmlTag = "<DateTime>";
        public const string DateTimeEXmlTag = "</DateTime>";
        public const string SupplierInfoEXmlTag = "</SupplierInfo>";

        // SPortalImport, SPortalAfter, SPortalCompl
        public const string SPortalImport = "SPortalImport";
        public const string SPortalAfter = "SPortalAfter";
        public const string SPortalCompl = "SPortalCompl";

        // For field validation files
        public const string offlineFieldValidationFile1 = "offlineFieldValidationFile1";
        public const string offlineFieldValidationFile2 = "offlineFieldValidationFile2";
        public const string offlineFieldValidationFile3 = "offlineFieldValidationFile3";
        public const string offlineFieldValidationFile4 = "offlineFieldValidationFile4";
        public const string offlineFieldValidationFile5 = "offlineFieldValidationFile5";
        public const string offlineFieldValidationFile6 = "offlineFieldValidationFile6";
        public const string offlineFieldValidationFile7 = "offlineFieldValidationFile7";
        public const string offlineFieldValidationFile8 = "offlineFieldValidationFile8";
        public const string offlineFieldValidationFile9 = "offlineFieldValidationFile9";
        public const string offlineFieldValidationFile10 = "offlineFieldValidationFile10";

        public const string onlineFieldValidationFolder = "onlineFieldValidationFolder";

        public const string __SupplierIdsPerUser = "__SupplierIdsPerUser";
    }
}
