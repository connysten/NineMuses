using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NineMuses.Models
{
    public class LikeDislikeModel
    {
        public int ID { get; set; }
        public bool Like { get; set; }

        public long UserID { get; set; }
        public UserModel User { get; set; }

        public int VideoID { get; set; }
        public VideoModel Video { get; set; }

    }
}