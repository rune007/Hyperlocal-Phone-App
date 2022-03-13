using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using System.Diagnostics;
using HLApp.HLServiceReference;


namespace HLApp.Views
{
    /* My implementation of Push Notification is based upon code I found in a book and have modified to my purpose for 2HandApp. The code example I am using was found
     in the book Lee/Chuvyrov, "Beginning Windows Phone 7 Development", Apress, USA 2010, p. 353 - 366. */
    public partial class RegisterPushNotification : PhoneApplicationPage
    {
        private HLServiceClient WS;
        Uri channelUri;


        public Uri ChannelUri
        {
            get { return channelUri; }
            set
            {
                channelUri = value;
                OnChannelUriChanged(value);
            }
        }


        private void OnChannelUriChanged(Uri value)
        {
            Dispatcher.BeginInvoke(() =>
            {
                /* Grabbing the userId we put into the static field upon LogOn. */
                var userid = LogOn.loggedInUserId;

                WS.RegisterPushNotificationChannelAsync(userid, Convert.ToString(value));
            
            });
            Debug.WriteLine("changing uri to " + value.ToString());
        }


        public RegisterPushNotification()
        {
            InitializeComponent();
            WS = new HLServiceClient();
            WS.RegisterPushNotificationChannelCompleted += new EventHandler<RegisterPushNotificationChannelCompletedEventArgs>(WS_RegisterPushNotificationChannelCompleted);
        }


        void WS_RegisterPushNotificationChannelCompleted(object sender, RegisterPushNotificationChannelCompletedEventArgs e)
        {
            txbRegister.Text = "Thank you, you can now receive Push Notifications on you Phone :-)";
        }


        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            SetupChannel();
        }


        private void SetupChannel()
        {
            HttpNotificationChannel httpChannel = null;
            string channelName = "DemoChannel";

            try
            {
                //if channel exists, retrieve existing channel
                httpChannel = HttpNotificationChannel.Find(channelName);
                if (httpChannel != null)
                {
                    //If we can't get it, then close and reopen it.
                    if (httpChannel.ChannelUri == null)
                    {
                        httpChannel.UnbindToShellToast();
                        httpChannel.Close();
                        SetupChannel();
                        return;
                    }
                    else
                    {
                        ChannelUri = httpChannel.ChannelUri;
                    }
                    BindToShell(httpChannel);
                }
                else
                {
                    httpChannel = new HttpNotificationChannel(channelName);
                    httpChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(httpChannel_ChannelUriUpdated);
                    httpChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(httpChannel_ShellToastNotificationReceived);
                    httpChannel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(httpChannel_HttpNotificationReceived);
                    httpChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(httpChannel_ExceptionOccurred);

                    httpChannel.Open();
                    BindToShell(httpChannel);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An exception setting up channel " + ex.ToString());
            }
        }


        void httpChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                txbRegister.Text = "Toast Notification Message Received: ";
                if (e.Collection != null)
                {
                    Dictionary<string, string> collection = (Dictionary<string, string>)e.Collection;
                    System.Text.StringBuilder messageBuilder = new System.Text.StringBuilder();
                    foreach (string elementName in collection.Keys)
                    {
                        txbRegister.Text += string.Format("Key: {0}, Value: {1}\r\n", elementName, collection[elementName]);
                    }
                }
            });
        }


        private static void BindToShell(HttpNotificationChannel httpChannel)
        {
            try
            {
                //toast notification binding
                httpChannel.BindToShellToast();
                //tile notification binding
                httpChannel.BindToShellTile();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An exception occurred binding to shell " + ex.ToString());
            }
        }


        void httpChannel_ExceptionOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            //Display Message on error
            Debug.WriteLine(e.Message);
        }


        void httpChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            //We get the new Uri (or maybe it's updated)
            ChannelUri = e.ChannelUri;
        }


        // Receiving a raw notification. 
        // Raw notifications are only delivered to the application when it is running in the foreground. 
        // If the application is not running in the foreground, the raw notification message is dropped.
        void httpChannel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            if (e.Notification.Body != null && e.Notification.Headers != null)
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(e.Notification.Body);
                Dispatcher.BeginInvoke(() =>
                {
                    txbRegister.Text = "Raw Notification Message Received";
                });
            }
        }
    }
}