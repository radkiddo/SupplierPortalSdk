using System;
using System.Collections.Generic;
using System.Text;

using TiS.Core.TisCommon;
using TiS.Core.Domain;
using TiS.Core.Common;
using TiS.Core.Application;
using TiS.Core.Application.Interfaces;
using TiS.Core.Application.DataModel.Dynamic;
using TiS.Core.Application.Workflow;

using eFlow.SupplierPortalLite;

namespace IRSupplierPortalDll
{
    public class SPImport : TiS.Core.Application.Events.Station.EventsAdapterSimpleAuto
    {
        bool iterating = false;

        public override void OnTimer(ITisClientServicesModule oCSM)
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
