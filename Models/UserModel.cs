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
    public class UserModel
    {
        private int userId;
        private string fullName;
        private string latestActivity;
        private int numberOfNewsItems;
        private string coverPhotoLarge;
        private string coverPhotoMediumSize;
        private string coverPhotoThumbnail;


        public int UserID
        {
            get { return userId; }
            set
            {
                userId = value;
                NotifyPropertyChanged("UserID");
            }
        }


        public string FullName
        {
            get { return fullName; }
            set
            {
                fullName = value;
                NotifyPropertyChanged("FullName");
            }
        }


        public string LatestActivity
        {
            get { return latestActivity; }
            set
            {
                latestActivity = value;
                NotifyPropertyChanged("LatestActivity");
            }
        }


        public int NumberOfNewsItems
        {
            get { return numberOfNewsItems; }
            set
            {
                numberOfNewsItems = value;
                NotifyPropertyChanged("NumberOfNewsItems");
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


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
