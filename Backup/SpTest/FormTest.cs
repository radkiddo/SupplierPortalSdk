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

using eFlow.SupplierPortalCore;
using eFlow.CollectionManagement;
using TiS.Core.eFlowAPI;

namespace SpTest
{
    public partial class FormTest : Form
    {
        public FormTest()
        {
            InitializeComponent();
        }

        private void btnPostCollecData_Click(object sender, EventArgs e)
        {
            // Posts the following HTTP Post request, with this data:
            // Customer name = topimagesystems.com (use the customer's domain name: i.e.: topimagesystems.com)
            // eFlow Application Name = CLS
            // "00000323" is the name of the collection data to POST
            // 1 indicates the version or instance of the collection data (should always be 1)
            // strData (Collection Data in string format), i.e.: TEST1|SYSTEM|CLS|00000323|FreeProcess|1|Invoice_Date=|Invoice_Number=|Net_Amount1=|PO_Number=963645|Supplier_Name=|Total_Amount=0.00|VAT_Amount1=|VAT1=|dc=2/7/2013 12:34:58 PM|rc=0|http://www.doksend.com/supplierportal/images/speedyservices.com_CLS-00000323.tif
            // Port (where the HTTP server runs) = 80 (change port if the Supplier Portal sites runs on a different HTTP port)
            // SSL connection = false (use true if SSL is used)

            using (Sp s = new Sp())
            {
                s.HttpPostSimple("topimagesystems.com", "CLS", "00000323",
                    "TEST1|SYSTEM|CLS|00000323|FreeProcess|1|Invoice_Date=|Invoice_Number=|Net_Amount1=|PO_Number=963645|Supplier_Name=|Total_Amount=0.00|VAT_Amount1=|VAT1=|dc=2/7/2013 12:34:58 PM|rc=0|http://www.doksend.com/supplierportal/images/speedyservices.com_CLS-00000323.tif",
                    80, false);
            }
        }

        private void btnExistsCollecData_Click(object sender, EventArgs e)
        {
            // Posts the following HTTP Post request with the following data, in order to know if a collection already exists or not
            // Customer name = speedyservices.com (use the customer's domain name: i.e.: topimagesystems.com)
            // eFlow Application Name = CLS
            // 'cln' indicates the 'Collection Name' (field name) on the Supplier Portal 
            // '00000323' indicates the value of collection name for which you want to get the data, if it exists or not
            // Port (where the HTTP server runs) = 80 (change port if the Supplier Portal sites runs on a different HTTP port)
            // SSL connection = false (use true if SSL is used)

            using (Sp s = new Sp())
            {
                // 'cln' indicates the Collection Name on the Supplier Portal backend
                string res = s.HttpPostCollectionQry("topimagesystems.com", "CLS", "cln", "00000323", 80, false);
            }
        }

        private void btnGetCollectionDataAndPost_Click(object sender, EventArgs e)
        {
            // Uses the Collection Management SDK to 'grab' a collection in order to retrieve it's data
            // and then Post this data to the Supplier Portal

            // More info about the Collection Management SDK can be found here:
            // http://doksend.com/eFlowSdk/


            // Create a new Batch instance
            // Logs onto the CSM with "CLS" application
            // and "FreeProcess" station

            // This sample assumes you have an eFlow application called 'AdvancedDemo' and a sample collection 
            // called '00000323' sitting at the 'FreeProcess' station

            string collectionName = "00000323";

            using (Batch b = new Batch("CLS", "FreeProcess"))
            {
                // Gets the collection "00000323"

                ITisCollectionData collData = b.Get(collectionName);

                // Get the collection data and post the data to the Supplier Portal
                using (Sp s = new Sp())
                {
                    s.HttpPostSimple("topimagesystems.com", "CLS", "00000323",
                        "TEST1|SYSTEM|CLS|00000323|FreeProcess|1|Invoice_Date=|Invoice_Number=|Net_Amount1=|PO_Number=963645|Supplier_Name=|Total_Amount=0.00|VAT_Amount1=|VAT1=|dc=2/7/2013 12:34:58 PM|rc=0|http://www.doksend.com/supplierportal/images/speedyservices.com_CLS-00000323.tif",
                        80, false);
                }

                // Liberates the collection (writes it back)
                // to the CSM without moving it to the next queue
                b.Free(collData);
            }
        }

        private void btnDeleteAllCollecDataInstances_Click(object sender, EventArgs e)
        {
            // Posts the following HTTP Post request in order to DELETE the first found collection instance with 'cln' as collectionName
            // Customer name = topimagesystems.com (use the customer's domain name: i.e.: topimagesystems.com)
            // eFlow Application Name = CLS
            // '00000323' indicates the value of collection name for which you want to DELETE the first found collection instance
            // Port (where the HTTP server runs) = 80 (change port if the Supplier Portal sites runs on a different HTTP port)
            // SSL connection = false (use true if SSL is used)

            // HttpPostCollectionDeleteFirstFound does NOT perform any filtering, so it looks to see if any given instance of the collection exists.

            using (Sp s = new Sp())
            {
                string res = s.HttpPostCollectionDeleteFirstFound("topimagesystems.com", "CLS", "00000323", 80, false);
            }
        }

        private void btnDeleteAllCollecDataInstances_Click_1(object sender, EventArgs e)
        {
            // Posts the following HTTP Post request in order to DELETE ALL collection instances with 'cln' as collectionName
            // Customer name = topimagesystems.com (use the customer's domain name: i.e.: topimagesystems.com)
            // eFlow Application Name = CLS
            // '00000323' indicates the value of collection name for which you want to DELETE all collections instances
            // Port (where the HTTP server runs) = 80 (change port if the Supplier Portal sites runs on a different HTTP port)
            // SSL connection = false (use true if SSL is used)

            using (Sp s = new Sp())
            {
                s.HttpPostCollectionDeleteAll("topimagesystems.com", "CLS", "00000323", 80, false);
            }
        }

    }
}