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

namespace HLApp.Views
{
    public partial class SearchResultNewsItem : PhoneApplicationPage
    {
        private HLServiceClient WS;
        ObservableCollection<NewsItemModel> newsItems = new ObservableCollection<NewsItemModel>();
        private List<Pushpin> myPins = new List<Pushpin>();
        public List<Pushpin> MyPins { get { return myPins; } }


        public SearchResultNewsItem()
        {
            InitializeComponent();

            WS = new HLServiceClient();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var dto = new SearchNewsItemDto();

            if (NavigationContext.QueryString.ContainsKey("searchRadius"))
            {
                var searchRadius = NavigationContext.QueryString["searchRadius"];
                dto.SearchRadius = Convert.ToInt32(searchRadius);
            }

            if (NavigationContext.QueryString.ContainsKey("categoryId"))
            {
                var categoryIdString = NavigationContext.QueryString["categoryId"];

                var categoryId = Convert.ToInt32(categoryIdString);

                if (categoryId > 0)
                    dto.CategoryID = categoryId;
            }

            if (NavigationContext.QueryString.ContainsKey("assignmentId"))
            {
                var assignmentIdString = NavigationContext.QueryString["assignmentId"];

                var assignmentId = Convert.ToInt32(assignmentIdString);

                if (assignmentId > 0)
                    dto.CategoryID = assignmentId;
            }

            if (NavigationContext.QueryString.ContainsKey("createUpdateDate"))
            {
                var createUpdateDate = NavigationContext.QueryString["createUpdateDate"];

                dto.CreateUpdateDate = Convert.ToDateTime(createUpdateDate);
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

            WS.SearchNewsItemsAsync(dto);
            WS.SearchNewsItemsCompleted += new EventHandler<SearchNewsItemsCompletedEventArgs>(WS_SearchNewsItemsCompleted);


            base.OnNavigatedTo(e);
        }


        void WS_SearchNewsItemsCompleted(object sender, SearchNewsItemsCompletedEventArgs e)
        {
            newsItems.Clear();
            myPins.Clear();

            var items = e.Result.ToList();
            ProcessNewsItems(items);
        }


        private void ProcessNewsItems(List<NewsItemDto> items)
        {
            AddNewsItemsToList(items);
            PutNewsItemPushpinsOnMap(items);
        }


        private void AddNewsItemsToList(List<NewsItemDto> items)
        {
            newsItems.Clear();

            foreach (var i in items)
            {
                newsItems.Add
                (
                    new NewsItemModel()
                    {
                        NewsItemID = i.NewsItemID,
                        Title = i.Title,
                        CategoryID = i.CategoryID,
                        CategoryName = i.CategoryName,
                        CoverPhotoMediumSize = i.CoverPhotoMediumSize,
                        CreateUpdateDate = i.CreateUpdateDate.ToString(),
                        NumberOfViews = i.NumberOfViews,
                        NumberOfComments = i.NumberOfComments,
                        NumberOfShares = i.NumberOfShares
                    }
                );
            }         
            lstNewsItems.ItemsSource = newsItems;
        }


        private void PutNewsItemPushpinsOnMap(List<NewsItemDto> items)
        {
            mapResult.Children.Clear();

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
                mapResult.Children.Add(pushPin);
            }

            var locations = from l in MyPins select l.Location;
            mapResult.SetView(LocationRect.CreateLocationRect(locations));
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