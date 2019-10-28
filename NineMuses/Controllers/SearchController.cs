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
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Index(string search)
        {
            var model = new SearchViewModel();
            var _videoRepo = new VideoRepository();

            SqlCommand command = new SqlCommand()
            {
                CommandText = "spVideoSearch",
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@SearchString", search);

            model.Videos = _videoRepo.GetVideoList(command);

            return View(model);
        }
    }
}