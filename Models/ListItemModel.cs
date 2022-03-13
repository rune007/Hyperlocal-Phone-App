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
    public class ListItemModel
    {
        private string itemType;
        private string name;
        private string id;
        private int idInt;
        private string thumbnailPhotoUri;


        public string ItemType
        {
            get { return itemType; }
            set
            {
                itemType = value;
                NotifyPropertyChanged("CoverPhotoMediumSize");
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


        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }


        public int IdInt
        {
            get { return idInt; }
            set
            {
                idInt = value;
                NotifyPropertyChanged("IdInt");
            }
        }


        public string ThumbnailPhotoUri
        {
            get { return thumbnailPhotoUri; }
            set
            {
                thumbnailPhotoUri = value;
                NotifyPropertyChanged("ThumbnailPhotoUri");
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
