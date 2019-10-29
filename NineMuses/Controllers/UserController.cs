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
using NineMuses.Repositories;

namespace NineMuses.Controllers
{
    public class UserController : Controller
    {
        private UserRepository _userRepo = new UserRepository();


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
                return RedirectToAction("Profile", "User", new { id = Session["UserID"].ToString() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(LoginViewModel model)
        {
            int userId = _userRepo.SignIn(model);

            if (userId != 0)
            {
                Session["UserID"] = userId;

                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction("Profile", "User", new { id = Session["UserID"].ToString() });
            }

            else
            {
                ModelState.AddModelError("", "Incorrect Username or Password");
                ModelState.SetModelValue("Password", new ValueProviderResult(string.Empty, string.Empty, CultureInfo.InvariantCulture));
                return View(model);
            }
        }


        public ActionResult SignOut()
        {
            Session["UserID"] = null;
            return RedirectToAction("SignIn", "User");
        }

        public new ActionResult Profile(long? id)
        {
            if(Session["UserID"] == null || string.IsNullOrEmpty(Session["UserID"].ToString()))
            {
                return RedirectToAction("SignIn", "User");
            }

            var model = new ProfileViewModel();

            if (id != null)
            {
                model.User = _userRepo.GetUser((long)id);
            }

            return View(model);
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SignUp(SignUpViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            int userId = _userRepo.SignUp(model);

            if(userId != 0)
            {
                Session["UserID"] = userId;
                return RedirectToAction("Profile", "User", new { id = Session["UserID"] });
            }

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdatePassword(UserModel model)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spChangePassword", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Username", model.Username);
                command.Parameters.AddWithValue("@Password", model.Password);
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
            return View(model);
        }
    }
}