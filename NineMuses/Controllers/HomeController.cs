using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using NineMuses.Models; 

namespace NineMuses.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserProfile(string UserID)
        {
            if (UserID == null || Session["UserID"].ToString() != UserID)
            {
                return RedirectToAction("Index", "Login");
            }
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spGetUser", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@Responsemessage", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.AddWithValue("@UserID", UserID);
                command.Parameters.Add("@Username", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

                conn.Open();

                command.ExecuteNonQuery();
                if (Convert.ToInt32(command.Parameters["@Responsemessage"].Value) == 1)
                {
                    var User = new UserModel
                    {
                        Username = (string)command.Parameters["@Username"].Value
                    };
                    return View(User);
                }
                //SqlDataReader reader = command.ExecuteReader();

                //if (Convert.ToInt32(command.Parameters["@Responsemessage"].Value) == 1)
                //{
                //    while (reader.Read())
                //    {
                //        var User = new UserModel
                //        {
                //            Username = (string)reader["Username"]
                //        };

                //    }
                //    return View(User);
                //}
                conn.Close();
            }

            return RedirectToAction("Index", "Login");
        }

        public ActionResult Video()
        {
            return View();
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