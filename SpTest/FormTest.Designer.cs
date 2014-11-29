namespace SpTest
{
    partial class FormTest
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
            this.btnPostCollecData = new System.Windows.Forms.Button();
            this.btnExistsCollecData = new System.Windows.Forms.Button();
            this.btnGetCollectionDataAndPost = new System.Windows.Forms.Button();
            this.btnDeleteFirstCollecDataInstances = new System.Windows.Forms.Button();
            this.btnDeleteAllCollecDataInstances = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPostCollecData
            // 
            this.btnPostCollecData.Location = new System.Drawing.Point(12, 12);
            this.btnPostCollecData.Name = "btnPostCollecData";
            this.btnPostCollecData.Size = new System.Drawing.Size(320, 23);
            this.btnPostCollecData.TabIndex = 0;
            this.btnPostCollecData.Text = "Post \'Dummy\' Collection Data";
            this.btnPostCollecData.UseVisualStyleBackColor = true;
            this.btnPostCollecData.Click += new System.EventHandler(this.btnPostCollecData_Click);
            // 
            // btnExistsCollecData
            // 
            this.btnExistsCollecData.Location = new System.Drawing.Point(12, 41);
            this.btnExistsCollecData.Name = "btnExistsCollecData";
            this.btnExistsCollecData.Size = new System.Drawing.Size(320, 23);
            this.btnExistsCollecData.TabIndex = 1;
            this.btnExistsCollecData.Text = "Check if \'any\' Collection Data \"Instance\" Exists (no filtering)";
            this.btnExistsCollecData.UseVisualStyleBackColor = true;
            this.btnExistsCollecData.Click += new System.EventHandler(this.btnExistsCollecData_Click);
            // 
            // btnGetCollectionDataAndPost
            // 
            this.btnGetCollectionDataAndPost.Location = new System.Drawing.Point(12, 159);
            this.btnGetCollectionDataAndPost.Name = "btnGetCollectionDataAndPost";
            this.btnGetCollectionDataAndPost.Size = new System.Drawing.Size(320, 23);
            this.btnGetCollectionDataAndPost.TabIndex = 3;
            this.btnGetCollectionDataAndPost.Text = "Get Collection Data from eFlow App and Post It";
            this.btnGetCollectionDataAndPost.UseVisualStyleBackColor = true;
            this.btnGetCollectionDataAndPost.Click += new System.EventHandler(this.btnGetCollectionDataAndPost_Click);
            // 
            // btnDeleteFirstCollecDataInstances
            // 
            this.btnDeleteFirstCollecDataInstances.Location = new System.Drawing.Point(12, 83);
            this.btnDeleteFirstCollecDataInstances.Name = "btnDeleteFirstCollecDataInstances";
            this.btnDeleteFirstCollecDataInstances.Size = new System.Drawing.Size(320, 23);
            this.btnDeleteFirstCollecDataInstances.TabIndex = 2;
            this.btnDeleteFirstCollecDataInstances.Text = "DELETE \'First Found\' Collection Data \"Instance\" (no filtering)";
            this.btnDeleteFirstCollecDataInstances.UseVisualStyleBackColor = true;
            this.btnDeleteFirstCollecDataInstances.Click += new System.EventHandler(this.btnDeleteAllCollecDataInstances_Click);
            // 
            // btnDeleteAllCollecDataInstances
            // 
            this.btnDeleteAllCollecDataInstances.Location = new System.Drawing.Point(12, 112);
            this.btnDeleteAllCollecDataInstances.Name = "btnDeleteAllCollecDataInstances";
            this.btnDeleteAllCollecDataInstances.Size = new System.Drawing.Size(320, 23);
            this.btnDeleteAllCollecDataInstances.TabIndex = 4;
            this.btnDeleteAllCollecDataInstances.Text = "DELETE \'ALL\' Collection Data \"Instances\" (no filtering)";
            this.btnDeleteAllCollecDataInstances.UseVisualStyleBackColor = true;
            this.btnDeleteAllCollecDataInstances.Click += new System.EventHandler(this.btnDeleteAllCollecDataInstances_Click_1);
            // 
            // FormTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 296);
            this.Controls.Add(this.btnDeleteAllCollecDataInstances);
            this.Controls.Add(this.btnDeleteFirstCollecDataInstances);
            this.Controls.Add(this.btnGetCollectionDataAndPost);
            this.Controls.Add(this.btnExistsCollecData);
            this.Controls.Add(this.btnPostCollecData);
            this.Name = "FormTest";
            this.Text = "Demo";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPostCollecData;
        private System.Windows.Forms.Button btnExistsCollecData;
        private System.Windows.Forms.Button btnGetCollectionDataAndPost;
        private System.Windows.Forms.Button btnDeleteFirstCollecDataInstances;
        private System.Windows.Forms.Button btnDeleteAllCollecDataInstances;
    }
}

