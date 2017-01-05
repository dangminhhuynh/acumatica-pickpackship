using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.DeviceHub
{
    public class MonitorMessage
    {
        public enum MonitorStates
        {
            Undefined,
            Ok,
            Warning,
            Error
        }

        public MonitorMessage(string text) : this(text, MonitorStates.Undefined)
        {
        }

        public MonitorMessage(string text, MonitorStates state)
        {
            this.Text = text;
            this.State = state;
        }

        public string Text { get; set; }
        public MonitorStates State { get; set; }
    }
}
