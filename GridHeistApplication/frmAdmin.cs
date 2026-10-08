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
            DatabaseAccessor dbAccessor = new DatabaseAccessor();
            try
            {
                // Fetch the table of players
                DataTable dtPlayers = dbAccessor.GetAdminPlayerList();

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
            if (lstPlayers.SelectedIndex >= 0 && lstPlayers.SelectedItem is DataRowView)
            {
                DataRowView selectedRow = (DataRowView)lstPlayers.SelectedItem;

                // Populate the UI text boxes using the bound database columns
                txtPlayerID.Text = selectedRow["PlayerID"].ToString();
                txtUsername.Text = selectedRow["PlayerName"].ToString();
                chkAccountLocked.Checked = Convert.ToBoolean(selectedRow["IsLockedOut"]);
            }
        }

        //Admin add new player to the game 
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

            DatabaseAccessor dbAccessor = new DatabaseAccessor();
            int status = dbAccessor.AdminAddPlayer(username, "Temp#123", false);

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

                bool unlockAccount = !chkAccountLocked.Checked;

                DatabaseAccessor dbAccessor = new DatabaseAccessor();

                // Passing an empty string for the password means we aren't changing it
                int status = dbAccessor.AdminUpdatePlayer(playerId, newUsername, "", unlockAccount, false);

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
            if (lstPlayers.SelectedIndex >= 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to remove this player?", "Remove Player", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Remove selected player logic
                    MessageBox.Show("Remove Player functionality would go here.", "Remove Player");
                }
            }
            else
            {
                MessageBox.Show("Please select a player first.", "No Selection");
            }
        }

        private void btnKillRunningGame_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to kill the running game? This will disconnect all players.", "Kill Running Game", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                // Kill running game logic
                MessageBox.Show("Running game has been terminated.", "Game Terminated");
            }
        }

        private void btnLoadPlayers_Click(object sender, EventArgs e)
        {
            LoadPlayers();
        }
    }
}
