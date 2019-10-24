using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NineMuses.Models;

namespace NineMuses.ViewModels
{
    public class SearchVIewModel
    {
        public List<VideoModel> Videos { get; set; } = new List<VideoModel>();
    }
}