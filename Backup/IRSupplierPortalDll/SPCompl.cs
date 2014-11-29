using System;
using System.Collections.Generic;
using System.Text;

using TiS.Core.eFlowAPI;
using TiS.Core.PlatformRuntime;
using TiS.Core.eFlowAPI.Events;

using eFlow.SupplierPortalLite;
using eFlow.CollectionManagement;

namespace IRSupplierPortalDll
{
    public class SPCompl : EventsAdapterSimpleAuto
    {
        bool iterating = false;
        string currentCollection = String.Empty;

        public override void OnPrePutCollections(ITisClientServicesModule oCSM, ref bool bCanPut)
        {
            try
            {
                foreach (ITisCollectionData cd in oCSM.Dynamic.AvailableCollections)
                {
                    bCanPut = false;
                    currentCollection = cd.Name;
                }
            }
            catch { }
        }

        public override void OnTimer(TiS.Core.eFlowAPI.ITisClientServicesModule oCSM)
        {
            if (!iterating)
            {
                try
                {
                    iterating = true;
                    using (SpLite p = new SpLite())
                    {
                        string[] collections = p.GetCollectionsFromStation(oCSM.Application.AppName, oCSM.Session.StationName);

                        for (int i = 0; i <= collections.Length - 1; i++)
                        {
                            p.ForceUnlock(oCSM.Application.AppName, oCSM.Session.StationName, collections[i]);

                            using (Batch b = new Batch(oCSM.Application.AppName, oCSM.Session.StationName))
                            {
                                ITisCollectionData cd = b.Get(collections[i]);

                                bool changed = false;

                                changed = p.GetDataFromPortal(ref cd, oCSM.Application.AppName, oCSM.Session.StationName, cd.Name, String.Empty);

                                if (changed)
                                {
                                    cd.NextStation = "PreExport";

                                    p.SendDataToPortal(cd, oCSM.Application.AppName, cd.NextStation, cd.Name, false, 1);
                                    b.Put(cd);
                                }
                                else
                                {
                                    p.SendDataToPortal(cd, oCSM.Application.AppName, oCSM.Session.StationName, cd.Name, false, 1);
                                    b.Free(cd);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    iterating = false;
                }
            }
        }
    }
}
