using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SupplierPortalDaemon
{
    public class TransDialog : Form
    {
        #region Constructor


        public TransDialog()
        {
            InitializeComponents();
        }

        void InitializeComponents()
        {
            this.components = new System.ComponentModel.Container();
            this.m_clock = new System.Windows.Forms.Timer(this.components);
            this.m_clock.Interval = 100;
            this.SuspendLayout();
            //m_clock
            this.m_clock.Tick += new EventHandler(Animate);
            //TransDialog
            this.Load += new EventHandler(TransDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        #region Event handlers
        private void TransDialog_Load(object sender, EventArgs e)
        {
            this.Opacity = 0.0;
            m_bShowing = true;

            m_clock.Start();
        }

        #endregion

        #region Private methods
        private void Animate(object sender, EventArgs e)
        {
            if (m_bShowing)
            {
                if (this.Opacity < 1)
                {
                    this.Opacity += 0.1;
                }
                else
                {
                    m_clock.Stop();
                }
            }
            else
            {
                if (this.Opacity > 0)
                {
                    this.Opacity -= 0.1;
                }
                else
                {
                    m_clock.Stop();
                    m_bForceClose = true;
                    this.Close();
                    if (m_bDisposeAtEnd)
                        this.Dispose();
                }
            }
        }

        #endregion

        #region overrides
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region private variables
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Timer m_clock;
        private bool m_bShowing = true;
        private bool m_bForceClose = false;
        private DialogResult m_origDialogResult;
        private bool m_bDisposeAtEnd = false;
        #endregion // private variables

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // TransDialog
            // 
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(334, 176);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "TransDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.TransDialog_Load_1);
            this.ResumeLayout(false);

        }

        private NotifyIcon notifyIcon1;

        protected void LoadOnCorner()
        {
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            this.Left = screenWidth - this.Width;
            this.Top = screenHeight - this.Height;
        }

        private void TransDialog_Load_1(object sender, EventArgs e)
        {
            LoadOnCorner();
        }
    }
}
