using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using NineMuses.Models;
using NineMuses.ViewModels;

namespace NineMuses.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        public ActionResult Index(string search)
        {
            var model = new SearchVIewModel();
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spVideoSearch", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SearchString", search);

                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var video = new VideoModel();
                    while (reader.Read())
                    {
                        video = new VideoModel()
                        {
                            UserID = (int)reader["UserID"],
                            VideoID = (int)reader["VideoID"],
                            Thumbnail = (string)reader["Thumbnail"],
                            Source = (string)reader["Source"],
                            Title = (string)reader["Title"],
                            Description = (string)reader["Description"],
                            Views = (int)reader["Views"],
                            UploadDate = (DateTime)reader["UploadDate"]
                        };
                        model.Videos.Add(video);
                    }

                }
            }

            return View(model);
        }
    }
}