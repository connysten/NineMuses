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
using System.Web.Mvc;

namespace NineMuses.Controllers
{
    public class VideoController : Controller
    {

        public ActionResult Upload()
        {
            if (Session["UserID"] == null)
            {

                return RedirectToAction("Index", "Login", new { returnUrl = Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped).ToString() });
            }

            else
            {
                return View();
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Upload(UploadVideoViewModel model)
        {
            if (model.VideoFile == null)
            {
                ModelState.AddModelError("", "Video file is required");
                return View(model);
            }
            if (model.Video.Title == null)
            {
                ModelState.AddModelError("", "Title is required");
                return View(model);
            }

            string videoUploadFolder = "/UserMedia/Videos";
            var fileGuid = Guid.NewGuid().ToString();
            var fileExt = Path.GetExtension(model.VideoFile.FileName);
            var fileName = fileGuid + fileExt;
            string VideoDbPath = videoUploadFolder + "/" + fileName;
            string path = Path.Combine(Server.MapPath(videoUploadFolder), fileName);

            if (System.IO.File.Exists(path))
            {
                fileName = fileName.Split('.')[0] + "(2)." + fileName.Split('.')[1];
                path = Path.Combine(Server.MapPath(videoUploadFolder), fileName);
            }
            model.VideoFile.SaveAs(path);

            string thumbnailUploadFolder = "/UserMedia/Thumbnails";
            string thumbnailDbPath = thumbnailUploadFolder + "/" + fileGuid + ".jpg";

            var inputFile = new MediaFile { Filename = path };
            var outputFile = new MediaFile { Filename = Path.Combine(Server.MapPath(thumbnailUploadFolder), fileGuid + ".jpg")};
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                // Sparar framen som är under den 5:e sekunden av videon
                var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(5) };
                engine.GetThumbnail(inputFile, outputFile, options);
            }

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spUploadVideo", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserID", (int)Session["UserID"]);
                command.Parameters.AddWithValue("@Title", model.Video.Title);
                command.Parameters.AddWithValue("@Description", model.Video.Description);
                command.Parameters.Add("@VideoID", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.AddWithValue("@Source", VideoDbPath);
                command.Parameters.AddWithValue("@Thumbnail", thumbnailDbPath);
                conn.Open();
                command.ExecuteNonQuery();

                int videoId = 0;
                if (int.TryParse(command.Parameters["@VideoID"].Value.ToString(), out videoId))
                {
                    return RedirectToAction("View", "Video", new { id = videoId });
                }
            }
            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = new ViewVideoViewModel();
            if (id != 0)
            {

                using (SqlConnection DBConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    SqlCommand sqlCmd = new SqlCommand("spGetVideo", DBConn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    sqlCmd.Parameters.Add("@VideoID", SqlDbType.Int).Value = id;

                    DBConn.Open();
                    var video = new VideoModel();

                    using (var DB = sqlCmd.ExecuteReader())
                    {
                        while (DB.Read())
                        {
                            video = new VideoModel()
                            {
                                Thumbnail = (string)DB["Thumbnail"],
                                Source = (string)DB["Source"],
                                Title = (string)DB["Title"],
                                Description = (string)DB["Description"],
                                Views = (int)DB["Views"],
                                UploadDate = (DateTime)DB["UploadDate"]
                            };
                           
                        }

                        model.Video = video;
                    }
                }

                return View(model);
            }
            return View(model);
        }
    }
}