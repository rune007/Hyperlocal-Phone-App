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

namespace HLApp.Views
{
    public partial class Area : PhoneApplicationPage
    {
        private string itemType;
        private string id;
        private string areaName;
        private HLServiceClient WS;
        ObservableCollection<NewsItemModel> newsItems = new ObservableCollection<NewsItemModel>();
        private List<Pushpin> myPins = new List<Pushpin>();
        public List<Pushpin> MyPins { get { return myPins; } }


        public Area()
        {
            InitializeComponent();
            WS = new HLServiceClient();

            /* Used for display of WrapPanel.*/
            _tileMargin = (Thickness)Resources["PhoneTouchTargetOverhang"];
            _tileOverhang = (int)_tileMargin.Left; // assume left, top, right, bottom all the same

            newsItems.Clear();
            myPins.Clear();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("itemType"))
            {
                itemType = NavigationContext.QueryString["itemType"];
            }

            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                id = NavigationContext.QueryString["id"];
            }

            if (NavigationContext.QueryString.ContainsKey("areaName"))
            {
                areaName = NavigationContext.QueryString["areaName"];

                /* Putting the area name as title of Panorama control. */
                areaName = areaName.ToLower();
                panorama.Title = areaName;

                DetermineItemType(itemType);
            }
            base.OnNavigatedTo(e);
        }


        /// <summary>
        /// Distingusihing what area type it is: Danmark, Region, Municipality or PostalCode
        /// </summary>
        public void DetermineItemType(string itemType)
        {
            switch (itemType)
            {
                case "DkCountry":
                    LoadCountryData();
                    break;
                case "Region":
                    LoadRegionData();
                    break;
                case "Municipality":
                    LoadMunicipalityData();
                    break;
                case "PostalCode":
                    LoadPostalCodeData();
                    break;
            }
        }


        #region Loading data for the Area view.

        public void LoadCountryData()
        {
            WS.GetNewestNewsItemsFromDkCountryAsync(12, 1);
            WS.GetNewestNewsItemsFromDkCountryCompleted += new EventHandler<GetNewestNewsItemsFromDkCountryCompletedEventArgs>(WS_GetNewestNewsItemsFromDkCountryCompleted);

            WS.GetLatestActiveUsersFromDkCountryAsync(16, 1);
            WS.GetLatestActiveUsersFromDkCountryCompleted += new EventHandler<GetLatestActiveUsersFromDkCountryCompletedEventArgs>(WS_GetLatestActiveUsersFromDkCountryCompleted);

            WS.GetDKCountryPolygonsAsync();
            WS.GetDKCountryPolygonsCompleted += new EventHandler<GetDKCountryPolygonsCompletedEventArgs>(WS_GetDKCountryPolygonsCompleted);
        }


        public void LoadRegionData()
        {
            WS.GetNewestNewsItemsFromRegionAsync(id, 12, 1);
            WS.GetNewestNewsItemsFromRegionCompleted += new EventHandler<GetNewestNewsItemsFromRegionCompletedEventArgs>(WS_GetNewestNewsItemsFromRegionCompleted);

            WS.GetLatestActiveUsersFromRegionAsync(id, 16, 1);
            WS.GetLatestActiveUsersFromRegionCompleted += new EventHandler<GetLatestActiveUsersFromRegionCompletedEventArgs>(WS_GetLatestActiveUsersFromRegionCompleted);

            WS.GetRegionPolygonsWithoutHolesAsync(id);
            WS.GetRegionPolygonsWithoutHolesCompleted += new EventHandler<GetRegionPolygonsWithoutHolesCompletedEventArgs>(WS_GetRegionPolygonsWithoutHolesCompleted);
        }


        public void LoadMunicipalityData()
        {
            WS.GetNewestNewsItemsFromMunicipalityAsync(id, 12, 1);
            WS.GetNewestNewsItemsFromMunicipalityCompleted += new EventHandler<GetNewestNewsItemsFromMunicipalityCompletedEventArgs>(WS_GetNewestNewsItemsFromMunicipalityCompleted);

            WS.GetLatestActiveUsersFromMunicipalityAsync(id, 16, 1);
            WS.GetLatestActiveUsersFromMunicipalityCompleted += new EventHandler<GetLatestActiveUsersFromMunicipalityCompletedEventArgs>(WS_GetLatestActiveUsersFromMunicipalityCompleted);

            WS.GetMunicipalityPolygonsWithoutHolesAsync(id);
            WS.GetMunicipalityPolygonsWithoutHolesCompleted += new EventHandler<GetMunicipalityPolygonsWithoutHolesCompletedEventArgs>(WS_GetMunicipalityPolygonsWithoutHolesCompleted);
        }


        public void LoadPostalCodeData()
        {
            WS.GetNewestNewsItemsFromPostalCodeAsync(id, 12, 1);
            WS.GetNewestNewsItemsFromPostalCodeCompleted += new EventHandler<GetNewestNewsItemsFromPostalCodeCompletedEventArgs>(WS_GetNewestNewsItemsFromPostalCodeCompleted);

            WS.GetLatestActiveUsersFromPostalCodeAsync(id, 16, 1);
            WS.GetLatestActiveUsersFromPostalCodeCompleted += new EventHandler<GetLatestActiveUsersFromPostalCodeCompletedEventArgs>(WS_GetLatestActiveUsersFromPostalCodeCompleted);

            WS.GetPostalCodePolygonsAsync(id);
            WS.GetPostalCodePolygonsCompleted += new EventHandler<GetPostalCodePolygonsCompletedEventArgs>(WS_GetPostalCodePolygonsCompleted);
        }


        void WS_GetNewestNewsItemsFromDkCountryCompleted(object sender, GetNewestNewsItemsFromDkCountryCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            ProcessNewsItems(items);
        }


        void WS_GetNewestNewsItemsFromRegionCompleted(object sender, GetNewestNewsItemsFromRegionCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            ProcessNewsItems(items);
        }


        void WS_GetNewestNewsItemsFromMunicipalityCompleted(object sender, GetNewestNewsItemsFromMunicipalityCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            ProcessNewsItems(items);
        }


        void WS_GetNewestNewsItemsFromPostalCodeCompleted(object sender, GetNewestNewsItemsFromPostalCodeCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            ProcessNewsItems(items);
        }


        void WS_GetLatestActiveUsersFromDkCountryCompleted(object sender, GetLatestActiveUsersFromDkCountryCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            ProcessUsers(items);
        }


        void WS_GetLatestActiveUsersFromRegionCompleted(object sender, GetLatestActiveUsersFromRegionCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            ProcessUsers(items);
        }


        void WS_GetLatestActiveUsersFromMunicipalityCompleted(object sender, GetLatestActiveUsersFromMunicipalityCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            ProcessUsers(items);
        }


        void WS_GetLatestActiveUsersFromPostalCodeCompleted(object sender, GetLatestActiveUsersFromPostalCodeCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            ProcessUsers(items);
        }


        void WS_GetDKCountryPolygonsCompleted(object sender, GetDKCountryPolygonsCompletedEventArgs e)
        {
            List<string> lstPolygonWkt = new List<string>();
            var items = e.Result.ToList();
            foreach (var i in items)
                lstPolygonWkt.Add(i.PolygonWkt);
            ProcessPolygons(lstPolygonWkt);
        }


        void WS_GetRegionPolygonsWithoutHolesCompleted(object sender, GetRegionPolygonsWithoutHolesCompletedEventArgs e)
        {
            List<string> lstPolygonWkt = new List<string>();
            var items = e.Result.ToList();
            foreach (var i in items)
                lstPolygonWkt.Add(i.PolygonWkt);
            ProcessPolygons(lstPolygonWkt);
        }


        void WS_GetMunicipalityPolygonsWithoutHolesCompleted(object sender, GetMunicipalityPolygonsWithoutHolesCompletedEventArgs e)
        {
            List<string> lstPolygonWkt = new List<string>();
            var items = e.Result.ToList();
            foreach (var i in items)
                lstPolygonWkt.Add(i.PolygonWkt);
            ProcessPolygons(lstPolygonWkt);
        }


        void WS_GetPostalCodePolygonsCompleted(object sender, GetPostalCodePolygonsCompletedEventArgs e)
        {
            List<string> lstPolygonWkt = new List<string>();
            var items = e.Result.ToList();
            foreach (var i in items)
                lstPolygonWkt.Add(i.PolygonWkt);
            ProcessPolygons(lstPolygonWkt);
        }


        private void ProcessNewsItems(List<NewsItemDto> items)
        {
            AddNewsItemsToList(items);
            PutNewsItemPushpinsOnMap(items);
        }


        private void ProcessUsers(List<UserDto> items)
        {
            horizontalWrapPanel.Children.Clear();

            foreach (var i in items)
            {
                if (i.MediumSizeBlobUri == null)
                    i.MediumSizeBlobUri = "";
                /* Adding items to WrapPanel. */
                horizontalWrapPanel.Children.Add(CreateWrapPanelItem(i.UserID, i.MediumSizeBlobUri));
            }
            AdjustHorizontalWrapPanelSize();
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
            mapNewsItem.Children.Clear();

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
                mapNewsItem.Children.Add(pushPin);
            }

            var locations = from l in MyPins select l.Location;

            mapNewsItem.SetView(LocationRect.CreateLocationRect(locations));
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


        private void NavigateToUserView(string id)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri(string.Format("/HLApp;component/Views/User.xaml?id={0}", id), UriKind.Relative));
        } 


        private void ProcessPolygons(List<string> lstWkt)
        {
            mapArea.Children.Clear();

            // List for gathering the Locations of all the polygons.
            LocationCollection polygonLocations = new LocationCollection();

            foreach (var l in lstWkt)
            {
                // The collections of Locations which will make up the Polygon.
                LocationCollection locations = new LocationCollection();

                // Removing 'POLYGON ((' & '))' from WKT.
                var wkt = l.Replace("POLYGON ((", "");
                wkt = wkt.Replace("))", "");

                // Getting an array of wktCoordinatePairs.
                string[] wktCoordinatePairs = wkt.Split(',');

                // Iterating through the wktCoordinatePairs
                for (var i = 0; i < wktCoordinatePairs.Length; i++)
                {
                    // Splitting each of the wktCoordinatePairs into wktCoordinateSets
                    string[] wktCoordinateSets = wktCoordinatePairs[i].Trim().Split(' ');

                    // Making a Location and adding it to the LocationCollection
                    Location location = new Location();
                    location.Latitude = Convert.ToDouble(wktCoordinateSets[1]);
                    location.Longitude = Convert.ToDouble(wktCoordinateSets[0]);
                    locations.Add(location);
                    // Collecting the Locations of all the polygons so we can rightfully adjust the map.
                    polygonLocations.Add(location);
                }

                // Creating the Polygon and adding it to the map.
                MapPolygon mapPolygon = new MapPolygon();
                mapPolygon.Fill = new SolidColorBrush(Colors.Purple);
                mapPolygon.Stroke = new SolidColorBrush(Colors.White);
                mapPolygon.Opacity = .8;
                mapPolygon.Locations = locations;

                mapArea.Children.Add(mapPolygon);
            }
            mapArea.SetView(LocationRect.CreateLocationRect(polygonLocations));
        }

        #endregion


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
            var userId = element.Tag.ToString();
            NavigateToUserView(userId);
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