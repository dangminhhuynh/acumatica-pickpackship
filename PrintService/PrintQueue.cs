using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintService
{
    [Serializable]
    public class PrintQueue 
    {
        public const int PrinterDefault = -1;

        public enum PrinterOrientation
        {
            PrinterDefault = PrintQueue.PrinterDefault,
            Portrait = 1,
            Landscape = 2,
        }

        public string QueueName { get; set; }
        public string PrinterName { get; set; }
        public int PaperSize { get; set; }
        public int PaperSource { get; set; }
        public PrinterOrientation Orientation { get; set; }
        public bool RawMode { get; set; }
    }
}
