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

namespace HLApp.Views
{
    public partial class Navigation : PhoneApplicationPage
    {
        private HLServiceClient WS;
        private List<ListItemModel> itemList = new List<ListItemModel>();


        public Navigation()
        {           
            InitializeComponent();
            WS = new HLServiceClient();
        }


        /// <summary>
        /// Determining what content the autocomplete textbox is fed.
        /// </summary>
        private void ChangeItemType(object sender, RoutedEventArgs e)
        {
            /* Clearing the ItemsSource*/ 
            this.acBox.ItemsSource = null;
            itemList.Clear();

            RadioButton radioButton = sender as RadioButton;
            choiceTextBlock.Text = "You chose: " + radioButton.Content;

            switch (radioButton.Name)
            {
                case "rbtnDanmark":
                    GetDanmark();
                    break;
                case "rbtnRegion":
                    GetAllRegions();
                    break;
                case "rbtnMunicipality":
                    GetAllMunicipalities();
                    break;
                case "rbtnPostalCode":
                    GetAllPostalCodes();
                    break;
                case "rbtnCommunity":
                    GetAllCommunities();
                    break;
                case "rbtnUser":
                    GetAllUsers();
                    break;
            }
        }


        /// <summary>
        /// Used by autocomplete textbox.
        /// </summary>
        bool SearchItems(string search, object value)
        {
            if (value != null)
            {
                ListItemModel datasourceValue = value as ListItemModel;
                string name = datasourceValue.Name;

                if (name.ToLower().StartsWith(search.ToLower()))
                    return true;
            }
            //... If no match, return false. 
            return false;
        }


        private void btnNavigate_Click(object sender, RoutedEventArgs e)
        {
            if (acBox.SelectedItem == null)
            {
                NavigateToAreaView("DkCountry", "DkCountry", "Danmark");
                return;
            }

            var item = (ListItemModel)acBox.SelectedItem;
            var itemType = item.ItemType;
            var id = item.Id;
            var name = item.Name;
            
            switch (itemType)
            {
                case "DkCountry":
                    NavigateToAreaView("DkCountry", "DkCountry", "Danmark");
                    break;
                case "Region":
                    NavigateToAreaView(itemType, id, name);
                    break;
                case "Municipality":
                    NavigateToAreaView(itemType, id, name);
                    break;
                case "PostalCode":
                    NavigateToAreaView(itemType, id, name);
                    break;
                case "Community":
                    NavigateToCommunityView(id);
                    break;
                case "User":
                    NavigateToUserView(id);
                    break;
            }
        }


        private void NavigateToAreaView(string itemType, string id, string areaName)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri(string.Format("/HLApp;component/Views/Area.xaml?itemType={0}&id={1}&areaName={2}", 
                itemType, id, areaName), UriKind.Relative));
        }


        private void NavigateToCommunityView(string id)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri(string.Format("/HLApp;component/Views/Community.xaml?id={0}", id), UriKind.Relative));
        }


        private void NavigateToUserView(string id)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri(string.Format("/HLApp;component/Views/User.xaml?id={0}", id), UriKind.Relative));
        } 


        #region Getting data for autocomplete textbox

        private void GetDanmark()
        {
            itemList.Add
            (
                new ListItemModel()
                {
                    ItemType = "DkCountry",
                    Name = "Danmark",
                    Id = "Danmark"
                }
            );
            this.acBox.SelectedItem = itemList.ElementAt(0);
        }


        private void GetAllRegions()
        {
            WS.GetAllRegionsAsync();
            WS.GetAllRegionsCompleted += new EventHandler<GetAllRegionsCompletedEventArgs>(WS_GetAllRegionsCompleted);
        }


        private void GetAllMunicipalities()
        {
            WS.GetAllMunicipalitiesAsync();
            WS.GetAllMunicipalitiesCompleted += new EventHandler<GetAllMunicipalitiesCompletedEventArgs>(WS_GetAllMunicipalitiesCompleted);
        }


        private void GetAllPostalCodes()
        {
            WS.GetAllPostalCodesAsync();
            WS.GetAllPostalCodesCompleted += new EventHandler<GetAllPostalCodesCompletedEventArgs>(WS_GetAllPostalCodesCompleted);
        }


        private void GetAllCommunities()
        {
            WS.GetAllCommunitiesAsync();
            WS.GetAllCommunitiesCompleted += new EventHandler<GetAllCommunitiesCompletedEventArgs>(WS_GetAllCommunitiesCompleted);
        }


        private void GetAllUsers()
        {
            WS.GetAllUsersAsync();
            WS.GetAllUsersCompleted += new EventHandler<GetAllUsersCompletedEventArgs>(WS_GetAllUsersCompleted);
        }


        void WS_GetAllRegionsCompleted(object sender, GetAllRegionsCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            foreach (var i in items)
            {
                itemList.Add
                (
                    new ListItemModel()
                    {
                        ItemType = "Region",
                        Name = i.REGIONNAVN,
                        Id = i.UrlRegionName  
                    }
               );
            }
            this.acBox.ItemsSource = itemList;
        }


        void WS_GetAllMunicipalitiesCompleted(object sender, GetAllMunicipalitiesCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            foreach (var i in items)
            {
                itemList.Add
                (
                    new ListItemModel()
                    {
                        ItemType = "Municipality",
                        Name = i.KOMNAVN,
                        Id = i.UrlMunicipalityName
                    }
                );
            }
            this.acBox.ItemsSource = itemList;
        }


        void WS_GetAllPostalCodesCompleted(object sender, GetAllPostalCodesCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            foreach (var i in items)
            {
                itemList.Add
                (
                    new ListItemModel()
                    {
                        ItemType = "PostalCode",
                        Name = i.POSTNR_TXT + " " + i.POSTBYNAVN,
                        Id = i.POSTNR_TXT
                    }
               );
            }
            this.acBox.ItemsSource = itemList;
        }


        void WS_GetAllCommunitiesCompleted(object sender, GetAllCommunitiesCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            foreach (var i in items)
            {
                itemList.Add
                (
                    new ListItemModel()
                    {
                        ItemType = "Community",
                        Name = i.Name,
                        Id = i.CommunityID.ToString(),
                        ThumbnailPhotoUri = i.ThumbnailBlobUri
                    }
               );
            }
            this.acBox.ItemsSource = itemList;
        }


        void WS_GetAllUsersCompleted(object sender, GetAllUsersCompletedEventArgs e)
        {
            var items = e.Result.ToList();
            foreach (var i in items)
            {
                itemList.Add
                (
                    new ListItemModel()
                    {
                        ItemType = "User",
                        Name = i.FullName,
                        Id = i.UserID.ToString(),
                        ThumbnailPhotoUri = i.ThumbnailBlobUri
                    }
               );
            }
            this.acBox.ItemsSource = itemList;
        }

        #endregion
    }
}