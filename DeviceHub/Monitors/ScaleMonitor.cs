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

        private const string ScaleScreen = "XX000000";
        private ScreenApi.Screen _screen;

        public Task Initialize(Progress<MonitorMessage> progress, CancellationToken cancellationToken)
        {
            _progress = progress;

            return Task.Run(() =>
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        LogoutFromAcumatica();
                        break;
                    }

                    try
                    {
                        //if (_screen != null || LoginToAcumatica())
                        //{
                        //    UpdateWeight();
                        //}

                        System.Threading.Thread.Sleep(Properties.Settings.Default.ScalePollingInterval);
                    }
                    catch (Exception ex)
                    {
                        // Assume the server went offline or our session got lost - new login will be attempted in next iteration
                        _progress.Report(new MonitorMessage(String.Format("An error occured while updating the scale weight: {0}", ex.Message), MonitorMessage.MonitorStates.Error));
                        _screen = null;
                        System.Threading.Thread.Sleep(Properties.Settings.Default.ErrorWaitInterval);
                    }
                }
            });
        }

        private bool LoginToAcumatica()
        {
            _progress.Report(new MonitorMessage(String.Format("Logging in to {0}...", Properties.Settings.Default.AcumaticaUrl)));
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
            _progress.Report(new MonitorMessage("Logging out..."));
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
