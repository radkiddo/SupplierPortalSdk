using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace TiS.Engineering.TiffDll90
{
    internal  class tiffDll90TesterForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.joinButton = new System.Windows.Forms.Button();
            this.splitButton = new System.Windows.Forms.Button();
            this.changeResolutionButton = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.imageInfoButton = new System.Windows.Forms.Button();
            this.annotateButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // joinButton
            // 
            this.joinButton.Location = new System.Drawing.Point(1, 2);
            this.joinButton.Name = "joinButton";
            this.joinButton.Size = new System.Drawing.Size(75, 23);
            this.joinButton.TabIndex = 0;
            this.joinButton.Text = "&Join...";
            this.joinButton.UseVisualStyleBackColor = true;
            this.joinButton.Click += new System.EventHandler(this.joinButton_Click);
            // 
            // splitButton
            // 
            this.splitButton.Location = new System.Drawing.Point(82, 2);
            this.splitButton.Name = "splitButton";
            this.splitButton.Size = new System.Drawing.Size(75, 23);
            this.splitButton.TabIndex = 1;
            this.splitButton.Text = "&Split...";
            this.splitButton.UseVisualStyleBackColor = true;
            this.splitButton.Click += new System.EventHandler(this.splitButton_Click);
            // 
            // changeResolutionButton
            // 
            this.changeResolutionButton.Location = new System.Drawing.Point(163, 2);
            this.changeResolutionButton.Name = "changeResolutionButton";
            this.changeResolutionButton.Size = new System.Drawing.Size(148, 23);
            this.changeResolutionButton.TabIndex = 2;
            this.changeResolutionButton.Text = "&Change resolution...";
            this.changeResolutionButton.UseVisualStyleBackColor = true;
            this.changeResolutionButton.Click += new System.EventHandler(this.changeResolutionButton_Click);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(1, 31);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(532, 177);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Message";
            this.columnHeader2.Width = 450;
            // 
            // imageInfoButton
            // 
            this.imageInfoButton.Location = new System.Drawing.Point(317, 2);
            this.imageInfoButton.Name = "imageInfoButton";
            this.imageInfoButton.Size = new System.Drawing.Size(75, 23);
            this.imageInfoButton.TabIndex = 4;
            this.imageInfoButton.Text = "Image info...";
            this.imageInfoButton.UseVisualStyleBackColor = true;
            this.imageInfoButton.Click += new System.EventHandler(this.imageInfoButton_Click);
            // 
            // annotateButton
            // 
            this.annotateButton.Location = new System.Drawing.Point(398, 2);
            this.annotateButton.Name = "annotateButton";
            this.annotateButton.Size = new System.Drawing.Size(75, 23);
            this.annotateButton.TabIndex = 5;
            this.annotateButton.Text = "&Annotate...";
            this.annotateButton.UseVisualStyleBackColor = true;
            this.annotateButton.Click += new System.EventHandler(this.annotateButton_Click);
            // 
            // tiffDll90TesterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 214);
            this.Controls.Add(this.annotateButton);
            this.Controls.Add(this.imageInfoButton);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.changeResolutionButton);
            this.Controls.Add(this.splitButton);
            this.Controls.Add(this.joinButton);
            this.Name = "tiffDll90TesterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TiffDll90 - tester";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button joinButton;
        private System.Windows.Forms.Button splitButton;
        private System.Windows.Forms.Button changeResolutionButton;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button imageInfoButton;
        private System.Windows.Forms.Button annotateButton;

        public tiffDll90TesterForm()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                TiffDll_Methods.errorList.Add(string.Format("Error in [{0}], error: [{1}]", MethodBase.GetCurrentMethod().Name, ex.ToString()));
            }
            finally
            {
                AddLoggerLines();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joinButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog opn = new OpenFileDialog())
                {
                    opn.Title = "Select images to join...";
                    opn.Multiselect = true;
                    opn.Filter = "Tiff files (*.tif)|*.tiff;*.tif|Any file (*.*)|*.*";
                    if (opn.ShowDialog() == DialogResult.OK && opn.FileNames.Length > 1)
                    {
                        using (SaveFileDialog sve = new SaveFileDialog())
                        {
                            sve.Title = "Please enter target path for the joined tiff";
                            sve.DefaultExt = "tif";
                            sve.Filter = "Tiff files (*.tif)|*.tiff;*.tif|Any file (*.*)|*.*";
                            if (sve.ShowDialog() == DialogResult.OK)
                            {
                                TiffDll_Methods.JoinTiffPages(sve.FileName, opn.FileNames);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TiffDll_Methods.errorList.Add(string.Format("Error in [{0}], error: [{1}]", MethodBase.GetCurrentMethod().Name, ex.ToString()));
            }
            finally
            {
                AddLoggerLines();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog opn = new OpenFileDialog())
                {
                    opn.Title = "Select image to split (result files will be placed next to it)";
                    opn.Multiselect = false;
                    opn.Filter = "Tiff files (*.tif)|*.tiff;*.tif|Any file (*.*)|*.*";
                    if (opn.ShowDialog() == DialogResult.OK)
                    {
                        TiffDll_Methods.SplitTiffPages(opn.FileName, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                TiffDll_Methods.errorList.Add(string.Format("Error in [{0}], error: [{1}]", MethodBase.GetCurrentMethod().Name, ex.ToString()));
            }
            finally
            {
                AddLoggerLines();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeResolutionButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog opn = new OpenFileDialog())
                {
                    opn.Title = "Select image to change resolution to 300dpi";
                    opn.Multiselect = false;
                    opn.Filter = "Tiff files (*.tif)|*.tiff;*.tif|Any file (*.*)|*.*";
                    if (opn.ShowDialog() == DialogResult.OK)
                    {
                        using (SaveFileDialog sve = new SaveFileDialog())
                        {
                            sve.Title = "Please enter target path for the changed resolution tiff";
                            sve.DefaultExt = "tif";
                            sve.Filter = "Tiff files (*.tif)|*.tiff;*.tif|Any file (*.*)|*.*";
                            if (sve.ShowDialog() == DialogResult.OK)
                            {
                               TiffDll_Methods.ChangeTiffResolution(opn.FileName, sve.FileName, 300);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TiffDll_Methods.errorList.Add(string.Format("Error in [{0}], error: [{1}]", MethodBase.GetCurrentMethod().Name, ex.ToString()));
            }
            finally
            {
                AddLoggerLines();
            }
        }

        public void AddLoggerLines()
        {
            try
            {
                if (TiffDll_Methods.errorList != null && TiffDll_Methods.errorList.Count > 0)
                {
                    string[] sget = TiffDll_Methods.errorList.ToArray();
                    TiffDll_Methods.errorList.Clear();

                    if (sget != null && sget.Length > 0)
                    {
                        listView1.BeginUpdate();
                        foreach (string s in sget)
                        {
                            listView1.Items.Add(new ListViewItem(new string[] { DateTime.Now.ToString("[dd/MM/yyyy HH:mm:ss:fffff]"), s }));
                        }

                        listView1.EndUpdate();

                    }
                }
            }
            catch (Exception ex)
            {
                TiffDll_Methods.errorList.Add(string.Format("Error in [{0}], error: [{1}]", MethodBase.GetCurrentMethod().Name, ex.ToString()));
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sve = new SaveFileDialog())
                {
                    sve.DefaultExt = "txt";
                    sve.Filter = "Text files (*.txt)|*.txt|Any file (*.*)|*.*";
                    sve.Title = "Save logger lines as...";
                    if (sve.ShowDialog() == DialogResult.OK)
                    {

                        if (listView1.Items.Count > 0)
                        {
                            List<string> lst = new List<string>();
                            foreach (ListViewItem itm in listView1.Items)
                            {
                                lst.Add(itm.Text + ", " + itm.SubItems[0].Text);
                            }
                            File.WriteAllLines(sve.FileName, lst.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TiffDll_Methods.errorList.Add(string.Format("Error in [{0}], error: [{1}]", MethodBase.GetCurrentMethod().Name, ex.ToString()));
            }
            finally
            {
                AddLoggerLines();
            }
        }

        private void imageInfoButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog opn = new OpenFileDialog())
                {
                    opn.Title = "Select image to get its information";
                    opn.Filter = "Tiff files (*.tif)|*.tiff;*.tif|Any file (*.*)|*.*";

                    if (opn.ShowDialog() == DialogResult.OK)
                    {
                        Size sz = new Size();
                        Size rs = new Size();
                        int pages=0;
                        int compression=-1;
                        int colorDepth=-1;
                        if (TiffDll_Methods.GetImageInfo(opn.FileName, ref sz, ref rs, ref pages, ref  compression, ref colorDepth))
                        {
                            MessageBox.Show(string.Format("Image Path: {0}\r\nSize: {1}\r\nResolution: {2}\r\nNumber Of Pages: {3}\r\nCompression: {4}\r\nColor Depth: {5}",
                                opn.FileName, sz, rs, pages, compression, colorDepth), "Image information",MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TiffDll_Methods.errorList.Add(string.Format("Error in [{0}], error: [{1}]", MethodBase.GetCurrentMethod().Name, ex.ToString()));
            }
            finally
            {
                AddLoggerLines();
            }
        }

        private void annotateButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog opn = new OpenFileDialog())
                {
                    opn.Title = "Select images to Annotate...";
                    opn.Multiselect = true;
                    opn.Filter = "Tiff files (*.tif)|*.tiff;*.tif|Any file (*.*)|*.*";
                    if (opn.ShowDialog() == DialogResult.OK)
                    {
                        using (SaveFileDialog sve = new SaveFileDialog())
                        {
                            sve.Title = "Please enter target path for the annotated tiff...";
                            sve.DefaultExt = "tif";
                            sve.Filter = "Tiff files (*.tif)|*.tiff;*.tif|Any file (*.*)|*.*";
                            if (sve.ShowDialog() == DialogResult.OK)
                            {
                                TiffDll_Methods.AnnotateTiff(opn.FileName, sve.FileName, "[ T E S T    A N N O T A T I O N ]    Date: "+DateTime.Now.ToString(), false, ContentAlignment.BottomRight);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TiffDll_Methods.errorList.Add(string.Format("Error in [{0}], error: [{1}]", MethodBase.GetCurrentMethod().Name, ex.ToString()));
            }
            finally
            {
                AddLoggerLines();
            }
        }
    }
}
