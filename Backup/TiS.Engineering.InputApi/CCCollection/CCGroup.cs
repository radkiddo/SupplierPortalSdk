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
        #region "CCGroup" class
        /// <summary>
        /// Field group data class.
        /// </summary>
        public class CCGroup : CCEflowObject, ICCGroup
        {
            #region class variables
            private List<CCField> fields;
            private List<CCTable> tables;
            #endregion

            #region class properties
            private ITisFormData parentForm;
            #region "ParentForm" property
            /// <summary>
            /// The field group's parent form
            /// </summary>
            [XmlIgnore, Description("The field group's parent form.")]
            public virtual ITisFormData ParentForm { get { return parentForm; } set { parentForm = value; } }
            #endregion

            #region "ParentPage" property
            private ITisPageData parentPage;
            /// <summary>
            /// The field group's parent page
            /// </summary>
            [XmlIgnore, Description("The field group's parent page.")]
            public virtual ITisPageData ParentPage { get { return parentPage; } set { parentPage = value; } }
            #endregion

            #region "LinkedFields" property
            /// <summary>
            /// The field group's linked fields (i.e. fields that linked directly to the field group).
            /// </summary>
            [Description("The field group's linked fields (i.e. fields that linked directly to the field group).")]
            public virtual CCField[] LinkedFields
            {
                get
                {
                    if (fields == null) fields = new List<CCField>();
                    return fields.ToArray();
                }
                set
                {
                    if (fields == null) fields = new List<CCField>();
                    else fields.Clear();

                    if (value != null) fields.AddRange(value);

                    if (fields.Count > 0)
                    {
                        this.SetParents();
                    }

                }
            }
            #endregion

            #region "Fields" property
            /// <summary>
            /// The field group's fields (all fields, those that are linked directly to the group and these that are linked to tables).
            /// </summary>
            [XmlIgnore, Description("The field group's fields (all fields, those that are linked directly to the group and these that are linked to tables).")]
            public virtual CCField[] Fields
            {
                get
                {
                    List<CCField> flds = new List<CCField>(fields);
                    foreach (CCTable ft in LinkedTables)
                    {
                        flds.AddRange(ft.LinkedFields);
                    }
                    return flds.ToArray();
                }
            }
            #endregion

            #region "LinkedTables" property
            /// <summary>
            /// The field tables linked to the field group.
            /// </summary>
            [Description("The field tables linked to the field group.")]
            public virtual CCTable[] LinkedTables
            {
                get
                {
                    if (tables == null) tables = new List<CCTable>();
                    return tables.ToArray();
                }
                set
                {
                    if (tables == null) tables = new List<CCTable>();
                    else tables.Clear();

                    if (value != null) tables.AddRange(value);

                    if (tables.Count > 0)
                    {
                        this.SetParents();
                    }
                }
            }
            #endregion
            #endregion

            #region class constructors
            public CCGroup()
            {
            }

            public CCGroup( String groupName, params CCField[] setFields) :
                this(null, groupName ?? String.Empty, setFields)
            {
            }
        

#if INTERNAL
            internal CCGroup(CCreator parent, String groupName, params CCField[] setFields) :
#else
            public CCGroup(CCreator parent, String groupName, params CCField[] setFields) :
#endif
                this(parent, groupName ?? String.Empty, setFields, null)
            {
                this.LinkedFields = setFields;
            }


            public CCGroup(String groupName, CCField[] setFields, params CCTable[] setTables) :
                this(null, groupName ?? String.Empty, setFields, setTables)
            {
            }

#if INTERNAL
            internal CCGroup(CCreator parent, String groupName, CCField[] setFields, params CCTable[] setTables) :
#else
            public CCGroup(CCreator parent, String groupName, CCField[] setFields, params CCTable[] setTables) :
#endif
                this(parent, groupName ?? String.Empty, setFields)
            {
                this.LinkedTables = setTables;
            }


            public CCGroup(String groupName, params CCTable[] setTables) :
                this(null, groupName ?? String.Empty, null, setTables)
            {
            }

#if INTERNAL
            internal CCGroup(CCreator parent, String groupName, params CCTable[] setTables) :
#else
            public CCGroup(CCreator parent, String groupName, params CCTable[] setTables) :
#endif
                this(parent, groupName ?? String.Empty, null, setTables)
            {
            }


            public CCGroup(ITisFieldGroupData group)
                : this(null, group)
            {
            }

#if INTERNAL
            internal CCGroup(CCreator parent, ITisFieldGroupData group)
#else
            public CCGroup(CCreator parent, ITisFieldGroupData group)
#endif
                : base(parent, group!=null? group.Name:String.Empty,
                parent == null || !parent.CurrentProfile.IgnoreExceptions ? CCUtils.GetSpecialTags(group) : null,
                parent == null || !parent.CurrentProfile.IgnoreNamedUserTags ? CCUtils.GetNamedUserTags(group,false) : null,
                parent == null || !parent.CurrentProfile.IgnoreUserTags ? CCUtils.GetUserTags(group,true) : null)
            {
                try
                {
                    if (fields == null) fields = new List<CCField>();
                    else fields.Clear();

                    if (tables == null) tables = new List<CCTable>();
                    else tables.Clear();

                    this.EflowOwner = group;
                    this.ParentCollection = group.ParentCollection;
                    this.ParentForm = group.ParentForm;
                    this.NamedParent = group.ParentForm.Name;

                    List<String> fieldNames = new List<String>();
                    foreach (ITisFieldData fld in group.LinkedFields)
                    {
                        if (fieldNames.Contains(fld.Name.ToUpper())) continue;
                        if (this.ParentPage == null) this.ParentPage = fld.ParentPage;
                        fields.Add(new CCField(this.ParentCreator, fld));
                        fieldNames.Add(fld.Name);
                    }

                    foreach (ITisFieldTableData fldTbl in group.LinkedFieldTables)
                    {
                        if (this.ParentPage == null) this.ParentPage = fldTbl.ParentPage;
                        tables.Add(new CCTable(this.ParentCreator, fldTbl));
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
            }


            public CCGroup(ITisFieldGroupParams group, bool createFields)
                : this(null, group, createFields)
            {
            }

#if INTERNAL
            internal CCGroup(CCreator parent, ITisFieldGroupParams group)
#else
            public CCGroup(CCreator parent, ITisFieldGroupParams group)
#endif
                : this(parent, group, true)
            {
            }

#if INTERNAL
            internal CCGroup(CCreator parent, ITisFieldGroupParams group, bool createFields)
#else
            public CCGroup(CCreator parent, ITisFieldGroupParams group, bool createFields)
#endif
                : base(parent, group.Name, null, null, null)
            {
                try
                {
                    if (fields == null) fields = new List<CCField>();
                    else fields.Clear();

                    if (tables == null) tables = new List<CCTable>();
                    else tables.Clear();

                    this.EflowOwner = group;
                    this.NamedParent = group.ParentForm.Name;

                    if (createFields)
                    {
                        List<String> fieldNames = new List<String>();
                        foreach (ITisFieldParams fld in group.LinkedFields)
                        {
                            if (fieldNames.Contains(fld.Name.ToUpper())) continue;
                            fields.Add(new CCField(this.ParentCreator, fld));
                            fieldNames.Add(fld.Name);
                        }

                        foreach (ITisFieldTableParams fldTbl in group.LinkedFieldTables)
                        {
                            tables.Add(new CCTable(this.ParentCreator, fldTbl));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
            }
            #endregion

            #region "GetTable" function
            /// <summary>
            /// Get a table from the table array, using the table name.
            /// </summary>
            /// <param name="tableName"></param>
            /// <returns></returns>
            public CCTable GetTable(String tableName)
            {
                try
                {
                    if (!String.IsNullOrEmpty(tableName))
                    {
                        foreach (CCTable tbl in LinkedTables)
                        {
                            if (String.Compare(tbl.Name, tableName, true) == 0)
                            {
                                return tbl;
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

            #region "GetField" function
            /// <summary>
            /// Get a field from the fields linked to this group, using the field name.
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

            #region "AddTable" functions
            /// <summary>
            /// Add a table to the List of tables of the group.
            /// </summary>
            /// <param name="tableName">The name of the table to create.</param>
            /// <returns>A CCTable when successfull, null when failed.</returns>
            public virtual CCTable AddTable(String tableName)
            {
                try
                {
                    if (tables == null) tables = new List<CCTable>();

                    //-- Add a new Table --\\
                    foreach (CCTable tbl in tables)
                    {
                        if (String.Compare(tbl.Name, tableName, true) == 0) return tbl;
                    }

                    tables.Add(new CCTable(this.ParentCreator, tableName, new CCFieldArray[0]));

                    this.SetParents();
                    return tables[tables.Count - 1];
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
            }

            /// <summary>
            /// Add a table to the List of tables of the group.
            /// </summary>
            /// <param name="table">The table to add.</param>
            /// <returns>A CCTable when successfull, null when failed.</returns>
            public virtual CCTable AddTable(CCTable table)
            {
                try
                {
                    if (tables == null) tables = new List<CCTable>();

                    //-- Add a new Table --\\
                    foreach (CCTable tbl in tables)
                    {
                        if (String.Compare(tbl.Name, table.Name, true) == 0) return tbl;
                    }

                    table.CCParent = this;
                    tables.Add(table);

                    this.SetParents();

                    return table;
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
            }
            #endregion

            #region "AddField" functions
            /// <summary>
            /// Add a field to the groups' List of fields.
            /// </summary>
            /// <param name="fieldName">The name of the Field to create.</param>
            /// <returns>A CCField when successfull, null when failed.</returns>
            public virtual CCField AddField(ITisFieldParams field, TIS_RECT fieldRect)
            {
                try
                {
                    if (fields == null) fields = new List<CCField>();

                    //-- Add a new field --\\
                    foreach (CCField cf in fields)
                    {
                        if (String.Compare(cf.Name, field.Name, true) == 0) return cf;
                    }

                    if (field.ParentFieldTableExists)
                    {
                       // TODO: finish testing here.
                        //\\ bool createTable = true;
                        // CCTable = this.tables.IndexOf(
                        foreach (CCTable tbl in this.tables)
                        {
                            if (String.Compare(tbl.Name, field.ParentFieldTable.Name, true) == 0)
                            {
                                CCFieldArray cfa = tbl.AddFieldArray(field.Name);
                                cfa.AddField(field, fieldRect);
                            }
                        }
                    }
                    else
                    {
                        fields.Add(new CCField(this.ParentCreator, field.Name, String.Empty, 0, fieldRect, null, null, null));
                    }

                    fields[fields.Count - 1].CCParent = this;

                    this.SetParents();
                    return fields[fields.Count - 1];
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
            }

            /// <summary>
            /// Add a field to the groups' List of fields.
            /// </summary>
            /// <param name="fieldName">The name of the Field to create.</param>
            /// <returns>A CCField when successfull, null when failed.</returns>
            public virtual CCField AddField(String fieldName, String contents, short confidence, TIS_RECT fieldRect)
            {
                try
                {
                    if (fields == null) fields = new List<CCField>();

                    //-- Add a new field --\\
                    foreach (CCField cf in fields)
                    {
                        if (String.Compare(cf.Name, fieldName, true) == 0) return cf;
                    }

                    CCField fld = new CCField(this.ParentCreator, fieldName, contents, confidence, fieldRect, null, null, null);
                    fields.Add(fld);

                    this.SetParents();
                    return fld;
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
            }

            /// <summary>
            /// Add a field to the groups' List of fields.
            /// </summary>
            /// <param name="field">The CCField to add 9if a field with the same does not exist)..</param>
            public virtual CCField AddField(CCField field)
            {
                try
                {
                    if (fields == null) fields = new List<CCField>();

                    if (field != null)
                    {
                        foreach (CCField fld in fields)
                        {
                            if (String.Compare(fld.Name, field.Name, true) == 0)
                            {
                                return fld;
                            }
                        }

                        fields.Add(field);
                        this.SetParents();
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                return field;
            }
            #endregion

            /// <summary>
            /// return groups data from page 
            /// </summary>
            /// <param name="creator"></param>
            /// <param name="pageParams"></param>
            /// <param name="pageID"></param>
            /// <returns></returns>
            internal static CCGroup[] GroupsFromPage(CCreator creator, ITisPageParams pageParams, String pageID)
            {
                List<CCGroup> ccGrps = new List<CCGroup>();

                try
                {
                    List<ITisFieldGroupParams> fldGroupParams = new List<ITisFieldGroupParams>();
                    List<String> groupNames = new List<String>();

                    if (!String.IsNullOrEmpty(pageID))
                    {
                        ITisEFIParams efi = pageParams.get_LinkedEFI(pageID);
                        foreach (ITisROIParams roi in efi.ROIs)
                        {
                            if (roi.Miscellaneous.OrderInField <= 1)
                            {
                                ITisFieldParams fp = pageParams.get_LinkedField(roi.Miscellaneous.FieldName);
                                if (fp != null)
                                {
                                    if (fp.ParentFieldGroupExists)
                                    {
                                        if (groupNames.Contains(fp.ParentFieldGroup.Name.ToUpper())) continue;
                                        groupNames.Add(fp.ParentFieldGroup.Name.ToUpper());
                                        ccGrps.Add(new CCGroup(creator, fp.ParentFieldGroup));
                                    }
                                    else if (fp.ParentFieldTableExists)
                                    {
                                        if (groupNames.Contains(fp.ParentFieldGroup.Name.ToUpper())) continue;
                                        groupNames.Add(fp.ParentFieldTable.ParentFieldGroup.Name.ToUpper());
                                        ccGrps.Add(new CCGroup(creator, fp.ParentFieldTable.ParentFieldGroup));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (ITisFieldParams fldParams in pageParams.LinkedFields)
                        {
                            if (fldParams.ParentFieldGroupExists)
                            {
                                if (!fldGroupParams.Contains(fldParams.ParentFieldGroup))
                                {
                                    fldGroupParams.Add(fldParams.ParentFieldGroup);
                                }
                            }
                            else if (fldParams.ParentFieldTableExists && fldParams.ParentFieldTable.ParentFieldGroupExists)
                            {
                                if (!fldGroupParams.Contains(fldParams.ParentFieldTable.ParentFieldGroup))
                                {
                                    fldGroupParams.Add(fldParams.ParentFieldTable.ParentFieldGroup);
                                }
                            }
                        }
                    }

                    foreach (ITisFieldGroupParams fldGrp in fldGroupParams)
                    {
                        ccGrps.Add(new CCGroup(creator, fldGrp));
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                return ccGrps.ToArray();
            }

            #region "RemoveField" function
            /// <summary>
            /// Remove a Field from the List of fields of the group.
            /// </summary>
            /// <param name="fieldArrayIndex">The field index to remove.</param>
            /// <returns>true when successfull.</returns>
            public virtual bool RemoveField(int fieldIndex)
            {
                try
                {
                    if (fields != null && fieldIndex >= 0 && fieldIndex < fields.Count)
                    {
                        fields.RemoveAt(fieldIndex);
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

            #region "RemoveTable" function
            /// <summary>
            /// Remove a table from the List of tables of the group.
            /// </summary>
            /// <param name="fieldArrayIndex">The table index to remove.</param>
            /// <returns>A true when successfull.</returns>
            public virtual bool RemoveTable(int tableIndex)
            {
                try
                {
                    if (tables != null && tableIndex >= 0 && tableIndex < tables.Count)
                    {
                        tables.RemoveAt(tableIndex);
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
                    if (fields != null)
                    {
                        foreach (CCField fd in fields)
                        {
                            fd.ParentCollection = this.ParentCollection;
                            fd.ParentCreator = this.ParentCreator;
                            fd.ParentForm = this.ParentForm;
                            fd.ParentPage = this.ParentPage;
                            fd.ParentGroup = this.EflowOwner as ITisFieldGroupData;
                            fd.CCParent = this;
                            fd.NamedParent = this.Name;
                        }
                    }

                    if (tables != null)
                    {
                        foreach (CCTable tb in tables)
                        {
                            tb.ParentCollection = this.ParentCollection;
                            tb.ParentCreator = this.ParentCreator;
                            tb.ParentForm = this.ParentForm;
                            tb.ParentPage = this.ParentPage;
                            tb.ParentGroup = this.EflowOwner as ITisFieldGroupData;
                            tb.CCParent = this;
                            tb.NamedParent = this.Name;
                            tb.SetParents();
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