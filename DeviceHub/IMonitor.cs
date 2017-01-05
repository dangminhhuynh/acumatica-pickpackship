using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Acumatica.DeviceHub
{
    interface IMonitor
    {
        Task Initialize(Progress<MonitorMessage> progress, CancellationToken cancellationToken);
    }
}
