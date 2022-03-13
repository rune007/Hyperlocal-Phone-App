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
using System.Windows.Media.Imaging;

namespace HLApp.Views
{
    /// <summary>
    /// A Users news stream (from those Communities which they follow.)
    /// </summary>
    public partial class Stream : PhoneApplicationPage
    {
        private HLServiceClient WS;
        ObservableCollection<NewsItemModel> newsItems = new ObservableCollection<NewsItemModel>();
        private List<Pushpin> myPins = new List<Pushpin>();
        public List<Pushpin> MyPins { get { return myPins; } }


        public Stream()
        {
            InitializeComponent();
            WS = new HLServiceClient();
            GetUsersNewsStream();
        }


        private void GetUsersNewsStream()
        {
            WS.GetNewsStreamForUserAsync(LogOn.loggedInUserId, 30, 12, 1);
            WS.GetNewsStreamForUserCompleted += new EventHandler<GetNewsStreamForUserCompletedEventArgs>(WS_GetNewsStreamForUserCompleted);
        }


        void WS_GetNewsStreamForUserCompleted(object sender, GetNewsStreamForUserCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            AddNewsItemsToList(items);
            PutNewsItemPushpinsOnMap(items);
        }


        private void AddNewsItemsToList(List<NewsItemDto> items)
        {
            foreach (var i in items)
            {
                // Used for making a request at Bing Map REST imagery service which responds with a static map with a pushpin for a particular area,
                // returned in the form of a .jpg image.
                // E.g. "http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/55.912272930063594, 11.645507812500017/5?mapSize=300,200&pp=55.912272930063594, 11.645507812500017;22&key=Atx6C4-6W6IofeQA3RsyCWSbKeE0Z9QVrmFxNIMagT0rrxQF6_R25A1d-N_r0bXY".              
                ImageSource mapImageSource = new BitmapImage(new Uri("http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/" + i.Latitude + "," + i.Longitude + "/15?mapSize=200,200&pp=" + i.Latitude + "," + i.Longitude + ";22&key=Atx6C4-6W6IofeQA3RsyCWSbKeE0Z9QVrmFxNIMagT0rrxQF6_R25A1d-N_r0bXY"));

                newsItems.Add
                (
                    new NewsItemModel()
                    {
                        NewsItemID = i.NewsItemID,
                        Title = i.Title,
                        CategoryID = i.CategoryID,
                        CategoryName = i.CategoryName,
                        CoverPhotoMediumSize = i.CoverPhotoMediumSize,
                        Latitude = i.Latitude.ToString(),
                        Longitude = i.Longitude.ToString(),
                        CreateUpdateDate = i.CreateUpdateDate.ToString(),
                        NumberOfViews = i.NumberOfViews,
                        NumberOfComments = i.NumberOfComments,
                        NumberOfShares = i.NumberOfShares,
                        LocatedInCommunityName = "@" + i.LocatedInCommunityName,
                        ImageryMapServiceRestRequestUrl = mapImageSource
                    }
                );
            }
            lstItems.ItemsSource = newsItems;
        }


        private void PutNewsItemPushpinsOnMap(List<NewsItemDto> items)
        {
            foreach (var i in items)
            {
                myPins.Add
                (
                    new Pushpin()
                    {
                        Location = new GeoCoordinate(i.Latitude, i.Longitude),
                        Content = i.CategoryName,
                        Tag = i.NewsItemID,
                    }
                );
            }

            foreach (var pushPin in MyPins)
            {
                pushPin.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(pushPin_Tap);
                mapItems.Children.Add(pushPin);
            }

            var locations = from l in MyPins select l.Location;
            mapItems.SetView(LocationRect.CreateLocationRect(locations));
        }


        void pushPin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pushpin = sender as Pushpin;
            var id = pushpin.Tag.ToString();
            NavigateToNewsItemView(id);
        }


        private void txbNewsItemTitle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            var id = textBlock.Tag.ToString();
            NavigateToNewsItemView(id);
        }


        private void NavigateToNewsItemView(string id)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri(string.Format("/HLApp;component/Views/NewsItem.xaml?id={0}", id), UriKind.Relative));
        }
    }
}