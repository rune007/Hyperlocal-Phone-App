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
    public partial class NewsItem : PhoneApplicationPage
    {
        private HLServiceClient WS;
        ObservableCollection<NewsItemMediaModel> ocMedia = new ObservableCollection<NewsItemMediaModel>();
        ObservableCollection<CommentModel> ocComments = new ObservableCollection<CommentModel>();
        private string newsItemIdString;

        /// <summary>
        /// The video SAS blobUri has contains a query string, therefore it is difficult to transport it with a query string to the VideoPlayer view!
        /// So I a transporting the blobUri via a static variable.
        /// </summary>
        public static string blobUri;


        public NewsItem()
        {
            InitializeComponent();
            panItemMedia.Visibility = System.Windows.Visibility.Collapsed;
            WS = new HLServiceClient();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                newsItemIdString = NavigationContext.QueryString["id"];
                var newsItemId = Convert.ToInt32(newsItemIdString);
                LoadNewsItemData(newsItemId);
            }
            base.OnNavigatedTo(e);
        }


        private void LoadNewsItemData(int newsItemId)
        {
            WS.GetNewsItemAsync(newsItemId);
            WS.GetNewsItemCompleted += new EventHandler<GetNewsItemCompletedEventArgs>(WS_GetNewsItemCompleted);

            GetCommentsOnNewsItem(newsItemId);
        }


        private void GetCommentsOnNewsItem(int newsItemId)
        {
            WS.GetCommentsOnNewsItemAsync(newsItemId, 32, 1);
            WS.GetCommentsOnNewsItemCompleted += new EventHandler<GetCommentsOnNewsItemCompletedEventArgs>(WS_GetCommentsOnNewsItemCompleted);
        }


        void WS_GetCommentsOnNewsItemCompleted(object sender, GetCommentsOnNewsItemCompletedEventArgs e)
        {
            txtComment.Text = "";
            var dtos = e.Result.ToList();
            AddCommentsToList(dtos);
        }


        void WS_GetNewsItemCompleted(object sender, GetNewsItemCompletedEventArgs e)
        {
            var dto = e.Result;

            panNewsItem.Title = dto.CreateUpdateDate.ToString().ToLower();

            panItemStory.Header = "@" + dto.CategoryName.ToLower();

            txbTitle.Text = dto.Title;
            txbStory.Text = dto.Story;
            txbNumberOfViews.Text = dto.NumberOfViews.ToString();
            txbNumberOfShares.Text = dto.NumberOfShares.ToString();
            txbNumberOfComments.Text = dto.NumberOfShares.ToString();
            txbPostedByUserName.Text = "by " + dto.PostedByUserName;
            txbPostedByUserName.Tag = dto.PostedByUserID;
            txbPostedByUserName.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(txbPostedByUserName_Tap);

            if (dto.HasPhoto)
            {
                var blobUri = dto.Photos[0].BlobUri;
                ImageSource imageSource = new BitmapImage(new Uri(blobUri));
                imgCoverPhoto.Source = imageSource;

                if (dto.Photos.Count() > 1)
                {
                    panItemMedia.Visibility = System.Windows.Visibility.Visible;
                    var photosDtos = dto.Photos;
                    // We will not show the cover photo twice.
                    photosDtos.RemoveAt(0);
                    AddPhotosToList(photosDtos);
                }
            }

            if (dto.HasVideo)
            {
                panItemMedia.Visibility = System.Windows.Visibility.Visible;
                var videoDtos = dto.Videos;
                AddVideosToList(videoDtos);
            }

            PutPushpinOnMap(dto);
        }


        private void PutPushpinOnMap(NewsItemDto dto)
        {
            mapNewsItem.CredentialsProvider = new ApplicationIdCredentialsProvider("Atx6C4-6W6IofeQA3RsyCWSbKeE0Z9QVrmFxNIMagT0rrxQF6_R25A1d-N_r0bXY");
            GeoCoordinate Location = new GeoCoordinate(dto.Latitude, dto.Longitude);
            pinMapLocator.Location = Location;
            mapNewsItem.SetView(Location, 15);
        }


        private void AddPhotosToList(ObservableCollection<NewsItemPhotoDto> items)
        {
            ocMedia.Clear();

            foreach (var i in items)
            {
                ocMedia.Add
                (
                    new NewsItemMediaModel()
                    {
                        Caption = i.Caption,
                        BlobUri = i.BlobUri,
                        VideoButtonVisibility = "Collapsed"
                    }
                );
            }
            lstMedia.ItemsSource = ocMedia;
        }


        private void AddVideosToList(ObservableCollection<NewsItemVideoDto> items)
        {
            //ocMedia.Clear();

            foreach (var i in items)
            {
                ocMedia.Add
                (
                    new NewsItemMediaModel()
                    {
                        Caption= i.Title,
                        BlobUri = i.BlobUri,
                        VideoButtonVisibility = "Visible"
                    }
                );
            }
            lstMedia.ItemsSource = ocMedia;
        }


        private void AddCommentsToList(List<CommentDto> items)
        {
            ocComments.Clear();

            foreach (var i in items)
            {
                ocComments.Add
                (
                    new CommentModel()
                    {
                        PostedByUserID = i.PostedByUserID,
                        PostedByUserName = i.PostedByUserName,
                        CommentBody = i.CommentBody,
                        CreateDate = i.CreateDate.ToString(),
                        BlobUri = i.MediumSizeBlobUri,         
                    }
                );
            }
            lstComment.ItemsSource = ocComments;
        }


        void txbPostedByUserName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            var id = textBlock.Tag.ToString();
            NavigateToUserView(id);
        }


        private void NavigateToUserView(string id)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri(string.Format("/HLApp;component/Views/User.xaml?id={0}", id), UriKind.Relative));
        }


        private void btnPlayVideo_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var blobUriString = button.Tag.ToString();

            /* The video SAS blobUri contains a query string, therefore it is difficult to transport it with a query 
             * string to the VideoPlayer view! So I a transporting the blobUri via a static variable. */
            blobUri = blobUriString;

            NavigateToVideoPlayerView(newsItemIdString);
        }


        private void NavigateToVideoPlayerView(string id)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri(string.Format("/HLApp;component/Views/VideoPlayer.xaml?id={0}", id), UriKind.Relative));
        }


        private void btnComment_Click(object sender, RoutedEventArgs e)
        {
            WS.CreateCommentAsync(Convert.ToInt32(newsItemIdString), LogOn.loggedInUserId, txtComment.Text);
            WS.CreateCommentCompleted += new EventHandler<CreateCommentCompletedEventArgs>(WS_CreateCommentCompleted);
        }


        void WS_CreateCommentCompleted(object sender, CreateCommentCompletedEventArgs e)
        {
            GetCommentsOnNewsItem(Convert.ToInt32(newsItemIdString));
        }
    }
}