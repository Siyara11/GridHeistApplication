using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GridHeistApplication.DataAccess;
using System.Collections.Generic;

namespace GridHeistApplication
{
    public partial class frmAdmin : Form
    {
        public frmAdmin()
        {
            InitializeComponent();
            LoadPlayers();
        }

        private void LoadPlayers()
        {
            AdminDAO adminDAO = new AdminDAO();
            try
            {
                // Fetch the table of players
                DataTable dtPlayers = adminDAO.GetAdminPlayerList();

                // Bind the data to the list box
                lstPlayers.DataSource = dtPlayers;
                lstPlayers.DisplayMember = "PlayerName";
                lstPlayers.ValueMember = "PlayerID";

                // Clear the text boxes if no players exist
                if (dtPlayers.Rows.Count == 0)
                {
                    txtPlayerID.Text = "";
                    txtUsername.Text = "";
                    chkAccountLocked.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lstPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ensure an item is actually selected and is a DataRowView
            if (lstPlayers.SelectedIndex >= 0 && lstPlayers.SelectedItem is DataRowView)
            {
                DataRowView selectedRow = (DataRowView)lstPlayers.SelectedItem;

                // Populate the UI text boxes using the bound database columns
                txtPlayerID.Text = selectedRow["PlayerID"].ToString();
                txtUsername.Text = selectedRow["PlayerName"].ToString();
                chkAccountLocked.Checked = Convert.ToBoolean(selectedRow["IsLockedOut"]);
            }
        }

        private void btnAddNewPlayer_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPlayerID.Text))
            {
                txtPlayerID.Text = "";
                txtUsername.Text = "";
                chkAccountLocked.Checked = false;

                // Deselect whatever is highlighted in the list box
                lstPlayers.ClearSelected();

                MessageBox.Show("Form cleared and ready. Type the new username and click 'Add New Player' again to save.",
                                "Ready for New Player", MessageBoxButtons.OK, MessageBoxIcon.Information);


                txtUsername.Focus();

                return;
            }

            string username = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username to add.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AdminDAO adminDAO = new AdminDAO();
            int status = adminDAO.AdminAddPlayer(username, "Temp#123", false);

            if (status == 1)
            {
                MessageBox.Show($"Player '{username}' added successfully with default password 'Temp#123'.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadPlayers(); // Refresh the list so the new player appears
            }
            else
            {
                MessageBox.Show("That username is already taken.", "Add Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdateSelected_Click(object sender, EventArgs e)
        {
            if (lstPlayers.SelectedIndex >= 0 && !string.IsNullOrEmpty(txtPlayerID.Text))
            {
                int playerId = Convert.ToInt32(txtPlayerID.Text);
                string newUsername = txtUsername.Text.Trim();

                // If the checkbox is UNCHECKED, it means the admin wants to unlock the account
                bool unlockAccount = !chkAccountLocked.Checked;

                AdminDAO adminDAO = new AdminDAO();

                // Passing an empty string for the password means we aren't changing it
                int status = adminDAO.AdminUpdatePlayer(playerId, newUsername, "", unlockAccount, false);

                if (status == 1)
                {
                    MessageBox.Show("Player updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPlayers();
                }
                else
                {
                    MessageBox.Show("Update failed. The new username may already be in use.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a player first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            if (lstPlayers.SelectedIndex >= 0 && !string.IsNullOrEmpty(txtPlayerID.Text))
            {
                int playerId = Convert.ToInt32(txtPlayerID.Text);
                string username = txtUsername.Text;

                DialogResult result = MessageBox.Show($"Are you sure you want to permanently delete {username}? This will wipe their inventory and game history.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    AdminDAO adminDAO = new AdminDAO();
                    int status = adminDAO.AdminDeletePlayer(playerId);

                    if (status == 1)
                    {
                        MessageBox.Show("Player deleted successfully.", "Removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPlayers();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete player.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a player first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnKillRunningGame_Click(object sender, EventArgs e)
        {
            try
            {
                AdminDAO adminDAO = new AdminDAO();
                DataTable activeGames = adminDAO.GetActiveGames();

                if (activeGames.Rows.Count == 0)
                {
                    MessageBox.Show("No active games found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Build a list of active games to show the admin
                string gameList = "Active Games:\n\n";
                foreach (DataRow row in activeGames.Rows)
                {
                    gameList += $"Game #{row["GameID"]} - Status: {row["GameStatus"]}\n";
                }
                gameList += "\nThe most recent game will be terminated.";

                DialogResult result = MessageBox.Show(gameList, "Kill Running Game", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    // Kill the most recent active game
                    int latestGameId = Convert.ToInt32(activeGames.Rows[0]["GameID"]);
                    int status = adminDAO.KillRunningGame(latestGameId);

                    if (status == 1)
                        MessageBox.Show($"Game #{latestGameId} terminated successfully.", "Game Terminated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Failed to terminate game.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoadPlayers_Click(object sender, EventArgs e)
        {
            LoadPlayers();
        }
    }
}


