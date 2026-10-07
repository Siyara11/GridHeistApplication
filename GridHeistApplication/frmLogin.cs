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

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            DatabaseAccessor dbAccessor = new DatabaseAccessor();
            int status = dbAccessor.ValidateLogin(username, password);

            if (status == 1) // Success
            {
                MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                frmGame gameForm = new frmGame();
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
    }
}
