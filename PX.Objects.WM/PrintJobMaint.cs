using System;
using System.Collections;
using PX.Data;

namespace PX.SM
{
    public class PrintJobMaint : PXGraph<PrintJobMaint, SMPrintJob>
    {
        public PXSelect<SMPrintJob> Job;
        public PXSelect<SMPrintJobParameter, Where<SMPrintJobParameter.jobID, Equal<Current<SMPrintJob.jobID>>>> Parameters;

        public virtual void SMPrintJob_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            const string fileIdParameter = "FILEID";
            bool isFileId = false;
            SMPrintJob smPrintJob = e.Row as SMPrintJob;
            
            if (smPrintJob != null && String.IsNullOrWhiteSpace(smPrintJob.ReportID))
            {
                foreach (SMPrintJobParameter smPrintJobParameter in Parameters.Select())
                {
                    if (smPrintJobParameter.ParameterName.Trim().ToUpperInvariant().Equals(fileIdParameter))
                    {
                        isFileId = true;
                        break;
                    }
                }

                if (!isFileId)
                    throw new PXRowPersistingException(typeof(SMPrintJob.reportID).Name, null, Objects.WM.Messages.MissingFileIdError);
            }
        }
    }
}
