using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using TiS.Core.eFlowAPI;
using System.ComponentModel;

namespace TiS.Engineering.InputApi
{
    #region "CCEflowObject" class
    /// <summary>
    /// A class base for eFlow objects with specialTags.
    /// </summary>
#if INTERNAL && !EXPOSE_CCColection
    internal class CCEflowObject : CCEflowBaseObject, ICCEflowObject
#else
    public class CCEflowObject : CCEflowBaseObject, ICCEflowObject
#endif
    {
        #region class variables
        private CCCollection.CCDictContainer specialTags;
        #endregion

        #region SpecialTags property
        /// <summary>
        /// The specialTags \ SpecialTags that are linked to this object.
        /// </summary>
        [Description("The specialTags \\ SpecialTags that are linked to this object.")]
        public virtual CCCollection.CCDictContainer SpecialTags
        {
            get
            {
                if (specialTags == null) specialTags = new CCCollection.CCDictContainer();
                return specialTags;
            }
            set { specialTags = value; }
        }
        #endregion

        #region class constructors
        public CCEflowObject()
        {
        }

        public CCEflowObject(String name, Dictionary<String, String> specialTags, Dictionary<String, String> namedTags, Dictionary<String, String> userTags) :
            this(null, name ?? String.Empty, specialTags, namedTags, userTags)
        {
        }

#if INTERNAL
        internal CCEflowObject(CCreator parent, String name, Dictionary<String, String> specialTags, Dictionary<String, String> namedTags, Dictionary<String, String> userTags) :
#else
        public CCEflowObject(CCreator parent, String name, Dictionary<String, String> specialTags, Dictionary<String, String> namedTags, Dictionary<String, String> userTags) :
#endif
            base(parent, name ?? String.Empty, namedTags, userTags)
        {
            this.SpecialTags.NativeDictionary = specialTags;
        }
        #endregion

        #region "SetSpecialTag" function
        /// <summary>
        /// Add or Set a Exception \SpecialTag item.
        /// </summary>
        /// <param name="key">The item Key</param>
        /// <param name="val">The item value.</param>
        /// <returns>true when added\updated, false when not.</returns>
        public virtual bool SetSpecialTag(String key, String val)
        {
            try
            {
                this.SpecialTags.NativeDictionary.Add(key, val);
                return this.SpecialTags.NativeDictionary.ContainsKey(key);
            }
            catch (Exception ex)
            {
                ILog.LogError(ex, false);
                throw ex;
            }
        }
        #endregion
    }
    #endregion
}