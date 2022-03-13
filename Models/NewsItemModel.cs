using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace HLApp.Models
{
    public class NewsItemModel : INotifyPropertyChanged
    {
        private int newsItemId;
        private int postedByUserId;
        private string postedByUserName;
        private int categoryId;
        private string categoryName;
        private int assignmentId;
        private string assignmentTitle;
        private string title;
        private string story;
        private string latitude;
        private string longitude;
        private string createUpdateDate;
        private int numberOfViews;
        private int numberOfComments;
        private int numberOfShares;
        private string coverPhotoLarge;
        private string coverPhotoMediumSize;
        private string coverPhotoThumbnail;
        private string locatedInCommunityName;
        /// <summary>
        /// Used for making a request at Bing Map REST imagery service which responds with a static map with a pushpin for a particular area,
        /// returned in the form of a .jpg image.
        /// E.g. "http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/55.912272930063594, 11.645507812500017/5?mapSize=300,200&pp=55.912272930063594, 11.645507812500017;22&key=Atx6C4-6W6IofeQA3RsyCWSbKeE0Z9QVrmFxNIMagT0rrxQF6_R25A1d-N_r0bXY".
        /// </summary>
        private ImageSource imageryMapServiceRestRequestUrl;


        public int NewsItemID
        {
            get { return newsItemId; }
            set
            {
                newsItemId = value;
                NotifyPropertyChanged("NewsItemID");
            }
        }


        public int PostedByUserID
        {
            get { return postedByUserId; }
            set
            {
                postedByUserId = value;
                NotifyPropertyChanged("PostedByUserID");
            }
        }


        public string PostedByUserName
        {
            get { return postedByUserName; }
            set
            {
                postedByUserName = value;
                NotifyPropertyChanged("PostedByUserName");
            }
        }


        public int CategoryID
        {
            get { return categoryId; }
            set
            {
                categoryId = value;
                NotifyPropertyChanged("CategoryID");
            }
        }


        public string CategoryName
        {
            get { return categoryName; }
            set
            {
                categoryName = value;
                NotifyPropertyChanged("CategoryName");
            }
        }


        public int AssignmentID
        {
            get { return assignmentId; }
            set
            {
                assignmentId = value;
                NotifyPropertyChanged("AssignmentID");
            }
        }


        public string AssignmentTitle
        {
            get { return assignmentTitle; }
            set
            {
                assignmentTitle = value;
                NotifyPropertyChanged("AssignmentTitle");
            }
        }


        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyPropertyChanged("Title");
            }
        }


        public string Story
        {
            get { return story; }
            set
            {
                story = value;
                NotifyPropertyChanged("Story");
            }
        }


        public string Latitude
        {
            get { return latitude; }
            set
            {
                latitude = value;
                NotifyPropertyChanged("Latitude");
            }
        }


        public string Longitude
        {
            get { return longitude; }
            set
            {
                longitude = value;
                NotifyPropertyChanged("Longitude");
            }
        }


        public string CreateUpdateDate
        {
            get { return createUpdateDate; }
            set
            {
                createUpdateDate = value;
                NotifyPropertyChanged("CreateUpdateDate");
            }
        }


        public int NumberOfViews
        {
            get { return numberOfViews; }
            set
            {
                numberOfViews = value;
                NotifyPropertyChanged("NumberOfViews");
            }
        }


        public int NumberOfComments
        {
            get { return numberOfComments; }
            set
            {
                numberOfComments = value;
                NotifyPropertyChanged("NumberOfComments");
            }
        }


        public int NumberOfShares
        {
            get { return numberOfShares; }
            set
            {
                numberOfShares = value;
                NotifyPropertyChanged("NumberOfShares");
            }
        }


        public string CoverPhotoLarge
        {
            get { return coverPhotoLarge; }
            set
            {
                coverPhotoLarge = value;
                NotifyPropertyChanged("CoverPhotoLarge");
            }
        }


        public string CoverPhotoMediumSize
        {
            get { return coverPhotoMediumSize; }
            set
            {
                coverPhotoMediumSize = value;
                NotifyPropertyChanged("CoverPhotoMediumSize");
            }
        }


        public string CoverPhotoThumbnail
        {
            get { return coverPhotoThumbnail; }
            set
            {
                coverPhotoThumbnail = value;
                NotifyPropertyChanged("CoverPhotoThumbnail");
            }
        }


        public string LocatedInCommunityName
        {
            get { return locatedInCommunityName; }
            set
            {
                locatedInCommunityName = value;
                NotifyPropertyChanged("LocatedInCommunityName");
            }
        }


        public ImageSource ImageryMapServiceRestRequestUrl
        {
            get { return imageryMapServiceRestRequestUrl; }
            set
            {
                imageryMapServiceRestRequestUrl = value;
                NotifyPropertyChanged("ImageryMapServiceRestRequestUrl");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }       
    }
}