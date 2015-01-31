using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp1.Resources;
using Microsoft.Phone.Notification;
using System.Diagnostics;

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        Uri channelUri;
        public Uri ChannelUri
        {
            get { return this.channelUri; }
            set
            {
                this.channelUri = value;
                OnChannelUriChanged(value);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string developerName = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("developer", out developerName))
            {
                tbxChannel.Text = string.Format("Developer: {0}", developerName);
            }
        }

        private void OnChannelUriChanged(Uri value)
        {
            Dispatcher.BeginInvoke(() =>
            {
                tbxChannel.Text = "changing uri to " + value.ToString();
                Debug.WriteLine("changing uri to " + value.ToString());
            });
        }
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void btnCreateChannel_Click(object sender, RoutedEventArgs e)
        {
            SetupChannel();
        }

        private void SetupChannel()
        {
            string channelName = "Demo channel";
            HttpNotificationChannel httpChannel = HttpNotificationChannel.Find(channelName);

            if (httpChannel == null)
            {
                httpChannel = new HttpNotificationChannel(channelName);
                httpChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(httpChannel_ChannelUriUpdated);
                httpChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(httpChannel_ExceptionOccured);
                httpChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(httpChannel_ShellToastNotificationReceived);
                httpChannel.Open();
                httpChannel.BindToShellToast();
            }
            else { 
                httpChannel.ChannelUriUpdated+=httpChannel_ChannelUriUpdated;
                httpChannel.ErrorOccurred+=httpChannel_ExceptionOccured;
                httpChannel.ShellToastNotificationReceived+=httpChannel_ShellToastNotificationReceived;
                Debug.WriteLine(httpChannel.ChannelUri.ToString());
            }

            //try
            //{
            //    if (httpChannel != null)
            //    {
            //        if (httpChannel.ChannelUri == null)
            //        {
            //            httpChannel.UnbindToShellToast();
            //            httpChannel.Close();
            //            SetupChannel();
            //            return;
            //        }
            //        else
            //        {
            //            ChannelUri = httpChannel.ChannelUri;
            //        }
            //        BindToShell(httpChannel);
            //    }
            //    else
            //    {
            //        httpChannel = new HttpNotificationChannel(channelName);
            //        httpChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(httpChannel_ChannelUriUpdated);
            //        httpChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(httpChannel_ShellToastNotificationReceived);
            //        httpChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(httpChannel_ExceptionOccured);

            //        httpChannel.Open();
            //        BindToShell(httpChannel);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine("An exception occured setting up channel: " + ex.ToString());
            //}
        }

        private void httpChannel_ExceptionOccured(object sender, NotificationChannelErrorEventArgs e)
        {
            Debug.WriteLine("Exception occured" + e.Message);
        }

        private void httpChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                tbxChannel.Text = "Toast notification message received: ";
                if (e.Collection != null)
                {
                    Dictionary<string, string> collection = (Dictionary<string, string>)e.Collection;
                    foreach (string elementName in collection.Keys)
                    {
                        tbxChannel.Text += string.Format("Key: {0}, Value: {1}\r\n", elementName, collection[elementName]);
                    }
                }
            });
        }

        private void httpChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            ChannelUri = e.ChannelUri;
        }

        private void BindToShell(HttpNotificationChannel httpChannel)
        {
            try
            {
                if (!httpChannel.IsShellToastBound)
                {
                    httpChannel.BindToShellToast();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception occured while binding to shell toast " + ex.Message);
            }
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}