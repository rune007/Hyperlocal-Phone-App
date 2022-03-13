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
    public partial class SearchResultCommunity : PhoneApplicationPage
    {
        private HLServiceClient WS;
        ObservableCollection<CommunityModel> oclItems = new ObservableCollection<CommunityModel>();
        private List<Pushpin> myPins = new List<Pushpin>();
        public List<Pushpin> MyPins { get { return myPins; } }


        public SearchResultCommunity()
        {
            InitializeComponent();
            WS = new HLServiceClient();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var dto = new SearchCommunityDto();

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

            WS.SearchCommunitiesAsync(dto);
            WS.SearchCommunitiesCompleted += new EventHandler<SearchCommunitiesCompletedEventArgs>(WS_SearchCommunitiesCompleted);

            base.OnNavigatedTo(e);
        }


        void WS_SearchCommunitiesCompleted(object sender, SearchCommunitiesCompletedEventArgs e)
        {
            var items = e.Result.ToList();

            var itemCount = items.Count();

            oclItems.Clear();
            myPins.Clear();


            List<string> lstPolygonWkt = new List<string>(itemCount);

            lstPolygonWkt.Clear();
            foreach (var i in items)
                lstPolygonWkt.Add(i.PolygonWkt);
            
            ProcessPolygons(lstPolygonWkt);
            PutCommunityPushpinsOnMap(items);
            AddCommunitiesToList(items);
        }


        private void ProcessPolygons(List<string> lstWkt)
        {
            mapResult.Children.Clear();

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

                mapResult.Children.Add(mapPolygon);
            }
            mapResult.SetView(LocationRect.CreateLocationRect(polygonLocations));
        }


        private void AddCommunitiesToList(List<CommunityDto> items)
        {
            oclItems.Clear();

            foreach (var i in items)
            {
                oclItems.Add
                (
                    new CommunityModel()
                    {
                        CommunityID = i.CommunityID,
                        Name = i.Name,
                        LatestActivity = i.LatestActivityToString,
                        NumberOfUsers = i.NumberOfUsersInCommunity,
                        CoverPhotoMediumSize = i.MediumSizeBlobUri
                    }
                );              
            }
            lstItems.ItemsSource = oclItems;       
        }


        /// <summary>
        /// We mark the center point of the Community with a pushpin, so that it can be seen from far away.
        /// </summary>
        private void PutCommunityPushpinsOnMap(List<CommunityDto> items)
        {
            mapResult.Children.Clear();

            foreach (var i in items)
            {
                myPins.Add
                (
                    new Pushpin()
                    {
                        Location = new GeoCoordinate(i.PolygonCenterLatitude, i.PolygonCenterLongitude),
                        Tag = i.CommunityID,
                    }
                );
            }

            foreach (var pushPin in MyPins)
            {
                pushPin.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(pushPin_Tap);
                mapResult.Children.Add(pushPin);
            }
        }


        void pushPin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pushpin = sender as Pushpin;
            var communityId = pushpin.Tag.ToString();
            NavigateToCommunityView(communityId);
        }


        private void NavigateToCommunityView(string communityId)
        {
            Uri uri = new Uri("/HLApp;component/Views/Community.xaml?id=" + communityId, UriKind.Relative);
            NavigationService.Navigate(uri);
        }


        private void txbCommunityName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            var id = textBlock.Tag.ToString();
            NavigateToCommunityView(id);
        }
    }
}