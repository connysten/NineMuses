using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NineMuses.Models
{
    public class VideoModel
    {
        public long UserID { get; set; }
        public UserModel User { get; set; }
        public int VideoID { get; set; }
        public string Thumbnail { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Views { get; set; }
        public DateTime UploadDate { get; set; }

        public string GetUserName
        {
            get
            {
                return this.User != null ? this.User.Username : null;
            }

        }
    }
}