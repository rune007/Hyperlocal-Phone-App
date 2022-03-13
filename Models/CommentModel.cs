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
using System.Collections.ObjectModel;

namespace HLApp.Models
{
    public class CommentModel
    {
        private int postedByUserId;
        private string postedByUserName;
        private string commentBody;
        private string createDate;
        private string blobUri;


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


        public string CommentBody
        {
            get { return commentBody; }
            set
            {
                commentBody = value;
                NotifyPropertyChanged("CommentBody");
            }
        }


        public string CreateDate
        {
            get { return createDate; }
            set
            {
                createDate = value;
                NotifyPropertyChanged("CreateDate");
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



        //private ObservableCollection<NewsItemVideoModel> ocVideos;
        //public ObservableCollection<NewsItemVideoModel> OcVideos
        //{
        //    get { return this.ocVideos; }
        //    private set
        //    {
        //        this.ocVideos = value;
        //        this.NotifyPropertyChanged("OcVideos");
        //    }
        //}


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        } 
    }
}
