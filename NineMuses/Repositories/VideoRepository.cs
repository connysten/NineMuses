using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using NineMuses.Models;
using NineMuses.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace NineMuses.Repositories
{
    public class VideoRepository
    {
        private UserRepository _userRepo;
        private LikeDislikeRepository _likeRepo;

        public VideoRepository()
        {
            _userRepo = new UserRepository();
            _likeRepo = new LikeDislikeRepository();
        }

        public VideoModel GetVideo(int id, bool withChilds)
        {
            var video = new VideoModel();

            using (SqlConnection DBConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand("spGetVideo", DBConn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                sqlCmd.Parameters.Add("@VideoID", SqlDbType.Int).Value = id;

                DBConn.Open();

                using (var DB = sqlCmd.ExecuteReader())
                {
                    while (DB.Read())
                    {
                        video = new VideoModel()
                        {
                            User = new UserModel() 
                            { 
                                UserID = (long)DB["UserID"] 
                            },
                            VideoID = (int)DB["VideoID"],
                            Thumbnail = (string)DB["Thumbnail"],
                            Source = (string)DB["Source"],
                            Title = (string)DB["Title"],
                            Description = DB["Description"] == DBNull.Value ? "No Description" : (string)DB["Description"],
                            Views = (int)DB["Views"],
                            UploadDate = (DateTime)DB["UploadDate"]
                        };
                    }
                }
            }

            //Om withChilds är true får man med sig användarnnamnet
            video.Likes = _likeRepo.GetVideoLikes(video.VideoID, true);

            if (video != null && withChilds)
            {
                video.User = _userRepo.GetUser(video.User.UserID);
            }

            return video;
        }



        public List<VideoModel> GetVideoList(SqlCommand command)
        {
            List<VideoModel> returnList = new List<VideoModel>();

            using (SqlConnection DBConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {

                command.Connection = DBConn;

                DBConn.Open();

                using (var DB = command.ExecuteReader())
                {
                    while(DB.Read())
                    {
                        var video = new VideoModel
                        {
                            User = new UserModel()
                            {
                                UserID = (long)DB["UserID"]
                            },
                            VideoID = (int)DB["VideoID"],
                            Thumbnail = (string)DB["Thumbnail"],
                            Source = (string)DB["Source"],
                            Title = (string)DB["Title"],
                            Description = DB["Description"] == DBNull.Value ? "No Description" : (string)DB["Description"],
                            Views = (int)DB["Views"],
                            UploadDate = (DateTime)DB["UploadDate"]
                        };

                        video.User = _userRepo.GetUser(video.User.UserID);
                        video.Likes = _likeRepo.GetVideoLikes(video.VideoID, true);

                        returnList.Add(video);
                    }
                }
            }

            return returnList;
        }

        public List<VideoModel> GetMostLiked(int videoCount)
        {
            SqlCommand command = new SqlCommand()
            {
                CommandText = "spGetAllVideos",
                CommandType = CommandType.StoredProcedure
            };

            var videos = GetVideoList(command);
            var videosOrdered = videos.OrderByDescending(x => x.GetLikes);
            var final = videosOrdered.Take(videoCount);

            return final.ToList();
            //return videos.OrderByDescending(x => x.GetLikes).Take(videoCount).ToList();
        }

        public int Upload(UploadVideoViewModel model)
        {
            string videoUploadFolder = "/UserMedia/Videos";
            var fileGuid = Guid.NewGuid().ToString();
            var fileExt = Path.GetExtension(model.VideoFile.FileName);
            var fileName = fileGuid + fileExt;
            string VideoDbPath = videoUploadFolder + "/" + fileName;
            string path = Path.Combine(HttpContext.Current.Server.MapPath(videoUploadFolder), fileName);
            model.VideoFile.SaveAs(path);

            string thumbnailDbPath = CreateThumbnail(fileGuid, path);

            if(model.Video.Description == null)
            {
                model.Video.Description = "No Description";
            }

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spUploadVideo", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserID", (long)HttpContext.Current.Session["UserID"]);
                command.Parameters.AddWithValue("@Title", model.Video.Title);
                command.Parameters.AddWithValue("@Description", model.Video.Description);
                command.Parameters.Add("@VideoID", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.AddWithValue("@Source", VideoDbPath);
                command.Parameters.AddWithValue("@Thumbnail", thumbnailDbPath);
                conn.Open();
                command.ExecuteNonQuery();

                if (int.TryParse(command.Parameters["@VideoID"].Value.ToString(), out int videoId))
                {
                    return videoId;
                }
            }

            return 0;
        }

        public void AddView(int id)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spIncreaseViewCount", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@VideoID", id);
                conn.Open();
                command.ExecuteNonQuery();
            }

        }

        public string CreateThumbnail(string fileGuid, string path)
        {
            string thumbnailUploadFolder = "/UserMedia/Thumbnails";
            string thumbnailDbPath = thumbnailUploadFolder + "/" + fileGuid + ".jpg";

            var inputFile = new MediaFile { Filename = path };
            var outputFile = new MediaFile { Filename = Path.Combine(HttpContext.Current.Server.MapPath(thumbnailUploadFolder), fileGuid + ".jpg") };
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                // Sparar framen som är under den 5:e sekunden av videon
                var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(5) };
                engine.GetThumbnail(inputFile, outputFile, options);
            }

            return thumbnailDbPath;
        }
    }
}