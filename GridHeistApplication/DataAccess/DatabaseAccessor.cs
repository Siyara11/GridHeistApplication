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

        public bool ValidateLogin(string username, string password)
        {
            
            string passwordHash = password; // This is a placeholder for real hashing logic

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_ValidatePlayer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PlayerName", username);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    connection.Open();
                    int result = (int)command.ExecuteScalar();

                    return (result == 1);
                }
            }
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

