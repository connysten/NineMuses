using NineMuses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NineMuses.ViewModels
{
    public class ProfileViewModel
    {
        public UserModel User { get; set; }
        public List <VideoModel> Videos { get; set; }
        public bool Admin { get; set; }
    }
}