using System;
using System.Collections.Generic;
using System.Text;

using TiS.DevGER.IR;

using TiS.Core.eFlowAPI;
using TiS.Core.PlatformRuntime;
using TiS.Core.eFlowAPI.Events;

using eFlow.SupplierPortalLite;

namespace IRSupplierPortalDll
{
    public class SPFreeProcess : IRFreeProcessStation
    {
        public override void OnPrePutCollections(ITisClientServicesModule oCSM, ref bool bCanPut)
        {
            base.OnPrePutCollections(oCSM, ref bCanPut);

            try
            {
                foreach (ITisCollectionData cd in oCSM.Dynamic.AvailableCollections)
                {
                    string sp = cd.get_NamedUserTags(Tags.SupplierPortalDomainTag);

                    if (sp != String.Empty)
                    {
                        cd.NextStation = Tags.SupplierPortalCompletion;
                        
                        using (SpLite p = new SpLite())
                        {
                            p.SendDataToPortal(cd, oCSM.Application.AppName, oCSM.Session.StationName, cd.Name, true, 1);
                        }
                    }
                }
            }
            catch
            {}
        }
    }
}
