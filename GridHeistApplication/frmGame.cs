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

namespace GridHeistApplication
{
    public partial class frmGame : Form
    {
        private int currentGameId = 0;
        private int currentHostPlayerId;
        private string currentHostPlayerName;

        // Update the constructor to accept the player's details
        public frmGame(int loggedInPlayerId, string loggedInPlayerName)
        {
            InitializeComponent();

            // Store the logged-in user's details
            currentHostPlayerId = loggedInPlayerId;
            currentHostPlayerName = loggedInPlayerName;

            // Clean up the dashboard UI
            lblYourPlayerValue.Text = currentHostPlayerName;
            lblYourGemsValue.Text = "0 / 3"; // Reset gems
            lblOpponentNameValue.Text = "Waiting...";
            lblOpponentGemsValue.Text = "0 / 3";

            // Clear the 9 dummy designer buttons to leave a blank panel for our 8x8 grid
            pnlGameBoard.Controls.Clear();

            this.Load += new EventHandler(frmGame_Load);
        }

        private void DrawGameBoard()
        {
            DatabaseAccessor dbAccessor = new DatabaseAccessor();
            DataTable dtBoard = dbAccessor.GetGameBoard(currentGameId);

            // Fetch where the player is currently standing
            int myCurrentTile = dbAccessor.GetPlayerCurrentTile(currentGameId, currentHostPlayerId);

            pnlGameBoard.Controls.Clear();

            int buttonSize = 45;
            int padding = 2;
            int startX = 20;
            int startY = 20;

            foreach (DataRow row in dtBoard.Rows)
            {
                int r = Convert.ToInt32(row["RowNumber"]) - 1;
                int c = Convert.ToInt32(row["ColumnNumber"]) - 1;
                int tileId = Convert.ToInt32(row["TileID"]);

                string tileType = row["TileType"].ToString();
                string itemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : "";

                Button btnTile = new Button();
                btnTile.Width = buttonSize;
                btnTile.Height = buttonSize;
                btnTile.Left = startX + (c * (buttonSize + padding));
                btnTile.Top = startY + (r * (buttonSize + padding));
                btnTile.Tag = tileId;

                // Attach the click event to make the tile interactive
                btnTile.Click += new EventHandler(Tile_Click);

                if (tileType == "Firewall") btnTile.BackColor = System.Drawing.Color.DarkGray;
                else if (tileType == "Home") btnTile.BackColor = System.Drawing.Color.LightBlue;
                else if (tileType == "Exit") btnTile.BackColor = System.Drawing.Color.LightGreen;
                else btnTile.BackColor = System.Drawing.Color.White;

                // Draw the player if they are standing here
                if (tileId == myCurrentTile)
                {
                    btnTile.Text = "P";
                    btnTile.BackColor = System.Drawing.Color.Yellow; // Highlight the player!
                    btnTile.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
                    btnTile.ForeColor = System.Drawing.Color.Black;
                }
                // Otherwise, draw the item if there is one
                else if (!string.IsNullOrEmpty(itemName))
                {
                    if (itemName == "Data Chip") btnTile.Text = "C";
                    else if (itemName.Contains("Fragment")) btnTile.Text = "F";
                    else btnTile.Text = "T";

                    btnTile.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
                    btnTile.ForeColor = System.Drawing.Color.DarkBlue;
                }

                pnlGameBoard.Controls.Add(btnTile);
            }
        }



        private void frmGame_Load(object sender, EventArgs e)
        {
            try
            {
                DatabaseAccessor dbAccessor = new DatabaseAccessor();

                txtActionLog.AppendText($"Welcome {currentHostPlayerName}!\r\n");
                txtActionLog.AppendText("Initializing game board in database...\r\n");

                //Generate the board in the database
                currentGameId = dbAccessor.GenerateGameBoard(currentHostPlayerId);

                if (currentGameId > 0)
                {
                    txtActionLog.AppendText($"Success! Game Session #{currentGameId} created with 64 tiles.\r\n");

                    // Place the initial items in the database
                    bool itemsPlaced = dbAccessor.PlaceInitialItems(currentGameId);
                    if (itemsPlaced)
                    {
                        txtActionLog.AppendText("Initial items placed successfully.\r\n");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to generate the game board.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Draw the visual grid on the screen based on the database
                DrawGameBoard();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Click Tile
        private void Tile_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null && clickedButton.Tag != null)
            {
                int targetTileId = Convert.ToInt32(clickedButton.Tag);

                DatabaseAccessor dbAccessor = new DatabaseAccessor();
                int moveStatus = dbAccessor.MovePlayer(currentGameId, currentHostPlayerId, targetTileId);

                if (moveStatus == 1)
                {
                    txtActionLog.AppendText("Moved successfully!\r\n");

                    // Redraw the board and update the score label
                    DrawGameBoard();
                    int currentScore = dbAccessor.GetPlayerScore(currentGameId, currentHostPlayerId);
                    lblYourGemsValue.Text = currentScore.ToString() + " / 30";
                }
                else if (moveStatus == 2)
                {
                    txtActionLog.AppendText(">>> MOVED AND ACQUIRED ITEM! +10 Points! <<<\r\n");

                    // Redraw the board and update the score label
                    DrawGameBoard();
                    int currentScore = dbAccessor.GetPlayerScore(currentGameId, currentHostPlayerId);
                    lblYourGemsValue.Text = currentScore.ToString() + " / 30";
                }
                else if s(moveStatus == -1)
                {
                    txtActionLog.AppendText("Move blocked: Tile is occupied by another player.\r\n");
                }
                else
                {
                    txtActionLog.AppendText("Invalid move: You can only move 1 space to a walkable floor.\r\n");
                }

                // scroll the action log to the bottom automatically
                txtActionLog.SelectionStart = txtActionLog.Text.Length;
                txtActionLog.ScrollToCaret();
            }
        }



        private void btnQuitGame_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to quit the game?", "Quit Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // In the future, this will call a DAO method to mark the player as "Left"
                this.Close();

                // Show the login screen again when closing the game
                Application.OpenForms["frmLogin"].Show();
            }
        }
    }
}
