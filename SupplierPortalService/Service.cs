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
using System.Globalization;
using System.Threading;
using System.Data.SqlClient;

using eFlow.CollectionManagement;
using eFlow.SupplierPortalCore;
using SupplierPortalCommon;

using TiS.Core.Application;
using TiS.Core.Common;
using TiS.Core.Domain;
using TiS.Core.TisCommon;
using TiS.Core.Application.DataModel.Dynamic;
using TiS.Core.Application.Interfaces;

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

                // eFLow 4.5
                //string sqlQueryStr = "SELECT E_WFUnitMetaTags.CreationTime, E_WFUnitMetaTags.BatchName, E_WFQueue.QueueName, R_WFQueueUnit.Status FROM E_WFUnitMetaTags INNER JOIN R_WFQueueUnit ON E_WFUnitMetaTags.WFUnit_FKID = R_WFQueueUnit.FK_WFUnitId INNER JOIN E_WFQueue ON R_WFQueueUnit.FK_WFQueueId = E_WFQueue.PKID";

                // eFlow 5
                string sqlQueryStr = "select E_Unit.TagCreationTime, E_Unit.Name, E_Queue.Name, E_Unit.QueueStatus FROM E_Unit, E_Queue where (E_Queue.ID = E_Unit.QueueID) ORDER BY E_Queue.Name ASC";

                Common.ExecuteMonitor(sqlQueryStr);
                Common.ServerCollectionsCleanUp(sqlQueryStr);
                Common.ClaimGarbage();

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

                Common.ClaimGarbage();

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
