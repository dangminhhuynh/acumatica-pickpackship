using System;
using PX.Data;

namespace PX.SM
{
    [Serializable]
    public class SMPrintQueue : IBqlTable
    {
        public abstract class printQueue : PX.Data.IBqlField { }
        [PXDBString(10, IsKey = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Print Queue", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string PrintQueue { get; set; }

        public abstract class descr : PX.Data.IBqlField { }
        [PXDBString(100, IsUnicode = true)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string Descr { get; set; }
    }
}
