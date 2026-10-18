using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridHeistApplication.DataAccess
{
    internal class AdminDAO
    {

        private readonly string connectionString = "Server=SIYARA\\MSSQLSERVER01;Database=GridHeistDB;Integrated Security=True;";




        //Admin Get ALl Payers
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

        // 1. Kill Running Game
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

        // 2. Add New Player (Admin)
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

        // 3. Update Existing Player
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

        // 4. Delete Player
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



    }
}
