using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NineMuses.Models;

namespace NineMuses.ViewModels
{
    public class SearchViewModel
    {
        public List<VideoModel> Videos { get; set; } = new List<VideoModel>();
        public List<SearchUserModel> Users { get; set; } = new List<SearchUserModel>();
    }
}