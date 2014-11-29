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

using TiS.StationDeclaration.Recognition;
using TiS.DevGER.MR;

namespace IRSupplierPortalDll
{
    //public class SPFreeProcess : TiS.Core.Application.Events.Station.EventsAdapterSimpleAuto
    public class SPFreeProcess : PostReco
    {
        public override void OnPrePutCollections(ITisClientServicesModule oCSM, ref bool bCanPut)
        {
            base.OnPrePutCollections(oCSM, ref bCanPut);

            try
            {
                foreach (ITisCollectionData cd in oCSM.Dynamic.AvailableCollections)
                {
                    string sp = cd.GetNamedUserTags(Tags.SupplierPortalDomainTag);

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
