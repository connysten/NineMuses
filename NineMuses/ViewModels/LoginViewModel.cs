using NineMuses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NineMuses.ViewModels
{
    public class LoginViewModel
    {
        public UserModel User{ get; set; }
        public string ReturnUrl { get; set; }
    }
}