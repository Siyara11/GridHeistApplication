using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridHeistApplication.DataAccess
{
    public class DatabaseAccessor
    {
        //connection string
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


        public string GetGameStatus(int gameId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetGameStatus", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@GameID", gameId);
                    connection.Open();
                    // ExecuteScalar is perfect for getting a single value back.
                    object result = command.ExecuteScalar();
                    return result.ToString();
                }
            }
        }


        public List<string> GetAllPlayerNames()
        {
            List<string> playerNames = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetAllPlayerNames", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    // Use a SqlDataReader to read multiple rows of results.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            playerNames.Add(reader["PlayerName"].ToString());
                        }
                    }
                }
            }
            return playerNames;
        }


    }
}

