using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.ComponentModel;

namespace TiS.Engineering.InputApi
{
    #region "CCGenericSerializer" class
    /// <summary>
    /// A generic class to serialize and deserailze from and to XML.
    /// </summary>
    public class CCGenericSerializer :ICCGenericSerializer
    {
        #region class variables
        internal static bool serializing;
        #endregion

        #region "Serializing" property
        /// <summary>
        /// "Runtime flag that tells users of this class that the class is serializing now (to avoid expanding environment variablesat this time).
        /// </summary>
        [XmlIgnore, Browsable(false), Description("Runtime flag that tells users of this class that the class is serializing now (like; to avoid expanding environment variablesat this time)."), Category("Runtime")]
        public static bool Serializing
        {
            get { return serializing; }
        }
        #endregion

        #region "Clone" method
        /// <summary>
        /// Clone this object to another one just like it (copy all properties).
        /// </summary>
        /// <param name="source">The source object to clone it's properties.</param>
        /// <returns>The new cloned object same type of the source.</returns>
        public virtual object Clone()
        {
            return Clone(this);
        }

        /// <summary>
        /// Clone this object to another one just like it (copy all properties).
        /// </summary>
        /// <param name="source">The source object to clone it's properties.</param>
        /// <returns>The new cloned object same type of the source.</returns>
        public static object Clone(object source)
        {
            object res = null;
            try
            {
                if (source == null) return res;

                serializing = true;
                res = Activator.CreateInstance(source.GetType());

                foreach (PropertyInfo pi in source.GetType().GetProperties())
                {
                    if (pi.GetType().GetInterface(typeof(ICloneable).Name) != null)
                    {
                        if (pi.CanWrite && pi.CanRead) pi.SetValue(res, (pi.GetValue(source, null) as ICloneable).Clone(), null);
                    }
                    //else if (pi.GetType().IsClass)
                    //{
                    //    if (pi.CanWrite && pi.CanRead)
                    //    {
                    //        object clsSub = Activator.CreateInstance(pi.GetType());
                    //        CloneTo(pi.GetValue(source, null), clsSub);
                    //        pi.SetValue(res, clsSub, null);
                    //    }
                    //    else
                    //    {
                    //        CloneTo(pi.GetValue(source, null), res.GetType().GetProperty(pi.Name));
                    //    }
                    //}
                    else
                    {
                        if (pi.CanWrite && pi.CanRead) pi.SetValue(res, pi.GetValue(source, null), null);
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                serializing = false;
            }
            return res;
        }
        #endregion

        #region "CloneTo" method
        /// <summary>
        /// Clone the current object to the specified target object.
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="target">Traget object to clone source.</param>
        /// <returns>The cloned object same type of the source.</returns>
        public virtual object CloneTo(object target)
        {
            return CloneTo(this, target);
        }

        /// <summary>
        /// Clone the current object to the specified target object.
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="target">Traget object to clone source.</param>
        /// <returns>The cloned object same type of the source.</returns>
        public static object CloneTo(object source, object target)
        {
            try
            {
                if (source == null) return null;

                serializing = true;
                if (target == null) target = Activator.CreateInstance(source.GetType());

                foreach (PropertyInfo pi in source.GetType().GetProperties())
                {
                    object obRet = pi.CanRead ? pi.GetValue(source, null) : null;
                    if (obRet is ICloneable)
                    {
                        if (pi.CanWrite && pi.CanRead) pi.SetValue(target, (obRet as ICloneable).Clone(), null);
                    }
                    //else if (pi.GetType().IsClass)
                    //{
                    //    if (pi.CanWrite && pi.CanRead)
                    //    {
                    //        object clsSub = Activator.CreateInstance(pi.GetType());
                    //        CloneTo(pi.GetValue(source, null), clsSub);
                    //        pi.SetValue(target, clsSub, null);
                    //    }
                    //    else
                    //    {
                    //        CloneTo(pi.GetValue(source, null), target.GetType().GetProperty(pi.Name));
                    //    }
                    //}
                    else
                    {
                        if (pi.CanWrite && pi.CanRead) pi.SetValue(target, obRet, null);
                    }
                }
                return target;
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                serializing = false;
            }
            return null;
        }
        #endregion

        #region "FromXml" functions
        /// <summary>
        /// Desrialize an XML file to the specified class type.
        /// </summary>
        /// <param name="xmlFilePath">The XML file path to deserialize it's data.</param>
        /// <param name="objectType">The object to deserialize</param>
        /// <returns>The deserialized object from XML when successfull, null when failed.</returns>
        public static object FromXml(String xmlFilePath, Type objectType)
        {
            try
            {
                serializing = true;

                XmlSerializer dataSerializer = new XmlSerializer(objectType);
                using (TextReader r = new StreamReader(xmlFilePath))
                {
                    object res = (object)dataSerializer.Deserialize(r);
                    try
                    {
                        PropertyInfo pi = res.GetType().GetProperty("XmlFilePath");
                        if (pi != null)
                        {
                            pi.SetValue(res, xmlFilePath, null);
                        }
                        else
                        {
                            pi = res.GetType().GetProperty("XmlPath");
                            if (pi != null) pi.SetValue(res, xmlFilePath, null);
                        }
                    }
                    catch { }
                    return res;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                serializing = false;
            }
            return null;
        }

        /// <summary>
        /// Deserialize an XML file to this class type.
        /// </summary>
        /// <param name="xmlFilePath">The XML file path to deserialize it's data.</param>
        /// <returns>The deserialized object from XML when successfull, null when failed.</returns>
        public virtual object FromXml(String xmlFilePath)
        {
            return FromXml(xmlFilePath, this.GetType());
        }
        #endregion        

        #region "ToXml" functions
        /// <summary>
        /// Save the class contenst to an XML file.
        /// </summary>
        /// <param name="xmlPath">The path to save the data to as XML.</param>
        /// <returns>tue when successfull.</returns>
        public virtual bool ToXml(String xmlPath)
        {
            return ToXml(this, xmlPath);
        }

        /// <summary>
        /// Save class contenst to an XML file.
        /// </summary>
        /// <param name="toSerialize">The class to serialize to XML.</param>
        /// <param name="xmlPath">The path to save the data to as XML.</param>
        /// <returns>true when successfull, false when not.</returns>
        public static bool ToXml(object toSerialize, String xmlPath)
        {
            try
            {
                serializing = true;

                System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(toSerialize.GetType());
                if (File.Exists(xmlPath)) File.Delete(xmlPath);
                if (!Directory.Exists(Path.GetDirectoryName(xmlPath))) Directory.CreateDirectory(Path.GetDirectoryName(xmlPath));

                StreamWriter streamWriter = new StreamWriter(xmlPath);
                mySerializer.Serialize(streamWriter, toSerialize);
                mySerializer = null;
                streamWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                serializing = false;
            }
            return false;
        }
        #endregion

        #region "ToXmlString" functions
        /// <summary>
        /// Save the class contenst to an XML string.
        /// </summary>
        /// <returns>XML String when successfull.</returns>
        public virtual String ToXmlString()
        {
            return ToXmlString(this);
        }

        /// <summary>
        /// Save class contenst to an XML string.
        /// </summary>
        /// <param name="toSerialize">The class to serialize to XML string.</param>
        /// <returns>XML String when successfull.</returns>
        public static String ToXmlString(object toSerialize)
        {
            try
            {
                serializing = true;
                System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(toSerialize.GetType());
                using (StringWriter stringWriter = new StringWriter())
                {
                    mySerializer.Serialize(stringWriter, toSerialize);
                    mySerializer = null;
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            finally
            {
                serializing = false;
            }
            return String.Empty;
        }
        #endregion

        #region "FromXmlString" function
        /// <summary>
        /// Desrialize class from  XML string
        /// </summary>
        /// <param name="xmlFileContents">The XML file contents to deserialize.</param>
        /// <param name="objectType">the typeof object to deserialize..</param>
        /// <returns>The desrialized class when successfull, null when failed.</returns>
        public static object FromXmlString(String xmlFileContents, Type objectType)
        {
            try
            {
                serializing = true;
                XmlSerializer dataSerializer = new XmlSerializer(objectType);
                using (StringReader r = new StringReader(xmlFileContents))
                {
                    object res = (object)dataSerializer.Deserialize(r);
                    return res;
                }
            }
            catch (Exception ex)
            {
                ILog.LogError("Could not deserialize data string in method [{1}] Error message: {2}, data string: {0}", xmlFileContents ?? String.Empty, MethodBase.GetCurrentMethod().Name, ex.Message);
                return null;
            }
            finally
            {
                serializing = false;
            }
        }

        /// <summary>
        /// Desrialize class from  XML string
        /// </summary>
        /// <param name="xmlFileContents">The XML file contents to deserialize.</param>
        /// <returns>The desrialized class when successfull, null when failed.</returns>
        public object FromXmlString(String xmlFileContents)
        {
            return FromXmlString(xmlFileContents, this.GetType());
        }
        #endregion
    } 
    #endregion
}
