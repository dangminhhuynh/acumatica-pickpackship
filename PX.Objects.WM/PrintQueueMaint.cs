using System;
using System.Collections;
using PX.Data;

namespace PX.SM
{
    public class PrintQueueMaint : PXGraph<PrintQueueMaint>
    {
        public PXSave<SMPrintQueue> Save;
        public PXCancel<SMPrintQueue> Cancel;
        public PXSelect<SMPrintQueue> Queues;
    }
}
