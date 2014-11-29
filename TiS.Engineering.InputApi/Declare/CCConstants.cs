using System;
using System.Collections.Generic;
using System.Text;

namespace TiS.Engineering.InputApi
{
    /// <summary>
    /// Blobal constants pool.
    /// </summary>
#if INTERNAL
    internal static class CCConstants
#else
    public static class CCConstants
#endif
    {
        /// <summary>
        /// (?i)^[a-z0-9\-_]{1,58}$
        /// </summary>
        public const String RX_COLLECTION_NAME = @"(?i)^[a-z0-9\-_]{1,58}$";

        /// <summary>
        /// (?i)_P[A-F0-9]{4}$
        /// </summary>
        public const String RX_PAGE_PREFIX = @"(?i)_P[A-F0-9]{4}$";

        /// <summary>
        /// (?i)^(A(bove( +)?Normal)?|H(igh)?|L(ow)?|N(ormal)?)$
        /// </summary>
        public const String RX_PRIORITY = "(?i)^(A(bove( +)?Normal)?|H(igh)?|L(ow)?|N(ormal)?)$";

        /// <summary>
        ///  "dd/MM/yyyy HH:mm:ss"
        /// </summary>
        public const String DATE_FORMAT = "dd/MM/yyyy HH:mm:ss";

        /// <summary>
        /// "Unexpected error";
        /// </summary>
        public const String E0000 = "Unexpected error";

        /// <summary>
        /// "Invalid name format";
        /// </summary>
        public const String E0010 = "Invalid name format";
        
        /// <summary>
        /// "Invalid date time format";
        /// </summary>
        public const String E0020 = "Invalid date time format";

        /// <summary>
        /// "End date before start date";
        /// </summary>
        public const String E0030 = "End date before start date";
        
        /// <summary>
        /// "No header information";
        /// </summary>
        public const String E0040 = "No header information";

        /// <summary>
        /// "DataTable not valid";
        /// </summary>
        public const String E0041 = "DataTable not valid";

        /// <summary>
        /// "Page count does not match specified value";
        /// </summary>
        public const String E0050 = "Page count does not match specified value";
        
        /// <summary>
        /// "Expected number of pages has to be higher than 0";
        /// </summary>
        public const String E0051 = "Expected number of pages has to be higher than 0";
        
        /// <summary>
        /// "Invalid collection image file path.";
        /// </summary>
        public const String E0060 = "Invalid collection image file path";
        
        /// <summary>
        /// "Failed opening or getting image count from image file path";
        /// </summary>
        public const String E0061 = "Failed opening or getting image count from image file path";
        
        /// <summary>
        /// "Invalid attachment file path";
        /// </summary>
        public const String E0065 = "Invalid attachment file path";
        
        /// <summary>
        ///  "Invalid Priority";
        /// </summary>
        public const String E0070 = "Invalid Priority";

        /// <summary>
        /// "Missing row data information";
        /// </summary>
        public const String E0080 = "Missing row data information";

        /// <summary>
        /// "Failed creating pages in eFlow system to match expected number of pages";
        /// </summary>
        public const String E0090 = "Failed creating pages in eFlow system to match expected number of pages";

        /// <summary>
        /// "Failed creating collection definition from DataTable definitions";
        /// </summary>
        public const String E0091 = "Failed creating collection definition from DataTable definitions";

         /// <summary>
        /// "Failed creating collection in eFlow system";
        /// </summary>
        public const String E0092 = "Failed creating collection in eFlow system";

        /// <summary>
        /// "Failed locating or loading settings file";
        /// </summary>
        public const String E0100 = "Failed locating or loading settings file";

        /// <summary>
        /// "Main profile not loaded";
        /// </summary>
        public const String E0101 = "Main profile not loaded";

        /// <summary>
        /// "Profile not loaded";
        /// </summary>
        public const String E0102 = "Profile not loaded";

        /// <summary>
        /// "eFlow CSM object must be specified in: "
        /// </summary>
        public const String E0200 = "Failed getting XmlPrdPath";

        /// <summary>
        /// "Failed deserializing  to PRD, file: "
        /// </summary>
        public const String E0201 = "Failed deserializing  to PRD, file: ";

        /// <summary>
        /// "Failed deserializing  to PRD files, unexpected error."
        /// </summary>
        public const String E0202 = "Failed deserializing  to PRD files, unexpected error.";

        /// <summary>
        /// "Failed initializing eFlow ClientServiceModule"
        /// </summary>
        public const String E0210 = "Failed initializing eFlow ClientServiceModule";

        /// <summary>
        /// "eFlow CSM object must be specified in: "
        /// </summary>
        public const String E0211 = "eFlow CSM object must be specified in: ";

        /// <summary>
        /// "Flow name \\ type must be specified"
        /// </summary>
        public const String E0212 = "Flow name \\ type must be specified";

        /// <summary>
        /// "Failed locking page attachment file\\s"
        /// </summary>
        public const String E0220 = "Failed locking page attachment file\\s";

        /// <summary>
        /// "Failed moving page attachment file\\s to work directory"
        /// </summary>
        public const String E0221 = "Failed moving page attachment file\\s to work directory";

        /// <summary>
        /// "Failed locking page attachment file\\s"
        /// </summary>
        public const String E0222 = "Failed locking collection attachment file\\s";

        /// <summary>
        /// "Failed moving page attachment file\\s to work directory"
        /// </summary>
        public const String E0223 = "Failed moving collection attachment file\\s to work directory";

        /// <summary>
        /// "Collection with specified name already exists in eFlow system"
        /// </summary>
        public const String E0230 = "Collection with specified name already exists in eFlow system";

        /// <summary>
        /// "Failed creating collection in eFlow system"
        /// </summary>
        public const String E0231 = "Failed creating collection in eFlow system";

        /// <summary>
        /// Specified flow does not exists in this eFlow application
        /// </summary>
        public const String E0232 = "Specified flow does not exists in this eFlow application";

        /// <summary>
        /// No forms were specified in collection information
        /// </summary>
        public const String E0233 = "No forms were specified in collection information";

        /// <summary>
        /// Collection form has no pages specified in collection information
        /// </summary>
        public const String E0234 = "Collection form has no pages specified in collection information";

        /// <summary>
        /// Form has more than one pages but only one was expected
        /// </summary>
        public const String E0235 = "Form has more than one pages but only one was expected";

        /// <summary>
        /// Form has less than two pages but more than one page per form expected
        /// </summary>
        public const String E0236 = "Form has less than two pages but more than one page per form expected";

        /// <summary>
        /// Collection has no pages specified in collection information
        /// </summary>
        public const String E0237 = "Collection has no pages specified in collection information";

        /// <summary>
        /// Collection information page count does not match collection image page count
        /// </summary>
        public const String E0238 = "Collection information page count does not match collection image page count";

        /// <summary>
        /// Specified form has no form type
        /// </summary>
        public const String E0239 = "Specified form has no form type";

        /// <summary>
        /// Specified form type does not exist in flow
        /// </summary>
        public const String E0240 = "Specified form type does not exist in flow";
    }
}
