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
