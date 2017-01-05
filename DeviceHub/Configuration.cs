using HidLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using System.Windows.Forms;

namespace Acumatica.DeviceHub
{
    public partial class Configuration : Form
    {
        private const string NewQueueName = "<New>";
        private List<PrintQueue> _queues;

        public Configuration()
        {
            InitializeComponent();
        }
        
        private void Configuration_Load(object sender, EventArgs e)
        {
            InitPrinterList();
            InitUsbScaleList();

            acumaticaUrlTextBox.Text = Properties.Settings.Default.AcumaticaUrl;
            loginTextBox.Text = Properties.Settings.Default.Login;
            passwordTextBox.Text = Settings.ToInsecureString(Settings.DecryptString(Properties.Settings.Default.Password));

            if (String.IsNullOrEmpty(Properties.Settings.Default.Queues))
            {
                _queues = new List<PrintQueue>();
            }
            else
            {
                _queues = new List<PrintQueue>(JsonConvert.DeserializeObject<IEnumerable<PrintQueue>>(Properties.Settings.Default.Queues));
                _queues.ForEach(q => queueList.Items.Add(q));
            }

            scalesDropDown.SelectedItem = Properties.Settings.Default.ScaleDeviceVendorId;
            acumaticaScaleIDTextBox.Text = Properties.Settings.Default.ScaleID;

            SetControlsState();
        }

        private void InitPrinterList()
        {
            var printers = new List<string>();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            printerCombo.DataSource = printers;
        }

        private void InitUsbScaleList()
        {
            var scales = new List<string>();
            foreach(var device in HidDevices.Enumerate())
            {
                scales.Add(device.Description);
            }
            scalesDropDown.DataSource = scales;
        }

        private void SetControlsState()
        {
            queueName.Enabled = (queueList.SelectedItem != null);
            removePrintQueue.Enabled = (queueList.SelectedItem != null);
            printerCombo.Enabled = (queueList.SelectedItem != null);
            paperSizeCombo.Enabled = (queueList.SelectedItem != null && rawModeCheckbox.Checked == false);
            orientationGroupBox.Enabled = (queueList.SelectedItem != null && rawModeCheckbox.Checked == false);
            paperSourceCombo.Enabled = (queueList.SelectedItem != null && rawModeCheckbox.Checked == false);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Uri validatedUri;
            if(!Uri.TryCreate(acumaticaUrlTextBox.Text, UriKind.Absolute, out validatedUri))
            {
                MessageBox.Show("Please enter a valid URL.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                mainTab.SelectedIndex = 0;
                acumaticaUrlTextBox.Focus();
                return;
            }

            if (String.IsNullOrEmpty(loginTextBox.Text))
            {
                MessageBox.Show("Please enter your login.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                mainTab.SelectedIndex = 0;
                loginTextBox.Focus();
                return;
            }

            if (String.IsNullOrEmpty(passwordTextBox.Text))
            {
                MessageBox.Show("Please enter your password.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                mainTab.SelectedIndex = 0;
                passwordTextBox.Focus();
                return;
            }

            if(_queues.Count == 0)
            {
                MessageBox.Show("Please configure at least one print queue to monitor.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                mainTab.SelectedIndex = 1;
                return;
            }
            else
            {
                PrintQueue unnamedQueue = _queues.FirstOrDefault(q => q.QueueName == NewQueueName);
                if (unnamedQueue != null)
                {
                    MessageBox.Show("Please give a name to this queue.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    mainTab.SelectedIndex = 1;
                    queueList.SelectedItem = unnamedQueue;
                    queueName.Focus();
                    return;
                }
            }
            
            var screen = new ScreenApi.Screen();
            screen.Url = acumaticaUrlTextBox.Text + "/Soap/.asmx";
            try
            {
                screen.Login(loginTextBox.Text, passwordTextBox.Text);
                screen.Logout();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error connecting to Acumatica: {0}", ex.Message), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                mainTab.SelectedIndex = 0;
                acumaticaUrlTextBox.Focus();
                return;
            }

            Properties.Settings.Default.AcumaticaUrl = acumaticaUrlTextBox.Text;
            Properties.Settings.Default.Login = loginTextBox.Text;
            Properties.Settings.Default.Password = Settings.EncryptString(Settings.ToSecureString(passwordTextBox.Text));
            Properties.Settings.Default.Queues = JsonConvert.SerializeObject(_queues);
            Properties.Settings.Default.ScaleID = acumaticaScaleIDTextBox.Text;
            Properties.Settings.Default.Save();
            
            this.DialogResult = DialogResult.OK;
        }
        

        private void queueList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetControlsState();

            var selectedItem = queueList.SelectedItem as PrintQueue;
            if(selectedItem != null)
            {
                queueName.Text = selectedItem.QueueName;
                printerCombo.SelectedItem = selectedItem.PrinterName;
                rawModeCheckbox.Checked = selectedItem.RawMode;
                paperSizeCombo.SelectedValue = selectedItem.PaperSize;
                paperSourceCombo.SelectedValue = selectedItem.PaperSource;
                
                switch(selectedItem.Orientation)
                {
                    case PrintQueue.PrinterOrientation.PrinterDefault:
                        orientationDefault.Checked = true;
                        break;
                    case PrintQueue.PrinterOrientation.Portrait:
                        orientationPortrait.Checked = true;
                        break;
                    case PrintQueue.PrinterOrientation.Landscape:
                        orientationLandscape.Checked = true;
                        break;
                }
            }
        }

        private void addPrintQueue_Click(object sender, EventArgs e)
        {
            var newQueue = new PrintQueue();
            newQueue.QueueName = NewQueueName;
            newQueue.PrinterName = new PrinterSettings().PrinterName;
            newQueue.PaperSize = PrintQueue.PrinterDefault;
            newQueue.PaperSource = PrintQueue.PrinterDefault;
            newQueue.Orientation = PrintQueue.PrinterOrientation.PrinterDefault;

            _queues.Add(newQueue);
            queueList.Items.Add(newQueue);
            queueList.SelectedItem = newQueue;
            SetControlsState();

            queueName.Focus();
        }

        private void removePrintQueue_Click(object sender, EventArgs e)
        {
            _queues.Remove((PrintQueue)queueList.SelectedItem);
            queueList.Items.Remove(queueList.SelectedItem);
            SetControlsState();
        }
        
        private void printerCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = queueList.SelectedItem as PrintQueue;
            if (selectedItem != null)
            {
                selectedItem.PrinterName = (string)printerCombo.SelectedItem;
            }

            // Retrieve paper sizes and sources from printer settings
            var printerSettings = new System.Drawing.Printing.PrinterSettings();
            printerSettings.PrinterName = (string) printerCombo.SelectedItem;
            
            var sizes = new List<PaperSize>();
            sizes.Add(new PaperSize() { PaperName = "<Printer Default>", RawKind = PrintQueue.PrinterDefault });
            foreach (PaperSize size in printerSettings.PaperSizes)
            {
                sizes.Add(size);
            }
            paperSizeCombo.DataSource = sizes;

            var bins = new List<PaperSource>();
            bins.Add(new PaperSource() { SourceName = "<Printer Default>", RawKind = PrintQueue.PrinterDefault } );
            foreach (PaperSource bin in printerSettings.PaperSources)
            {
                bins.Add(bin);
            }
            paperSourceCombo.DataSource = bins;
        }

        private void queueName_TextChanged(object sender, EventArgs e)
        {
            var selectedItem = queueList.SelectedItem as PrintQueue;
            if (selectedItem != null)
            {
                selectedItem.QueueName = queueName.Text;
            }
        }

        private void rawModeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            var selectedItem = queueList.SelectedItem as PrintQueue;
            if (selectedItem != null)
            {
                selectedItem.RawMode = rawModeCheckbox.Checked;
            }

            SetControlsState();
        }

        private void paperSizeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = queueList.SelectedItem as PrintQueue;
            if (selectedItem != null)
            {
                selectedItem.PaperSize = (int) paperSizeCombo.SelectedValue;
            }
        }

        private void paperSourceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = queueList.SelectedItem as PrintQueue;
            if (selectedItem != null)
            {
                selectedItem.PaperSource = (int)paperSourceCombo.SelectedValue;
            }
        }

        private void orientationDefault_CheckedChanged(object sender, EventArgs e)
        {
            if(orientationDefault.Checked)
            { 
                var selectedItem = queueList.SelectedItem as PrintQueue;
                if (selectedItem != null)
                {
                    selectedItem.Orientation = PrintQueue.PrinterOrientation.PrinterDefault;
                }
            }
        }

        private void orientationPortrait_CheckedChanged(object sender, EventArgs e)
        {
            if (orientationPortrait.Checked)
            {
                var selectedItem = queueList.SelectedItem as PrintQueue;
                if (selectedItem != null)
                {
                    selectedItem.Orientation = PrintQueue.PrinterOrientation.Portrait;
                }
            }
        }

        private void orientationLandscape_CheckedChanged(object sender, EventArgs e)
        {
            if (orientationLandscape.Checked)
            {
                var selectedItem = queueList.SelectedItem as PrintQueue;
                if (selectedItem != null)
                {
                    selectedItem.Orientation = PrintQueue.PrinterOrientation.Landscape;
                }
            }
        }

        private void queueName_Validated(object sender, EventArgs e)
        {
            //Force refresh of text in listbox.
            queueList.Items[queueList.SelectedIndex] = queueList.SelectedItem;
        }
    }
}
