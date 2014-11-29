using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TiS.Engineering.TiffDll90
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new tiffDll90TesterForm());
        }
    }
}
