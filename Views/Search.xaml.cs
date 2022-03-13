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
using HLApp.Models;
using System.Device.Location;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls.Maps;

namespace HLApp.Views
{
    public partial class Search : PhoneApplicationPage
    {
        private HLServiceClient WS;
        private GeocodeServiceClient GeoWS;
        private float searchLatitude;
        private float searchLongitude;
        private int searchRadius;


        public Search()
        {
            InitializeComponent();

            WS = new HLServiceClient();
            GeoWS = new GeocodeServiceClient();

            LoadContextMenuAndListPickerData();
            PerformGeocoding();
        }


        #region Search

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var searchItemType = btnItemType.Content.ToString();
            searchRadius = Convert.ToInt32(txtRadius.Text);

            switch (searchItemType)
            {
                case "News":
                    SearchNewsItems();
                    break;
                case "Communities":
                    SearchCommunities();
                    break;
                case "Users":
                    SearchUsers();
                    break;
            }
        }


        private void SearchNewsItems()
        {
            var categoryModel = (ListItemModel)this.lpkCategory.SelectedItem;
            var categoryId = categoryModel.IdInt;

            var assignmentModel = (ListItemModel)this.lpkAssignment.SelectedItem;
            var assignmentId = assignmentModel.IdInt;

            var goBackDate = dpkNewsItem.Value;

            /* Navigate to view SearchResultNewsItem where we will perform the actual search. */
            Uri searchUri = new Uri
            (
                "/HLApp;component/Views/SearchResultNewsItem.xaml?searchRadius=" + searchRadius + "&categoryId=" + categoryId +
                "&assignmentId=" + assignmentId + "&createUpdateDate=" + goBackDate +
                "&searchLatitude=" + searchLatitude + "&searchLongitude=" + searchLongitude,
                UriKind.Relative
            );
            NavigationService.Navigate(searchUri);
        }


        private void SearchCommunities()
        {
            /* Navigate to view SearchResultCommunity where we will perform the actual search. */
            Uri searchUri = new Uri
            (
                "/HLApp;component/Views/SearchResultCommunity.xaml?searchRadius=" + searchRadius + 
                "&searchLatitude=" + searchLatitude + "&searchLongitude=" + searchLongitude,
                UriKind.Relative
            );
            NavigationService.Navigate(searchUri);
        }


        private void SearchUsers()
        {
            /* Navigate to view SearchResultUser where we will perform the actual search. */
            Uri searchUri = new Uri
            (
                "/HLApp;component/Views/SearchResultUser.xaml?searchRadius=" + searchRadius +
                "&searchLatitude=" + searchLatitude + "&searchLongitude=" + searchLongitude,
                UriKind.Relative
            );
            NavigationService.Navigate(searchUri);
        }

        #endregion


        #region Geocoding

        /* A good deal of the code that does the address plotting against virtualearth.net's GeoCodeService I have copied, pasted and adapted to HLApp 
         * from a code example I found in the book: Lee/Chuvyrov, "Beginning Windows Phone 7 Development", Apress, USA 2010, p. 303 - 310. */
        private void PerformGeocoding()
        {
            /* The GeoCoordinateWatchers Position property obtains the physical geo coordinates of an actual physical device. */
            GeoCoordinateWatcher myWatcher = new GeoCoordinateWatcher();

            var myPosition = myWatcher.Position;

            /* We have to hard code the latitude / longitude because GeoCoordinateWather can only determine our position when we are running the code on an actual
             * physical device. These coordinates should be Vimmelskaftet 39, 1161, Copenhagen, Denmark. */
            searchLatitude = (float)55.67853;
            searchLongitude = (float)12.57576;

            if (!myPosition.Location.IsUnknown)
            {
                searchLatitude = (float)myPosition.Location.Latitude;
                searchLongitude = (float)myPosition.Location.Longitude;
            }

            this.SetLocation(searchLatitude, searchLongitude, 17, true);

            GeoWS.GeocodeCompleted += (s, e) =>
            {
                // sort the returned record by ascending confidence in order for
                // highest confidence to be on the top. Based on the numeration High value is
                // at 0, Medium value at 1 and Low volue at 2
                var geoResult = (from r in e.Result.Results
                                 orderby (int)r.Confidence ascending
                                 select r).FirstOrDefault();
                if (geoResult != null)
                {
                    this.SetLocation(geoResult.Locations[0].Latitude,
                        geoResult.Locations[0].Longitude,
                        17,
                        true);

                    /* Grabing the latitude and longitude to be passed on to SearchResults.xaml, where we displays the search results. */
                    searchLatitude = (float)geoResult.Locations[0].Latitude;
                    searchLongitude = (float)geoResult.Locations[0].Longitude;
                }
            };
        }


        private void SetLocation(double latitude, double longitude, double zoomLevel, bool showLocator)
        {
            // Move the pushpin to geo coordinate
            Microsoft.Phone.Controls.Maps.Platform.Location location = new Microsoft.Phone.Controls.Maps.Platform.Location();
            location.Latitude = latitude;
            location.Longitude = longitude;
            mapSearch.SetView(location, zoomLevel);
            bingMapLocator.Location = location;
            if (showLocator)
            {
                locator.Visibility = Visibility.Visible;
            }
            else
            {
                locator.Visibility = Visibility.Collapsed;
            }
        }


        private void btnPlotLocation_Click(object sender, RoutedEventArgs e)
        {
            GeocodeService.GeocodeRequest request = new GeocodeService.GeocodeRequest();

            // Only accept results with high confidence.
            request.Options = new GeocodeOptions()
            {
                Filters = new ObservableCollection<FilterBase>
                {
                    new ConfidenceFilter()
                    {
                        MinimumConfidence = Confidence.High
                    }
                }
            };

            request.Credentials = new Credentials()
            {
                ApplicationId = "Atx6C4-6W6IofeQA3RsyCWSbKeE0Z9QVrmFxNIMagT0rrxQF6_R25A1d-N_r0bXY"
            };

            request.Query = txtLocation.Text;

            // Make asynchronous call to fetch the geo coordinate data.
            GeoWS.GeocodeAsync(request);
        }

        #endregion


        #region ContextMenu & ListPicker

        private void LoadContextMenuAndListPickerData()
        {
            // DatePicker
            var todayDate = DateTime.Now;
            var goBackDate = todayDate.AddDays(-21);
            dpkNewsItem.Value = goBackDate;

            // Search item types.
            List<string> itemTypes = new List<string>() { "News", "Communities", "Users" };
            var itemTypeMenuItems = new List<MenuItem>();
            var id = 1;
            foreach (var i in itemTypes)
            {
                var item = new MenuItem()
                {
                    Tag = id,
                    Header = i
                };
                item.Click += new RoutedEventHandler(menuItemType_Click);
                itemTypeMenuItems.Add(item);
                id++;
            }
            this.menuItemType.ItemsSource = itemTypeMenuItems;
            btnItemType.Content = itemTypeMenuItems[0].Header;

            // NewsItemCategories
            WS.GetNewsItemCategoriesAsync();
            WS.GetNewsItemCategoriesCompleted += new EventHandler<GetNewsItemCategoriesCompletedEventArgs>(WS_GetNewsItemCategoriesCompleted);

            // Assignments
            WS.GetAssignmentsForDropDownListAsync();
            WS.GetAssignmentsForDropDownListCompleted += new EventHandler<GetAssignmentsForDropDownListCompletedEventArgs>(WS_GetAssignmentsForDropDownListCompleted);
        }


        /// <summary>
        /// Changing the search item. Either NewsItems, Communities or Users.
        /// </summary>
        void menuItemType_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            btnItemType.Content = menuItem.Header.ToString();

            // If we chose another item type than NewsItem, for our search, we hide lpkCategory, lpkAssignment and dpkNewsItem.
            if (Convert.ToInt32(menuItem.Tag) != 1)
            {
                dpkNewsItem.Visibility = System.Windows.Visibility.Collapsed;
                lpkCategory.Visibility = System.Windows.Visibility.Collapsed;
                lpkAssignment.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                dpkNewsItem.Visibility = System.Windows.Visibility.Visible;
                lpkCategory.Visibility = System.Windows.Visibility.Visible;
                lpkAssignment.Visibility = System.Windows.Visibility.Visible;
            }
        }


        void WS_GetNewsItemCategoriesCompleted(object sender, GetNewsItemCategoriesCompletedEventArgs e)
        {
            var categoryDtos = e.Result.ToList();
            var categoryModels = new List<ListItemModel>();
            var dummyItem = new ListItemModel { IdInt = 0, Name = "Category" };
            categoryModels.Add(dummyItem);

            foreach (var c in categoryDtos)
            {
                var item = new ListItemModel()
                {
                    IdInt = c.CategoryID,
                    Name = c.CategoryName
                };
                categoryModels.Add(item);
            }
            this.lpkCategory.ItemsSource = categoryModels;
            this.lpkCategory.SelectedIndex = 0;
        }


        void WS_GetAssignmentsForDropDownListCompleted(object sender, GetAssignmentsForDropDownListCompletedEventArgs e)
        {
            var assignmentDtos = e.Result.ToList();
            var assignmentModels = new List<ListItemModel>();
            var dummyItem = new ListItemModel { IdInt = 0, Name = "Assignment" };
            assignmentModels.Add(dummyItem);

            foreach (var a in assignmentDtos)
            {
                var item = new ListItemModel()
                {
                    IdInt = a.AssignmentID,
                    Name = a.Title
                };
                assignmentModels.Add(item);
            }
            this.lpkAssignment.ItemsSource = assignmentModels;
            this.lpkAssignment.SelectedIndex = 0;
        }

        #endregion
    }
}