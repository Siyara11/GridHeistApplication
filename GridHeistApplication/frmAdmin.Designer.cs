namespace GridHeistApplication
{
    partial class frmAdmin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnKillRunningGame = new System.Windows.Forms.Button();
            this.pnlSelectPlayer = new System.Windows.Forms.Panel();
            this.lstPlayers = new System.Windows.Forms.ListBox();
            this.lblSelectPlayer = new System.Windows.Forms.Label();
            this.pnlPlayerDetails = new System.Windows.Forms.Panel();
            this.chkAccountLocked = new System.Windows.Forms.CheckBox();
            this.btnRemoveSelected = new System.Windows.Forms.Button();
            this.btnUpdateSelected = new System.Windows.Forms.Button();
            this.btnAddNewPlayer = new System.Windows.Forms.Button();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtPlayerID = new System.Windows.Forms.TextBox();
            this.lblPlayerID = new System.Windows.Forms.Label();
            this.lblPlayerDetails = new System.Windows.Forms.Label();
            this.pnlSelectPlayer.SuspendLayout();
            this.pnlPlayerDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(200, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(382, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Administrator Panel";
            // 
            // btnKillRunningGame
            // 
            this.btnKillRunningGame.BackColor = System.Drawing.Color.Red;
            this.btnKillRunningGame.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKillRunningGame.ForeColor = System.Drawing.Color.White;
            this.btnKillRunningGame.Location = new System.Drawing.Point(285, 60);
            this.btnKillRunningGame.Name = "btnKillRunningGame";
            this.btnKillRunningGame.Size = new System.Drawing.Size(210, 35);
            this.btnKillRunningGame.TabIndex = 1;
            this.btnKillRunningGame.Text = "! Kill Running Game !";
            this.btnKillRunningGame.UseVisualStyleBackColor = false;
            this.btnKillRunningGame.Click += new System.EventHandler(this.btnKillRunningGame_Click);
            // 
            // pnlSelectPlayer
            // 
            this.pnlSelectPlayer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSelectPlayer.Controls.Add(this.lstPlayers);
            this.pnlSelectPlayer.Controls.Add(this.lblSelectPlayer);
            this.pnlSelectPlayer.Location = new System.Drawing.Point(30, 115);
            this.pnlSelectPlayer.Name = "pnlSelectPlayer";
            this.pnlSelectPlayer.Size = new System.Drawing.Size(320, 310);
            this.pnlSelectPlayer.TabIndex = 2;
            // 
            // lstPlayers
            // 
            this.lstPlayers.FormattingEnabled = true;
            this.lstPlayers.Location = new System.Drawing.Point(15, 45);
            this.lstPlayers.Name = "lstPlayers";
            this.lstPlayers.Size = new System.Drawing.Size(290, 250);
            this.lstPlayers.TabIndex = 1;
            this.lstPlayers.SelectedIndexChanged += new System.EventHandler(this.lstPlayers_SelectedIndexChanged);
            // 
            // lblSelectPlayer
            // 
            this.lblSelectPlayer.AutoSize = true;
            this.lblSelectPlayer.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectPlayer.Location = new System.Drawing.Point(15, 15);
            this.lblSelectPlayer.Name = "lblSelectPlayer";
            this.lblSelectPlayer.Size = new System.Drawing.Size(157, 16);
            this.lblSelectPlayer.TabIndex = 0;
            this.lblSelectPlayer.Text = "Select a player to manage";
            // 
            // pnlPlayerDetails
            // 
            this.pnlPlayerDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPlayerDetails.Controls.Add(this.chkAccountLocked);
            this.pnlPlayerDetails.Controls.Add(this.btnRemoveSelected);
            this.pnlPlayerDetails.Controls.Add(this.btnUpdateSelected);
            this.pnlPlayerDetails.Controls.Add(this.btnAddNewPlayer);
            this.pnlPlayerDetails.Controls.Add(this.txtUsername);
            this.pnlPlayerDetails.Controls.Add(this.lblUsername);
            this.pnlPlayerDetails.Controls.Add(this.txtPlayerID);
            this.pnlPlayerDetails.Controls.Add(this.lblPlayerID);
            this.pnlPlayerDetails.Controls.Add(this.lblPlayerDetails);
            this.pnlPlayerDetails.Location = new System.Drawing.Point(370, 115);
            this.pnlPlayerDetails.Name = "pnlPlayerDetails";
            this.pnlPlayerDetails.Size = new System.Drawing.Size(400, 310);
            this.pnlPlayerDetails.TabIndex = 3;
            // 
            // chkAccountLocked
            // 
            this.chkAccountLocked.AutoSize = true;
            this.chkAccountLocked.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAccountLocked.Location = new System.Drawing.Point(30, 180);
            this.chkAccountLocked.Name = "chkAccountLocked";
            this.chkAccountLocked.Size = new System.Drawing.Size(112, 20);
            this.chkAccountLocked.TabIndex = 8;
            this.chkAccountLocked.Text = "Account Locked";
            this.chkAccountLocked.UseVisualStyleBackColor = true;
            // 
            // btnRemoveSelected
            // 
            this.btnRemoveSelected.BackColor = System.Drawing.Color.LightCoral;
            this.btnRemoveSelected.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveSelected.Location = new System.Drawing.Point(265, 245);
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.Size = new System.Drawing.Size(120, 40);
            this.btnRemoveSelected.TabIndex = 7;
            this.btnRemoveSelected.Text = "Remove\r\nSelected";
            this.btnRemoveSelected.UseVisualStyleBackColor = false;
            this.btnRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);
            // 
            // btnUpdateSelected
            // 
            this.btnUpdateSelected.BackColor = System.Drawing.Color.LightGreen;
            this.btnUpdateSelected.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateSelected.Location = new System.Drawing.Point(140, 245);
            this.btnUpdateSelected.Name = "btnUpdateSelected";
            this.btnUpdateSelected.Size = new System.Drawing.Size(120, 40);
            this.btnUpdateSelected.TabIndex = 6;
            this.btnUpdateSelected.Text = "Update\r\nSelected";
            this.btnUpdateSelected.UseVisualStyleBackColor = false;
            this.btnUpdateSelected.Click += new System.EventHandler(this.btnUpdateSelected_Click);
            // 
            // btnAddNewPlayer
            // 
            this.btnAddNewPlayer.BackColor = System.Drawing.Color.LightGray;
            this.btnAddNewPlayer.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddNewPlayer.Location = new System.Drawing.Point(15, 245);
            this.btnAddNewPlayer.Name = "btnAddNewPlayer";
            this.btnAddNewPlayer.Size = new System.Drawing.Size(120, 40);
            this.btnAddNewPlayer.TabIndex = 5;
            this.btnAddNewPlayer.Text = "Add New\r\nPlayer";
            this.btnAddNewPlayer.UseVisualStyleBackColor = false;
            this.btnAddNewPlayer.Click += new System.EventHandler(this.btnAddNewPlayer_Click);
            // 
            // txtUsername
            // 
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsername.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.Location = new System.Drawing.Point(30, 130);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(355, 23);
            this.txtUsername.TabIndex = 4;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsername.Location = new System.Drawing.Point(30, 110);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(75, 16);
            this.lblUsername.TabIndex = 3;
            this.lblUsername.Text = "Username:";
            // 
            // txtPlayerID
            // 
            this.txtPlayerID.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.txtPlayerID.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPlayerID.Location = new System.Drawing.Point(30, 70);
            this.txtPlayerID.Name = "txtPlayerID";
            this.txtPlayerID.ReadOnly = true;
            this.txtPlayerID.Size = new System.Drawing.Size(355, 23);
            this.txtPlayerID.TabIndex = 2;
            // 
            // lblPlayerID
            // 
            this.lblPlayerID.AutoSize = true;
            this.lblPlayerID.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayerID.Location = new System.Drawing.Point(30, 50);
            this.lblPlayerID.Name = "lblPlayerID";
            this.lblPlayerID.Size = new System.Drawing.Size(64, 16);
            this.lblPlayerID.TabIndex = 1;
            this.lblPlayerID.Text = "Player ID:";
            // 
            // lblPlayerDetails
            // 
            this.lblPlayerDetails.AutoSize = true;
            this.lblPlayerDetails.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayerDetails.Location = new System.Drawing.Point(30, 15);
            this.lblPlayerDetails.Name = "lblPlayerDetails";
            this.lblPlayerDetails.Size = new System.Drawing.Size(108, 17);
            this.lblPlayerDetails.TabIndex = 0;
            this.lblPlayerDetails.Text = "Player Details";
            // 
            // frmAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnlPlayerDetails);
            this.Controls.Add(this.pnlSelectPlayer);
            this.Controls.Add(this.btnKillRunningGame);
            this.Controls.Add(this.lblTitle);
            this.Name = "frmAdmin";
            this.Text = "Administrator Panel";
            this.pnlSelectPlayer.ResumeLayout(false);
            this.pnlSelectPlayer.PerformLayout();
            this.pnlPlayerDetails.ResumeLayout(false);
            this.pnlPlayerDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnKillRunningGame;
        private System.Windows.Forms.Panel pnlSelectPlayer;
        private System.Windows.Forms.ListBox lstPlayers;
        private System.Windows.Forms.Label lblSelectPlayer;
        private System.Windows.Forms.Panel pnlPlayerDetails;
        private System.Windows.Forms.CheckBox chkAccountLocked;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.Button btnUpdateSelected;
        private System.Windows.Forms.Button btnAddNewPlayer;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtPlayerID;
        private System.Windows.Forms.Label lblPlayerID;
        private System.Windows.Forms.Label lblPlayerDetails;
    }
}