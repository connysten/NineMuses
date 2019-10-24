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
using NineMuses.ViewModels;

namespace NineMuses.Controllers
{
    public class UserController : Controller
    {
        // GET: Login
        public ActionResult SignIn(string returnUrl)
        {
            if (Session["UserID"] == null)
            {
                var model = new LoginViewModel();
                model.ReturnUrl = Server.UrlDecode(returnUrl);
                return View(model);
            }
            else
            {
                return RedirectToAction("Profile", "User", new { UserID = Session["UserID"].ToString() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(LoginViewModel m)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spValidateUser", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Username", m.User.Username);
                command.Parameters.AddWithValue("@Password", m.User.Password);
                command.Parameters.Add("@Responsemessage", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@UserID", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                command.ExecuteNonQuery();

                if (Convert.ToInt32(command.Parameters["@Responsemessage"].Value) == 1)
                {
                    Session["UserID"] = Convert.ToInt32(command.Parameters["@UserID"].Value);

                    if(!string.IsNullOrEmpty(m.ReturnUrl))
                    {
                        return Redirect(m.ReturnUrl);
                    }

                    return RedirectToAction("Profile", "User", new { UserID = Session["UserID"].ToString() });
                }
            }

            ModelState.AddModelError("", "Incorrect Login");
            ModelState.SetModelValue("Password", new ValueProviderResult(string.Empty, string.Empty, CultureInfo.InvariantCulture));

            return View(m);
        }

        public new ActionResult Profile(string UserID)
        {
            if (UserID == null || Session["UserID"].ToString() != UserID)
            {
                return RedirectToAction("SignIn", "User");
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
                conn.Close();
            }

            return RedirectToAction("SignIn", "User");
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
                    return RedirectToAction("Profile", "User", new { UserID = Session["UserID"] });
                }
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult UpdatePassword(UserModel m)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spChangePassword", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Username", m.Username);
                command.Parameters.AddWithValue("@Password", m.Password);
                command.Parameters.Add("@Responsemessage", SqlDbType.Int).Direction = ParameterDirection.Output;
                //command.Parameters.Add("@UserID", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                command.ExecuteNonQuery();

                if (Convert.ToInt32(command.Parameters["@Responsemessage"].Value) == 1)
                {
                    ViewData["Message"] = "Password was successfully changed!";
                    //Session["UserID"] = Convert.ToInt32(command.Parameters["@UserID"].Value);
                    return RedirectToAction("Profile", "User", new { UserID = Session["UserID"].ToString() });
                    //new { UserID = Session["UserID"] });
                }
            }
            return View(m);
        }
    }
}