using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NineMuses.Models
{
    public class VideoModel
    {
        public UserModel User { get; set; }
        public int VideoID { get; set; }
        public string Thumbnail { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Views { get; set; }
        public DateTime UploadDate { get; set; }
        public List<LikeDislikeModel> Likes { get; set; } = new List<LikeDislikeModel>();
        
        public float GetLikePrecentage
        {
            get
            {
                if (this.GetLikes != 0 && this.GetDislikes != 0)
                {
                    return (float)((decimal)this.GetLikes / (decimal)this.Likes.Count) * 100;
                }

                if (this.GetLikes > 0 && this.GetDislikes == 0)
                {
                    return 100;
                }

                return 0;
            }
        }
        
        public int GetLikes
        {
            get
            {
                return this.Likes
                    .Where(x => x.Like)
                    .Count();
            }
        }

        public int GetDislikes
        {
            get
            {
                return this.Likes
                    .Where(x => !x.Like)
                    .Count();
            }
        }

        public string GetUserName
        {
            get
            {
                return this.User != null ? this.User.Username : null;
            }

        }

        public bool? UserHasLiked(long userId)
        {
            if (HttpContext.Current.Session["UserID"] != null)
            {
                var like = this.Likes
                    .FirstOrDefault(x => x.UserID == (long)HttpContext.Current.Session["UserID"]);

                if (like != null)
                {
                    return like.Like;
                }
                else
                {
                    return null;
                }
            }
            
            else
            {
                return null;
            }
        }
    }
}