using System;
using PX.Data;

namespace PX.SM
{
    [Serializable]
    public class SMPrintJobParameter : IBqlTable
    {
        public abstract class jobID : PX.Data.IBqlField { }
        [PXDBGuid(IsKey = true)]
        [PXDefault(typeof(SMPrintJob.jobID))]
        [PXParent(typeof(Select<SMPrintJob, Where<SMPrintJob.jobID, Equal<Current<SMPrintJobParameter.jobID>>>>))]
        [PXUIField(DisplayName = "Job ID")]
        public virtual Guid? JobID { get; set; }

        public abstract class parameterName : PX.Data.IBqlField { }
        [PXDBString(255, IsKey = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Parameter Name")]
        public virtual string ParameterName { get; set; }

        public abstract class parameterValue : PX.Data.IBqlField { }
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Parameter Value")]
        public virtual string ParameterValue { get; set; }
    }
}
