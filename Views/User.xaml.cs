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
using HLApp.Models;
using HLApp.Utility;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Tasks;

namespace HLApp.Views
{
    public partial class User : PhoneApplicationPage
    {
        private string id;
        private int userId;
        private UserDto userDto;
        private HLServiceClient WS;


        public User()
        {
            InitializeComponent();
            WS = new HLServiceClient();

            /* Used for display of WrapPanel.*/
            _tileMargin = (Thickness)Resources["PhoneTouchTargetOverhang"];
            _tileOverhang = (int)_tileMargin.Left; // assume left, top, right, bottom all the same
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                id = NavigationContext.QueryString["id"];
                userId = Convert.ToInt32(id);
                LoadUserData();
            }
            base.OnNavigatedTo(e);
        }


        private void LoadUserData()
        {
            /* Whether we are going to enable email, SMS and Phone functionality depends on whether the 
             * logged in User and the User being viewed are sharing contact information. */ 
            WS.AreUsersSharingContactInfoAsync(LogOn.loggedInUserId, userId);
            WS.AreUsersSharingContactInfoCompleted += new EventHandler<AreUsersSharingContactInfoCompletedEventArgs>(WS_AreUsersSharingContactInfoCompleted);

            WS.GetUserAsync(userId);
            WS.GetUserCompleted += new EventHandler<GetUserCompletedEventArgs>(WS_GetUserCompleted);

            WS.GetNewsItemsCreatedByUserAsync(userId, 16, 1);
            WS.GetNewsItemsCreatedByUserCompleted += new EventHandler<GetNewsItemsCreatedByUserCompletedEventArgs>(WS_GetNewsItemsCreatedByUserCompleted);

        }


        void WS_GetNewsItemsCreatedByUserCompleted(object sender, GetNewsItemsCreatedByUserCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            PutItemsOnWrapPanel(items);
        }


        /// <summary>
        /// Whether we are going to enable email, SMS and Phone functionality depends on whether the 
        /// logged in User and the User being viewed are sharing contact information. 
        /// </summary>
        void WS_AreUsersSharingContactInfoCompleted(object sender, AreUsersSharingContactInfoCompletedEventArgs e)
        {
            var areUsersSharingContactInformation = e.Result;

            if (areUsersSharingContactInformation)
            {
                btnPhone.Visibility = Visibility.Visible;      
                btnSMS.Visibility = Visibility.Visible;
                btnEmail.Visibility = Visibility.Visible;
            }
            else
            {
                btnPhone.Visibility = Visibility.Collapsed;
                btnSMS.Visibility = Visibility.Collapsed;
                btnEmail.Visibility = Visibility.Collapsed;
            }
        }


        void WS_GetUserCompleted(object sender, GetUserCompletedEventArgs e)
        {
            userDto = e.Result;

            ImageSource imgSource = new BitmapImage(new Uri(userDto.ImageBlobUri));
            imgUser.Source = imgSource;

            pivotOne.Header = userDto.FirstName.ToLower(); ;
            pivotTwo.Header = userDto.LastName.ToLower(); ;

            mapUser.CredentialsProvider = new ApplicationIdCredentialsProvider("Atx6C4-6W6IofeQA3RsyCWSbKeE0Z9QVrmFxNIMagT0rrxQF6_R25A1d-N_r0bXY");
            GeoCoordinate Location = new GeoCoordinate(userDto.Latitude, userDto.Longitude);
            pinMapLocator.Location = Location;
            mapUser.SetView(Location, 12);
        }


        private void btnPhone_Click(object sender, RoutedEventArgs e)
        {
            PhoneCallTask callNumber = new PhoneCallTask();
            callNumber.DisplayName = userDto.FullName;
            callNumber.PhoneNumber = userDto.PhoneNumber;
            callNumber.Show();

        }


        private void btnSMS_Click(object sender, RoutedEventArgs e)
        {
            SmsComposeTask sendSMS = new SmsComposeTask();
            sendSMS.To = userDto.PhoneNumber;
            sendSMS.Body = "";
            sendSMS.Show();
        }


        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask composeTask = new EmailComposeTask();
            composeTask.Body = "";
            composeTask.To = userDto.Email;
            composeTask.Subject = "";
            composeTask.Show();
        }


        private void PutItemsOnWrapPanel(List<NewsItemDto> items)
        {
            horizontalWrapPanel.Children.Clear();

            foreach (var i in items)
            {
                if (i.CoverPhotoMediumSize == null)
                    i.CoverPhotoMediumSize = "";
                /* Adding items to WrapPanel. */
                horizontalWrapPanel.Children.Add(CreateWrapPanelItem(i.NewsItemID, i.CoverPhotoMediumSize));
            }
            AdjustHorizontalWrapPanelSize();
        }


        #region WrapPanel Code

        /// <summary>
        /// Creates an item for the WrapPanel.
        /// </summary>
        private FrameworkElement CreateWrapPanelItem(int id, string photoUri)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

            Border border = new Border();
            border.Width = TileDimension;
            border.Height = TileDimension;
            border.Margin = _tileMargin;
            border.Background = brush;
            border.BorderThickness = new Thickness(2);
            border.BorderBrush = (SolidColorBrush)Resources["PhoneForegroundBrush"];
            border.CacheMode = new BitmapCache();
            border.Tag = id;

            Image image = new Image();
            image.Source = new BitmapImage(new Uri(photoUri, UriKind.RelativeOrAbsolute));
            image.Stretch = System.Windows.Media.Stretch.UniformToFill;
            image.Height = 100;

            border.Child = image;

            GestureListener wrapPanelListener = GestureService.GetGestureListener(border);
            wrapPanelListener.Tap += new EventHandler<Microsoft.Phone.Controls.GestureEventArgs>(wrapPanelListener_Tap);
            return border;
        }


        /// <summary>
        /// Tapping an element in the WrapPanel
        /// </summary>
        void wrapPanelListener_Tap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            var id = element.Tag.ToString();
            NavigateToNewsItemView(id);

        }


        private void NavigateToNewsItemView(string id)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri(string.Format("/HLApp;component/Views/NewsItem.xaml?id={0}", id), UriKind.Relative));
        }


        /// <summary>
        /// Adjusting the markup of the WrapPanel.
        /// </summary>
        private void AdjustHorizontalWrapPanelSize()
        {
            // The goal here is to configure the WrapPanel to lay the items out in rows, while
            // taking as few columns as possible. 
            int columnsRequired = Math.Max(ColumnsPerPage, (int)Math.Ceiling((double)horizontalWrapPanel.Children.Count / (double)RowsPerPage));

            horizontalWrapPanel.Width = columnsRequired * TileDimensionWithMargins;
            horizontalWrapPanel.Height = RowsPerPage * TileDimensionWithMargins;
            horizontalWrapPanel.Height = 4 * TileDimensionWithMargins;
        }
        private int TileDimension { get { return (int)(432.0 / 4 - _tileOverhang * 2); } }
        private int TileDimensionWithMargins { get { return TileDimension + _tileOverhang * 2; } }
        private int ColumnsPerPage { get { return (int)((420.0 + _tileOverhang * 2) / TileDimensionWithMargins); } }
        private int RowsPerPage { get { return (int)((466.0 + _tileOverhang * 2) / TileDimensionWithMargins); } }
        /// <summary>
        /// Used for formating display of WrapPanel.
        /// </summary>
        Thickness _tileMargin;
        /// <summary>
        /// Used for formating display of WrapPanel.
        /// </summary>
        int _tileOverhang;

        #endregion
    }
}