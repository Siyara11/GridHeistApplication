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

        private Timer npcTimer;

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
            GameDAO gameDAO = new GameDAO();
            DataTable dtBoard = gameDAO.GetGameBoard(currentGameId);

            // Fetch where the player is currently standing
            int myCurrentTile = gameDAO.GetPlayerCurrentTile(currentGameId, currentHostPlayerId);

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

                // Attach the click event to make the tile interactive!
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

            // Update opponent info
            try
            {
                DataTable otherPlayers = gameDAO.GetOtherPlayers(currentGameId, currentHostPlayerId);
                if (otherPlayers.Rows.Count > 0)
                {

                    string opponentName = otherPlayers.Rows[0][1].ToString();
                    lblOpponentNameValue.Text = opponentName;

                    if (otherPlayers.Columns.Count > 2)
                    {
                        lblOpponentGemsValue.Text = otherPlayers.Rows[0][2].ToString() + " / 30";
                    }
                }
                else
                {
                    lblOpponentNameValue.Text = "Waiting...";
                    lblOpponentGemsValue.Text = "0 / 3";
                }
            }
            catch (Exception ex)
            {
                txtActionLog.AppendText("Opponent error: " + ex.Message + "\r\n");
            }
        }



        private void frmGame_Load(object sender, EventArgs e)
        {
            try
            {
                GameDAO gameDAO = new GameDAO();
                txtActionLog.AppendText($"Welcome {currentHostPlayerName}!\r\n");

                // Check if there is an available game to join first
                DataTable availableGames = gameDAO.GetAvailableGames();

                if (availableGames.Rows.Count > 0)
                {
                    // Join the first available game
                    int existingGameId = Convert.ToInt32(availableGames.Rows[0]["GameID"]);
                    int joinResult = gameDAO.JoinGame(existingGameId, currentHostPlayerId);

                    if (joinResult == 1)
                    {
                        currentGameId = existingGameId;
                        txtActionLog.AppendText($"Joined existing Game Session #{currentGameId}!\r\n");
                    }
                    else
                    {
                        // Join failed, create new game
                        CreateNewGame(gameDAO);
                    }
                }
                else
                {
                    // No available games, create a new one
                    CreateNewGame(gameDAO);
                }

                // Draw the visual grid on the screen based on the database
                DrawGameBoard();
                npcTimer = new Timer();
                npcTimer.Interval = 3000;
                npcTimer.Tick += new EventHandler(NpcTimer_Tick);
                npcTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateNewGame(GameDAO gameDAO)
        {
            txtActionLog.AppendText("Initializing game board in database...\r\n");
            currentGameId = gameDAO.GenerateGameBoard(currentHostPlayerId);

            if (currentGameId > 0)
            {
                txtActionLog.AppendText($"Success! Game Session #{currentGameId} created with 64 tiles.\r\n");

                bool itemsPlaced = gameDAO.PlaceInitialItems(currentGameId);
                if (itemsPlaced)
                {
                    txtActionLog.AppendText("Initial items placed successfully.\r\n");
                }
            }
            else
            {
                MessageBox.Show("Failed to generate the game board.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //Click Tile
        private void Tile_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null && clickedButton.Tag != null)
            {
                int targetTileId = Convert.ToInt32(clickedButton.Tag);

                GameDAO gameDAO = new GameDAO();
                int moveStatus = gameDAO.MovePlayer(currentGameId, currentHostPlayerId, targetTileId);

                if (moveStatus == 3) // Game Won!
                {
                    txtActionLog.AppendText(">>> YOU ESCAPED! GAME WON! +50 Points! <<<\r\n");
                    DrawGameBoard();
                    int currentScore = gameDAO.GetPlayerScore(currentGameId, currentHostPlayerId);
                    lblYourGemsValue.Text = currentScore.ToString() + " / 30";

                    // Stop the game timer and show victory
                    npcTimer.Stop();
                    MessageBox.Show("Congratulations! You successfully escaped the Blackout Protocol and collected all fragments!",
                                    "VICTORY", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ShowVictoryScreen(gameDAO, currentScore);
                }
                else if (moveStatus == -2) // Not enough fragments to exit
                {
                    txtActionLog.AppendText("Access Denied: You need at least 3 fragments to use the Exit Portal.\r\n");
                }
                else if (moveStatus == 1)
                {
                    txtActionLog.AppendText("Moved successfully!\r\n");

                    // Redraw the board and update the score label
                    DrawGameBoard();
                    int currentScore = gameDAO.GetPlayerScore(currentGameId, currentHostPlayerId);
                    lblYourGemsValue.Text = currentScore.ToString() + " / 30";
                }
                else if (moveStatus == 2)
                {
                    txtActionLog.AppendText(">>> MOVED AND ACQUIRED ITEM! +10 Points! <<<\r\n");

                    // Redraw the board and update the score label
                    DrawGameBoard();
                    int currentScore = gameDAO.GetPlayerScore(currentGameId, currentHostPlayerId);
                    lblYourGemsValue.Text = currentScore.ToString() + " / 30";
                }
                else if (moveStatus == -1)
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



        /// <summary>
        /// Display victory screen with final game results
        /// </summary>
        private void ShowVictoryScreen(GameDAO gameDAO, int finalScore)
        {
            try
            {
                DataTable results = gameDAO.GetGameResults(currentGameId);

                string resultMessage = "=== GAME RESULTS ===\r\n\r\n";
                resultMessage += $"Your Final Score: {finalScore} points\r\n\r\n";
                resultMessage += "All Players:\r\n";

                if (results.Rows.Count > 0)
                {
                    int rank = 1;
                    foreach (DataRow row in results.Rows)
                    {
                        int score = Convert.ToInt32(row["Score"]);
                        bool isWinner = Convert.ToBoolean(row["IsWinner"]);
                        string status = row["PlayerStatus"].ToString();
                        string winnerTag = isWinner ? " [WINNER]" : "";

                        resultMessage += $"{rank}. Score: {score} | Status: {status}{winnerTag}\r\n";
                        rank++;
                    }
                }

                MessageBox.Show(resultMessage, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Close the game form and return to login/lobby
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving game results: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //NPC
        private void NpcTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                GameDAO gameDAO = new GameDAO();
                gameDAO.MoveNPC(currentGameId);

                // Redraw the board to show the movement
                DrawGameBoard();
            }
            catch (Exception ex)
            {
                // If the database blocks the move, print the exact error to the screen!
                txtActionLog.AppendText("NPC Move Error: " + ex.Message + "\r\n");

                // Scroll to the bottom of the log
                txtActionLog.SelectionStart = txtActionLog.Text.Length;
                txtActionLog.ScrollToCaret();
            }
        }





        private void btnQuitGame_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to quit the game?", "Quit Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                npcTimer.Stop();
                this.Close();

                // Show the login screen again safely
                Form loginForm = Application.OpenForms["frmLogin"];
                if (loginForm != null)
                {
                    loginForm.Show();
                }
                else
                {
                    Application.Run(new frmLogin());
                }
            }
        }
    }
}


