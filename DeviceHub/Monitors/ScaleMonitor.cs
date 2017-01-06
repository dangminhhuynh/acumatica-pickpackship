using Acumatica.DeviceHub.Properties;
using Acumatica.DeviceHub.ScreenApi;
using HidLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Acumatica.DeviceHub
{
    class ScaleMonitor : IMonitor
    {
        private IProgress<MonitorMessage> _progress;

        private const string ScaleScreen = "SM206530";
        private ScreenApi.Screen _screen;
        private decimal _lastWeightSentToAcumatica = 0;

        public Task Initialize(Progress<MonitorMessage> progress, CancellationToken cancellationToken)
        {
            _progress = progress;

            if(String.IsNullOrEmpty(Properties.Settings.Default.ScaleID))
            {
                _progress.Report(new MonitorMessage(Strings.ScaleConfigurationMissingWarning));
                return null;
            }

            return Task.Run(() =>
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        LogoutFromAcumatica();
                        break;
                    }

                    decimal currentWeight = 0;
                    HidDevice scale = HidDevices.Enumerate(Properties.Settings.Default.ScaleDeviceVendorId, Properties.Settings.Default.ScaleDeviceProductId).FirstOrDefault();
                    if (scale != null)
                    {
                        using (scale)
                        {
                            scale.OpenDevice();

                            int waitTries = 0;
                            while (!scale.IsConnected)
                            {
                                // Sometimes the scale doesn't open immediately, retry a few times.
                                Thread.Sleep(50);

                                waitTries++;
                                if (waitTries > 10)
                                {
                                    throw new ApplicationException(Strings.ScaleConnectionError);
                                }
                            }

                            var inData = scale.Read(250);

                            // This is unconfirmed - try to find documentation for the DYMO scales?
                            // Byte 0 == Report ID?
                            // Byte 1 == Scale Status (1 == Fault, 2 == Stable @ 0, 3 == In Motion, 4 == Stable, 5 == Under 0, 6 == Over Weight, 7 == Requires Calibration, 8 == Requires Re-Zeroing)
                            // Byte 2 == Weight Unit? (2=GR, 11=OZ)
                            // Byte 3 == Data Scaling (decimal placement) - signed byte is power of 10?
                            // Byte 4 == Weight LSB
                            // Byte 5 == Weight MSB
                            currentWeight = (Convert.ToDecimal(inData.Data[4]) +
                                Convert.ToDecimal(inData.Data[5]) * 256) * 
                                Properties.Settings.Default.ScaleWeightMultiplier;

                            _progress.Report(new MonitorMessage(String.Format(Strings.ScaleWeightNotify, currentWeight)));
                            scale.CloseDevice();
                        }
                        
                        try
                        {
                            if (_lastWeightSentToAcumatica != currentWeight)
                            {
                                if (_screen != null || LoginToAcumatica())
                                {
                                    UpdateWeight(Properties.Settings.Default.ScaleID, currentWeight);
                                    _lastWeightSentToAcumatica = currentWeight;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Assume the server went offline or our session got lost - new login will be attempted in next iteration
                            _progress.Report(new MonitorMessage(String.Format(Strings.ScaleWeightError, ex.Message), MonitorMessage.MonitorStates.Error));
                            _screen = null;
                            System.Threading.Thread.Sleep(Properties.Settings.Default.ErrorWaitInterval);
                        }
                    }

                    System.Threading.Thread.Sleep(Properties.Settings.Default.ScaleReadInterval);
                }
            });
        }

        private void UpdateWeight(string scaleID, decimal weight)
        {
            _progress.Report(new MonitorMessage(String.Format(Strings.UpdateScaleWeightNotify, scaleID)));
            var commands = new Command[]
            {
                new Key { ObjectName = "Scale", FieldName = "ScaleID", Value = "=[Scale.ScaleID]" },
                new ScreenApi.Action { FieldName = "Cancel", ObjectName = "Scale" },
                new Value { Value = scaleID, ObjectName = "Scale", FieldName = "ScaleID", Commit = true },
                new Value { Value = weight.ToString(System.Globalization.CultureInfo.InvariantCulture), ObjectName = "Scale", FieldName = "LastWeight" },
                new ScreenApi.Action { FieldName = "Save", ObjectName = "Scale" },
            };
            var result = _screen.Submit(ScaleScreen, commands);
            _progress.Report(new MonitorMessage(String.Format(Strings.UpdateScaleWeightSuccessNotify, scaleID), MonitorMessage.MonitorStates.Ok));
        }

        private bool LoginToAcumatica()
        {
            _progress.Report(new MonitorMessage(String.Format(Strings.LoginNotify, Properties.Settings.Default.AcumaticaUrl)));
            _screen = new ScreenApi.Screen();
            _screen.Url = Properties.Settings.Default.AcumaticaUrl + "/Soap/.asmx";
            _screen.CookieContainer = new System.Net.CookieContainer();

            try
            {
                _screen.Login(Properties.Settings.Default.Login, Settings.ToInsecureString(Settings.DecryptString(Properties.Settings.Default.Password)));
                return true;
            }
            catch
            {
                _screen = null;
                throw;
            }
        }

        private void LogoutFromAcumatica()
        {
            _progress.Report(new MonitorMessage(Strings.LogoutNotify));
            if (_screen != null)
            {
                try
                {
                    _screen.Logout();
                }
                catch
                {
                    //Ignore all errors in logout.
                }
                _screen = null;
            }
        }
    }
}
