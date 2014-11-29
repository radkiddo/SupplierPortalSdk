#region "about"

//
// eFLOW Supplier Portal SDK
// 2013 (c) - Top Image Systems (a project initiated by the UK branch)
//
// The purpose of this SDK is to communicate with the Supplier Portal for eFlow Invoices.
// Developed by: Eduardo Freitas
//

#endregion "about"

using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace SupplierPortalService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            #if (!DEBUG)
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new Service() };

            ServiceBase.Run(ServicesToRun);
            #else
            Service service = new Service();
            service.RunService();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            #endif 
        }
    }
}