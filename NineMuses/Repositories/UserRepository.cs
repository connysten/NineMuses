using NineMuses.Models;
using NineMuses.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace NineMuses.Repositories
{
    public class UserRepository
    {

        public UserRepository()
        {

        }

        public int SignIn(LoginViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spValidateUser", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Username", model.User.Username);
                command.Parameters.AddWithValue("@Password", model.User.Password);
                command.Parameters.Add("@Responsemessage", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@UserID", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                command.ExecuteNonQuery();

                if (Convert.ToInt32(command.Parameters["@Responsemessage"].Value) == 1)
                {
                    return Convert.ToInt32(command.Parameters["@UserID"].Value);
                }

                else
                {
                    return 0;
                }
            }
        }

        public int SignUp(SignUpViewModel model)
        {
            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spNewUser", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Username", model.Username);
                command.Parameters.AddWithValue("@Password", model.Password);
                command.Parameters.Add("@Responsemessage", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@UserID", SqlDbType.Int).Direction = ParameterDirection.Output;

                conn.Open();
                command.ExecuteNonQuery();

                if (Convert.ToInt32(command.Parameters["@Responsemessage"].Value) == 1)
                {
                    return Convert.ToInt32(command.Parameters["@UserID"].Value);
                }

                else
                {
                    return 0;
                }
            }
        }

        public UserModel GetUser(long id)
        {
            UserModel User = null;

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spGetUser", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserID", id);

                conn.Open();

                using (var DB = command.ExecuteReader())
                {
                    while (DB.Read())
                    {
                        User = new UserModel
                        {
                            UserID = (long)DB["UserID"],
                            Username = (string)DB["Username"]
                        };
                    }
                }
                conn.Close();
            }

            return User;
        }

        public List<SearchUserModel> GetUserList(SqlCommand command)
        {
            List<SearchUserModel> UserList = new List<SearchUserModel>();

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (command)
            {
                command.Connection = conn;

                conn.Open();

                using (var DB = command.ExecuteReader())
                {
                    SearchUserModel User = null;
                    while (DB.Read())
                    {
                        User = new SearchUserModel
                        {
                            UserID = (long)DB["UserID"],
                            Username = (string)DB["Username"],
                            NrOfVideos = (int)DB["NrOfVideos"]
                        };
                        UserList.Add(User);
                    }
                }
                conn.Close();
            }
            return UserList;
        }

    }
}