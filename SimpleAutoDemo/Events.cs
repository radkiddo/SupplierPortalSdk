using System;
using System.Collections.Generic;
using System.Text;

using TiS.Core.eFlowAPI;
using TiS.Core.PlatformRuntime;

using eFlow.SupplierPortalLite;
using SupplierPortalCommon;

namespace SimpleAutoDemo
{
    public class Events : TiS.Core.eFlowAPI.Events.EventsAdapterSimpleAuto
    {
        public override void OnPostGetCollections(ITisClientServicesModule oCSM)
        {
            foreach (ITisCollectionData cd in oCSM.Dynamic.AvailableCollections)
            {
                using (SpLite p = new SpLite())
                {
                    ITisCollectionData collec = cd;
                    p.SendDataToPortal(ref collec, oCSM.Application.AppName,
                        p._getSetting(CommonConst.supplierPortalStationName), cd.Name, true);
                }
            }
        }
        
        public override void OnPrePutCollections(ITisClientServicesModule oCSM, ref bool bCanPut)
        {
            for (int i = 0; i <= oCSM.Dynamic.AvailableCollections.Count - 1; i++)
            {
                ITisCollectionData cd = oCSM.Dynamic.AvailableCollections.GetByIndex(0);

                using (SpLite p = new SpLite())
                {
                    bool changed = false;

                    do
                    {
                        changed = p.GetDataFromPortal(ref cd, oCSM.Application.AppName,
                            p._getSetting(CommonConst.supplierPortalStationName), cd.Name, "topimagesystems.com");

                    } while (!changed);

                    if (changed)
                    {
                        bCanPut = true;
                    }
                }
            }
        }
    }
}
