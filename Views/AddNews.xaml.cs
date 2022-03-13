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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using HLApp.HLServiceReference;
using HLApp.GeocodeService;
using Microsoft.Phone;
using System.IO.IsolatedStorage;
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;

namespace HLApp.Views
{
    /* The code which handles a photo taken with the Windows Phone 7 Emulator is based on a code example I discovered on
    * Chris Love's blog ProfessionalAspNet.com
    * http://professionalaspnet.com/archive/2011/01/13/Posting-A-Photo-From-Windows-Phone-7-To-A-Web-Service.aspx
    * (23-05-2012) */
    public partial class AddNews : PhoneApplicationPage
    {
        private CameraCaptureTask cameraTask = new CameraCaptureTask();
        private PhotoChooserTask choosePhoto = new PhotoChooserTask();
        private HLServiceClient WS;
        private GeocodeServiceClient GeoWS;
        private float latitude;
        private float longitude;
        private WriteableBitmap picture;
        private string tempfolder = "temp";
        private string fileName = "fileName";
        private int pixelHeight = 0;
        private int pixelWidth = 0;
        private int newsItemId = 0;


        public AddNews()
        {
            InitializeComponent();

            WS = new HLServiceClient();
            GeoWS = new GeocodeServiceClient();

            this.cameraTask.Completed += new EventHandler<PhotoResult>(cameraTask_Completed);
            this.choosePhoto.Completed += new EventHandler<PhotoResult>(choosePhoto_Completed);

            // NewsItemCategories
            WS.GetNewsItemCategoriesAsync();
            WS.GetNewsItemCategoriesCompleted += new EventHandler<GetNewsItemCategoriesCompletedEventArgs>(WS_GetNewsItemCategoriesCompleted);

            PerformGeocoding();
        }


        void WS_GetNewsItemCategoriesCompleted(object sender, GetNewsItemCategoriesCompletedEventArgs e)
        {
            var categoryDtos = e.Result.ToList();

            IEnumerable<ListBoxItem> lstCategory = categoryDtos.Select(cat => new ListBoxItem
            {
                Content = cat.CategoryName
            });
            lsbCategory.ItemsSource = lstCategory;
        }


        private void btnCamera_Click(object sender, RoutedEventArgs e)
        {
            cameraTask.Show();
        }


        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            choosePhoto.PixelHeight = pixelHeight;
            choosePhoto.PixelWidth = pixelWidth;
            choosePhoto.ShowCamera = true;
            choosePhoto.Show();
        }


        private void btnAddNews_Click(object sender, RoutedEventArgs e)
        {
            if (lsbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("You have to Choose a Category!", "Required Field", MessageBoxButton.OK);
                lsbCategory.Focus();
                return;
            }

            if (txtTitle.Text.Length == 0)
            {
                MessageBox.Show("You have to enter a Title!", "Required Field", MessageBoxButton.OK);
                txtTitle.Focus();
                return;
            }

            if (txtStory.Text.Length == 0)
            {
                MessageBox.Show("You have to enter a Story!", "Required Field", MessageBoxButton.OK);
                txtStory.Focus();
                return;
            }

            var newsItemDto = new HLServiceReference.NewsItemDto();
            newsItemDto.PostedByUserID = LogOn.loggedInUserId;
            newsItemDto.CategoryID = lsbCategory.SelectedIndex + 1; /* The + 1 is because the ListBox index starts at 0, whereas my NewsItemCategoryID starts at 1. */;
            newsItemDto.AssignmentID = null;
            newsItemDto.Title = txtTitle.Text;
            newsItemDto.Story = txtTitle.Text;
            newsItemDto.IsLocalBreakingNews = Convert.ToBoolean(cbBreakingNews.IsChecked);
            newsItemDto.Latitude = latitude;
            newsItemDto.Longitude = longitude;

            WS.CreateNewsItemAsync(newsItemDto);
            WS.CreateNewsItemCompleted += new EventHandler<CreateNewsItemCompletedEventArgs>(WS_CreateNewsItemCompleted);
        }


        void WS_CreateNewsItemCompleted(object sender, CreateNewsItemCompletedEventArgs e)
        {
            newsItemId = e.Result;

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("News Item Posted :-)");
            }

            /* Uploading the photo to the web service. */
            PostImageToWebService(picture);
        }


        void choosePhoto_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                CompleteCameraTask(e);
            }
        }


        void cameraTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                CompleteCameraTask(e);
            }
        }


        private void CompleteCameraTask(PhotoResult e)
        {
            var txt = e.OriginalFileName;

            // Display the photo as taken by the camera
            picture = PictureDecoder.DecodeJpeg(e.ChosenPhoto);

            this.imgPhoto.Source = picture;

            pixelHeight = picture.PixelHeight;
            pixelWidth = picture.PixelWidth;
        }


        private void PostImageToWebService(WriteableBitmap picture)
        {
            var fileName = "tempPhoto-" + Guid.NewGuid().ToString() + ".jpg";

            SavePictureToIsolatedStorage(picture, fileName);

            var client = new HLServiceClient();

            /* Here we are actually starting to save the image. */
            client.SaveMediaFromPhoneAsync(newsItemId, GetPhotoBytes(fileName));
            client.SaveMediaFromPhoneCompleted += new EventHandler<SaveMediaFromPhoneCompletedEventArgs>(client_SaveMediaFromPhoneCompleted);

            //client.SaveImageAsync(newsItemId, "jpg", GetPhotoBytes(fileName));
            //client.SaveImageCompleted += new EventHandler<SaveImageCompletedEventArgs>(client_SaveImageCompleted);


        }

        void client_SaveImageCompleted(object sender, SaveImageCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK);
            }

            using (var appStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                appStorage.DeleteFile(String.Format(@"{0}\{1}", tempfolder, fileName));
            }
        }

        void client_SaveMediaFromPhoneCompleted(object sender, SaveMediaFromPhoneCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK);
            }

            using (var appStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                appStorage.DeleteFile(String.Format(@"{0}\{1}", tempfolder, fileName));
            }
        }


        public void SavePictureToIsolatedStorage(WriteableBitmap chosenPhoto, string fileName)
        {
            if (chosenPhoto != null)
            {
                using (var appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!appStorage.DirectoryExists(tempfolder))
                    {
                        appStorage.CreateDirectory(tempfolder);
                    }
                    using (var isoStream = appStorage.OpenFile(String.Format(@"{0}\{1}", tempfolder, fileName), FileMode.OpenOrCreate))
                    {
                        chosenPhoto.SaveJpeg(isoStream, chosenPhoto.PixelWidth, chosenPhoto.PixelHeight, 0, 100);
                    }
                }
            }
        }


        public byte[] GetPhotoBytes(string fileName)
        {
            using (var appStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                IsolatedStorageFileStream isoStream = appStorage.OpenFile(String.Format(@"{0}\{1}", tempfolder, fileName), System.IO.FileMode.Open);
                byte[] buffer = new byte[isoStream.Length];
                isoStream.Read(buffer, 0, (int)isoStream.Length); isoStream.Close();
                return buffer;
            }
        }


        #region Map & Geocoding

        /* A good deal of the code that does the address plotting against virtualearth.net's GeoCodeService I have copied, pasted and adapted to HLApp 
         * from a code example I found in the book: Lee/Chuvyrov, "Beginning Windows Phone 7 Development", Apress, USA 2010, p. 303 - 310. */
        private void PerformGeocoding()
        {
            /* The GeoCoordinateWatchers Position property obtains the physical geo coordinates of an actual physical device. */
            GeoCoordinateWatcher myWatcher = new GeoCoordinateWatcher();

            var myPosition = myWatcher.Position;

            /* We have to hard code the latitude / longitude because GeoCoordinateWather can only determine our position when we are running the code on an actual
             * physical device. These coordinates should be Vimmelskaftet 39, 1161, Copenhagen, Denmark. */
            latitude = (float)55.67853;
            longitude = (float)12.57576;

            if (!myPosition.Location.IsUnknown)
            {
                latitude = (float)myPosition.Location.Latitude;
                longitude = (float)myPosition.Location.Longitude;
            }

            this.SetLocation(latitude, longitude, 17, true);

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
                    latitude = (float)geoResult.Locations[0].Latitude;
                    longitude = (float)geoResult.Locations[0].Longitude;
                }
            };
        }


        private void SetLocation(double latitude, double longitude, double zoomLevel, bool showLocator)
        {
            // Move the pushpin to geo coordinate
            Microsoft.Phone.Controls.Maps.Platform.Location location = new Microsoft.Phone.Controls.Maps.Platform.Location();
            location.Latitude = latitude;
            location.Longitude = longitude;
            mapAddNews.SetView(location, zoomLevel);
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
    }
}