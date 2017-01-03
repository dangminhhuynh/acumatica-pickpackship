using System;
using System.Collections;
using PX.Data;

namespace PX.SM
{
    public class ScaleMaint : PXGraph<ScaleMaint, SMScale>
    {
        public PXSelect<SMScale> Scale;
    }
}
