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
        #region "CCField" class
        /// <summary>
        /// The class to serialize and desrialize eflow field
        /// </summary>
        public class CCField : CCEflowObject, ICCField
        {
            #region class variables      
            #endregion

            #region class properties
            private int confidence;
            /// <summary>
            /// Get or set the field confidence
            /// </summary>
            [Description("Get or set the field confidence")]
            public virtual int Confidence { get { return confidence; } set { confidence = value; } }

            private int index;
            /// <summary>
            /// The object's index.
            /// </summary>
            [Description("The object's index")]
            public virtual int Index { get { return index; } set { index = value; } }

            #region "NamedParent" property, override, to appear in XML output
            /// <summary>
            /// NamedParent property, an override, to appear in XML output.
            /// </summary>
            [Description("NamedParent property, an override, to appear in XML output.")]
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

            #region "Rect" property
            private FieldRect rect;
            /// <summary>
            /// Get or set the field rectangle (ROI).
            /// </summary>
            [Description("Get or set the field rectangle (ROI).")]
            public virtual FieldRect Rect { get { return rect; } set { rect = value; } }
            #endregion

            #region "Contents" property
            private String contents;
            /// <summary>
            /// Get or set the field contents
            /// </summary>
            [Description("Get or set the field contents")]
            public virtual String Contents { get { return contents; } set { contents = value; } }
            #endregion

            #region "ParentGroup" property
            private ITisFieldGroupData parentGroup;
            /// <summary>
            /// Get or set the field's parent FieldGroup (if exists).
            /// </summary>
            [XmlIgnore, Description("Get or set the field's parent FieldGroup (if exists).")]
            public virtual ITisFieldGroupData ParentGroup { get { return parentGroup; } set { parentGroup = value; } }
            #endregion

            #region "ParentFieldArray" property
            private ITisFieldArrayData parentFieldArray;
            /// <summary>
            /// Get or set the field's Parent field array (if exists).
            /// </summary>
            [XmlIgnore, Description("Get or set the field's Parent field array (if exists).")]
            public virtual ITisFieldArrayData ParentFieldArray { get { return parentFieldArray; } set { parentFieldArray = value; } }
            #endregion

            #region "ParentTable" proprty
            private ITisFieldTableData parentTable;
            /// <summary>
            /// Get or set the field's parent table (if exists).
            /// </summary>
            [XmlIgnore, Description("Get or set the field's parent table (if exists).")]
            public virtual ITisFieldTableData ParentTable { get { return parentTable; } set { parentTable = value; } }
            #endregion

            #region "ParentForm" property
            private ITisFormData parentForm;
            /// <summary>
            /// Get or set the field's parent form.
            /// </summary>
            [XmlIgnore, Description("Get or set the field's parent form.")]
            public virtual ITisFormData ParentForm { get { return parentForm; } set { parentForm = value; } }
            #endregion

            #region "ParentPage" property
            private ITisPageData parentPage;
            /// <summary>
            /// Get or set the field's parent page.
            /// </summary>
            [XmlIgnore, Description("Get or set the field's parent page.")]
            public virtual ITisPageData ParentPage { get { return parentPage; } set { parentPage = value; } }
            #endregion
            #endregion

            #region class constructors
            public CCField():base()
            {
                this.Contents = String.Empty;
            }

            public CCField(ITisFieldData field)
                : this(null, field)
            {
            }

#if INTERNAL
            internal CCField(CCreator parent, ITisFieldData field) :
#else
            public CCField(CCreator parent, ITisFieldData field) :
#endif
            this(parent, field!=null? field.Name:String.Empty, field!=null? field.Contents:String.Empty, field!=null? field.Confidence:short.MinValue, field!=null? field.FieldBoundingRect:new TIS_RECT(),
                 parent == null || !parent.CurrentProfile.IgnoreExceptions ? CCUtils.GetSpecialTags(field) : null,
                 parent == null || !parent.CurrentProfile.IgnoreNamedUserTags ? CCUtils.GetNamedUserTags(field,false) : null,
                 parent == null || !parent.CurrentProfile.IgnoreUserTags ? CCUtils.GetUserTags(field, true) : null)
            {
                this.EflowOwner = field;
                this.ParentCollection = field.ParentCollection;
                this.ParentForm = field.ParentForm;
                this.ParentPage = field.ParentPage;
                this.ParentGroup = field.ParentFieldGroup;
                this.ParentFieldArray = field.ParentFieldArray;
                this.Index = field.TableRepetitionIndex;
                this.ParentTable = field.ParentFieldTable;
                if (field.ParentFieldArrayExists) this.NamedParent = field.ParentFieldArray.Name;
                else if (field.ParentFieldGroupExists) this.NamedParent = field.ParentFieldGroup.Name;
            }

            public CCField(ITisFieldParams field) :
                this(null, field)
            {
            }

#if INTERNAL
            internal CCField(CCreator parent, ITisFieldParams field) :
#else
            public CCField(CCreator parent, ITisFieldParams field) :
#endif
            this(parent,field!=null? field.Name:String.Empty, String.Empty, 0, new TIS_RECT(),null,null,null)
            {
                this.EflowOwner = field;
                if (field.ParentFieldTableExists) this.NamedParent = field.ParentFieldTable.Name;
                else if (field.ParentFieldGroupExists) this.NamedParent = field.ParentFieldGroup.Name;
            }

            public CCField(String name, String contents, short confidence, TIS_RECT fieldRect, Dictionary<String, String> specialTags, Dictionary<String, String> namedTags, Dictionary<String, String> userTags) :
                this(null, name ?? String.Empty, contents ?? String.Empty, confidence, fieldRect, specialTags, namedTags, userTags)
            {
            }

#if INTERNAL
            internal CCField(CCreator parent, String name, String contents, short confidence, TIS_RECT fieldRect, Dictionary<String, String> specialTags, Dictionary<String, String> namedTags, Dictionary<String, String> userTags) :
#else
            public CCField(CCreator parent, String name, String contents, short confidence, TIS_RECT fieldRect, Dictionary<String, String> specialTags, Dictionary<String, String> namedTags, Dictionary<String, String> userTags) :
#endif
                base(parent, name ?? String.Empty, specialTags, namedTags, userTags)
            {
                this.Contents = contents ?? String.Empty;
                this.Confidence = confidence;
                this.Rect = new FieldRect(fieldRect);
            }
            #endregion
        }
        #endregion
    }
}