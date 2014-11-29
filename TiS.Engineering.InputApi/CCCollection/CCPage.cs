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
        #region "CCPage" class
        /// <summary>
        /// A class to serialize and desrialize eFlow page.
        /// </summary>
        public class CCPage : CCEflowBaseObject, ICCPage
        {
            #region class variables
            private List<CCGroup> groups;
            private String[] attach;
            #endregion

            #region class properties
            #region "Attachments" property
            /// <summary>
            /// The attachment file\s to add from page level.
            /// </summary>
            [Description("The attachment file\\s to add from page level.")]
            public virtual String[] Attachments
            {
                get
                {
                    if (attach == null) attach = new String[0];
                    return attach;
                }
                set { attach = value; }
            } 
            #endregion

            #region "LinkedTables" property
            /// <summary>
            /// The table\s linked to this page.
            /// </summary>
            [XmlIgnore, Description("The table\\s linked to this page.")]
            public virtual CCTable[] LinkedTables
            {
                get
                {
                    List<CCTable> tbls = new List<CCTable>();
                    foreach (CCGroup grp in LinkedGroups) tbls.AddRange(grp.LinkedTables);
                    return tbls.ToArray();
                }
            } 
            #endregion

            #region "PageID" property
            private String pageID;
            /// <summary>
            /// The page's ID (EFI when and if recoginzed).
            /// </summary>
            [Description("The page's ID (EFI when and if recoginzed).")]
            public virtual String PageID { get { return pageID; } set { pageID = value; } }
            #endregion

            #region "BackPage" property
            private bool backPage;
            /// <summary>
            /// The page's  back state.
            /// </summary>
            [Description("The page's  back state.")]
            public virtual bool BackPage { get { return backPage; } set { backPage = value; } }
            #endregion

            #region "FrontPage" property
            /// <summary>
            /// The page's front state (when using CO).
            /// </summary>
            [XmlIgnore, Description("The page's front state.")]
            public virtual bool FrontPage
            {
                get { return !BackPage; }
                set { BackPage = !value; }
            } 
            #endregion

            #region "ParentForm" property
            private ITisFormData parentForm;
            /// <summary>
            /// The page's parent form
            /// </summary>
            [XmlIgnore, Description("The page's parent form.")]
            public virtual ITisFormData ParentForm { get { return parentForm; } set { parentForm = value; } }
            #endregion

            #region "LinkedFields" property
            /// <summary>
            /// The fields linked to this page.
            /// </summary>
            [XmlIgnore, Description("The fields linked to this page.")]
            public virtual CCField[] LinkedFields
            {
                get
                {
                    List<CCField> fields = new List<CCField>();

                    if (groups != null && groups.Count > 0)
                    {
                        foreach (CCGroup group in groups)
                        {
                            if (group.Fields != null && group.Fields.Length > 0) fields.AddRange(group.Fields);
                        }
                    }
                    return fields.ToArray();
                }
            } 
            #endregion

            #region "LinkedGroups" property
            /// <summary>
            /// The field groups linked to this page.
            /// </summary>
            [Description("The field groups linked to this page.")]
            public virtual CCGroup[] LinkedGroups
            {
                get
                {
                    if (groups == null) groups = new List<CCGroup>();
                    return groups.ToArray();
                }
                set
                {
                    if (groups == null) groups = new List<CCGroup>();
                    else groups.Clear();

                    if (value != null) groups.AddRange(value);

                    if (groups.Count > 0)
                    {
                        this.SetParents();
                    }
                }
            } 
            #endregion
            #endregion

            #region class constructors
            public CCPage():base()
            {
                PageID = String.Empty;
            }


            public CCPage(String pageName, String pageId, CCGroup[] linkGroups) :
                this(null, pageName ?? String.Empty, pageId, linkGroups)
            {
            }

#if INTERNAL
            internal CCPage(CCreator parent, String pageName, String pageId, CCGroup[] linkGroups) :
#else
            public CCPage(CCreator parent, String pageName, String pageId, CCGroup[] linkGroups) :
#endif
                base(parent, pageName ?? String.Empty, null, null)
            {
                this.LinkedGroups = linkGroups;
                this.PageID = pageId;
            }

            public CCPage(ITisPageData page)
                : this(null, page)
            {
            }

#if INTERNAL
            internal CCPage(CCreator parent, ITisPageData page)
#else
            public CCPage(CCreator parent, ITisPageData page)
#endif

                : base(parent, page!=null? page.Name:String.Empty,
                parent == null || !parent.CurrentProfile.IgnoreNamedUserTags ? CCUtils.GetNamedUserTags(page, false) : null,
                parent == null || !parent.CurrentProfile.IgnoreUserTags ? CCUtils.GetUserTags(page,true) : null)
            {
                try
                {
                    if (groups == null) groups = new List<CCGroup>();
                    else groups.Clear();

                    this.Attachments = CCUtils.GetAttachments(page);
                    this.EflowOwner = page;
                    this.ParentCollection = page.ParentCollection;
                    this.ParentForm = page.ParentForm;
                    this.PageID = page.PageId;
                    this.NamedParent = page.ParentFormExists ? page.ParentForm.Name : String.Empty;
                    List<String> grpNames = new List<String>();

                    foreach (ITisFieldData fld in page.LinkedFields)
                    {
                        if (grpNames.Contains(fld.ParentFieldGroup.Name.ToUpper())) continue;
                        grpNames.Add(fld.ParentFieldGroup.Name.ToUpper());
                        groups.Add(new CCGroup(this.ParentCreator, fld.ParentFieldGroup));
                    }

                    foreach (ITisFieldTableData fldTbl in page.LinkedFieldTables)
                    {
                        if (grpNames.Contains(fldTbl.ParentFieldGroup.Name.ToUpper())) continue;
                        grpNames.Add(fldTbl.ParentFieldGroup.Name.ToUpper());
                        groups.Add(new CCGroup(this.ParentCreator, fldTbl.ParentFieldGroup));
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
            }

            public CCPage( ITisPageParams page, String page_id)
                : this(null, page, page_id ?? String.Empty)
            {
            }

#if INTERNAL
            internal CCPage(CCreator parent, ITisPageParams page, String page_id)
#else
            public CCPage(CCreator parent, ITisPageParams page, String page_id)
#endif
                : base(parent, page!=null? page.Name:String.Empty, null, null)
            {
                try
                {
                    if (groups == null) groups = new List<CCGroup>();
                    else groups.Clear();

                    this.EflowOwner = page;
                    this.PageID = page_id;
                    this.NamedParent = page.ParentForm.Name;
                    bool hasEfi = false;
                    CCGroup grp = null;
                    List<CCTable> tables = new List<CCTable>();

                    if (!String.IsNullOrEmpty(page_id))
                    {
                        //-- Get matched EFI --\\
                        ITisEFIParams efi = page.get_LinkedEFI(page_id);
                        hasEfi = efi != null; 
                        if (hasEfi)
                        {
                            //-- Get all fields and field-groups that has ROIs --\\
                            foreach (ITisROIParams roi in efi.ROIs)
                            {
                                if (roi.Miscellaneous.OrderInField == 1)
                                {
                                    ITisFieldParams fp = page.get_LinkedField(roi.Miscellaneous.FieldName);

                                    if (fp != null)
                                    {
                                        if (fp.ParentFieldGroupExists)
                                        {
                                            //-- A standard field --\\
                                            grp = GetGroup(fp.ParentFieldGroup.Name);
                                            if (grp == null)
                                            {
                                                grp = new CCGroup(parent, fp.ParentFieldGroup, false);
                                                groups.Add(grp);
                                            }

                                            CCField cf = grp.GetField(fp.Name);

                                            //-- Create field and join sll ROIs --\\
                                            if (cf == null) cf = grp.AddField(fp.Name, String.Empty, 0, CCUtils.MergerRoisRect(CCUtils.GetLinkedRois(efi, fp.Name)));
                                            else cf.Rect = new FieldRect(CCUtils.MergerRoisRect(CCUtils.GetLinkedRois(efi, fp.Name)));

                                            grp.AddField(cf);
                                        }
                                        else if (fp.ParentFieldTableExists)
                                        {
                                            //-- A table field --\\
                                             grp = AddGroup(fp.ParentFieldTable.ParentFieldGroup.Name);
                                             CCTable tbl = grp.AddTable(fp.ParentFieldTable.Name);

                                            if (!tables.Contains(tbl)) tables.Add(tbl);
                                            CCFieldArray fldArr = tbl.AddFieldArray(fp.Name);
                                            fldArr.AddField(fp, CCUtils.MergerRoisRect(CCUtils.GetLinkedRois(efi, fp.Name)));
                                            //CCField cf = fldArr.AddField(fldArr.fp.Name);

                                            ////-- Create field and join sll ROIs --\\
                                            //if (cf == null) cf = grp.AddField(fp.Name, String.Empty, 0, CCUtils.MergerRoisRect(CCUtils.GetLinkedRois(efi, fp.Name)));
                                            //else cf.Rect = new FieldRect(CCUtils.MergerRoisRect(CCUtils.GetLinkedRois(efi, fp.Name)));

                                            //CCFieldArray cfAr = tbl.AddFieldArray(fp.Name);

                                            //cfAr.AddField(cf);
                                        }
                                    }
                                }
                            }

                            //-- Add rows if KeepEmptyRows is marked --\\
                            if (tables != null && tables.Count > 0)
                            {
                                foreach (CCTable tbl in tables)
                                {
                                    ITisFieldTableParams efTable = page.get_LinkedFieldTable(tbl.Name);
                                    if (efTable != null && efTable.Table.KeepEmptyRows && efTable.Table.NumberOfRows > tbl.NumberOfRows)
                                    {
                                        tbl.AddRow();
                                    }
                                }
                            }
                        }
                    }
                    
                    if (!hasEfi)
                    {
                        foreach (ITisFieldParams fld in page.LinkedFields)
                        {
                            if (fld.ParentFieldGroupExists)
                            {
                                grp = GetGroup(fld.ParentFieldGroup.Name);
                                if (grp == null)
                                {
                                    grp = new CCGroup(parent, fld.ParentFieldGroup, false);
                                    groups.Add(grp);
                                }
                            }
                            else if (fld.ParentFieldTableExists)
                            {
                                grp = GetGroup(fld.ParentFieldTable.ParentFieldGroup.Name);
                                if (grp == null)
                                {
                                    grp = new CCGroup(parent, fld.ParentFieldTable.ParentFieldGroup, false);
                                    groups.Add(grp);
                                }
                            }
                        }

                        foreach (ITisFieldTableParams fldTbl in page.LinkedFieldTables)
                        {
                            grp = GetGroup(fldTbl.ParentFieldGroup.Name);
                            if (grp == null)
                            {
                                grp = new CCGroup(parent, fldTbl.ParentFieldGroup, false);
                                groups.Add(grp);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
            }
            #endregion

            #region "AddGroup" functions
            /// <summary>
            /// Add a group to the List of groups of the page.
            /// </summary>
            /// <param name="groupName">The name of the group to create.</param>
            /// <returns>A CCGroup when successfull, null when failed.</returns>
            public virtual CCGroup AddGroup(String groupName)
            {
                try
                {
                    if (groups == null) groups = new List<CCGroup>();

                    //-- Add a new group --\\
                    foreach (CCGroup grp in groups)
                    {
                        if (String.Compare(grp.Name, groupName, true) == 0) return grp;
                    }

                    groups.Add(new CCGroup(this.ParentCreator, groupName, new CCField[0]));

                    this.SetParents();
                    return groups[groups.Count - 1];
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
            }

            /// <summary>
            /// Add a group to the List of groups of the page.
            /// </summary>
            /// <param name="group">The group to add.</param>
            /// <returns>A CCGroup when successfull, null when failed.</returns>
            public virtual CCGroup AddGroup(CCGroup group)
            {
                try
                {
                    if (groups == null) groups = new List<CCGroup>();

                    //-- Add or find a new group --\\
                    foreach (CCGroup grp in groups)
                    {
                        if (String.Compare(grp.Name, group.Name, true) == 0) return grp;
                    }

                    groups.Add(group);
                    this.SetParents();
                    return group;
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
            /// Get a field from the fields linked to this page, using the field name.
            /// </summary>
            /// <param name="fieldName">The name of the field to get.</param>
            /// <returns>CCField when successfull, null when not.</returns>
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

            #region "GetGroup" function
            /// <summary>
            /// Get a group from the groups linked to this page, using the group name.
            /// </summary>
            /// <param name="groupName">The name of the group to get.</param>
            /// <returns>CCGroup when successfull, nulll when not.</returns>
            public CCGroup GetGroup(String groupName)
            {
                try
                {
                    if (!String.IsNullOrEmpty(groupName))
                    {
                        foreach (CCGroup grp in LinkedGroups)
                        {
                            if (String.Compare(grp.Name, groupName, true) == 0)
                            {
                                return grp;
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

            #region "GetTable" function
            /// <summary>
            /// Get a table from the table array, using the table name.
            /// </summary>
            /// <param name="tableName">The name of the table to get.</param>
            /// <returns>CCTable when successfull, null when not.</returns>
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

            #region "RemoveGroup" function
            /// <summary>
            /// Remove a group from the List of groups of the page.
            /// </summary>
            /// <param name="fieldArrayIndex">The group index to remove.</param>
            /// <returns>A true when successfull.</returns>
            public virtual bool RemoveGroup(int groupIndex)
            {
                try
                {
                    if (groups != null && groupIndex >= 0 && groupIndex < groups.Count)
                    {
                        groups.RemoveAt(groupIndex);
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
                    if (groups != null)
                    {
                        foreach (CCGroup gp in groups)
                        {
                            gp.ParentCollection = this.ParentCollection;
                            gp.ParentCreator = this.ParentCreator;
                            gp.ParentForm = this.ParentForm;
                            gp.ParentPage = this.EflowOwner as ITisPageData;
                            gp.NamedParent = this.Name;
                            gp.CCParent = this;
                            gp.SetParents();
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