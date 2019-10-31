using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NineMuses.Models
{
    public class SearchUserModel
    {
        public string Username { get; set; }
        public long UserID { get; set; }
        public int NrOfVideos { get; set; }
    }
}