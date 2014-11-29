using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace TiS.Engineering.InputApi
{
    partial class CCCollection
    {
        #region "CCDictContainer" class
        /// <summary>
        /// A class to contain a serialized Dictionary of String, String.
        /// </summary>
        public class CCDictContainer
        {
            #region class variables
            private Dictionary<String, String> dct;
            #endregion

            #region class constructors
            public CCDictContainer()
            {
            }

            public CCDictContainer(Dictionary<String, String> add)
            {
                dct = add;
            }

            public CCDictContainer(Row[]  add)
            {
                this.Dictionary = add;
            }
            #endregion

            #region "AddOrSet" functions
            public int AddOrSet(String key, String value)
            {
                try
                {
                    if (String.IsNullOrEmpty(key)) return -1;

                    if (NativeDictionary.ContainsKey(key))
                    {
                        NativeDictionary[key] = value;
                    }
                    else
                    {
                        NativeDictionary.Add(key, value);
                    }

                    return Count - 1;
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                return -1;
            }

            public int AddOrSet(KeyValuePair<String,String> kvp)
            {
                return AddOrSet(kvp.Key, kvp.Value);
            }

            public int AddOrSet(Row kvp)
            {
                return AddOrSet(kvp.Key, kvp.Val);
            } 
            #endregion

            #region "AddRange" methods
            public void AddRange(bool removePrevious, Dictionary<String, String> dct)
            {
                if (removePrevious) NativeDictionary.Clear();

                foreach (KeyValuePair<String, String> kvp in dct)
                {
                    AddOrSet(kvp);
                }
            }

            public void AddRange(bool removePrevious,params Row[] dct)
            {
                if (removePrevious) NativeDictionary.Clear();

                foreach (Row kvr in dct)
                {
                    AddOrSet(kvr.Key, kvr.Val);
                }
            }
            #endregion

            #region "Count" property
            /// <summary>
            /// Get how many items in this dictionary.
            /// </summary>
            [XmlIgnore, ReadOnly(true), Description("Get how many items in this dictionary.")]
            public virtual int Count
            {
                get { return dct != null ? dct.Count : -1; }
            }
            #endregion

            #region "Dictionary" property
            /// <summary>
            /// The dictionary the XML serialization exposed object.
            /// </summary>
            [Description("The dictionary the XML serialization exposed object.")]
            public virtual Row[] Dictionary
            {
                get
                {
                    List<Row> kfl = new List<Row>();
                    foreach (KeyValuePair<String, String> kvp in NativeDictionary)
                    {
                        kfl.Add(new Row(kvp.Key, kvp.Value));
                    }
                    return kfl.ToArray();
                }

                set
                {
                    NativeDictionary.Clear();
                    foreach (Row kf in value)
                    {
                        if (NativeDictionary.ContainsKey(kf.Key)) NativeDictionary[kf.Key] = kf.Val;
                        else NativeDictionary.Add(kf.Key, kf.Val);
                    }
                }
            }
            #endregion

            #region "NativeDictionary" property
            /// <summary>
            /// The underline Dictionary that is storing the information (that can be serialized and deseialized using the exposed property <see cref="Dictionary"/>)
            /// </summary>
            [XmlIgnore, Description("The underline Dictionary that is storing the information (that can be serialized and deseialized using the exposed property Dictionary).")]
            public virtual Dictionary<String, String> NativeDictionary
            {
                get
                {
                    if (dct == null) dct = new Dictionary<String, String>();
                    return dct;
                }
                set { dct = value; }
            }
            #endregion

            #region "Row" class
            /// <summary>
            /// The base class to conatin a key and value items
            /// </summary>
            public class Row : ICCDict
            {
                #region class variables
                private String mKey;
                private String mVal;
                #endregion

                #region class ctors'
                public Row() { }
                public Row(String key, String val)
                {
                    mKey = key;
                    mVal = val;
                }
                #endregion

                #region class properties
                #region "Key" property
                /// <summary>
                /// Key item property
                /// </summary>
                [Description("Key item property")]
                public virtual String Key { get { return mKey ?? String.Empty; } set { mKey = value ?? String.Empty; } }
                #endregion

                #region "Val" property
                /// <summary>
                /// /// Val (value) item property
                /// </summary>
                [Description("Val (value) item property")]
                public virtual String Val { get { return mVal ?? String.Empty; } set { mVal = value ?? String.Empty; } }
                #endregion
                #endregion
            }
            #endregion
        }
        #endregion
    }
}