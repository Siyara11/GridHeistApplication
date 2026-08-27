namespace GridHeistApplication
{
    partial class frmGame
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
            this.pnlYourStatus = new System.Windows.Forms.Panel();
            this.lblYourGemsValue = new System.Windows.Forms.Label();
            this.lblYourGemsLabel = new System.Windows.Forms.Label();
            this.lblYourPlayerValue = new System.Windows.Forms.Label();
            this.lblYourPlayerLabel = new System.Windows.Forms.Label();
            this.lblYourStatusTitle = new System.Windows.Forms.Label();
            this.pnlOpponentStatus = new System.Windows.Forms.Panel();
            this.lblOpponentGemsValue = new System.Windows.Forms.Label();
            this.lblOpponentGemsLabel = new System.Windows.Forms.Label();
            this.lblOpponentNameValue = new System.Windows.Forms.Label();
            this.lblOpponentNameLabel = new System.Windows.Forms.Label();
            this.lblOpponentStatusTitle = new System.Windows.Forms.Label();
            this.pnlGameBoard = new System.Windows.Forms.Panel();
            this.lblClickTile = new System.Windows.Forms.Label();
            this.btn33 = new System.Windows.Forms.Button();
            this.btn32 = new System.Windows.Forms.Button();
            this.btn31 = new System.Windows.Forms.Button();
            this.btn23 = new System.Windows.Forms.Button();
            this.btn22 = new System.Windows.Forms.Button();
            this.btn21 = new System.Windows.Forms.Button();
            this.btn13 = new System.Windows.Forms.Button();
            this.btn12 = new System.Windows.Forms.Button();
            this.btn11 = new System.Windows.Forms.Button();
            this.pnlActionLog = new System.Windows.Forms.Panel();
            this.txtActionLog = new System.Windows.Forms.TextBox();
            this.lblActionLogTitle = new System.Windows.Forms.Label();
            this.btnQuitGame = new System.Windows.Forms.Button();
            this.pnlYourStatus.SuspendLayout();
            this.pnlOpponentStatus.SuspendLayout();
            this.pnlGameBoard.SuspendLayout();
            this.pnlActionLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlYourStatus
            // 
            this.pnlYourStatus.BackColor = System.Drawing.Color.LightBlue;
            this.pnlYourStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlYourStatus.Controls.Add(this.lblYourGemsValue);
            this.pnlYourStatus.Controls.Add(this.lblYourGemsLabel);
            this.pnlYourStatus.Controls.Add(this.lblYourPlayerValue);
            this.pnlYourStatus.Controls.Add(this.lblYourPlayerLabel);
            this.pnlYourStatus.Controls.Add(this.lblYourStatusTitle);
            this.pnlYourStatus.Location = new System.Drawing.Point(35, 20);
            this.pnlYourStatus.Name = "pnlYourStatus";
            this.pnlYourStatus.Size = new System.Drawing.Size(330, 120);
            this.pnlYourStatus.TabIndex = 0;
            // 
            // lblYourGemsValue
            // 
            this.lblYourGemsValue.AutoSize = true;
            this.lblYourGemsValue.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYourGemsValue.Location = new System.Drawing.Point(155, 80);
            this.lblYourGemsValue.Name = "lblYourGemsValue";
            this.lblYourGemsValue.Size = new System.Drawing.Size(33, 16);
            this.lblYourGemsValue.TabIndex = 4;
            this.lblYourGemsValue.Text = "3 / 5";
            // 
            // lblYourGemsLabel
            // 
            this.lblYourGemsLabel.AutoSize = true;
            this.lblYourGemsLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYourGemsLabel.Location = new System.Drawing.Point(50, 80);
            this.lblYourGemsLabel.Name = "lblYourGemsLabel";
            this.lblYourGemsLabel.Size = new System.Drawing.Size(50, 16);
            this.lblYourGemsLabel.TabIndex = 3;
            this.lblYourGemsLabel.Text = "Gems: ";
            // 
            // lblYourPlayerValue
            // 
            this.lblYourPlayerValue.AutoSize = true;
            this.lblYourPlayerValue.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYourPlayerValue.Location = new System.Drawing.Point(155, 55);
            this.lblYourPlayerValue.Name = "lblYourPlayerValue";
            this.lblYourPlayerValue.Size = new System.Drawing.Size(88, 16);
            this.lblYourPlayerValue.TabIndex = 2;
            this.lblYourPlayerValue.Text = "[Player Name]";
            // 
            // lblYourPlayerLabel
            // 
            this.lblYourPlayerLabel.AutoSize = true;
            this.lblYourPlayerLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYourPlayerLabel.Location = new System.Drawing.Point(50, 55);
            this.lblYourPlayerLabel.Name = "lblYourPlayerLabel";
            this.lblYourPlayerLabel.Size = new System.Drawing.Size(50, 16);
            this.lblYourPlayerLabel.TabIndex = 1;
            this.lblYourPlayerLabel.Text = "Player: ";
            // 
            // lblYourStatusTitle
            // 
            this.lblYourStatusTitle.AutoSize = true;
            this.lblYourStatusTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYourStatusTitle.Location = new System.Drawing.Point(50, 20);
            this.lblYourStatusTitle.Name = "lblYourStatusTitle";
            this.lblYourStatusTitle.Size = new System.Drawing.Size(93, 19);
            this.lblYourStatusTitle.TabIndex = 0;
            this.lblYourStatusTitle.Text = "Your Status";
            // 
            // pnlOpponentStatus
            // 
            this.pnlOpponentStatus.BackColor = System.Drawing.Color.LightCoral;
            this.pnlOpponentStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlOpponentStatus.Controls.Add(this.lblOpponentGemsValue);
            this.pnlOpponentStatus.Controls.Add(this.lblOpponentGemsLabel);
            this.pnlOpponentStatus.Controls.Add(this.lblOpponentNameValue);
            this.pnlOpponentStatus.Controls.Add(this.lblOpponentNameLabel);
            this.pnlOpponentStatus.Controls.Add(this.lblOpponentStatusTitle);
            this.pnlOpponentStatus.Location = new System.Drawing.Point(435, 20);
            this.pnlOpponentStatus.Name = "pnlOpponentStatus";
            this.pnlOpponentStatus.Size = new System.Drawing.Size(330, 120);
            this.pnlOpponentStatus.TabIndex = 1;
            // 
            // lblOpponentGemsValue
            // 
            this.lblOpponentGemsValue.AutoSize = true;
            this.lblOpponentGemsValue.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOpponentGemsValue.Location = new System.Drawing.Point(155, 80);
            this.lblOpponentGemsValue.Name = "lblOpponentGemsValue";
            this.lblOpponentGemsValue.Size = new System.Drawing.Size(33, 16);
            this.lblOpponentGemsValue.TabIndex = 4;
            this.lblOpponentGemsValue.Text = "2 / 5";
            // 
            // lblOpponentGemsLabel
            // 
            this.lblOpponentGemsLabel.AutoSize = true;
            this.lblOpponentGemsLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOpponentGemsLabel.Location = new System.Drawing.Point(50, 80);
            this.lblOpponentGemsLabel.Name = "lblOpponentGemsLabel";
            this.lblOpponentGemsLabel.Size = new System.Drawing.Size(50, 16);
            this.lblOpponentGemsLabel.TabIndex = 3;
            this.lblOpponentGemsLabel.Text = "Gems: ";
            // 
            // lblOpponentNameValue
            // 
            this.lblOpponentNameValue.AutoSize = true;
            this.lblOpponentNameValue.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOpponentNameValue.Location = new System.Drawing.Point(155, 55);
            this.lblOpponentNameValue.Name = "lblOpponentNameValue";
            this.lblOpponentNameValue.Size = new System.Drawing.Size(100, 16);
            this.lblOpponentNameValue.TabIndex = 2;
            this.lblOpponentNameValue.Text = "[Opponent Name]";
            // 
            // lblOpponentNameLabel
            // 
            this.lblOpponentNameLabel.AutoSize = true;
            this.lblOpponentNameLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOpponentNameLabel.Location = new System.Drawing.Point(50, 55);
            this.lblOpponentNameLabel.Name = "lblOpponentNameLabel";
            this.lblOpponentNameLabel.Size = new System.Drawing.Size(74, 16);
            this.lblOpponentNameLabel.TabIndex = 1;
            this.lblOpponentNameLabel.Text = "Opponent: ";
            // 
            // lblOpponentStatusTitle
            // 
            this.lblOpponentStatusTitle.AutoSize = true;
            this.lblOpponentStatusTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOpponentStatusTitle.Location = new System.Drawing.Point(50, 20);
            this.lblOpponentStatusTitle.Name = "lblOpponentStatusTitle";
            this.lblOpponentStatusTitle.Size = new System.Drawing.Size(124, 19);
            this.lblOpponentStatusTitle.TabIndex = 0;
            this.lblOpponentStatusTitle.Text = "Opponent\'s Status";
            // 
            // pnlGameBoard
            // 
            this.pnlGameBoard.BackColor = System.Drawing.Color.White;
            this.pnlGameBoard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGameBoard.Controls.Add(this.lblClickTile);
            this.pnlGameBoard.Controls.Add(this.btn33);
            this.pnlGameBoard.Controls.Add(this.btn32);
            this.pnlGameBoard.Controls.Add(this.btn31);
            this.pnlGameBoard.Controls.Add(this.btn23);
            this.pnlGameBoard.Controls.Add(this.btn22);
            this.pnlGameBoard.Controls.Add(this.btn21);
            this.pnlGameBoard.Controls.Add(this.btn13);
            this.pnlGameBoard.Controls.Add(this.btn12);
            this.pnlGameBoard.Controls.Add(this.btn11);
            this.pnlGameBoard.Location = new System.Drawing.Point(35, 160);
            this.pnlGameBoard.Name = "pnlGameBoard";
            this.pnlGameBoard.Size = new System.Drawing.Size(550, 280);
            this.pnlGameBoard.TabIndex = 2;
            // 
            // lblClickTile
            // 
            this.lblClickTile.AutoSize = true;
            this.lblClickTile.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClickTile.Location = new System.Drawing.Point(170, 20);
            this.lblClickTile.Name = "lblClickTile";
            this.lblClickTile.Size = new System.Drawing.Size(140, 17);
            this.lblClickTile.TabIndex = 9;
            this.lblClickTile.Text = "Click a tile to move";
            // 
            // btn33
            // 
            this.btn33.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn33.Location = new System.Drawing.Point(370, 210);
            this.btn33.Name = "btn33";
            this.btn33.Size = new System.Drawing.Size(80, 50);
            this.btn33.TabIndex = 8;
            this.btn33.Text = "(G)";
            this.btn33.UseVisualStyleBackColor = true;
            // 
            // btn32
            // 
            this.btn32.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn32.Location = new System.Drawing.Point(220, 210);
            this.btn32.Name = "btn32";
            this.btn32.Size = new System.Drawing.Size(80, 50);
            this.btn32.TabIndex = 7;
            this.btn32.Text = "()";
            this.btn32.UseVisualStyleBackColor = true;
            // 
            // btn31
            // 
            this.btn31.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn31.Location = new System.Drawing.Point(70, 210);
            this.btn31.Name = "btn31";
            this.btn31.Size = new System.Drawing.Size(80, 50);
            this.btn31.TabIndex = 6;
            this.btn31.Text = "(X)";
            this.btn31.UseVisualStyleBackColor = true;
            // 
            // btn23
            // 
            this.btn23.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn23.Location = new System.Drawing.Point(370, 130);
            this.btn23.Name = "btn23";
            this.btn23.Size = new System.Drawing.Size(80, 50);
            this.btn23.TabIndex = 5;
            this.btn23.Text = "()";
            this.btn23.UseVisualStyleBackColor = true;
            // 
            // btn22
            // 
            this.btn22.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn22.Location = new System.Drawing.Point(220, 130);
            this.btn22.Name = "btn22";
            this.btn22.Size = new System.Drawing.Size(80, 50);
            this.btn22.TabIndex = 4;
            this.btn22.Text = "(P2)";
            this.btn22.UseVisualStyleBackColor = true;
            // 
            // btn21
            // 
            this.btn21.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn21.Location = new System.Drawing.Point(70, 130);
            this.btn21.Name = "btn21";
            this.btn21.Size = new System.Drawing.Size(80, 50);
            this.btn21.TabIndex = 3;
            this.btn21.Text = "()";
            this.btn21.UseVisualStyleBackColor = true;
            // 
            // btn13
            // 
            this.btn13.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn13.Location = new System.Drawing.Point(370, 50);
            this.btn13.Name = "btn13";
            this.btn13.Size = new System.Drawing.Size(80, 50);
            this.btn13.TabIndex = 2;
            this.btn13.Text = "(G)";
            this.btn13.UseVisualStyleBackColor = true;
            // 
            // btn12
            // 
            this.btn12.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn12.Location = new System.Drawing.Point(220, 50);
            this.btn12.Name = "btn12";
            this.btn12.Size = new System.Drawing.Size(80, 50);
            this.btn12.TabIndex = 1;
            this.btn12.Text = "(B)";
            this.btn12.UseVisualStyleBackColor = true;
            // 
            // btn11
            // 
            this.btn11.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn11.Location = new System.Drawing.Point(70, 50);
            this.btn11.Name = "btn11";
            this.btn11.Size = new System.Drawing.Size(80, 50);
            this.btn11.TabIndex = 0;
            this.btn11.Text = "(P1)";
            this.btn11.UseVisualStyleBackColor = true;
            // 
            // pnlActionLog
            // 
            this.pnlActionLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlActionLog.Controls.Add(this.txtActionLog);
            this.pnlActionLog.Controls.Add(this.lblActionLogTitle);
            this.pnlActionLog.Location = new System.Drawing.Point(35, 455);
            this.pnlActionLog.Name = "pnlActionLog";
            this.pnlActionLog.Size = new System.Drawing.Size(550, 140);
            this.pnlActionLog.TabIndex = 3;
            // 
            // txtActionLog
            // 
            this.txtActionLog.BackColor = System.Drawing.Color.White;
            this.txtActionLog.Location = new System.Drawing.Point(15, 35);
            this.txtActionLog.Multiline = true;
            this.txtActionLog.Name = "txtActionLog";
            this.txtActionLog.ReadOnly = true;
            this.txtActionLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtActionLog.Size = new System.Drawing.Size(520, 90);
            this.txtActionLog.TabIndex = 1;
            // 
            // lblActionLogTitle
            // 
            this.lblActionLogTitle.AutoSize = true;
            this.lblActionLogTitle.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActionLogTitle.Location = new System.Drawing.Point(15, 10);
            this.lblActionLogTitle.Name = "lblActionLogTitle";
            this.lblActionLogTitle.Size = new System.Drawing.Size(75, 16);
            this.lblActionLogTitle.TabIndex = 0;
            this.lblActionLogTitle.Text = "Action Log";
            // 
            // btnQuitGame
            // 
            this.btnQuitGame.BackColor = System.Drawing.Color.LightCoral;
            this.btnQuitGame.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuitGame.Location = new System.Drawing.Point(635, 455);
            this.btnQuitGame.Name = "btnQuitGame";
            this.btnQuitGame.Size = new System.Drawing.Size(130, 140);
            this.btnQuitGame.TabIndex = 4;
            this.btnQuitGame.Text = "Quit Game";
            this.btnQuitGame.UseVisualStyleBackColor = false;
            this.btnQuitGame.Click += new System.EventHandler(this.btnQuitGame_Click);
            // 
            // frmGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 620);
            this.Controls.Add(this.btnQuitGame);
            this.Controls.Add(this.pnlActionLog);
            this.Controls.Add(this.pnlGameBoard);
            this.Controls.Add(this.pnlOpponentStatus);
            this.Controls.Add(this.pnlYourStatus);
            this.Name = "frmGame";
            this.Text = "Grid Heist - Game";
            this.pnlYourStatus.ResumeLayout(false);
            this.pnlYourStatus.PerformLayout();
            this.pnlOpponentStatus.ResumeLayout(false);
            this.pnlOpponentStatus.PerformLayout();
            this.pnlGameBoard.ResumeLayout(false);
            this.pnlGameBoard.ResumeLayout(false);
            this.pnlActionLog.ResumeLayout(false);
            this.pnlActionLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlYourStatus;
        private System.Windows.Forms.Label lblYourGemsValue;
        private System.Windows.Forms.Label lblYourGemsLabel;
        private System.Windows.Forms.Label lblYourPlayerValue;
        private System.Windows.Forms.Label lblYourPlayerLabel;
        private System.Windows.Forms.Label lblYourStatusTitle;
        private System.Windows.Forms.Panel pnlOpponentStatus;
        private System.Windows.Forms.Label lblOpponentGemsValue;
        private System.Windows.Forms.Label lblOpponentGemsLabel;
        private System.Windows.Forms.Label lblOpponentNameValue;
        private System.Windows.Forms.Label lblOpponentNameLabel;
        private System.Windows.Forms.Label lblOpponentStatusTitle;
        private System.Windows.Forms.Panel pnlGameBoard;
        private System.Windows.Forms.Label lblClickTile;
        private System.Windows.Forms.Button btn33;
        private System.Windows.Forms.Button btn32;
        private System.Windows.Forms.Button btn31;
        private System.Windows.Forms.Button btn23;
        private System.Windows.Forms.Button btn22;
        private System.Windows.Forms.Button btn21;
        private System.Windows.Forms.Button btn13;
        private System.Windows.Forms.Button btn12;
        private System.Windows.Forms.Button btn11;
        private System.Windows.Forms.Panel pnlActionLog;
        private System.Windows.Forms.TextBox txtActionLog;
        private System.Windows.Forms.Label lblActionLogTitle;
        private System.Windows.Forms.Button btnQuitGame;
    }
}