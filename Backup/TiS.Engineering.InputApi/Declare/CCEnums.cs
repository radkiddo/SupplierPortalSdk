using System;
using System.Collections.Generic;
using System.Text;

namespace TiS.Engineering.InputApi
{
    /// <summary>
    /// Contains all the enumerrators defined for the API.
    /// </summary>
#if INTERNAL
    public class CCEnums
#else
    public class CCEnums
#endif
    {
        #region class enums

        #region "CCNames" enum
        /// <summary>
        /// The names \ strings used in this classs
        /// </summary>
        internal enum CCNames
        {
            Default,
            ExtProc,
            ReportMessage,
            Settings,
            SourceName,
            SourcePageID
        }; 
        #endregion

        #region "CompareTypeEnm" enum
        /// <summary>
        /// All the sorting possibilities enum.
        /// </summary>
        public enum CompareTypeEnm
        {
            None = 0,
            Alpha,
            FileAccessedDate,
            FileCreationDate,
            FileExtension,
            FileModifiedDate,
            FileName,
            FilePath,
            FileSize,
            Length,
            Numeric,
            NumericDecimal
        };
        #endregion

        #region "CCFilesExt" enm
        /// <summary>
        /// The extensions used in the API.
        /// </summary>
        internal enum CCFilesExt
        {
            DIF,
            EFI,
            JPG,
            PRD,
            REG,
            TIF,
            XML
        }; 
        #endregion

        #region "CCErrorCodes" enum
        /// <summary>
        /// The error codes deined for the API to be used as string or as the code value.
        /// </summary>
        public enum CCErrorCodes
        {
            E0000 = 0,
            E0001 = 1,
            E0010 = 10,
            E0020 = 20,
            E0030 = 30,
            E0040 = 40,
            E0041 = 41,
            E0050 = 50,
            E0051 = 51,
            E0060 = 60,
            E0061 = 61,
            E0065 = 65,
            E0070 = 70,
            E0080 = 80,
            E0090 = 90,
            E0091 = 91,
            E0092 = 92,
            E0100 = 100,
            E0101 = 101,
            E0102 = 102,
            E0200 = 200,
            E0201 = 201,
            E0202 = 202,
            E0210 = 210,
            E0211 = 211,
            E0212 = 212,
            E0220 = 220,
            E0221 = 221,
            E0222 = 222,
            E0223 = 223,
            E0230 = 230,
            E0231 = 231,
            E0232 = 232,
            E0233 = 233,
            E0234 = 234,
            E0235 = 235,
            E0236 = 236,
            E0237 = 237,
            E0238 = 238,
            E0239 = 239,
            E0240 = 240
        };
        #endregion

        #region "CCHedaerDataType" enum
        /// <summary>
        /// DataTable parse, header data key values
        /// </summary>
        public enum CCHedaerDataType
        {
            AbsolutePriority,
            ApplicationName,
            Attachments,
            BatchType,
            DeviceID,
            EndDate,
            FlowType,
            ImagePath,
            MachineID,
            MetaData,
            Name,
            PageType,
            Priority,
            StartDate,
            StationName,
            TotalPagesInBatch,
            UserID,
            XmlPrdPath
        }; 
        #endregion

        #region "CCPagesDataType" enum
        /// <summary>
        /// DataTable parse, page\\s data key values
        /// </summary>
        public enum CCPagesDataType
        {
            Attachments,
            EndDate,
            MetaData,
            PageType,
            StartDate,
            XmlPrdPath
        }; 
        #endregion

        #region "CCTableColumns" enum
        /// <summary>
        /// DataTable parser, the table to parse column names.
        /// </summary>
        public enum CCTableColumns
        {
            Level,
            DataType,
            Key,
            Data            
        }; 
        #endregion

        #region "CCUpdateState" enum
        /// <summary>
        /// Defines the update mode\state
        /// </summary>
        public enum CCUpdateState
        {
            CreateNew,
            CreateOrUpdate,
            UpdateOnly
        }; 
        #endregion
        #endregion
    }
}
