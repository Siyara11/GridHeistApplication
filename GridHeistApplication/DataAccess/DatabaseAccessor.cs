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


        //Validate login for user identification
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


        //Register a new user in the database
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


        //Admin Get ALll Payers
        public DataTable GetAdminPlayerList()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AdminGetAllPlayers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Database error fetching players: " + ex.Message);
                    }
                }
            }
            return dt;
        }

        //Admin add new player
        public int AdminAddPlayer(string username, string password, bool isAdmin)
        {
            int status = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AdminAddPlayer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PlayerName", username);
                    cmd.Parameters.AddWithValue("@PasswordHash", password);
                    cmd.Parameters.AddWithValue("@IsAdmin", isAdmin);
                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null) status = Convert.ToInt32(result);
                    }
                    catch (SqlException ex) { throw new Exception("Database error: " + ex.Message); }
                }
            }
            return status;
        }


        //Update Existing Player
        public int AdminUpdatePlayer(int targetPlayerId, string newUsername, string newPassword, bool unlockAccount, bool isAdmin)
        {
            int status = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AdminUpdatePlayer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TargetPlayerID", targetPlayerId);
                    cmd.Parameters.AddWithValue("@NewPlayerName", newUsername);
                    cmd.Parameters.AddWithValue("@ResetPasswordHash", newPassword); // Pass empty string if no change
                    cmd.Parameters.AddWithValue("@UnlockAccount", unlockAccount);
                    cmd.Parameters.AddWithValue("@IsAdmin", isAdmin);
                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null) status = Convert.ToInt32(result);
                    }
                    catch (SqlException ex) { throw new Exception("Database error: " + ex.Message); }
                }
            }
            return status;
        }

        //Delete Player
        public int AdminDeletePlayer(int targetPlayerId)
        {
            int status = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AdminDeletePlayer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TargetPlayerID", targetPlayerId);
                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null) status = Convert.ToInt32(result);
                    }
                    catch (SqlException ex) { throw new Exception("Database error: " + ex.Message); }
                }
            }
            return status;
        }

        // Kill Running Game
        public int KillRunningGame(int gameId)
        {
            int status = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AdminKillGame", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GameID", gameId);
                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null) status = Convert.ToInt32(result);
                    }
                    catch (SqlException ex) { throw new Exception("Database error: " + ex.Message); }
                }
            }
            return status;
        }

        //game relatd
        //Generate Game Board
        public int GenerateGameBoard(int hostPlayerId)
        {
            int newGameId = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GenerateGameBoard", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@HostPlayerID", hostPlayerId);

                    // Set up the output parameter to capture the new Game ID
                    SqlParameter outputIdParam = new SqlParameter("@NewGameID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputIdParam);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        if (outputIdParam.Value != DBNull.Value)
                        {
                            newGameId = Convert.ToInt32(outputIdParam.Value);
                        }
                    }
                    catch (SqlException ex) { throw new Exception("Database error generating board: " + ex.Message); }
                }
            }
            return newGameId;
        }

        // Place Initial Items
        public bool PlaceInitialItems(int gameId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_PlaceInitialItems", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GameID", gameId);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (SqlException ex) { throw new Exception("Database error placing items: " + ex.Message); }
                }
            }
        }

        //Game board
        public DataTable GetGameBoard(int gameId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetGameBoard", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GameID", gameId);
                    try
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                    catch (SqlException ex) { throw new Exception("Database error fetching board: " + ex.Message); }
                }
            }
            return dt;
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

