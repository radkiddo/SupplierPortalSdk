using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using TiS.Core.eFlowAPI;
using System.ComponentModel;

namespace TiS.Engineering.InputApi
{
    #region "CCCollection" class
    /// <summary>
    /// A class that will serialise collection data
    /// </summary>
#if INTERNAL  && !EXPOSE_CCColection
    internal partial class CCCollection : CCEflowObject, ICCCollection
#else
    public partial class CCCollection : CCEflowObject, ICCCollection
#endif
    {
        #region class variables
        private List<CCForm> forms;        
        private String[] attach;        
        #endregion

        #region "AddForm" functions
        /// <summary>
        /// Add a form to the List of forms in the collection.
        /// </summary>
        /// <param name="formType">The type of the form to create (can be null).</param>
        /// <param name="createSubPage">Create a sub page to the form.</param>
        /// <returns>A CCForm when successfull, null when failed.</returns>
        public virtual CCForm AddForm(String formType, bool createSubPage)
        {
            try
            {
                if (forms == null) forms = new List<CCForm>();

                //-- Add a new form --\\
                CCForm frm = new CCForm(this.ParentCreator, null, formType, createSubPage ? new CCPage(this.ParentCreator, null, null, null) : null);
                forms.Add(frm);

                frm.CCParent = this;
                frm.ParentCreator = this.ParentCreator;
                frm.NamedParent = this.Name;
                frm.ParentCollection = this.EflowOwner as ITisCollectionData;
                frm.SetParents();
                return frm;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex, false);
                throw ex;
            }
        }

        /// <summary>
        /// Add a form to the List of forms in the collection.
        /// </summary>
        /// <param name="formType">The form to add.</param>
        /// <returns>A CCForm when successfull, null when failed.</returns>
        public virtual CCForm AddForm(CCForm form)
        {
            try
            {
                if (forms == null) forms = new List<CCForm>();

                //-- Add a new form --\\
                if (form != null)
                {
                    form.CCParent = this;
                    forms.Add(form);
                    form.CCParent = this;
                    form.ParentCreator = this.ParentCreator;
                    form.NamedParent = this.Name;
                    form.ParentCollection = this.EflowOwner as ITisCollectionData;
                    form.SetParents();
                }

                return form;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex, false);
                throw ex;
            }
        } 
        #endregion

        #region "RemoveForm" function
        /// <summary>
        /// Remove a form from the List of forms in the collection.
        /// </summary>
        /// <param name="formIndex">The form index to remove.</param>
        /// <returns>A true when successfull.</returns>
        public virtual bool RemoveFrom(int formIndex)
        {
            try
            {
                if (forms!=null && formIndex >= 0 && formIndex < forms.Count)
                {
                    forms.RemoveAt(formIndex);
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

        #region class properties
        private double absolutePriority;
        [Description("The collection's absolute priority")]
        public virtual double AbsolutePriority { get { return absolutePriority; } set { absolutePriority = value; } }

        #region "Attachments" property
        /// <summary>
        /// The collection's attachments, such as image
        /// </summary>
        [Description("The collection's attachments, such as image")]
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

        #region "AllAttachments" property
        /// <summary>
        /// The collection's attachments including pages.
        /// </summary>
        [XmlIgnore, Description("The collection's attachments including pages")]
        public virtual String[] AllAttachments
        {
            get
            {
                List<String> att = new List<String>(this.Attachments );
                att.AddRange(PagesAttachments);
                return att.ToArray();
            }
        }
        #endregion

        #region "Files" property
        /// <summary>
        /// The collection's files and attachments including image path and pages.
        /// </summary>
        [XmlIgnore, Description("The collection's files and attachments including image path and pages")]
        public virtual String[] Files
        {
            get
            {
                List<String> att = new List<String>(this.AllAttachments);
                att.Add(this.ImagePath);
                return att.ToArray();
            }
        }
        #endregion

        #region "PagesAttachments" property
        /// <summary>
        /// The collection's pages attachment\s, such as image
        /// </summary>
        [XmlIgnore, Description("The collection's pages attachment\\s, such as image")]
        public virtual String[] PagesAttachments
        {
            get
            {
                if (Pages != null && Pages.Length > 0)
                {
                    List<String> att = new List<String>();
                    foreach (CCPage pg in Pages)
                    {
                        if (pg.Attachments != null && pg.Attachments.Length > 0) att.AddRange(pg.Attachments);
                    }
                    return att.ToArray();
                }
                return new String[0];
            }
        }
        #endregion

        #region "CopySourceFiles" property
        private bool copySourceFiles;
        /// <summary>
        /// Copy source attachments when true, move when false.
        /// </summary>
        [Description("Copy source attachments when true, move when false")]
        public bool CopySourceFiles { get { return copySourceFiles; } set { copySourceFiles = value; } }
        #endregion

        #region "FlowType" property
        private string flowType;
        /// <summary>
        /// The collection's FlowType.
        /// </summary>
        [Description("The collection's FlowType")]
        public virtual String FlowType { get { return flowType; } set { flowType = value; } }
        #endregion

        #region "ImagePath" property
        private String imagePath;
        /// <summary>
        /// Get or set the collection's image path.
        /// </summary>
        [Description("Get or set the collection's image path")]
        public String ImagePath { get { return imagePath; } set { imagePath = value; } }
        #endregion

        #region "Forms" property
        /// <summary>
        /// The collection's attached CCForms.
        /// </summary>
        [Description("The collection's attached CCForms.")]
        public virtual CCForm[] Forms
        {
            get
            {
                if (forms == null) forms = new List<CCForm>();
                return forms.ToArray();
            }
            set
            {
                if (forms == null) forms = new List<CCForm>();
                else forms.Clear();

                if (value != null) forms.AddRange(value);

                if (forms.Count > 0)
                {
                    this.SetParents();
                }
            }
        }
        #endregion

        #region "LinkedFields" property
        /// <summary>
        /// Get the collection's fields (derived from the collection <see cref="CCForms"/>).
        /// </summary>
        [XmlIgnore, Description("Get the collection's fields (derived from the collection CCForm\\s->CCPage\\s->CCGroup\\s)")]
        public virtual CCField[] LinkedFields
        {
            get
            {
                List<CCField> fields = new List<CCField>();

                if (forms != null && forms.Count > 0)
                {
                    foreach (CCForm from in forms)
                    {
                        if (from.LinkedFields != null && from.LinkedFields.Length > 0) fields.AddRange(from.LinkedFields);
                    }
                }
                return fields.ToArray();
            }
        }
        #endregion

        #region "LinkedGroups" property
        /// <summary>
        /// The collection's field groups (derived from the collection <see cref="Forms"/>).
        /// </summary>
        [XmlIgnore, Description("Get the collection's field groups (derived from the collection CCForm\\s->CCPage\\s)")]
        public virtual CCGroup[] LinkedGroups
        {
            get
            {
                List<CCGroup> groups = new List<CCGroup>();

                if (forms != null && forms.Count > 0)
                {
                    foreach (CCForm from in forms)
                    {
                        if (from.LinkedGroups != null && from.LinkedGroups.Length > 0) groups.AddRange(from.LinkedGroups);
                    }
                }
                return groups.ToArray();
            }
        }
        #endregion

        #region "LinkedTables" property
        /// <summary>
        /// Get the tables that are linked to this form.
        /// </summary>
        [XmlIgnore, Description("Get the collection's field tables (derived from the collection CCForm\\s->CCPage\\s->CCGroup\\s)")]
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

        #region "Pages" property
        /// <summary>
        /// The collection's pages (derived from the collection <see cref="Forms"/>).
        /// </summary>
        [XmlIgnore, Description("Get the collection's pages (derived from the collection CCForm\\s)")]
        public virtual CCPage[] Pages
        {
            get
            {
                List<CCPage> pages = new List<CCPage>();

                if (forms != null && forms.Count > 0)
                {
                    foreach (CCForm from in forms)
                    {
                        if (from.Pages != null && from.Pages.Length > 0) pages.AddRange(from.Pages);
                    }
                }
                return pages.ToArray();
            }
        }
        #endregion

        #region "Priority" property
        private WorkflowPriorityLevel priority;
        /// <summary>
        /// The collection's priority
        /// </summary>
        [Description("The collection's priority")]
        public virtual WorkflowPriorityLevel Priority { get { return priority; } set { priority = value; } }
        #endregion

        #region "LoginApplication" property
        private String loginApplication;
        /// <summary>
        /// The collection's LoginApplication.
        /// </summary>
        [Description("The collection's LoginApplication")]
        public virtual String LoginApplication { get { return loginApplication; } set { loginApplication = value; } }
        #endregion

        #region "LoginStation" property
        private String loginStation;
        /// <summary>
        /// The collection's LoginStation.
        /// </summary>
        [Description("The collection's LoginStation")]
        public virtual String LoginStation { get { return loginStation; } set { loginStation = value; } }
        #endregion

        #region "TargetQueue" property
        private String targetQueue;
        /// <summary>
        /// The collection's target queue (Will be set to the one next to the creating station when none specified).
        /// </summary>
        [Description("The collection's target queue (Will be set to the one next to the creating station when none specified)")]
        public virtual String TargetQueue { get { return targetQueue; } set { targetQueue = value; } }
        #endregion

        #region "UpdateExistingCollection" property
        private bool updateExistingCollection;
        /// <summary>
        /// The collection's UpdateOnly Existing state (do not create a collection when true, try to get existing and update it).
        /// </summary>
        [Description("The collection's UpdateOnly Existing state (do not create a collection when true, try to get existing and update it)")]
        public virtual bool UpdateExistingCollection { get { return updateExistingCollection; } set { updateExistingCollection = value; } }
        #endregion
        #endregion

        #region class constructors
        /// <summary>
        /// Create an empty CCCollection.
        /// </summary>
        public CCCollection()
        {
            Priority = WorkflowPriorityLevel.Normal;
            FlowType = String.Empty;
            TargetQueue = String.Empty;
            LoginApplication = String.Empty;
            LoginStation = String.Empty;
            ImagePath = String.Empty;
        }

        /// <summary>
        /// Create a CCCollection with the number of specified forms and pages.
        /// </summary>
        /// <param name="collectionName">The collection name to assign to the created collection (can be omitted using null).</param>
        /// <param name="collectionImagePath"></param>
        /// <param name="collectionFlowType">The flow type to assign to the collection (can be omitted using null).</param>
        /// <param name="formType">The form type to assing to the forms that will be created under the collection (can be omitted using null).</param>
        /// <param name="numberOfFormsAndPages">The number of forms (and a sub page for each) to create  (can be omitted using 0).</param>
        public CCCollection(String collectionName, String collectionImagePath, String collectionFlowType, String formType, int numberOfFormsAndPages):
            this(null, collectionName ?? String.Empty, collectionImagePath ?? String.Empty, collectionFlowType ?? String.Empty, formType ?? String.Empty, numberOfFormsAndPages)
        {
        }

        /// <summary>
        /// Create a CCCollection with the number of specified forms and pages.
        /// </summary>
        /// <param name="parent">The parent creator object (can be omitted using null).</param>
        /// <param name="collectionName">The collection name to assign to the created collection (can be omitted using null).</param>
        /// <param name="collectionImagePath"></param>
        /// <param name="collectionFlowType">The flow type to assign to the collection (can be omitted using null).</param>
        /// <param name="formType">The form type to assing to the forms that will be created under the collection (can be omitted using null).</param>
        /// <param name="numberOfFormsAndPages">The number of forms (and a sub page for each) to create  (can be omitted using 0).</param>
#if INTERNAL
        internal CCCollection(CCreator parent , String collectionName, String collectionImagePath, String collectionFlowType, String formType, int numberOfFormsAndPages):
#else
        public CCCollection(CCreator parent , String collectionName, String collectionImagePath, String collectionFlowType, String formType, int numberOfFormsAndPages):
#endif
            base(parent, collectionName ?? String.Empty, null, null, null)
        {
            Priority = WorkflowPriorityLevel.Normal;
            TargetQueue = String.Empty;
            LoginApplication = String.Empty;
            LoginStation = String.Empty;

            ParentCreator = parent;
            ImagePath = collectionImagePath;
            FlowType = collectionFlowType;
            if (numberOfFormsAndPages > 0)
            {
                if (forms == null) forms = new List<CCForm>();
                while (forms.Count < numberOfFormsAndPages)
                {
                    AddForm(formType, true);
                }
            }
        }

        public CCCollection(String collectionName, params CCForm[] frms) :
            this(null, collectionName ?? String.Empty, frms)
        {
        }

        /// <summary>
        /// Create a CCCollection from the specified forms.
        /// </summary>
        /// <param name="parent">Parent creator.</param>
        /// <param name="collectionName">The name of the collection to create.</param>
        /// <param name="frms">The form\\s to create in the collection</param>
#if INTERNAL
        internal CCCollection(CCreator parent, String collectionName, params CCForm[] frms) :
#else
        public CCCollection(CCreator parent, String collectionName, params CCForm[] frms) :
#endif
            base(parent, collectionName ?? String.Empty, null, null, null)
        {
            this.Forms = frms;
            Priority = WorkflowPriorityLevel.Normal;
            FlowType = String.Empty;
            TargetQueue = String.Empty;
            LoginApplication = String.Empty;
            LoginStation = String.Empty;
            ImagePath = String.Empty;
        }

        public CCCollection(ITisClientServicesModule csm, ITisCollectionData collection)
            : this(null, csm, collection)
        {
            Priority = WorkflowPriorityLevel.Normal;
            FlowType = String.Empty;
            TargetQueue = String.Empty;
            LoginApplication = String.Empty;
            LoginStation = String.Empty;
            ImagePath = String.Empty;
        }

#if INTERNAL
        internal CCCollection(CCreator parent, ITisClientServicesModule csm, ITisCollectionData collection)
#else
        public CCCollection(CCreator parent, ITisClientServicesModule csm, ITisCollectionData collection)
#endif
            : base(parent, collection!=null? collection.Name:String.Empty,
            parent == null || !parent.CurrentProfile.IgnoreExceptions ? CCUtils.GetSpecialTags(collection) : null,
            parent == null || !parent.CurrentProfile.IgnoreNamedUserTags ? CCUtils.GetNamedUserTags(collection,false) : null,
            parent == null || !parent.CurrentProfile.IgnoreUserTags ? CCUtils.GetUserTags(collection,true) : null)
        {
            try
            {
                Priority = WorkflowPriorityLevel.Normal;
                FlowType = String.Empty;
                TargetQueue = String.Empty;
                LoginApplication = String.Empty;
                LoginStation = String.Empty;
                ImagePath = String.Empty;

                if (forms == null) forms = new List<CCForm>();
                else forms.Clear();

                this.LoginStation = csm.Session.StationName;
                this.LoginApplication = csm.Application.AppName;
                this.ParentCollection = collection;
                this.FlowType = collection.FlowType;
                this.EflowOwner = collection;
                this.ImagePath = collection.GetAttachmentFileName(CCEnums.CCFilesExt.TIF.ToString());

                #region remove the collection image path from the attachments.
                List<String> att = new List<string>();
                att.AddRange(CCUtils.GetAttachments(csm, collection.AttachedFileManager));

                for (int ia = att.Count - 1; ia >= 0; ia--)
                {
                    if (String.Compare(Path.GetFileName(att[ia]), this.ImagePath, true) == 0)
                    {
                        att.RemoveAt(ia);
                    }
                } 
                #endregion

                this.Attachments = att.ToArray();

                this.Priority = collection.PriorityLevel;

                //-- Create forms --\\
                foreach (ITisFormData frm in collection.Forms)
                {
                    forms.Add(new CCForm(this.ParentCreator, frm));
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
        }
        #endregion

        #region "FromXml" function
        /// <summary>
        /// Get CCCollection from XML file
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to deserialize the data from.</param>
        /// <param name="deleteSource">Delete the source file\s when true, copy them when false.</param>
        /// <returns>The CCCollection as deserialized from XML.</returns>
        public static CCCollection FromXml(String xmlFilePath, bool deleteSource)
        {
            try
            {
                return FromXml(xmlFilePath, typeof(CCCollection)) as CCCollection;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                if (deleteSource && File.Exists(xmlFilePath)) File.Delete(xmlFilePath);
            }
            return null;
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
                if (forms != null)
                {
                    foreach (CCForm frm in forms)
                    {
                        frm.ParentCollection = this.EflowOwner as ITisCollectionData;
                        frm.ParentCreator = this.ParentCreator;
                        frm.NamedParent = this.Name;
                        frm.CCParent = this;
                        frm.SetParents();
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
