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
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.IO;
using System.Windows.Forms;
using eFlow.SupplierPortalCore;
using System.Data.SqlClient;
using eFlow.CollectionManagement;
using TiS.Core.eFlowAPI;
using SupplierPortalCommon;
using System.Globalization;
using System.Threading;

namespace SupplierPortalService
{
    public partial class Service : ServiceBase
    {
        private bool busy = false;
        private bool busyExecuteMonitor = false;

        private System.Timers.Timer t = null;
        
        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            RunService();
        }

        public void RunService()
        {
            t = new System.Timers.Timer();
            t.Interval = 2000;
            t.Elapsed += new ElapsedEventHandler(timer_elapsed);

            Logging.CreateLog();
            Logging.InfoLog("Started SupplierPortalService");

            Common.GetInterval(ref t);
            t.Start();
        }

        protected override void OnStop()
        {
            t.Stop();
            Logging.InfoLog("Stopped SupplierPortalService");
        }

        private void timer_elapsed(object sender, EventArgs e)
        {
            if (!busy)
            {
                backgroundWorker.RunWorkerAsync();
            }

            if (!busyExecuteMonitor)
            {
                backgroundWorkerExecuteMonitor.RunWorkerAsync();
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!busy)
            {
                RunDaemon();
            }
        }

        private void RunDaemonExecuteMonitor()
        {
            if (!busyExecuteMonitor)
            {
                busyExecuteMonitor = true;

                Common.CleanUnusedImages();

                string sqlQueryStr = "SELECT E_WFUnitMetaTags.CreationTime, E_WFUnitMetaTags.BatchName, E_WFQueue.QueueName, R_WFQueueUnit.Status FROM E_WFUnitMetaTags INNER JOIN R_WFQueueUnit ON E_WFUnitMetaTags.WFUnit_FKID = R_WFQueueUnit.FK_WFUnitId INNER JOIN E_WFQueue ON R_WFQueueUnit.FK_WFQueueId = E_WFQueue.PKID";
                
                Common.ExecuteMonitor(sqlQueryStr);
                Common.ServerCollectionsCleanUp(sqlQueryStr);

                busyExecuteMonitor = false;
            }
        }

        private void RunDaemon()
        {
            if (!busy)
            {
                busy = true;

                Common.GetFromPortal();

                Common.SyncValidations();
                Common.ClearSupplierUser2SupplierIds();
                Common.SupplierUser2SupplierIds();
                Common.RefDbFetch();

                busy = false;
            }
        }

        private void backgroundWorkerExecuteMonitor_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!busyExecuteMonitor)
            {
                RunDaemonExecuteMonitor();
            }
        }
    }
}
