namespace SupplierPortalService
{
    partial class Service
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerExecuteMonitor = new System.ComponentModel.BackgroundWorker();
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            // 
            // backgroundWorkerExecuteMonitor
            // 
            this.backgroundWorkerExecuteMonitor.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerExecuteMonitor_DoWork);
            // 
            // Service
            // 
            this.ServiceName = "Service1";

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.ComponentModel.BackgroundWorker backgroundWorkerExecuteMonitor;
    }
}
