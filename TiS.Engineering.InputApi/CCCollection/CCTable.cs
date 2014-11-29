using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TiS.Core.eFlowAPI;
using System.ComponentModel;

namespace TiS.Engineering.InputApi
{
    partial class CCCollection
    {
        #region "CCTable" class
        /// <summary>
        /// Field table class.
        /// </summary>
        public class CCTable : CCEflowObject, ICCTable
        {
            #region class variables
            private List<CCFieldArray> fieldArrays;
            #endregion

            #region class properties
            #region "LinkedFields" property
            /// <summary>
            /// Get the fields linked to this table
            /// </summary>
            [XmlIgnore, Description("Get the fields linked to this table.")]
            public virtual CCField[] LinkedFields
            {
                get
                {
                    List<CCField> flds = new List<CCField>();
                    foreach (CCFieldArray cca in fieldArrays)
                    {
                        flds.AddRange(cca.LinkedFields);
                    }
                    return flds.ToArray();
                }
            }
            #endregion

            #region "NumberOfRows" property
            /// <summary>
            /// Get the number of rows in this table.
            /// </summary>
            [XmlIgnore, Description("Get the number of rows in this table.")]
            public virtual int NumberOfRows
            {
                get
                {
                    foreach (CCFieldArray fa in FieldArrays)
                    {
                        if (fa != null && fa.LinkedFields.Length > 0) return fa.LinkedFields.Length;
                    }
                    return 0;
                }
            }
            #endregion

            #region "FieldArrays" property
            /// <summary>
            /// Get or set the FieldArrays linked to this table
            /// </summary>
            [Description("Get or set the FieldArrays linked to this table.")]
            public virtual CCFieldArray[] FieldArrays
            {
                get
                {
                    if (fieldArrays == null) fieldArrays = new List<CCFieldArray>();
                    return fieldArrays.ToArray();
                }
                set
                {
                    if (fieldArrays == null) fieldArrays = new List<CCFieldArray>();
                    else fieldArrays.Clear();

                    if (value != null) fieldArrays.AddRange(value);


                    if (fieldArrays.Count > 0)
                    {
                        this.SetParents();
                    }
                }
            }
            #endregion

            #region "ParentGroup" property
            private ITisFieldGroupData parentGroup;
            /// <summary>
            /// Get the table's parent field group.
            /// </summary>
            [XmlIgnore, Description("Get the table's parent field group.")]
            public virtual ITisFieldGroupData ParentGroup { get { return parentGroup; } set { parentGroup = value; } }
            #endregion

            #region "ParentForm" property
            private ITisFormData parentForm;
            /// <summary>
            /// Get the table's parent form.
            /// </summary>
            [XmlIgnore, Description("Get the table's parent form.")]
            public virtual ITisFormData ParentForm { get { return parentForm; } set { parentForm = value; } }
            #endregion

            #region "ParentPage" propertry
            private ITisPageData parentPage;
            /// <summary>
            /// Get the table's parent page.
            /// </summary>
            [XmlIgnore, Description("Get the table's parent page.")]
            public virtual ITisPageData ParentPage { get { return parentPage; } set { parentPage = value; } }
            #endregion
            #endregion

            #region class constructors
            public CCTable():base()
            {
            }

            public CCTable(String tableName, params CCFieldArray[] linkFieldArrays) :
                this(null, tableName ?? String.Empty, linkFieldArrays)
            {
            }

#if INTERNAL
            internal CCTable(CCreator parent, String tableName, params CCFieldArray[] linkFieldArrays) :
#else
            public CCTable(CCreator parent, String tableName, params CCFieldArray[] linkFieldArrays) :
#endif
                base(parent, tableName, null, null, null)
            {
                this.FieldArrays = linkFieldArrays;
            }

            public CCTable(ITisFieldTableData fieldTable)
                : this(null, fieldTable)
            {
            }

#if INTERNAL
            internal CCTable(CCreator parent, ITisFieldTableData fieldTable)
#else
            public CCTable(CCreator parent, ITisFieldTableData fieldTable)
#endif
                : this(parent, fieldTable.Name,
                parent == null || !parent.CurrentProfile.IgnoreExceptions ? CCUtils.GetSpecialTags(fieldTable) : null,
                parent == null || !parent.CurrentProfile.IgnoreNamedUserTags ? CCUtils.GetNamedUserTags(fieldTable,false) : null,
                parent == null || !parent.CurrentProfile.IgnoreUserTags ? CCUtils.GetUserTags(fieldTable,true) : null)
            {
                this.EflowOwner = fieldTable;
                this.ParentCollection = fieldTable.ParentCollection;
                this.ParentForm = fieldTable.ParentForm;
                this.ParentPage = fieldTable.ParentPage;
                this.ParentGroup = fieldTable.ParentFieldGroup;
                this.NamedParent = fieldTable.ParentFieldGroupExists ? fieldTable.ParentFieldGroup.Name : String.Empty;

                List<CCFieldArray> fas = new List<CCFieldArray>();

                if (fieldArrays == null) fieldArrays = new List<CCFieldArray>();
                else fieldArrays.Clear();

                foreach (ITisFieldArrayData fa in fieldTable.FieldArrays)
                {
                    fieldArrays.Add(new CCFieldArray(this.ParentCreator, fa));
                }
            }

            public CCTable(ITisFieldTableParams fieldTable, bool createSubFields)
                : this(null, fieldTable, createSubFields)
            {
            }


#if INTERNAL
            internal CCTable(CCreator parent, ITisFieldTableParams fieldTable)
#else
            public CCTable(CCreator parent, ITisFieldTableParams fieldTable)
#endif
                : this(parent, fieldTable, true)
            {
            }

#if INTERNAL
            internal CCTable(CCreator parent, ITisFieldTableParams fieldTable, bool createSubFields)
#else
            public CCTable(CCreator parent, ITisFieldTableParams fieldTable, bool createSubFields)
#endif
                : this(parent, fieldTable.Name, new Dictionary<String, String>(), null, null)
            {
                this.EflowOwner = fieldTable;
                this.NamedParent = fieldTable.ParentFieldGroupExists ? fieldTable.ParentFieldGroup.Name : String.Empty;

                if (fieldArrays == null) fieldArrays = new List<CCFieldArray>();
                else fieldArrays.Clear();

                foreach (ITisFieldParams fa in fieldTable.LinkedFields)
                {
                    for (int inr = 0; inr < Math.Max(1, (int)fieldTable.Table.NumberOfRows); inr++)
                    {
                        fieldArrays.Add(new CCFieldArray(this.ParentCreator, fa, createSubFields));
                        if (!fieldTable.Table.KeepEmptyRows) break;
                    }
                }
            }

            public CCTable( String name, Dictionary<String, String> specialTags, Dictionary<String, String> namedTags, Dictionary<String, String> userTags) :
                this(null, name, specialTags, namedTags, userTags)
            {
            }

#if INTERNAL
            internal CCTable(CCreator parent, String name, Dictionary<String, String> specialTags, Dictionary<String, String> namedTags, Dictionary<String, String> userTags) :
#else
            public CCTable(CCreator parent, String name, Dictionary<String, String> specialTags, Dictionary<String, String> namedTags, Dictionary<String, String> userTags) :
#endif
                base(parent, name??String.Empty, specialTags, namedTags, userTags)
            {
            }
            #endregion

            #region "AddFieldArray" functions
            /// <summary>
            /// Add a field array to the List of field array in the table.
            /// </summary>
            /// <param name="fieldArrayName">The name of the Field array to create.</param>
            /// <returns>A CCFieldArray when successfull, null when failed.</returns>
            public virtual CCFieldArray AddFieldArray(String fieldArrayName)
            {
                try
                {
                    if (fieldArrays == null) fieldArrays = new List<CCFieldArray>();

                    //-- Find or add a new FieldArray --\\
                    foreach (CCFieldArray cfa in fieldArrays)
                    {
                        if (String.Compare(cfa.Name, fieldArrayName, true) == 0) return cfa;
                    }

                    fieldArrays.Add(new CCFieldArray(this.ParentCreator, fieldArrayName, null));

                    this.SetParents();
                    return fieldArrays[fieldArrays.Count - 1];
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
            }

            /// <summary>
            /// Add a field array to the List of field array in the table.
            /// </summary>
            /// <param name="fieldArray">The Field array to add.</param>
            /// <returns>A CCFieldArray when successfull, null when failed.</returns>
            public virtual CCFieldArray AddFieldArray(CCFieldArray fieldArray)
            {
                try
                {
                    if (fieldArrays == null) fieldArrays = new List<CCFieldArray>();

                    //-- Find or Add a new field array --\\
                    foreach (CCFieldArray cfa in fieldArrays)
                    {
                        if (String.Compare(cfa.Name, fieldArray.Name, true) == 0) return cfa;
                    }

                    this.SetParents();
                    return fieldArray;
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
            }
            #endregion

            #region "GetField" function
            /// <summary>
            /// Get a field from the fields linked to this table, using the field name.
            /// </summary>
            /// <param name="fieldName"></param>
            /// <returns></returns>
            public CCField GetField(String fieldName)
            {
                try
                {
                    if (!String.IsNullOrEmpty(fieldName))
                    {
                        foreach (CCField fld in LinkedFields)
                        {
                            if (String.Compare(fld.Name, fieldName, true) == 0)
                            {
                                return fld;
                            }
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
            }
            #endregion

            #region "GetFieldArray" function
            /// <summary>
            /// Get a field-array from the field-arrays linked to this table, using the field-array name.
            /// </summary>
            /// <param name="fieldArrayName"></param>
            /// <returns></returns>
            public CCFieldArray GetFieldArray(String fieldArrayName)
            {
                try
                {
                    if (!String.IsNullOrEmpty(fieldArrayName))
                    {
                        foreach (CCFieldArray fldAr in FieldArrays)
                        {
                            if (String.Compare(fldAr.Name, fieldArrayName, true) == 0)
                            {
                                return fldAr;
                            }
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
            }
            #endregion

            #region "AddRow" function
            /// <summary>
            /// Add a table row .
            /// </summary>
            /// <returns></returns>
            public int AddRow()
            {
                try
                {
                    int pos = 0;
                    foreach (CCFieldArray fa in FieldArrays)
                    {
                        if (fa != null)
                        {
                            fa.AddField(fa.NextFieldName, String.Empty, 0, new TIS_RECT());
                            if (pos <= 0) pos = fa.LinkedFields.Length;
                        }
                    }
                    return pos;
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
            }
            #endregion

            #region "GetRow" function
            /// <summary>
            /// Get table row .
            /// </summary>
            /// <param name="rowIndex"></param>
            /// <returns></returns>
            public CCField[] GetRow(int rowIndex)
            {
                try
                {
                    if (rowIndex < 0) return null;

                    List<CCField> row = new List<CCField>();
                    foreach (CCFieldArray fa in FieldArrays)
                    {
                        if (fa != null && fa.LinkedFields.Length > 0 && fa.LinkedFields.Length < rowIndex)
                        {
                            row.Add(fa.LinkedFields[rowIndex]);
                        }
                    }
                    return row.ToArray();
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
            }
            #endregion

            #region "RemoveFieldArray" function
            /// <summary>
            /// Remove a FieldArray from the List of field arrays in the collection.
            /// </summary>
            /// <param name="fieldArrayIndex">The field array index to remove.</param>
            /// <returns>A true when successfull.</returns>
            public virtual bool RemoveFieldArray(int fieldArrayIndex)
            {
                try
                {
                    if (fieldArrays != null && fieldArrayIndex >= 0 && fieldArrayIndex < fieldArrays.Count)
                    {
                        fieldArrays.RemoveAt(fieldArrayIndex);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
                return false;
            }
            #endregion

            #region "SetParents" method
            /// <summary>
            /// Set the object and it's sub objects parents.
            /// </summary>
            internal void SetParents()
            {
                try
                {
                    if (fieldArrays != null)
                    {
                        foreach (CCFieldArray fa in fieldArrays)
                        {
                            fa.ParentCollection = this.ParentCollection;
                            fa.ParentCreator = this.ParentCreator;
                            fa.ParentForm = this.ParentForm;
                            fa.ParentPage = this.ParentPage;
                            fa.ParentGroup = this.ParentGroup;
                            fa.ParentTable = this.EflowOwner as ITisFieldTableData;
                            fa.CCParent = this;
                            fa.NamedParent = this.Name;
                            fa.SetParents();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
            }
            #endregion
        }
        #endregion
    }
}