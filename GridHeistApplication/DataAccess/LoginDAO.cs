using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridHeistApplication.DataAccess
{
    public class LoginDAO
    {

        private readonly string connectionString = "Server=DESKTOP-133\\SQLEXPRESS;Database=GridHeist3;Integrated Security=True;";

        public int ValidateLogin(string username, string password, out int playerId)
        {
            playerId = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_ValidatePlayer", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@PlayerName", SqlDbType.VarChar, 50).Value = username;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar, 100).Value = password;

                SqlParameter idParam = cmd.Parameters.Add("@PlayerID", SqlDbType.Int);
                idParam.Direction = ParameterDirection.Output;

                conn.Open();
                int status = Convert.ToInt32(cmd.ExecuteScalar());

                if (status == 1 && idParam.Value != DBNull.Value)
                {
                    playerId = (int)idParam.Value;
                }

                return status;
            }
        }

        public int RegisterPlayer(string username, string password, out int playerId)
        {
            playerId = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_RegisterPlayer", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@PlayerName", SqlDbType.VarChar, 50).Value = username;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar, 100).Value = password;

                SqlParameter idParam = cmd.Parameters.Add("@PlayerID", SqlDbType.Int);
                idParam.Direction = ParameterDirection.Output;

                conn.Open();
                int status = Convert.ToInt32(cmd.ExecuteScalar());

                if (status == 1 && idParam.Value != DBNull.Value)
                {
                    playerId = (int)idParam.Value;
                }

                return status;
            }
        }

        /// <summary>
        /// Check if a user is an admin
        /// </summary>
        public bool GetUserAdminStatus(string username)
        {
            bool isAdmin = false;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT IsAdmin FROM Players WHERE LOWER(PlayerName) = LOWER(@PlayerName)", conn))
                {
                    cmd.Parameters.AddWithValue("@PlayerName", username);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            isAdmin = Convert.ToBoolean(result);
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Database error checking admin status: " + ex.Message);
                    }
                }
            }

            return isAdmin;
        }
    }
}


