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
                // Clear any existing items in the list box
                lstPlayers.Items.Clear();

                // Get the list of player names from the database
                List<string> playerNames = dbAccessor.GetAllPlayerNames();

                // Add each name from the list to the list box
                foreach (string name in playerNames)
                {
                    lstPlayers.Items.Add(name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error");
            }
        }

        private void lstPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPlayers.SelectedIndex >= 0)
            {
                string selectedPlayer = lstPlayers.SelectedItem.ToString();
                // Populate player details based on selection
                // This would typically load data from the database
                txtPlayerID.Text = ""; // Load from database
                txtUsername.Text = selectedPlayer;
                chkAccountLocked.Checked = false; // Load from database
            }
        }

        private void btnAddNewPlayer_Click(object sender, EventArgs e)
        {
            // Add new player logic
            MessageBox.Show("Add New Player functionality would go here.", "Add Player");
        }

        private void btnUpdateSelected_Click(object sender, EventArgs e)
        {
            if (lstPlayers.SelectedIndex >= 0)
            {
                // Update selected player logic
                MessageBox.Show("Update Player functionality would go here.", "Update Player");
            }
            else
            {
                MessageBox.Show("Please select a player first.", "No Selection");
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
