using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;

namespace TiS.Engineering.InputApi.Helpers
{
    #region "CCTDataRows" class
    /// <summary>
    /// A clss that loads and converts CSV or UserInput data to CSV and or DataTable.
    /// </summary>
    public class CCTDataRows : CCGenericSerializer
    {
        #region class variables
        protected List<CCTRow> rows;
        #endregion

        #region class ctors'
        public CCTDataRows() { }

        public CCTDataRows(params DataRow[] dataRows)
            : this()
        {
            foreach (DataRow dRow in dataRows)
            {
                if (dRow != null)
                {
                    this.Add(new CCTRow(dRow));
                }
            }
        }

        public CCTDataRows(DataTable table)
            : this(table.Select())
        {
        }
        #endregion

        #region "Count" property
        /// <summary>
        /// Get the number of rows stored in this class.
        /// </summary>
        [XmlIgnore, ReadOnly(true), Browsable(false), Description("Get the number of rows stored in this class.")]
        public virtual int Count
        {
            get { return rows != null ? rows.Count : 0; }
        }
        #endregion

        #region "Rows" property
        /// <summary>
        /// The collection creator data rows stored in this class.
        /// </summary>
        [Description("The collection creator data rows stored in this class."), Category("Data")]
        public virtual CCTRow[] Rows
        {
            get
            {
                if (rows == null) rows = new List<CCTRow>();
                return rows.ToArray();
            }

            set
            {
                rows = new List<CCTRow>();
                if (value != null) rows.AddRange(value);
            }
        }
        #endregion

        #region "Add" function
        /// <summary>
        /// Add an item.
        /// </summary>
        /// <param name="row">The item to add.</param>
        /// <returns>The row index when successfull, -1 when failed.</returns>
        public virtual int Add(CCTRow row)
        {
            if (row != null)
            {
                if (rows == null) rows = new List<CCTRow>();
                row.parent = this;
                rows.Add(row);
                return rows.Count - 1;
            }
            return -1;
        }
        #endregion

        #region "Clear" method
        /// <summary>
        /// Clear all items stored in this class.
        /// </summary>
        public virtual void Clear()
        {
            rows = new List<CCTRow>();
        }
        #endregion

        #region "Remove" method
        /// <summary>
        /// Remove the specified item from the items stored in this class.
        /// </summary>
        /// <param name="row"></param>
        public virtual void Remove(CCTRow row)
        {
            if (row != null && rows != null) rows.Remove(row);
        }
        #endregion

        #region "RemoveAt" method
        /// <summary>
        /// Remove the specified item index from the items stored in this class.
        /// </summary>
        /// <param name="index"></param>
        public virtual void RemoveAt(int index)
        {
            if (index >= 0 && rows != null && rows.Count > index) rows.RemoveAt(index);
        }
        #endregion

        #region "ToCCCollection" function
        /// <summary>
        /// Convert the class data to a CCCOllection (the internal object used to create collections).
        /// </summary>
        /// <param name="cfg">The configuration to use.</param>
        /// <param name="errCode">Returns the erroro code for this function.</param>
        /// <param name="errMsg">Will contain all the errors that this function has encountered</param>
        /// <param name="copySourceFiles">Copy source files</param>
        /// <param name="createdFiles">A list of files created durning process. (like XML to PRD)</param>
        /// <returns>A CCCollection class when successfull, null when not.</returns>
        internal virtual CCCollection ToCCCollection(CCConfiguration.CCConfigurationData cfg, out int errCode, out String errMsg, bool copySourceFiles, ref   String[] lstFiles)
        {
            if (Count > 0)
            {
                return CCDataTable.FromDataTable(cfg, out errCode, out errMsg, copySourceFiles, out lstFiles, ToDataTable());
            }
            else
            {
                errCode = (int)CCEnums.CCErrorCodes.E0040;
                errMsg = "No header information";
                return null;
            }
        } 
        #endregion

        #region "ToCsvFile" functions
        /// <summary>
        /// Write the contenst of the class as a CSV file.
        /// </summary>
        /// <param name="filePath">The path of the file to save.</param>
        /// <param name="rowDelimiter">The row (line) delimiter.</param>
        /// <param name="columnDelimiter">The column (inline\items) delimiter.</param>
        /// <returns>true when successfull..</returns>
        public virtual bool ToCsvFile(String filePath)
        {
            return ToCsvFile(filePath, "\r\n", ",");// this.RowDelimiter, this.ColumnDelimiter);
        }

        /// <summary>
        /// Write the contenst of the class as a CSV file.
        /// </summary>
        /// <param name="filePath">The path of the file to save.</param>
        /// <param name="rowDelimiter">The row (line) delimiter.</param>
        /// <param name="columnDelimiter">The column (inline\items) delimiter.</param>
        /// <returns>true when successfull..</returns>
        public virtual bool ToCsvFile(String filePath, String rowDelimiter, String columnDelimiter)
        {
            try
            {
                //-- Validate input and data existance --\\
                if (String.IsNullOrEmpty(filePath) || this.Count <= 0) return false;

                //-- Remove previous file, and create target folder if necessary --\\
                if (File.Exists(filePath)) File.Delete(filePath);
                if (!Directory.Exists(Path.GetDirectoryName(filePath))) Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                //-- Write the file contents and return true whne target file was created --\\
                File.WriteAllText(filePath, this.ToString(rowDelimiter ?? "\r\n", columnDelimiter ?? ","));
                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return false;
        }
        #endregion

        #region "ToDataTable" function
        /// <summary>
        /// Get the contents of this class as DataTable.
        /// </summary>
        /// <returns></returns>
        public virtual DataTable ToDataTable()
        {
            try
            {
                if (this.Count > 0)
                {
                    DataTable res = new DataTable(String.Format("{0}_{1:yyyy-MM-dd_HHmmss}", this.GetType().Name, DateTime.Now));

                    //-- Create table columns --\\
                    res.Columns.Add(CCEnums.CCTableColumns.Level.ToString(), typeof(string));//-- Item level column --\\
                    res.Columns.Add(CCEnums.CCTableColumns.DataType.ToString(), typeof(string));//-- Item data-type column --\\
                    res.Columns.Add(CCEnums.CCTableColumns.Key.ToString(), typeof(string));//-- item key column --\\
                    res.Columns.Add(CCEnums.CCTableColumns.Data.ToString(), typeof(string));//-- Item data\value  column --\\

                    //-- Add data rows --\\
                    foreach (CCTRow crt in this.rows)
                    {
                        res.Rows.Add(crt.ToArray());
                    }
                    return res;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return null;
        }
        #endregion

        #region "ToList" function
        /// <summary>
        /// Get the contents of this class as List of arrays (4 column rows).
        /// </summary>
        /// <returns>A list of row arrays when successfull, null when not.</returns>
        public virtual List<String[]> ToList()
        {
            try
            {
                if (this.Count > 0)
                {
                    List<String[]> res = new List<String[]>();

                    //-- Add data rows --\\
                    foreach (CCTRow crt in this.rows)
                    {
                        res.Add(crt.ToArray());
                    }
                    return res;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return null;
        }
        #endregion

        #region "ToString" function
        /// <summary>
        /// Get the contents of this class as a delimited string
        /// </summary>
        /// <returns>A delimited when successfull (and the class contains row items), null when not.</returns>
        public virtual String ToString(String rowDelimiter, String columnDelimiter)
        {
            try
            {
                if (this.Count > 0)
                {
                    List<String> res = new List<String>();

                    //-- Add data rows --\\
                    foreach (CCTRow crt in this.rows)
                    {
                        res.Add(String.Join(columnDelimiter, crt.ToArray()));
                    }
                    return String.Join(rowDelimiter, res.ToArray());
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return base.ToString();
        }

        /// <summary>
        /// Get the contents of this class as a comma delimited string
        /// </summary>
        public override string ToString()
        {
            //            try { return ToString(this.RowDelimiter?? "\r\n", this.ColumnDelimiter??","); }
            try { return ToString("\r\n", ","); }
            catch { }
            return base.ToString();
        }
        #endregion

        #region "FromCsvFile" function
        /// <summary>
        /// Load\\create the cloass from a CSV file.
        /// </summary>
        /// <param name="filePath">The path to the CSV file.</param>
        /// <param name="columnDelimiters">The column delimiter\\s</param>
        /// <returns>A CCTDataRows class when successfull, null when not.</returns>
        public static CCTDataRows FromCsvFile(String filePath, params String[] columnDelimiters)
        {
            return FromCsvFile(filePath, new String[] { "\r\n" }, columnDelimiters);
        }

        /// <summary>
        /// Load\\create the cloass from a CSV file.
        /// </summary>
        /// <param name="filePath">The path to the CSV file.</param>
        /// <param name="rowDelimiters">The row\\line delimiters.</param>
        /// <param name="columnDelimiters">The column delimiter\\s</param>
        /// <returns>A CCTDataRows class when successfull, null when not.</returns>
        public static CCTDataRows FromCsvFile(String filePath, String[] rowDelimiters, params String[] columnDelimiters)
        {
            try
            {
                String[] lines = File.ReadAllText(filePath).Split(rowDelimiters, StringSplitOptions.RemoveEmptyEntries);
                if (lines != null && lines.Length > 0)
                {
                    CCTDataRows res = new CCTDataRows();
                    res.rows = new List<CCTRow>();
                    foreach (String s in lines)
                    {
                        if (!String.IsNullOrEmpty(s))
                        {
                            res.rows.Add(CCTRow.FromString(s, columnDelimiters));
                        }
                    }

                    return res;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return null;
        }
        #endregion

        #region "FromXml" function
        /// <summary>
        /// Return a 'CCTDataRows' class from an XML file path.
        /// </summary>
        /// <param name="xmlFile">The XML file path to deserialize.</param>
        /// <returns>A CCTDataRows when successfull, null when not.</returns>
        public static new CCTDataRows FromXml(String xmlFile)
        {
            return FromXml(xmlFile, typeof(CCTDataRows)) as CCTDataRows;
        }
        #endregion

        #region "FromXmlString function
        /// <summary>
        /// Return a 'CCTDataRows' class from an XML string.
        /// </summary>
        /// <param name="xmlStr">The XML string to deserialize</param>
        /// <returns>A CCTDataRows when successfull, null when not.</returns>
        public static new CCTDataRows FromXmlString(String xmlStr)
        {
            return FromXmlString(xmlStr, typeof(CCTDataRows)) as CCTDataRows;
        }
        #endregion

        #region "FromDataTable" function
        /// <summary>
        /// Get the contents of this class as DataTable.
        /// </summary>
        /// <param name="table">The table to load it's contents.</param>
        /// <returns>CCTDataRows class when successfull.</returns>
        public static CCTDataRows FromDataTable(DataTable table)
        {
            return new CCTDataRows(table.Select());

        }
        #endregion

        #region "FromDataRows" function
        /// <summary>
        /// Create \\ convert a DataRows to a  <see cref="CCTDataRows"/> class
        /// </summary>
        /// <param name="dataRows">The Datarows to use to initialize the new <see cref="DataRow"/> class with.</param>
        /// <returns>CCTDataRows class when successfull.</returns>
        public static CCTDataRows FromDataRows(params DataRow[] dataRows)
        {
            return new CCTDataRows(dataRows);
        }
        #endregion

        #region "CCTRow" class
        /// <summary>
        /// CCTRow class (one CSV collection creation table data row).
        /// </summary>
        public class CCTRow : CCGenericSerializer
        {
            #region class variables and constants.
            internal const string CAT = "Data row";
            internal CCTDataRows parent;
            #endregion

            #region class properties

            #region "Level" property
            private uint level;
            /// <summary>
            /// The level of the data to set, where 0 is collection\\document level and 1-N is page\\from level.
            /// </summary>
            [Description("The level of the data to set, where 0 is collection\\document level and 1-N is page\\from level."), Category(CAT), XmlAttribute()]
            public virtual uint Level { get { return level; } set { level = value; } }
            #endregion

            #region "DataType" property
            private CCEnums.CCHedaerDataType dataType;
            /// <summary>
            /// The data type that this data row contains.
            /// </summary>
            [Description("The data type that this data row contains."), Category(CAT), XmlAttribute()]
            public virtual CCEnums.CCHedaerDataType DataType { get { return dataType; } set { dataType = value; } }
            #endregion

            #region "Key" property
            /// <summary>
            /// The data key (Applies only when 'DataType' is of 'MetaData')\r\nIn eFLOW terms these will create NamedUserTags on the specified level, all page level NamedUserTags will also be copied to form level.
            /// </summary>
            private String key;
            [Description("The data key (Applies only when 'DataType' is of 'MetaData')\r\nIn eFLOW terms these will create NamedUserTags on the specified level, all page level NamedUserTags will also be copied to form level."),
            Category(CAT), XmlAttribute()]
            public virtual String Key { get { return key; } set { key = value; } }
            #endregion

            #region "Data" property
            /// <summary>
            /// The data value to set.
            /// </summary>
            private String data;
            [Description("The data value to set."), Category(CAT), XmlAttribute()]
            public virtual String Data { get { return data; } set { data = value; } }
            #endregion

            #region "Parent" property
            /// <summary>
            /// The level of the data to set, where 0 is collection\\document level and 1-N is page\\from level.
            /// </summary>
            [XmlIgnore, Browsable(false), ReadOnly(true), Category("Runtime"),
            Description("The parent list object.")]
            public virtual CCTDataRows Parent
            {
                get { return parent; }
            }
            #endregion
            #endregion

            #region class ctors'
            public CCTRow() { }

            public CCTRow(uint level, CCEnums.CCHedaerDataType dataType, String key, object data)
                : this()
            {
                Level = level;
                DataType = dataType;
                Key = key;
                Data = data != null ? data.ToString() : String.Empty;
            }

            public CCTRow(String source, params string[] delimiters)
                : this()
            {
                String[] splt = source.Split(delimiters, StringSplitOptions.None);
                uint lvl = 0;
                String rowVal = splt[0] ?? String.Empty;

                if (uint.TryParse(rowVal, out lvl))
                {
                    CCEnums.CCHedaerDataType dtTp = (CCEnums.CCHedaerDataType)Enum.Parse(typeof(CCEnums.CCHedaerDataType), splt[1] ?? String.Empty, true);

                    Level = lvl;
                    DataType = dtTp;
                    Key = splt[2];
                    Data = splt[3];
                }
                else throw new Exception("Failed parsing level as unsinged integer, value:" + rowVal);
            }

            public CCTRow(DataRow dataRow)
                : this()
            {
                if (dataRow != null && dataRow.ItemArray.Length > 3)
                {
                    uint iLvl = 0;
                    String rowVal = dataRow.ItemArray[0].ToString() ?? String.Empty;
                    if (uint.TryParse(rowVal, out iLvl))
                    {
                        Level = iLvl;
                        DataType = (CCEnums.CCHedaerDataType)Enum.Parse(typeof(CCEnums.CCHedaerDataType), dataRow.ItemArray[1].ToString(), true);
                        Key = dataRow.ItemArray[2].ToString();
                        Data = dataRow.ItemArray[3].ToString();
                    }
                    else throw new Exception("Failed parsing level as unsinged integer, value: " + rowVal);
                }
            }

            public CCTRow(params String[] sourceRow)
            {
                uint lvl = 0;
                String rowVal = sourceRow[0] ?? String.Empty;

                if (uint.TryParse(rowVal, out lvl))
                {
                    Level = lvl;
                    DataType = (CCEnums.CCHedaerDataType)Enum.Parse(typeof(CCEnums.CCHedaerDataType), sourceRow.Length > 1 ? sourceRow[1] ?? String.Empty : String.Empty, true);
                    Key = sourceRow.Length > 2 ? sourceRow[2] : String.Empty;
                    Data = sourceRow.Length > 3 ? sourceRow[3] : String.Empty;
                }
                else throw new Exception("Failed parsing level as unsinged integer, value: " + rowVal);
            }
            #endregion

            #region "ToArray" function
            /// <summary>
            /// Return the contents of this data row as a string array.
            /// </summary>
            /// <returns></returns>
            public virtual String[] ToArray()
            {
                try
                {
                    return new String[] { Level.ToString(), DataType.ToString(), Key ?? String.Empty, Data ?? String.Empty };
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                return null;
            }
            #endregion

            #region "ToString" functions
            /// <summary>
            /// Return the contents of this class as a delimited string.
            /// </summary>
            /// <param name="delimiter">the delimiter to use inbetween values.</param>
            /// <returns>A delimited string.</returns>
            public virtual string ToString(String delimiter)
            {
                return String.Format("{0}{4}{1}{4}{2}{4}{3}", Level, DataType, Key ?? String.Empty, Data ?? String.Empty, delimiter ?? ", ");
            }

            /// <summary>
            /// Return the contents of this class as a comma delimited string.
            /// </summary>
            public override string ToString()
            {
                try { return ToString(", "); }
                catch { }
                return base.ToString();
            }
            #endregion

            #region "FromDataRow" functions
            /// <summary>
            /// Creates a CCTRow from a string, specify the delimiters ti be used.
            /// </summary>
            /// <param name="dataRow">The source  data row.</param>
            /// <returns>A CCTRow when successfull, null when not.</returns>
            public static CCTRow FromDataRow(DataRow dataRow)
            {
                return new CCTRow(dataRow);
            }
            #endregion

            #region "FromString" functions
            /// <summary>
            /// Creates a CCTRow from a string, specify the delimiters ti be used.
            /// </summary>
            /// <param name="source">The source string to extract it's row data.</param>
            /// <param name="delimiters">The source delimiters to use.</param>
            /// <returns>A CCTRow when successfull, null when not.</returns>
            public static CCTRow FromString(String source, params string[] delimiters)
            {
                return new CCTRow(source, delimiters);
            }

            /// <summary>
            /// Creates a CCTRow from a string array.
            /// </summary>
            /// <param name="sourceRow">The source string array to extract it's row data (4 items at maximum).</param>
            /// <returns>A CCTRow when successfull, null when not.</returns>
            public static CCTRow FromString(params string[] sourceRow)
            {
                return new CCTRow(sourceRow);
            }
            #endregion
        }
        #endregion
    }
    #endregion
}
