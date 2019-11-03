using NineMuses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NineMuses.ViewModels
{
    public class IndexViewModel
    {
        public List<VideoModel> MostViewed { get; set; } = new List <VideoModel>();
        public List<VideoModel> RecentUploads { get; set; } = new List<VideoModel>();
        public List<VideoModel> MostLiked { get; set; } = new List<VideoModel>();
    }
}