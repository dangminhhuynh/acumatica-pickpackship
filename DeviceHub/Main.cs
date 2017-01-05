using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using PdfPrintingNet;
using System.Net.Http;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Threading;
using Acumatica.DeviceHub.ScreenApi;

namespace Acumatica.DeviceHub
{
    public partial class Main : Form
    {
        private const string PrintJobsScreen = "SM206500";
        private const string PrintQueuesScreen = "SM206510";
        private ScreenApi.Screen _screen;
        private Dictionary<string, PrintQueue> _queues;

        private bool _processing = false;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.AcumaticaUrl))
            {
                using (var form = new Configuration())
                {
                    if(form.ShowDialog() != DialogResult.OK)
                    {
                        Application.Exit();
                        return;
                    }
                }
            }

            LoginToAcumaticaAndStartMonitoringQueues();
        }

        private void WriteToLog(string message, params object[] args)
        {
            logListBox.Items.Insert(0, (object) DateTime.Now.ToString() + " - " + String.Format(message, args));
            if (logListBox.Items.Count > 100) logListBox.Items.RemoveAt(100);
            Application.DoEvents(); //Should use background thread instead...
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //Some print drivers like Microsoft XPS driver will show dialog and pump message queue
            //which will cause the timer to be reentrant; global variable is to prevent to simulatenous processing of the print jobs.
            if(!_processing)
            { 
                _processing = true;
                PollPrintJobs();
                _processing = false;
            }
        }

        private void LoginToAcumaticaAndStartMonitoringQueues()
        {
            WriteToLog("Logging in to {0}...", Properties.Settings.Default.AcumaticaUrl);
            _screen = new ScreenApi.Screen();
            _screen.Url = Properties.Settings.Default.AcumaticaUrl + "/Soap/.asmx";
            _screen.CookieContainer = new System.Net.CookieContainer();

            try
            { 
                _screen.Login(Properties.Settings.Default.Login, Settings.ToInsecureString(Settings.DecryptString(Properties.Settings.Default.Password)));
            }
            catch(Exception ex)
            {
                WriteToLog("Login failed. Please verify configuration. Error: {0}", ex.Message);
                _screen = null;
                return;
            }

            if(InitializeQueues())
            { 
                timer.Enabled = true;
            }
        }

        private void LogoutFromAcumatica()
        {
            WriteToLog("Logging out...");
            if (_screen != null)
            {
                _screen.Logout();
                _screen = null;
            }
        }

        private bool InitializeQueues()
        {
            WriteToLog("Initializing print queues...");
            _queues = JsonConvert.DeserializeObject<IEnumerable<PrintQueue>>(Properties.Settings.Default.Queues).ToDictionary<PrintQueue, string>(q=>q.QueueName);
            
            var configuredQueues = GetAvailableQueuesFromAcumatica();
            foreach(var queue in _queues)
            {
                if (configuredQueues.Contains(queue.Key))
                {
                    WriteToLog(String.Format("Print Queue {0} initialized.", queue.Key));
                }
                else
                {
                    WriteToLog(String.Format("Print Queue {0} is not configured on this Acumatica instance. Please check configuration.", queue.Key));
                    return false;
                }
            }

            return true;
        }

        private HashSet<string> GetAvailableQueuesFromAcumatica()
        {
            var commands = new Command[]
            {
                new Field { FieldName = "PrintQueue", ObjectName = "Queues" }
            };

            var results = _screen.Export(PrintQueuesScreen, commands, null, 0, false, true);
            var queueNames = new HashSet<string>();

            for (int i = 0; i < results.Length; i++)
            {
                queueNames.Add(results[i][0]);
            }

            return queueNames;
        }

        private void PollPrintJobs()
        {
            WriteToLog("Polling for new print jobs...");
            var commands = new Command[]
            {
                new Field { FieldName = "JobID", ObjectName = "Job" },
                new Field { FieldName = "ReportID", ObjectName = "Job" },
                new Field { FieldName = "PrintQueue", ObjectName = "Job" },
                new Field { FieldName = "ParameterName", ObjectName = "Parameters" },
                new Field { FieldName = "ParameterValue", ObjectName = "Parameters" },
                new Field { FieldName = "ParameterValue", ObjectName = "Parameters" },
            };

            var filters = new List<Filter>();
            foreach(var queue in _queues)
            { 
                filters.Add(new Filter { Field = new Field { FieldName = "PrintQueue", ObjectName = "Job" }, Value = queue.Key, Condition = FilterCondition.Equals, Operator = FilterOperator.Or });
            }

            var results = _screen.Export(PrintJobsScreen, commands, filters.ToArray(), 0, false, true);

            string currentJobID = null;
            string reportID = null;
            string queueName = null;
            Dictionary<string, string> parameters = null;

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i][0] != currentJobID)
                {
                    if (currentJobID != null) ProcessJob(_queues[queueName], currentJobID, reportID, parameters);

                    currentJobID = results[i][0];
                    reportID = results[i][1];
                    queueName = results[i][2];

                    parameters = new Dictionary<string, string>();
                }

                parameters.Add(results[i][3], results[i][4]);
            }

            if (currentJobID != null) ProcessJob(_queues[queueName], currentJobID, reportID, parameters);
        }

        private void ProcessJob(PrintQueue queue, string jobID, string reportID, Dictionary<string, string> parameters)
        {
            WriteToLog("Queue {0} - processing print job {1}...", queue.QueueName, jobID);
            byte[] data = null;
            if (reportID == String.Empty)
            {
                data = GetFileID(parameters["FILEID"]);
            }
            else
            {
                data = GetReportPdf(reportID, parameters);
            }
            
            if (queue.RawMode)
            {
                if (IsPdf(data))
                {
                    WriteToLog("Queue {0} - print job {1} contains a PDF file that can't be sent in raw mode...", queue.QueueName, jobID);
                }
                else
                {
                    PrintRaw(queue, data);
                }
            }
            else
            {
                if (IsPdf(data))
                {
                    PrintPdf(queue, data);
                }
                else
                {
                    WriteToLog("Queue {0} - print job {1} contains a file which doesn't look like a valid PDF file...", queue.QueueName, jobID);
                }
            }

            DeleteJobFromQueue(jobID);
        }

        private static bool IsPdf(byte[] data)
        {
            //%PDF−1.0
            return (data.Length > 4 && data[0] == 0x25 && data[1] == 0x50 && data[2] == 0x44 && data[3] == 0x46);
        }
        
        private void PrintRaw(PrintQueue queue, byte[] rawData)
        {
            WriteToLog("Queue {0} - printing raw data to {1}...", queue.QueueName, queue.PrinterName);
            RawPrinterHelper.SendRawBytesToPrinter(queue.PrinterName, rawData);
        }

        private void PrintPdf(PrintQueue queue, byte[] pdfReport)
        {
            WriteToLog("Queue {0} - printing PDF to {1}...", queue.QueueName, queue.PrinterName);
            var pdfPrint = new PdfPrint("demoCompany", "demoKey");
            pdfPrint.PrinterName = queue.PrinterName;

            // Retrieve paper size, source and orientation
            var printerSettings = new System.Drawing.Printing.PrinterSettings();
            printerSettings.PrinterName = queue.PrinterName;

            if (queue.PaperSize != PrintQueue.PrinterDefault)
            {
                bool paperSizeSet = false;
                foreach (PaperSize paperSize in printerSettings.PaperSizes)
                {
                    if (paperSize.RawKind == queue.PaperSize)
                    {
                        pdfPrint.PaperSize = paperSize;
                        paperSizeSet = true;
                        break;
                    }
                }

                if(!paperSizeSet)
                {
                    WriteToLog("Paper Size {0} not found for printer {1}. Please verify configuration.", queue.PrinterName, queue.PaperSize);
                }
            }

            if(queue.PaperSource != PrintQueue.PrinterDefault)
            {
                bool paperSourceSet = false;
                foreach (PaperSource paperSource in printerSettings.PaperSources)
                {
                    if (paperSource.RawKind == queue.PaperSource)
                    {
                        pdfPrint.PaperSource = paperSource;
                        paperSourceSet = true;
                        break;
                    }
                }

                if (!paperSourceSet)
                {
                    WriteToLog("Paper Source {0} not found for printer {1}. Please verify configuration.", queue.PrinterName, queue.PaperSource);
                }
            }

            if(queue.Orientation == PrintQueue.PrinterOrientation.Landscape)
            {
                pdfPrint.IsLandscape = true;
            }
            
            pdfPrint.Print(pdfReport);
        }

        private byte[] GetFileID(string fileID)
        {
            using (var handler = new HttpClientHandler() { CookieContainer = _screen.CookieContainer })
            using (var client = new HttpClient(handler))
            using (var result = client.GetAsync(Properties.Settings.Default.AcumaticaUrl + "/entity/Default/6.00.001/files/" + fileID).Result)
            {
                result.EnsureSuccessStatusCode();
                return result.Content.ReadAsByteArrayAsync().Result;
            }
        }

        private byte[] GetReportPdf(string reportID, Dictionary<string, string> parameters)
        {
            var commands = new List<Command>();
            foreach (string parameterName in parameters.Keys)
            {
                commands.Add(new Value { Value = parameters[parameterName], ObjectName = "Parameters", FieldName = parameterName });
            }

            commands.Add(new Field { FieldName = "PdfContent", ObjectName = "ReportResults" });

            var result = _screen.Submit(reportID, commands.ToArray());
            if (result != null && result.Length > 0)
            {
                var field = result[0]
                    .Containers.Where(c => c != null && c.Name == "ReportResults").FirstOrDefault()
                    .Fields.Where(f => f != null && f.FieldName == "PdfContent").FirstOrDefault();

                return Convert.FromBase64String(field.Value);
            }
            else
            {
                throw new ApplicationException(String.Format("Nothing returned by webservice for report {0}", reportID));
            }
        }

        private void DeleteJobFromQueue(string jobID)
        {
            WriteToLog("Deleting job {0} from queue...", jobID);
            var commands = new Command[]
            {
                new Key { ObjectName = "Job", FieldName = "JobID", Value = "=[Job.JobID]" },
                new ScreenApi.Action { FieldName = "Cancel", ObjectName = "Job" },
                new Value { Value = jobID, ObjectName = "Job", FieldName = "JobID", Commit = true },
                new ScreenApi.Action { FieldName = "Delete", ObjectName = "Job" }
            };

            var result = _screen.Submit(PrintJobsScreen, commands);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogoutFromAcumatica();
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon.Visible = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            LogoutFromAcumatica();

            using (var form = new Configuration())
            {
                form.ShowDialog();
                LoginToAcumaticaAndStartMonitoringQueues();
            }
        }
    }
}
