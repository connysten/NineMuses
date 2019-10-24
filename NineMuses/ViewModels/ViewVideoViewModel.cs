using NineMuses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NineMuses.ViewModels
{
    public class ViewVideoViewModel
    {
        public VideoModel Video { get; set; }
        public List<VideoModel> VideoList { get; set; } = new List<VideoModel>();

    }
}