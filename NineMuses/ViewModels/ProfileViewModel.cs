using NineMuses.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NineMuses.ViewModels
{
    public class ProfileViewModel
    {
        public UserModel User { get; set; }
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password Not Matching")]
        public string PasswordConfirm { get; set; }
        public List <VideoModel> Videos { get; set; } = new List<VideoModel>();
    }
}