using GridHeistApplication.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GridHeistApplication
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        //Login button click event handler
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            DatabaseAccessor dbAccessor = new DatabaseAccessor();
            int status = dbAccessor.ValidateLogin(username, password);

            if (status == 1) // Success
            {
                MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                frmGame gameForm = new frmGame(8, username);
                gameForm.Show();
                this.Hide();
            }
            else if (status == -1) // Locked Out
            {
                MessageBox.Show("Account locked due to too many failed attempts. Please contact an administrator.",
                                "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else // Invalid (status == 0)
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Register button click event handler
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Basic validation to prevent empty fields
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both a username and a password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DatabaseAccessor dbAccessor = new DatabaseAccessor();

            // Call the DAO method you just added to DatabaseAccessor.cs
            int status = dbAccessor.RegisterPlayer(username, password);

            if (status == 1) // Success
            {
                MessageBox.Show("Registration Successful! You are now logged in.", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open the Game form and hide the login form
                frmGame gameForm = new frmGame(8, username);
                gameForm.Show();
                this.Hide();
            }
            else // Username taken (status == 0)
            {
                MessageBox.Show("That username is already taken. Please choose another one.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
