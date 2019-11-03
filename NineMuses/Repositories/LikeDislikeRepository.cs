using NineMuses.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace NineMuses.Repositories
{
    public class LikeDislikeRepository
    {
        private UserRepository _userRepo;

        public LikeDislikeRepository()
        {
            _userRepo = new UserRepository();
        }

        public List<LikeDislikeModel> GetVideoLikes(int id, bool withChilds)
        {
            var likes = new List<LikeDislikeModel>();

            using (SqlConnection DBConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("spGetVideoLikes", DBConn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCmd.Parameters.Add("@VideoID", SqlDbType.Int).Value = id;

                DBConn.Open();

                using (var DB = sqlCmd.ExecuteReader())
                {
                    var like = new LikeDislikeModel();

                    while (DB.Read())
                    {
                        like = new LikeDislikeModel()
                        {
                            ID = (int)DB["Id"],
                            VideoID = (int)DB["VideoID"],
                            UserID = (long)DB["UserID"],
                            Like = (bool)DB["Like"]
                        };

                        //Om withChilds är true får man med sig användarnnamnet
                        if (withChilds)
                        {
                            like.User = _userRepo.GetUser(like.UserID);
                        }

                        likes.Add(like);
                    }
                }
            }

            return likes;
        }

        public bool? LikeDislike(int videoID, bool like, long userID)
        {

            using (SqlConnection DBConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("spLikeDislike", DBConn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCmd.Parameters.Add("@VideoID", SqlDbType.Int).Value = videoID;
                sqlCmd.Parameters.Add("@Like", SqlDbType.Bit).Value = like;
                sqlCmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = userID;

                DBConn.Open();

                bool? returnValue = null;

                using (var DB = sqlCmd.ExecuteReader())
                {
                    if(DB.Read())
                    {
                        returnValue = (bool)DB["LikeValue"];
                    }
                    
                }

                return returnValue;
            }
        }
    }
}