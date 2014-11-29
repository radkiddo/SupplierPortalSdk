using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;

namespace TiS.Engineering.InputApi
{
    #region "FolderBrowseProp" property editor class
    /// <summary>
    /// A class that allows you to browse for a folder from the property grid control.
    /// </summary>
    /// <example>How to implement:<code>
    /// private String folderName;
    /// 
    /// [Editor(typeof(FolderBrowseProp), typeof(UITypeEditor)),
    /// Description("A file name."), Category("Folder")]
    /// public String FolderName
    /// {
    /// 	get { return this.folderName ?? String.Empty; }
    /// 	set { this.folderName = value ?? String.Empty; }
    /// }
    /// </code></example>
    internal class FolderBrowseProp : UITypeEditor
    {
        /// <summary>
        /// Model gives you the browse button (...), DropDown and None are your other choices
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Implements the custom method to edit the property (folder browse dialog in this case).
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(ITypeDescriptorContext context,
          IServiceProvider provider, object value)
        {
            using (FolderBrowserDialog opn = new FolderBrowserDialog())
            {
                if (value != null) opn.SelectedPath = value.ToString();
                opn.Description = "Browse for folder...";
                if (opn.ShowDialog() == DialogResult.OK)
                {
                    return opn.SelectedPath;
                }
            }
            return value;
        }
    }
    #endregion "FolderBrowseProp" class
}
