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
    public class NewsItemMediaModel
    {
        private string blobUri;
        private string caption;
        private string videoButtonVisibility;


        public string Caption
        {
            get { return caption; }
            set
            {
                caption = value;
                NotifyPropertyChanged("Caption");
            }
        }


        public string BlobUri
        {
            get { return blobUri; }
            set
            {
                blobUri = value;
                NotifyPropertyChanged("BlobUri");
            }
        }


        public string VideoButtonVisibility
        {
            get { return videoButtonVisibility; }
            set
            {
                videoButtonVisibility = value;
                NotifyPropertyChanged("VideoButtonVisibility");
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
