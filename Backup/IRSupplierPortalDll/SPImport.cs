using System;
using System.Collections.Generic;
using System.Text;

using TiS.Core.eFlowAPI;
using TiS.Core.PlatformRuntime;
using TiS.Core.eFlowAPI.Events;

using eFlow.SupplierPortalLite;

namespace IRSupplierPortalDll
{
    public class SPImport : EventsAdapterSimpleAuto
    {
        bool iterating = false;

        public override void OnTimer(TiS.Core.eFlowAPI.ITisClientServicesModule oCSM)
        {
            if (!iterating)
            {
                try
                {
                    iterating = true;
                    using (SpLite p = new SpLite())
                    {
                        p.CreateCollectionFromImportFolder(oCSM.Application.AppName, oCSM, "PageOCR");
                    }
                }
                finally
                {
                    iterating = false;
                }
            }
        }
    }
}
