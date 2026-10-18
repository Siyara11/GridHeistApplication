using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridHeistApplication.DataAccess
{
    internal class GameDAO
    {

        private readonly string connectionString = "Server=SIYARA\\MSSQLSERVER01;Database=GridHeistDB;Integrated Security=True;";

        //generate game Board
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

        //  Place Initial Items
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

        // Get the player's current tile ID to draw them on the board
        public int GetPlayerCurrentTile(int gameId, int playerId)
        {
            int tileId = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT CurrentTileID FROM GamePlayers WHERE GameID = @GameID AND PlayerID = @PlayerID", conn))
                {
                    cmd.Parameters.AddWithValue("@GameID", gameId);
                    cmd.Parameters.AddWithValue("@PlayerID", playerId);
                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null) tileId = Convert.ToInt32(result);
                    }
                    catch (Exception) { /* Ignore for prototype */ }
                }
            }
            return tileId;
        }

        //Move Player
        public int MovePlayer(int gameId, int playerId, int targetTileId)
        {
            int status = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_MovePlayer", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GameID", gameId);
                    cmd.Parameters.AddWithValue("@PlayerID", playerId);
                    cmd.Parameters.AddWithValue("@TargetTileID", targetTileId);
                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null) status = Convert.ToInt32(result);
                    }
                    catch (SqlException ex) { throw new Exception("Database error moving: " + ex.Message); }
                }
            }
            return status;
        }

        public int GetPlayerScore(int gameId, int playerId)
        {
            int score = 0;
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                string query = "SELECT Score FROM GamePlayers WHERE GameID = @GameID AND PlayerID = @PlayerID";
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GameID", gameId);
                    cmd.Parameters.AddWithValue("@PlayerID", playerId);
                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            score = Convert.ToInt32(result);
                        }
                    }
                    catch (Exception) { /* ignore */ }
                }
            }
            return score;
        }

        //NPC
        //  Move Item / NPC
        public void MoveNPC(int gameId)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("sp_MoveNPC", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GameID", gameId);

                    conn.Open();
                    cmd.ExecuteNonQuery(); // We removed the silent catch here!
                }
            }
        }

        // Add this method for the Game Form
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


        public bool CheckPlayerWinner(int gameId, int playerId)
        {
            bool isWinner = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT IsWinner FROM GamePlayers WHERE GameID = @GameID AND PlayerID = @PlayerID", conn))
                {
                    cmd.Parameters.AddWithValue("@GameID", gameId);
                    cmd.Parameters.AddWithValue("@PlayerID", playerId);
                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            isWinner = Convert.ToBoolean(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Database error checking winner status: " + ex.Message);
                    }
                }
            }
            return isWinner;
        }


        public DataTable GetGameResults(int gameId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT PlayerID, CurrentTileID, Score, PlayerStatus, IsWinner FROM GamePlayers WHERE GameID = @GameID ORDER BY Score DESC",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@GameID", gameId);
                    try
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Database error retrieving game results: " + ex.Message);
                    }
                }
            }
            return dt;
        }


        public DataTable GetGameWinner(int gameId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT GamePlayerID, PlayerID, Score FROM GamePlayers WHERE GameID = @GameID AND IsWinner = 1",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@GameID", gameId);
                    try
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Database error retrieving winner: " + ex.Message);
                    }
                }
            }
            return dt;
        }
    }
}
