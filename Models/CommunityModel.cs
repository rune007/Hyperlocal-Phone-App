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
    public class CommunityModel
    {
        private int communityId;
        private string name;
        private string description;
        private int numberOfUsers;
        private string latestActivity;
        private int addedByUserId;
        private string addedByUserName;
        private string coverPhotoLarge;
        private string coverPhotoMediumSize;
        private string coverPhotoThumbnail;


        public int CommunityID
        {
            get { return communityId; }
            set
            {
                communityId = value;
                NotifyPropertyChanged("CommunityID");
            }
        }


        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }


        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                NotifyPropertyChanged("Description");
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


        public int NumberOfUsers
        {
            get { return numberOfUsers; }
            set
            {
                numberOfUsers = value;
                NotifyPropertyChanged("NumberOfUsers");
            }
        }


        public int AddedByUserID
        {
            get { return addedByUserId; }
            set
            {
                addedByUserId = value;
                NotifyPropertyChanged("AddedByUserID");
            }
        }


        public string AddedByUserName
        {
            get { return addedByUserName; }
            set
            {
                addedByUserName = value;
                NotifyPropertyChanged("AddedByUserName");
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
