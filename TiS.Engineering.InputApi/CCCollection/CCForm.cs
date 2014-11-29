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
        #region "CCForm" class
        /// <summary>
        /// A class to serialize and desrialize eFlow form.
        /// </summary>
        public class CCForm : CCEflowObject
        {
            #region class variables
            private List<CCPage> pages;
            #endregion

            #region class properties
            private String formType;
            /// <summary>
            /// The form type.
            /// </summary>
            [Description("The form type.")]
            public virtual String FormType { get { return formType; } set { formType = value; } }

            #region "NamedParent" property, override, to avoid XML output
            /// <summary>
            /// NamedParent property, override, to avoid XML output.
            /// </summary>
            [XmlIgnore, Description("NamedParent property, override, to avoid XML output.")]
            public override String NamedParent
            {
                get
                {
                    return base.NamedParent;
                }
                set
                {
                    base.NamedParent = value;
                }
            }
            #endregion

            #region "LinkedTables" property
            /// <summary>
            /// Get the tables that are linked to this form.
            /// </summary>
            [XmlIgnore, Description("Get the tables that are linked to this form.")]
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

            #region "LinkedFields" property
            /// <summary>
            /// Get the fields that are linked to this form.
            /// </summary>
            [XmlIgnore, Description("Get the fields that are linked to this form.")]
            public virtual CCField[] LinkedFields
            {
                get
                {
                    List<CCField> fields = new List<CCField>();

                    if (pages != null && pages.Count > 0)
                    {
                        foreach (CCPage page in pages)
                        {
                            if (page.LinkedFields != null && page.LinkedFields.Length > 0) fields.AddRange(page.LinkedFields);
                        }
                    }
                    return fields.ToArray();
                }
            } 
            #endregion

            #region "LinkedGroups" property
            /// <summary>
            /// Get the field groups linked to this form.
            /// </summary>
            [XmlIgnore, Description("Get the field groups linked to this form.")]
            public virtual CCGroup[] LinkedGroups
            {
                get
                {
                    List<CCGroup> groups = new List<CCGroup>();

                    if (pages != null && pages.Count > 0)
                    {
                        foreach (CCPage page in pages)
                        {
                            if (page.LinkedGroups != null && page.LinkedGroups.Length > 0) groups.AddRange(page.LinkedGroups);
                        }
                    }
                    return groups.ToArray();
                }
            } 
            #endregion

            #region "Pages" property
            /// <summary>
            /// Get or set the pages linked to this form.
            /// </summary>
            [Description("Get or set the pages linked to this form.")]
            public virtual CCPage[] Pages
            {
                get
                {
                    if (pages == null) pages = new List<CCPage>();
                    return pages.ToArray();
                }
                set
                {
                    if (pages == null) pages = new List<CCPage>();
                    else pages.Clear();

                    if (value != null) pages.AddRange(value);

                    if (pages.Count > 0)
                    {
                        this.SetParents();
                    }
                }
            } 
            #endregion
            #endregion

            #region class constructors
            public CCForm():base()
            {
                FormType = String.Empty;
            }

#if INTERNAL
            internal CCForm(CCreator parent, String formName, String formtype, params CCPage[] linkPages) :
#else
            public CCForm(CCreator parent, String formName, String formtype, params CCPage[] linkPages) :
#endif
                this(parent, formName ?? String.Empty, formtype ?? String.Empty)
            {
                this.Pages = linkPages;
            }

#if INTERNAL
            internal CCForm(CCreator parent, String formName, String formtype) :
#else
            public CCForm(CCreator parent, String formName, String formtype) :
#endif
                base(parent, formName ?? String.Empty, null, null, null)
            {
                this.FormType = formtype;
            }


            public CCForm(ITisFormData form)
                : this(null, form)
            {
            }

#if INTERNAL
            internal CCForm(CCreator parent, ITisFormData form)
#else
            public CCForm(CCreator parent, ITisFormData form)
#endif
                : base(parent, form!=null? form.Name:String.Empty,
                parent == null || !parent.CurrentProfile.IgnoreExceptions ? CCUtils.GetSpecialTags(form) : null,
                parent == null || !parent.CurrentProfile.IgnoreNamedUserTags ? CCUtils.GetNamedUserTags(form,false) : null,
                parent == null || !parent.CurrentProfile.IgnoreUserTags ? CCUtils.GetUserTags(form,true) : null)
            {
                try
                {
                    if (pages == null) pages = new List<CCPage>();
                    else pages.Clear();

                    this.ParentCollection = form.ParentCollection;
                    this.FormType = form.FormType;
                    this.EflowOwner = form;
                    this.NamedParent = form.ParentCollection.Name;

                    foreach (ITisPageData pg in form.LinkedPages)
                    {
                        pages.Add(new CCPage(this.ParentCreator, pg));
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
            }


            public CCForm(ITisFormParams form)
                : this(null, form)
            {
            }

#if INTERNAL
            internal CCForm(CCreator parent, ITisFormParams form)
#else
            public CCForm(CCreator parent, ITisFormParams form)
#endif
                : base(parent,form!=null? form.Name:String.Empty,null, null, null)
            {
                try
                {
                    if (pages == null) pages = new List<CCPage>();
                    else pages.Clear();

                    this.FormType = form.Name;
                    this.EflowOwner = form;
                    this.NamedParent = form.ParentFlow.Name;

                    foreach (ITisPageParams pg in form.Pages)
                    {
                        pages.Add(new CCPage(this.ParentCreator, pg, pg.NumberOfLinkedEFIs == 1 ? pg.get_LinkedEFIByIndex(0).Name : form.ParentFlow.Process.DefaultEFI ?? String.Empty));
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
            }
            #endregion

            #region "AddPage" functions
            /// <summary>
            /// Add a page to the List of pages in the form.
            /// </summary>
            /// <returns>A CCPage when successfull, null when failed.</returns>
            public virtual CCPage AddPage()
            {
                try
                {
                    if (pages == null) pages = new List<CCPage>();

                    //-- Add a new page --\\
                    CCPage pg = new CCPage(this.ParentCreator, null, null, null);
                    pages.Add(pg);

                    pg.CCParent = this;
                    pg.ParentForm = this.EflowOwner as ITisFormData;
                    pg.NamedParent = this.Name;
                    pg.ParentCollection = this.ParentCollection;
                    pg.ParentCreator = this.ParentCreator;
                    pg.SetParents();
                    return pg;
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex, false);
                    throw ex;
                }
            }

            /// <summary>
            /// Add a page to the List of pages in the form.
            /// </summary>
            /// <returns>A CCPage when successfull, null when failed.</returns>
            public virtual CCPage AddPage(CCPage page)
            {
                try
                {
                    if (pages == null) pages = new List<CCPage>();

                    //-- Add a new page --\\
                    if (page != null)
                    {
                        pages.Add(page);
                        page.CCParent = this;
                        page.ParentForm = this.EflowOwner as ITisFormData;
                        page.NamedParent = this.Name;
                        page.ParentCollection = this.ParentCollection;
                        page.ParentCreator = this.ParentCreator;
                        page.SetParents();                     
                    }
                    return page;
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
            /// Get a field from the fields linked to this form, using the field name.
            /// </summary>
            /// <param name="fieldName">The name of the field to get.</param>
            /// <returns>A CCField when successfull, null when not.</returns>
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
            /// Get a group from the groups linked to this form, using the group name.
            /// </summary>
            /// <param name="groupName">The name of the group to get.</param>
            /// <returns>A CCGroup when successfull, null when not.</returns>
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

            #region "RemovePage" function
            /// <summary>
            /// Remove a page to the List of pages in the form.
            /// </summary>
            /// <param name="pageIndex">The page index to remove.</param>
            /// <returns>true when successfull.</returns>
            public virtual bool RemovePage(int pageIndex)
            {
                try
                {
                    if (pages != null && pageIndex >= 0 && pageIndex < pages.Count)
                    {
                        pages.RemoveAt(pageIndex);
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
                    if (pages != null)
                    {
                        foreach (CCPage pg in pages)
                        {
                            pg.ParentCollection = this.ParentCollection;
                            pg.ParentCreator = this.ParentCreator;
                            pg.ParentForm = this.EflowOwner as ITisFormData;
                            pg.NamedParent = this.Name;
                            pg.SetParents();
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