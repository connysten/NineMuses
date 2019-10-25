using NineMuses.Models;
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

        public UserModel GetUser(int id)
        {
            var User = new UserModel();

            using (SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            using (SqlCommand command = new SqlCommand("spGetUser", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@Responsemessage", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.AddWithValue("@UserID", id);
                command.Parameters.Add("@Username", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

                conn.Open();

                command.ExecuteNonQuery();
                if (Convert.ToInt32(command.Parameters["@Responsemessage"].Value) == 1)
                {
                    User = new UserModel
                    {
                        Username = (string)command.Parameters["@Username"].Value
                    };
                }
                conn.Close();
            }

            return User;
        }
    }
}