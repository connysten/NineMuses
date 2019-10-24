using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NineMuses.Models
{
    public class VideoModel
    {
        public int UserID { get; set; }
        public int VideoID { get; set; }
        public string Thumbnail { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Views { get; set; }
        public DateTime UploadDate { get; set; }
    }
}