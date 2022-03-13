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
using Microsoft.Phone.Controls.Maps;
using System.Collections.ObjectModel;
using HLApp.Models;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps.Platform;

namespace HLApp.Views
{
    public partial class SearchResultUser : PhoneApplicationPage
    {
        private HLServiceClient WS;
        ObservableCollection<UserModel> oclItems = new ObservableCollection<UserModel>();
        private List<Pushpin> myPins = new List<Pushpin>();
        public List<Pushpin> MyPins { get { return myPins; } }


        public SearchResultUser()
        {
            InitializeComponent();
            WS = new HLServiceClient();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var dto = new SearchUserDto();

            if (NavigationContext.QueryString.ContainsKey("searchRadius"))
            {
                var searchRadius = NavigationContext.QueryString["searchRadius"];
                dto.SearchRadius = Convert.ToInt32(searchRadius);
            }

            if (NavigationContext.QueryString.ContainsKey("searchLatitude"))
            {
                var searchLatitude = NavigationContext.QueryString["searchLatitude"];
                dto.SearchCenterLatitude = Convert.ToDouble(searchLatitude);
            }

            if (NavigationContext.QueryString.ContainsKey("searchLongitude"))
            {
                var searchLongitude = NavigationContext.QueryString["searchLongitude"];
                dto.SearchCenterLongitude = Convert.ToDouble(searchLongitude);
            }

            dto.PageSize = 12;
            dto.PageNumber = 1;

            WS.SearchUsersAsync(dto);
            WS.SearchUsersCompleted += new EventHandler<SearchUsersCompletedEventArgs>(WS_SearchUsersCompleted);

            base.OnNavigatedTo(e);
        }

        void WS_SearchUsersCompleted(object sender, SearchUsersCompletedEventArgs e)
        {
            var items = e.Result.ToList();

            oclItems.Clear();
            myPins.Clear();

            PutUserPushpinsOnMap(items);
            AddUsersToList(items);
        }


        private void AddUsersToList(List<UserDto> items)
        {
            oclItems.Clear();

            foreach (var i in items)
            {
                oclItems.Add
                (
                    new UserModel()
                    {
                        UserID = i.UserID,
                        FullName = i.FullName,
                        LatestActivity = i.LatestActivityToString,
                        NumberOfNewsItems = i.NumberOfNewsItemsPostedByUser,
                        CoverPhotoMediumSize = i.MediumSizeBlobUri
                    }
                );              
            }
            lstItems.ItemsSource = oclItems;       
        }


        /// <summary>
        /// We mark the center point of the Community with a pushpin, so that it can be seen from far away.
        /// </summary>
        private void PutUserPushpinsOnMap(List<UserDto> items)
        {
            mapResult.Children.Clear();

            foreach (var i in items)
            {
                myPins.Add
                (
                    new Pushpin()
                    {
                        Content = i.FirstName,
                        Location = new GeoCoordinate(i.Latitude, i.Longitude),
                        Tag = i.UserID,
                    }
                );
            }

            foreach (var pushPin in MyPins)
            {
                pushPin.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(pushPin_Tap);
                mapResult.Children.Add(pushPin);
            }

            var locations = from l in MyPins select l.Location;
            mapResult.SetView(LocationRect.CreateLocationRect(locations));
        }


        void pushPin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pushpin = sender as Pushpin;
            var userId = pushpin.Tag.ToString();
            NavigateToUserView(userId);
        }


        private void NavigateToUserView(string userId)
        {
            Uri uri = new Uri("/HLApp;component/Views/User.xaml?id=" + userId, UriKind.Relative);
            NavigationService.Navigate(uri);
        }


        private void txbUserFullName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            var id = textBlock.Tag.ToString();
            NavigateToUserView(id);
        }
    }
}