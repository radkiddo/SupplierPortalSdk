using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using TiS.Core.eFlowAPI;
using System.ComponentModel;

namespace TiS.Engineering.InputApi
{
    #region "CCEflowBaseObject" class
    /// <summary>
    /// The base class for eFlow data objects (field, field-group, table, fieldArray, page form and collection).
    /// </summary>
#if INTERNAL  && !EXPOSE_CCColection
    internal class CCEflowBaseObject : CCGenericSerializer, ICCEflowBaseObject
#else
    public class CCEflowBaseObject  : CCGenericSerializer, ICCEflowBaseObject
#endif
    {
        #region class variables                
        private CCCollection.CCDictContainer userTags;
        private CCCollection.CCDictContainer namedUserTags;        
        #endregion

        #region class properties
        #region "ParentCreator" property
        /// <summary>
        /// The object's parent creator class (access settings and other stuff).
        /// </summary>
        private CCreator parentCreator;
        [XmlIgnore, Description("The object's parent creator class (access settings and other stuff).")]
#if INTERNAL
        internal virtual CCreator ParentCreator 
        {
            get { return parentCreator; }
            set { parentCreator = value; }
        }
#else
        
        public virtual CCreator ParentCreator 
        {
            get { return parentCreator; }
            set { parentCreator = value; }
        }
#endif
        #endregion

        #region "CCParent" property
        private CCEflowBaseObject cCParent;
        /// <summary>
        /// The collection creator parent\owner class (to allow browsing up the class tree).
        /// </summary>
        [XmlIgnore, Description("The collection creator parent\\owner class (to allow browsing up the class tree).")]
        public virtual CCEflowBaseObject CCParent { 
            get { return cCParent; } 
            set { cCParent = value;} }
        #endregion

        #region "Name" property
        private String name;
        /// <summary>
        /// The object's name
        /// </summary>
        [Description("The object's name.")]
        public virtual String Name { get { return name; } set { name = value; } }
        #endregion

        #region "NamedParent" property
        private String namedParent;
        /// <summary>
        /// The object's named parent
        /// </summary>
        [XmlIgnore, Description("The object's named parent")]
        public virtual String NamedParent { get { return namedParent; } set { namedParent = value; } }
        #endregion

        #region "NamedUserTags" property
        /// <summary>
        /// The object's NamedUserTags
        /// </summary>
        [Description("The object's NamedUserTags")]
        public virtual CCCollection.CCDictContainer NamedUserTags
        {
            get
            {
                if (namedUserTags == null) namedUserTags = new CCCollection.CCDictContainer();
                return namedUserTags;
            }
            set { namedUserTags = value; }
        } 
        #endregion

        #region "UserTags" property
        /// <summary>
        /// The object's user tags
        /// </summary>
        [Description("The object's user tags")]
        public virtual CCCollection.CCDictContainer UserTags
        {
            get
            {
                if (userTags == null) userTags = new CCCollection.CCDictContainer();
                return userTags;
            }
            set { userTags = value; }
        } 
        #endregion

        #region "ParentCollection" property
        private ITisCollectionData parentCollection;
        /// <summary>
        /// The object's parent collection
        /// </summary>
        [XmlIgnore, Description("The object's parent collection")]
        public virtual ITisCollectionData ParentCollection { get { return parentCollection; } set { parentCollection = value; } }
        #endregion

        #region "EflowOwner" proprty
        private ITisDataLayerTreeNode eflowOwner;
        /// <summary>
        /// The object's parent eFlow object.
        /// </summary>
        [XmlIgnore, Description("The object's parent eFlow object.")]
        public virtual ITisDataLayerTreeNode EflowOwner { get { return eflowOwner; } set { eflowOwner = value; } }
        #endregion
        #endregion

        #region class constructors
        public CCEflowBaseObject()
        {
            Name = String.Empty;
            NamedParent = String.Empty;
        }

        public CCEflowBaseObject(String name) :
            this(null, name, null, null)
        {
        }

        public CCEflowBaseObject(String name, Dictionary<String, String> namedTags, Dictionary<String, String> userTags) :
            this(null, name, namedTags, userTags)
        {
        }

#if INTERNAL
        internal CCEflowBaseObject(CCreator parent, String name, Dictionary<String, String> namedTags, Dictionary<String, String> userTags)
#else
        public CCEflowBaseObject(CCreator parent, String name, Dictionary<String, String> namedTags, Dictionary<String, String> userTags)
#endif
        {
            this.NamedParent = String.Empty;
            this.ParentCreator = parent;
            this.NamedUserTags.NativeDictionary = namedTags;
            this.UserTags.NativeDictionary = userTags;
            this.Name = name;
        }
        #endregion

        #region "GetTopParent" function
        /// <summary>
        /// Get the top object in the tree.
        /// </summary>
        /// <returns>The top CCParent.</returns>
        public CCEflowBaseObject GetTopParent()
        {
            try
            {
                CCEflowBaseObject res = this.CCParent;
                while (res.CCParent != null)
                {
                    res = res.CCParent;
                }
                return res;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex, false);
                throw ex;
            }
        }
        #endregion

        #region "SetUserTag" function
        /// <summary>
        /// Add or Set a UserTag CCDict item.
        /// </summary>
        /// <param name="key">The item Key</param>
        /// <param name="val">The item value.</param>
        /// <returns>true when added\updated, false when not.</returns>
        public virtual bool SetUserTag(String key, String val)
        {
            try
            {
                this.UserTags.NativeDictionary.Add(key, val);
                return this.UserTags.NativeDictionary.ContainsKey(key);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex, false);
                throw ex;
            }
        }
        #endregion

        #region "SetNamedUserTag" function
        /// <summary>
        /// Add or Set a NamedUserTag  item.
        /// </summary>
        /// <param name="key">The item Key</param>
        /// <param name="val">The item value.</param>
        /// <returns>true when added\updated, false when not.</returns>
        public virtual bool SetNamedUserTag(String key, String val)
        {
            try
            {
                NamedUserTags.NativeDictionary.Add(key, val);
                return NamedUserTags.NativeDictionary.ContainsKey(key);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex, false);
                throw ex;
            }
        }
        #endregion

        #region "FromEflowObject" function
        /// <summary>
        /// Get a CCEflowBaseObject  (that can be casted to the appropriate return type) from an eFlow dynamic or setup object (form, page, group, filed, table, field-array).
        /// </summary>
        /// <param name="creator">The CCreator to link to the objects ((optional)</param>
        /// <param name="eflowObject">The eFlow object to create a </param>
        /// <returns>a CCEflowBaseObject when successfull, null when not.</returns>
#if INTERNAL
        internal CCEflowBaseObject FromEflowObject(CCreator creator, ITisDataLayerTreeNode eflowObject)
#else
        public CCEflowBaseObject FromEflowObject(CCreator creator, ITisDataLayerTreeNode eflowObject)
#endif
        {
            try
            {
                if (eflowObject is ITisFieldData)
                {
                    return new CCCollection.CCField(creator, eflowObject as ITisFieldData);
                }
                else if (eflowObject is ITisFieldParams)
                {
                    return new CCCollection.CCField(creator, eflowObject as ITisFieldParams);
                }
                else if (eflowObject is ITisFieldGroupData)
                {
                    return new CCCollection.CCGroup(creator, eflowObject as ITisFieldGroupData);
                }
                else if (eflowObject is ITisFieldGroupParams)
                {
                    return new CCCollection.CCGroup(creator, eflowObject as ITisFieldGroupParams);
                }
                else if (eflowObject is ITisFieldTableData)
                {
                    return new CCCollection.CCTable(creator, eflowObject as ITisFieldTableData);
                }
                else if (eflowObject is ITisFieldTableParams)
                {
                    return new CCCollection.CCTable(creator, eflowObject as ITisFieldTableParams);
                }
                else if (eflowObject is ITisFieldArrayData)
                {
                    return new CCCollection.CCFieldArray(creator, eflowObject as ITisFieldArrayData);
                }
                else if (eflowObject is ITisPageData)
                {
                    return new CCCollection.CCPage(creator, eflowObject as ITisPageData);
                }
                else if (eflowObject is ITisPageParams)
                {
                    ITisPageParams pg = eflowObject as ITisPageParams;
                    return new CCCollection.CCPage(creator, pg, pg.NumberOfLinkedEFIs == 1 ? pg.get_LinkedEFIByIndex(0).Name : pg.ParentForm.ParentFlow.Process.DefaultEFI ?? String.Empty);
                }
                else if (eflowObject is ITisFormData)
                {
                    return new CCCollection.CCForm(creator, eflowObject as ITisFormData);
                }
                else if (eflowObject is ITisFormParams)
                {
                    return new CCCollection.CCForm(creator, eflowObject as ITisFormParams);
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return null;
        } 
        #endregion
    }
    #endregion
}
