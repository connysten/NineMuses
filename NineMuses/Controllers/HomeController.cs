using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using NineMuses.Models;
using NineMuses.Repositories;
using NineMuses.ViewModels;

namespace NineMuses.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IndexViewModel model = new IndexViewModel();
            var _videoRepo = new VideoRepository();

            SqlCommand command = new SqlCommand()
            {
                CommandText = "spGetMostViewedVideos",
                CommandType = CommandType.StoredProcedure
            };

            model.MostViewed = _videoRepo.GetVideoList(command);

            command.CommandText = "spGetRecentUploads";
            model.RecentUploads = _videoRepo.GetVideoList(command);

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}