#region "about"

//
// eFLOW Supplier Portal SDK
// 2013 (c) - Top Image Systems (a project initiated by the UK branch)
//
// The purpose of this SDK is to communicate with the Supplier Portal for eFlow Invoices.
// Developed by: Eduardo Freitas
//

#endregion "about"

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Data.SqlClient;

using TiS.Core.eFlowAPI;
using TiS.Core.PlatformRuntime;

using eFlow.CollectionManagement;
using eFlow.SupplierPortalCore;
using SupplierPortalCommon;

namespace SupplierPortalDaemon
{
    public partial class MainForm : TransDialog
    {
        private const string cStrMonitoring = "Monitoring collections...";
        private bool hide = true, busy = false;

        private string ConnectionStr = String.Empty;

        private const string cStrNofityMainText = "SupplierPortal Daemon";

        public const string cStrDateFormat = "dd-MMM-yyyy";
        public const string cStrTimeFormat = "HH:mm:ss";

        private const int WM_QUERYENDSESSION = 0x0011;
        private const int WM_CLOSE = 0x10;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void LoadCulture()
        {
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            ci.DateTimeFormat.ShortDatePattern = cStrDateFormat;
            ci.DateTimeFormat.ShortTimePattern = cStrTimeFormat;
            ci.DateTimeFormat.FullDateTimePattern = cStrDateFormat;
            ci.DateTimeFormat.LongDatePattern = cStrDateFormat;
            ci.DateTimeFormat.LongTimePattern = cStrTimeFormat;
        }

        private void SetToLoadOnOSStartup()
        {
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            regKey.SetValue(Application.ProductName, Application.ExecutablePath);
        }

        protected override void WndProc(ref Message m)
        {
            bool isShuttingDown = false;

            if (m.Msg == WM_QUERYENDSESSION)
                isShuttingDown = true;

            base.WndProc(ref m);

            if (isShuttingDown)
                CloseSession();
        }

        private void CloseSession()
        {
            hide = false;

            Logging.InfoLog("Stopped SupplierPortalDaemon (standalone .exe)");

            this.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadCulture();
            SetToLoadOnOSStartup();
            base.LoadOnCorner();

            Logging.CreateLog();
            Logging.InfoLog("Started SupplierPortalDaemon (standalone .exe)");

            Common.GetInterval(ref timerExecute);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = hide;
            this.Hide();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            CloseSession();
        }

        private void About_Click(object sender, EventArgs e)
        {
            using (AboutBox ab = new AboutBox())
            {
                ab.ShowDialog();
            }
        }

        private void timerExecute_Tick(object sender, EventArgs e)
        {
            if (!busy)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!busy)
            {
                busy = true;

                Common.GetFromPortal();

                string sqlQueryStr = "SELECT E_WFUnitMetaTags.CreationTime, E_WFUnitMetaTags.BatchName, E_WFQueue.QueueName, R_WFQueueUnit.Status FROM E_WFUnitMetaTags INNER JOIN R_WFQueueUnit ON E_WFUnitMetaTags.WFUnit_FKID = R_WFQueueUnit.FK_WFUnitId INNER JOIN E_WFQueue ON R_WFQueueUnit.FK_WFQueueId = E_WFQueue.PKID";
                Common.ExecuteMonitor(sqlQueryStr);

                Common.SyncValidations();
                Common.SupplierUser2SupplierIds();
                Common.RefDbFetch();
                Common.ServerCollectionsCleanUp(sqlQueryStr);

                busy = false;
            }
        }
    }
}