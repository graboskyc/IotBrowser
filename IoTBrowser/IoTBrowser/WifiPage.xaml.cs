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
using Windows.Devices.WiFi;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using System.Threading.Tasks;
using System.Text;
using Windows.Networking.Connectivity;
using Windows.Security.Credentials;
using Windows.Devices.Enumeration;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WifiPage : Page
    {
        private WiFiScanner _wifiScanner;
        private bool _scannerInitialized = false;
        private string _selectedSSID = "";
        WiFiNetworkReport _report;

        public WifiPage()
        {
            this.InitializeComponent();

            this._wifiScanner = new WiFiScanner();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeScanner();
        }

        private async Task InitializeScanner()
        {
            if (!_scannerInitialized)
            {
                await this._wifiScanner.InitializeScanner();
                _scannerInitialized = true;
            }
        }

        private async void btnScan_Click(object sender, RoutedEventArgs e)
        {
            await InitializeScanner();
            this.btnScan.IsEnabled = false;

            try
            {
                await RunWifiScan();
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }

            this.btnScan.IsEnabled = true;
        }


        private async Task RunWifiScan()
        {
            await this._wifiScanner.ScanForNetworks();

            _report = this._wifiScanner.WiFiAdapter.NetworkReport;

            ObservableCollection<WifiList> ocwl = new ObservableCollection<WifiList>();

            foreach (var availableNetwork in _report.AvailableNetworks)
            {
                
                if (availableNetwork.Ssid.Length > 1)
                {
                    string icon = "";
                    if(availableNetwork.SecuritySettings.NetworkEncryptionType.ToString() == "None") { icon = "open_";  } 
                    else { icon = "secure_";  }
                    icon = icon + availableNetwork.SignalBars + "bar.png";

                    WifiList wl = new WifiList { Icon = "Assets/"+icon, SSID = availableNetwork.Ssid };

                    ocwl.Add(wl);
                }
            }

            report_lv.ItemsSource = ocwl;

            
        }


        private async Task ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        private void btnBrowser_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            
            var access = await WiFiAdapter.RequestAccessAsync();
            var wifiAdapterResults = await DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
            WiFiAdapter wa = await WiFiAdapter.FromIdAsync(wifiAdapterResults[0].Id);

            WiFiReconnectionKind reconnectionKind = WiFiReconnectionKind.Automatic;
            WiFiConnectionResult result;

            foreach (var availableNetwork in _report.AvailableNetworks)
            {
                if (availableNetwork.Ssid == _selectedSSID)
                {
                    if (txtPassword.Text.Length > 1)
                    {
                        var credential = new PasswordCredential();
                        credential.Password = txtPassword.Text;
                        result = await wa.ConnectAsync(availableNetwork, reconnectionKind, credential);
                    }
                    else
                    {
                        result = await wa.ConnectAsync(availableNetwork, reconnectionKind);
                    }
                }
            }

            

        }

        private void report_lv_ItemClick(object sender, ItemClickEventArgs e)
        {

            WifiList item = (WifiList)e.ClickedItem;
            //txtPassword.Text = item.SSID;
            _selectedSSID = item.SSID;
        }
    }
}

public class WifiList
{
    public string SSID { get; set;  }
    public string Icon { get; set; }
}