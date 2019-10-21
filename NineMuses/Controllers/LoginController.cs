using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using NineMuses.Models;
using System.Data;
using System.Globalization;

namespace NineMuses.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("UserProfile", "Home", new { UserID = Session["UserID"].ToString() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index (UserModel m)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spValidateUser", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Username", m.Username);
                command.Parameters.AddWithValue("@Password", m.Password);
                command.Parameters.Add("@Responsemessage", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@UserID", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                command.ExecuteNonQuery();

                if (Convert.ToInt32(command.Parameters["@Responsemessage"].Value) == 1)
                {
                    Session["UserID"] = Convert.ToInt32(command.Parameters["@UserID"].Value);
                    return RedirectToAction("UserProfile", "Home", new { UserID = Session["UserID"].ToString() });
                }  
            }



            ModelState.AddModelError("", "Incorrect Login");
            ModelState.SetModelValue("Password", new ValueProviderResult(string.Empty, string.Empty, CultureInfo.InvariantCulture));

            return View(m);
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(UserModel m)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spNewUser", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Username", m.Username);
                command.Parameters.AddWithValue("@Password", m.Password);
                command.Parameters.Add("@Responsemessage", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@UserID", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                command.ExecuteNonQuery();

                if (Convert.ToInt32(command.Parameters["@Responsemessage"].Value) == 1)
                {
                    Session["UserID"] = Convert.ToInt32(command.Parameters["@UserID"].Value);
                    return RedirectToAction("UserProfile", "Home", new { UserID = Session["UserID"] });
                }
            }

            return View(m);
        }
    }
}