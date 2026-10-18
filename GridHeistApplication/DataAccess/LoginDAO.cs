using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridHeistApplication.DataAccess
{
    internal class LoginDAO
    {
        private readonly string connectionString = "Server=SIYARA\\MSSQLSERVER01;Database=GridHeistDB;Integrated Security=True;";
        public int ValidateLogin(string username, string password)
        {
            int loginStatus = 0; // Default to invalid

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ValidatePlayer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PlayerName", username);
                    cmd.Parameters.AddWithValue("@PasswordHash", password);

                    try
                    {
                        conn.Open();
                        // ExecuteScalar grabs the single 'LoginStatus' value returned by our SELECT in SQL
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            loginStatus = Convert.ToInt32(result);
                        }
                    }
                    catch (SqlException ex)
                    {

                        throw new Exception("Database error during login: " + ex.Message);
                    }
                }
            }

            return loginStatus;
        }

        public int RegisterPlayer(string username, string password)
        {
            int regStatus = 0; // Default to failure (0)

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_RegisterPlayer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PlayerName", username);
                    cmd.Parameters.AddWithValue("@PasswordHash", password);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            regStatus = Convert.ToInt32(result);
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Database error during registration: " + ex.Message);
                    }
                }
            }

            return regStatus;
        }

        /// <summary>
        /// Check if a user is an admin
        /// </summary>
        public bool GetUserAdminStatus(string username)
        {
            bool isAdmin = false;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT IsAdmin FROM Players WHERE LOWER(PlayerName) = LOWER(@PlayerName)", conn)) conn))
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
