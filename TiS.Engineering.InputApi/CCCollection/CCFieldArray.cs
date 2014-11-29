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
    /// <summary>
    /// CCOllection main class
    /// </summary>
     partial class CCCollection
    {
        #region "CCFieldArray" class
        /// <summary>
        /// Field array class.
        /// </summary>
        public class CCFieldArray : CCEflowObject, ICCFieldArray
        {
            #region class variables         
            private List<CCField> fields;
            #endregion

            #region class properties
            #region "NamedParent" property, override, to appear XML output
            /// <summary>
            /// Parent name property
            /// </summary>
            [Description("Parent name property.")]
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

            #region "NextFieldName" property
            /// <summary>
            /// Get the next Field name in the Field array (use this property to add a field to the field-array).
            /// </summary>
            [Description("Get the next Field name in the Field array (use this property to add a field to the field-array).")]
            public virtual String NextFieldName
            {
                get { return String.Format("{0}${1}", this.Name, LinkedFields.Length.ToString("X").PadLeft(4, '0')); }
            } 
            #endregion

            #region "LinkedFields" property
            /// <summary>
            /// The fields linked to this field array
            /// </summary>
            [Description("The fields linked to this field array.")]
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

                    if (fields.Count > 0)
                    {
                        this.SetParents();
                    }
                }
            }
            #endregion

            #region "ParentGroup" property
            private ITisFieldGroupData parentGroup;
            /// <summary>
            /// The field array's parent field group
            /// </summary>
            [XmlIgnore, Description("The field array's parent field group.")]
            public virtual ITisFieldGroupData ParentGroup { get { return parentGroup; } set { parentGroup = value; } }
            #endregion

            #region "ParentTable" property
            private ITisFieldTableData parentTable;
            /// <summary>
            /// The field array's parent table
            /// </summary>
            [XmlIgnore, Description("The field array's parent table.")]
            public virtual ITisFieldTableData ParentTable { get { return parentTable; } set { parentTable = value; } }
            #endregion

            #region "ParentForm" property
            private ITisFormData parentForm;
            /// <summary>
            /// The field array's parent form
            /// </summary>
            [XmlIgnore, Description("The field array's parent form.")]
            public virtual ITisFormData ParentForm { get { return parentForm; } set { parentForm = value; } }
            #endregion

            #region "ParentPage" property
            private ITisPageData parentPage;
            /// <summary>
            /// The field array's parent page
            /// </summary>
            [XmlIgnore, Description("The field array's parent page.")]
            public virtual ITisPageData ParentPage { get { return parentPage; } set { parentPage = value; } }
            #endregion
            #endregion

            #region class constructors
            public CCFieldArray()
            {
            }

            public CCFieldArray(ITisFieldArrayData fieldArray)
                : this(null, fieldArray)
            {
            }


            internal CCFieldArray(CCreator parent, ITisFieldArrayData fieldArray)
                : this(parent, fieldArray!=null? fieldArray.Name:String.Empty,
                parent == null || !parent.CurrentProfile.IgnoreExceptions ? CCUtils.GetSpecialTags(fieldArray) : null,
                parent == null || !parent.CurrentProfile.IgnoreNamedUserTags ? CCUtils.GetNamedUserTags(fieldArray,false) : null,
                parent == null || !parent.CurrentProfile.IgnoreUserTags ? CCUtils.GetUserTags(fieldArray,true) : null)
            {
                this.EflowOwner = fieldArray;
                this.ParentCollection = fieldArray.ParentCollection;
                this.ParentForm = fieldArray.ParentForm;
                this.ParentPage = fieldArray.ParentPage;
                this.ParentGroup = fieldArray.ParentFieldGroup;
                this.ParentTable = fieldArray.ParentFieldTable;
                this.NamedParent = fieldArray.ParentFieldTable.Name;

                if (fields == null) fields = new List<CCField>();
                else fields.Clear();

                foreach (ITisFieldData fd in fieldArray.LinkedFields)
                {
                    fields.Add(new CCField(this.ParentCreator, fd));
                }
            }


            public CCFieldArray(ITisFieldParams fieldParent)
                : this(null, fieldParent)
            {
            }

#if INTERNAL
            internal CCFieldArray(CCreator parent, ITisFieldParams fieldParent)
#else
            public CCFieldArray(CCreator parent, ITisFieldParams fieldParent)
#endif
                : this(parent, fieldParent, true)
            {
            }

            public CCFieldArray(ITisFieldParams fieldParent, bool createSubFields)
                : this(null, fieldParent, createSubFields)
            {
            }

#if INTERNAL
            internal CCFieldArray(CCreator parent, ITisFieldParams fieldParent, bool createSubFields)
#else
            public CCFieldArray(CCreator parent, ITisFieldParams fieldParent, bool createSubFields)
#endif
                : this(parent,fieldParent!=null? fieldParent.Name:String.Empty, new Dictionary<String,String>(), null, null)
            {
                this.EflowOwner = fieldParent;
                this.NamedParent = fieldParent.ParentFieldTableExists ? fieldParent.ParentFieldTable.Name : String.Empty;

                if (fields == null) fields = new List<CCField>();
                else fields.Clear();

                if (createSubFields)
                {
                    fields.Add(new CCField(this.ParentCreator, fieldParent));
                }
            }

            public CCFieldArray(String fieldArrayName, params CCField[] linkFields) :
                this(null, fieldArrayName ?? String.Empty, linkFields)
            {
            }

#if INTERNAL
            internal CCFieldArray(CCreator parent, String fieldArrayName, params CCField[] linkFields) :
#else
            public CCFieldArray(CCreator parent, String fieldArrayName, params CCField[] linkFields) :
#endif
                base(parent, fieldArrayName??String.Empty, null, null, null)
            {
                this.LinkedFields = linkFields;
            }

            public CCFieldArray(String name, Dictionary<String, String> specialTags, Dictionary<String, String> namedTags, Dictionary<String, String>  userTags) :
                this(null, name ?? String.Empty, specialTags, namedTags, userTags)
            {
            }

            
#if INTERNAL
            internal CCFieldArray(CCreator parent, String name, Dictionary<String, String> specialTags, Dictionary<String, String> namedTags, Dictionary<String, String> userTags) :
#else
            public CCFieldArray(CCreator parent, String name, Dictionary<String, String> specialTags, Dictionary<String, String> namedTags, Dictionary<String, String> userTags) :
#endif
                base(parent, name ?? String.Empty, specialTags, namedTags, userTags)
            {
            }
            #endregion

            #region "AddField" functions
            /// <summary>
            /// Add a field to the List of fields of the field array.
            /// </summary>
            /// <param name="field">The Field settings to create a CCField from.</param>
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

                    fields.Add(new CCField(this.ParentCreator, field.Name, String.Empty, 0, fieldRect, null, null, null));

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
            /// Add a field to the List of fields of the field array.
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

                    fields.Add(new CCField(this.ParentCreator, fieldName, contents, confidence, fieldRect, null, null, null));

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
            /// Add a field to the List of fields of the field array.
            /// </summary>
            /// <param name="field">The  Field to add.</param>
            /// <returns>A CCField when successfull, null when failed.</returns>
            public virtual CCField AddField(CCField field)
            {
                try
                {
                    if (fields == null) fields = new List<CCField>();

                    //-- Add a new field --\\
                    foreach (CCField cf in fields)
                    {
                        if (String.Compare(cf.Name, field.Name, true) == 0) return cf;
                    }

                    this.SetParents();
                    return field;
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
            /// Get a field from the fields linked to this field array, using the field name.
            /// </summary>
            /// <param name="fieldName">The name of the field to get.</param>
            /// <returns>The requested CCField when successfull, null when not.</returns>
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

            #region "RemoveField" function
            /// <summary>
            /// Remove a Field from the List of fields of the FieldArray.
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
                            fd.ParentGroup = this.ParentGroup;
                            fd.ParentTable = this.ParentTable;
                            fd.ParentFieldArray = this.EflowOwner as ITisFieldArrayData;
                            fd.CCParent = this;
                            fd.NamedParent = this.Name;
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