using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TiS.Core.eFlowAPI;

namespace TiS.Engineering.InputApi
{
    /// <summary>
    /// Deleagte methods for the various API events.
    /// </summary>
#if INTERNAL
      internal class CCDelegates
#else
    public class CCDelegates
#endif
    {
        public delegate void OnPreCreateCollectionEvt(CCTimerSearch source, ref CCCollection collection);
        public delegate void OnPostCreateCollectionEvt(CCTimerSearch source, String collectionName);
        public delegate void OnSearchedFilesEvt(CCTimerSearch source, ref CCFileList[] collectedFiles);
        public delegate void OnBaseActionEvt(CCTimerSearch source);
        public delegate void OnPreFileLockEvt(CCTimerSearch source, object filesHandler, ref string fileName);
        public delegate CCCollection OnCustomSourceFileReadEvt(CCreator creator, bool deleteSource, string fileName);

        public delegate void OnPostFileLockEvt(CCTimerSearch source, object fileHandler, string fileName);
        public delegate void OnPageReadEvt(object source, String filePath, int pageIndex, Bitmap page);
        public delegate void OnCollectionCreatedEvt(CCTimerSearch source, CCreator creator, ITisClientServicesModule csm, ITisCollectionData collection, ref bool canPut);
    }
}
