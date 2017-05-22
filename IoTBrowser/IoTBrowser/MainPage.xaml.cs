using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace IoTBrowser
{
    /// <summary>
    /// Main page which is a browser
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // track which tab is selected and a private observablecollection of tabs
        int _selectedTab = 0;
        OCTab _octab = new OCTab();

        public MainPage()
        {
            this.InitializeComponent();

            // handlers for progress bar
            webView.NavigationStarting += WebView_NavigationStarting;
            webView.NavigationCompleted += WebView_NavigationCompleted;

            // load main page
            DoWebNavigate();
            lv_tabs.ItemsSource = _octab;
            lv_tabs.SelectedIndex = 0;
        }

        private void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            // hide and stop progress bar
            prog.Visibility = Visibility.Collapsed;
            prog.IsActive = false;
            Go_Web.Visibility = Visibility.Visible;

            // update url in case it is a sub page we clicked on
            Web_Address.Text = webView.Source.ToString();

            // if title of page is long, cut off at 22 and set name in tab bar
            if (webView.DocumentTitle.Length > 24)
            {
                _octab[_selectedTab].Name = webView.DocumentTitle.Substring(0, 22) + "...";
            }
            else
            {
                _octab[_selectedTab].Name = webView.DocumentTitle;
            }
        }

        private void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            // show progres bar over go button
            prog.Visibility = Visibility.Visible;
            prog.IsActive = true;
            Go_Web.Visibility = Visibility.Collapsed;
        }

        private void lv_tabs_ItemClick(object sender, ItemClickEventArgs e)
        {
            // when a tab is clicked, update tracker and go there
            Tab t = (Tab)e.ClickedItem;
            _selectedTab = t.Index;

            Web_Address.Text = t.Url;
            DoWebNavigate();
        }

        private void MyPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // fullscreen mode. not yet implemented
            Web_Address.Text = e.Key.ToString();
            if (e.Key == Windows.System.VirtualKey.F11)
            {
                if (BrowserControlRow.Height.ToString() == "50")
                {
                    BrowserControlRow.Height = new GridLength(0);
                }
                else
                {
                    BrowserControlRow.Height = new GridLength(50);
                }
            }
        }

        private void DoWebNavigate()
        {
            DismissMessage();

            try
            {
                if (Web_Address.Text.Length > 0)
                {
                    if (Web_Address.Text.Contains("http"))
                    {
                        // user put full site URL so go there and update tab list
                        webView.Navigate(new Uri(Web_Address.Text));
                        _octab[_selectedTab].Url = Web_Address.Text;
                    }
                    else if (Web_Address.Text == "wifi")
                    {
                        // shortcut to wifi settings page
                        this.Frame.Navigate(typeof(WifiPage));
                    }
                    else if (Web_Address.Text == "debug")
                    {
                        // access to debug page
                        this.Frame.Navigate(typeof(DebugPage));
                    }
                    else
                    {
                        // user didnt put http or https in the front, so do that, then update tabs
                        Web_Address.Text = "http://" + Web_Address.Text;
                        webView.Navigate(new Uri(Web_Address.Text));
                        _octab[_selectedTab].Url = Web_Address.Text;
                    }
                }
                else
                {
                    DisplayMessage("You need to enter a web address.");
                }
            }
            catch (Exception e)
            {
                DisplayMessage("Error: " + e.Message);
            }
        }

        // Navigation buttons
        private void Go_Web_Click(object sender, RoutedEventArgs e)
        {
            DoWebNavigate();
        }

        private void Go_Wifi_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(WifiPage));
        }

        private void Web_Address_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                DoWebNavigate();
            }
        }
        
        private void DisplayMessage(String message)
        {
            Message.Text = message;
            MessageStackPanel.Visibility = Visibility.Visible;
            webView.Visibility = Visibility.Collapsed;

        }

        private void OnMessageDismiss_Click(object sender, RoutedEventArgs e)
        {
            DismissMessage();
        }

        private void DismissMessage()
        {
            webView.Visibility = Visibility.Visible;
            MessageStackPanel.Visibility = Visibility.Collapsed;
        }

    }
}

public class OCTab : ObservableCollection<Tab>
{
    public OCTab() : base()
    {
        Add(new Tab("Bing", "http://bing.com", 0));
        Add(new Tab("New Tab", "http://bing.com", 1));
        Add(new Tab("New Tab", "http://bing.com", 2));
        Add(new Tab("New Tab", "http://bing.com", 3));
        Add(new Tab("New Tab", "http://bing.com", 4));
    }
    
}

public class Tab : INotifyPropertyChanged
{
    private string _name;
    public string Name { get { return this._name; } set { this._name = value; NotifyPropertyChanged("Name"); } }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string v)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(v));
    }

    public string Url { get; set; }
    public int Index { get; set; }

    public Tab(string name, string url, int index)
    {
        this.Name = name;
        this.Url = url;
        this.Index = index;
    }


}
