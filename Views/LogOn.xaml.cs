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
using HLApp.HLServiceReference;
using HLApp.GeocodeService;
using System.Device.Location;

namespace HLApp.Views
{
    public partial class LogOn : PhoneApplicationPage
    {
        private HLServiceClient WS;
        private GeocodeServiceClient GeoWS;
        /// <summary>
        /// We will store the UserID here, for usage around in the application.
        /// </summary>
        public static int loggedInUserId;


        public LogOn()
        {
            InitializeComponent();
            WS = new HLServiceClient();
            GeoWS = new GeocodeServiceClient();
        }


        private void btnLogOn_Click(object sender, RoutedEventArgs e)
        {
            if ((!string.IsNullOrEmpty(txtEmailAddress.Text)) && (!string.IsNullOrEmpty(txtPassword.Password)))
            {
                txbLogonMessage.Text = "";

                WS.UserLoginAsync(txtEmailAddress.Text, txtPassword.Password);
                WS.UserLoginCompleted += new EventHandler<UserLoginCompletedEventArgs>(WS_UserLoginCompleted);
            }
        }


        void WS_UserLoginCompleted(object sender, UserLoginCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                /* Capturing UserID. */
                loggedInUserId = e.Result;

                if (e.Result > 0)
                {
                    UpdateUserLoginPosition();
                    NavigationService.Navigate(new Uri("/HLApp;component/Views/Stream.xaml", UriKind.Relative));
                }
                else
                    txbLogonMessage.Text = "Login was unsuccessful. The user name or password provided is incorrect. Please correct the errors and try again.";
            }
        }


        /// <summary>
        /// Tracking the login position of the User.
        /// </summary>
        private void UpdateUserLoginPosition()
        {
            /* The GeoCoordinateWatchers Position property obtains the physical geo coordinates of an actual physical device. */
            GeoCoordinateWatcher myWatcher = new GeoCoordinateWatcher();

            var myPosition = myWatcher.Position;

            /* We have to hard code the latitude / longitude because GeoCoordinateWather can only determine our position when we are running the code on an actual
             * physical device. These coordinates should be Vimmelskaftet 39, 1161, Copenhagen, Denmark. */
            var latitude = 55.67853;
            var longitude = 12.57576;

            if (!myPosition.Location.IsUnknown)
            {
                latitude = myPosition.Location.Latitude;
                longitude = myPosition.Location.Longitude;
            }

            WS.UpdateLastLoginPositionAsync(loggedInUserId, latitude, longitude);
            WS.UpdateLastLoginPositionCompleted += new EventHandler<UpdateLastLoginPositionCompletedEventArgs>(WS_UpdateLastLoginPositionCompleted);
        }


        void WS_UpdateLastLoginPositionCompleted(object sender, UpdateLastLoginPositionCompletedEventArgs e)
        {
        }
    }
}