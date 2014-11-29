namespace SupplierPortalSdkLiteDemo
{
    partial class MainForm
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
            this.btnSendCollectionData = new System.Windows.Forms.Button();
            this.btnReceiveData = new System.Windows.Forms.Button();
            this.btnRemoveCollectionData = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSendCollectionData
            // 
            this.btnSendCollectionData.Location = new System.Drawing.Point(12, 12);
            this.btnSendCollectionData.Name = "btnSendCollectionData";
            this.btnSendCollectionData.Size = new System.Drawing.Size(241, 23);
            this.btnSendCollectionData.TabIndex = 0;
            this.btnSendCollectionData.Text = "Send Collection Data";
            this.btnSendCollectionData.UseVisualStyleBackColor = true;
            this.btnSendCollectionData.Click += new System.EventHandler(this.btnSendCollectionData_Click);
            // 
            // btnReceiveData
            // 
            this.btnReceiveData.Location = new System.Drawing.Point(12, 41);
            this.btnReceiveData.Name = "btnReceiveData";
            this.btnReceiveData.Size = new System.Drawing.Size(241, 23);
            this.btnReceiveData.TabIndex = 1;
            this.btnReceiveData.Text = "Get Collection Data";
            this.btnReceiveData.UseVisualStyleBackColor = true;
            this.btnReceiveData.Click += new System.EventHandler(this.btnReceiveData_Click);
            // 
            // btnRemoveCollectionData
            // 
            this.btnRemoveCollectionData.Location = new System.Drawing.Point(12, 70);
            this.btnRemoveCollectionData.Name = "btnRemoveCollectionData";
            this.btnRemoveCollectionData.Size = new System.Drawing.Size(241, 23);
            this.btnRemoveCollectionData.TabIndex = 2;
            this.btnRemoveCollectionData.Text = "Remove Collection Data";
            this.btnRemoveCollectionData.UseVisualStyleBackColor = true;
            this.btnRemoveCollectionData.Click += new System.EventHandler(this.btnRemoveCollectionData_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 337);
            this.Controls.Add(this.btnRemoveCollectionData);
            this.Controls.Add(this.btnReceiveData);
            this.Controls.Add(this.btnSendCollectionData);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Supplier SDK Lite Demo";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSendCollectionData;
        private System.Windows.Forms.Button btnReceiveData;
        private System.Windows.Forms.Button btnRemoveCollectionData;
    }
}

