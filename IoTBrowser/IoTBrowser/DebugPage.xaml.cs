using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.System;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DebugPage : Page
    {
        public DebugPage()
        {
            this.InitializeComponent();

            var hostname = NetworkInformation.GetHostNames()[0];
            var icp = NetworkInformation.GetInternetConnectionProfile();

            //txt_addr.Text = hostname.IPInformation.NetworkAdapter.ToString();
            txt_device.Text = hostname.CanonicalName;
            txt_network.Text = icp.ProfileName;
        }

        private async void btn_run_Click(object sender, RoutedEventArgs e)
        {
            string status = "unknown";
            int count = 0;
            int capacity = -1;

            byte addr_handle = 0x0b;
            byte addr_capacity = 0x0d;
            byte addr_current = 0x0a;

            var settings = new I2cConnectionSettings(addr_handle);
            settings.BusSpeed = I2cBusSpeed.FastMode;

            string aqs = I2cDevice.GetDeviceSelector();                     /* Get a selector string that will return all I2C controllers on the system */
            var dis = await DeviceInformation.FindAllAsync(aqs);            /* Find the I2C bus controller devices with our selector string             */
            I2cDevice i2cd = await I2cDevice.FromIdAsync(dis[0].Id, settings);    /* Create an I2cDevice with our selected bus controller and I2C settings    */
            if (i2cd == null)
            {
                txt_main.Text = string.Format(
                    "Slave address {0} on I2C Controller {1} is currently in use by " +
                    "another application. Please ensure that no other applications are using I2C.",
                    settings.SlaveAddress,
                    dis[0].Id);
                return;
            }
            else
            {
                byte[] RegAddrBuf = new byte[] { addr_capacity }; /* Register address we want to read from                                         */
                byte[] ReadBuf = new byte[1];                   /* We read 6 bytes sequentially to get all 3 two-byte axes registers in one read */
                i2cd.WriteRead(RegAddrBuf, ReadBuf);
                capacity = BitConverter.ToInt16(ReadBuf, 0);
                txt_main.Text = capacity.ToString();
            }

        }

        private void btn_shutdown_Click(object sender, RoutedEventArgs e)
        {
            ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, new TimeSpan(0));
        }

        private void btn_restart_Click(object sender, RoutedEventArgs e)
        {
            ShutdownManager.BeginShutdown(ShutdownKind.Restart, new TimeSpan(0));
        }
    }
}
