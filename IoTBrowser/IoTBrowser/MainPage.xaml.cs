﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            DoWebNavigate();
        }

        // fullscreen mode
        private void MyPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
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

        private void Go_Web_Click(object sender, RoutedEventArgs e)
        {
            DoWebNavigate();
        }


        private void Web_Address_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                DoWebNavigate();
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
                        webView.Navigate(new Uri(Web_Address.Text));
                    }
                    else
                    {
                        Web_Address.Text = "http://" + Web_Address.Text;
                        webView.Navigate(new Uri(Web_Address.Text));
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