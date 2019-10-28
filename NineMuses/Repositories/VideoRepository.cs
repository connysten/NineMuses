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

        public VideoRepository()
        {
            _userRepo = new UserRepository();
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
                            UserID = (long)DB["UserID"],
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
            if (video != null && withChilds)
            {
                video.User = _userRepo.GetUser(video.UserID);
            }

            return video;
        }



        public List<VideoModel> GetIndexView(string storedProcedure)
        {
            List<VideoModel> returnList = new List<VideoModel>();

            using (SqlConnection DBConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                SqlCommand sqlCmd = new SqlCommand(storedProcedure, DBConn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                DBConn.Open();

                using (var DB = sqlCmd.ExecuteReader())
                {
                    while(DB.Read())
                    {
                        var video = new VideoModel
                        {
                            UserID = (long)DB["UserID"],
                            VideoID = (int)DB["VideoID"],
                            Thumbnail = (string)DB["Thumbnail"],
                            Source = (string)DB["Source"],
                            Title = (string)DB["Title"],
                            Description = DB["Description"] == DBNull.Value ? "No Description" : (string)DB["Description"],
                            Views = (int)DB["Views"],
                            UploadDate = (DateTime)DB["UploadDate"]
                        };

                        video.User = _userRepo.GetUser(video.UserID);

                        returnList.Add(video);
                    }
                }
            }

            return returnList;
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
                command.Parameters.AddWithValue("@UserID", (int)HttpContext.Current.Session["UserID"]);
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