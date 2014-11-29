using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using eFlow.SupplierPortalLite;

namespace SupplierPortalSdkLiteDemo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSendCollectionData_Click(object sender, EventArgs e)
        {
            // This sample assumes that you have AdvancedDemo installed with a collection named '00000002'
            // you can use another of the AdvancedDemo sample images and import them through FilePortal
            // and use that collection.

            using (SpLite s = new SpLite())
            {
                s.SendDataToPortal("AdvancedDemo", "OfficeForms", "00000002", false, 3);
            }
        }

        private void btnReceiveData_Click(object sender, EventArgs e)
        {
            // This sample assumes that you have AdvancedDemo installed with a collection named '00000002'

            using (SpLite s = new SpLite())
            {
                s.GetDataFromPortal("AdvancedDemo", "OfficeForms", "00000002", "topimagesystems.com");
                //s.GetDataFromPortal("CLS", "FreeProcess", "00000323", "topimagesystems.com");
            }
        }

        private void btnRemoveCollectionData_Click(object sender, EventArgs e)
        {
            using (SpLite s = new SpLite())
            {
                s.RemoveDataFromPortal("CLS", "00000345", "speedyservices.com");
            }
        }
    }
}