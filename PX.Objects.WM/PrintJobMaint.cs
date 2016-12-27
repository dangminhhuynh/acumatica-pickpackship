using System;
using System.Collections;
using PX.Data;

namespace PX.SM
{
    public class PrintJobMaint : PXGraph<PrintJobMaint, SMPrintJob>
    {
        public PXSelectOrderBy<SMPrintJob, OrderBy<Asc<SMPrintJob.createdDateTime>>> Job;
        public PXSelect<SMPrintJobParameter, Where<SMPrintJobParameter.jobID, Equal<Current<SMPrintJob.jobID>>>> Parameters;
    }
}
