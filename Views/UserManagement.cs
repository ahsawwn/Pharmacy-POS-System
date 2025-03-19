using POS.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Data.SqlClient;
using POS.Helpers;
using System.Security.Cryptography;


namespace POS.Views
{
    public partial class UserManagement: Form
    {
        public UserManagement()
        {
            InitializeComponent();
            LoadUsers();
        }
        private void LoadUsers()
        {
            UserDataAccess userData = new UserDataAccess();
            dgvUsers.DataSource = userData.GetUsers();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Text) && !string.IsNullOrEmpty(txtRole.Text))
            {
                UserDataAccess userData = new UserDataAccess();
                userData.AddUser(txtUsername.Text, txtPassword.Text, txtRole.Text);
                MessageBox.Show("User added successfully!");
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Please fill all fields.");
            }
            txtUsername.Clear();
            txtPassword.Clear();
            if (txtRole.Items.Count > 0)
            {
                txtRole.SelectedIndex = -1; // Deselect any selection
            }

            dgvUsers.ClearSelection(); // Deselect any selected row
        }

        private void btnUpdateUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                int userID = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserID"].Value);
                UserDataAccess userData = new UserDataAccess();
                userData.UpdateUser(userID, txtUsername.Text, txtPassword.Text, txtRole.Text);
                MessageBox.Show("User updated successfully!");
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Please select a user to update.");
            }
            txtUsername.Clear();
            txtPassword.Clear();
            if (txtRole.Items.Count > 0)
            {
                txtRole.SelectedIndex = -1; // Deselect any selection
            }

            dgvUsers.ClearSelection(); // Deselect any selected row

        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                int userID = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["UserID"].Value);
                UserDataAccess userData = new UserDataAccess();
                userData.DeleteUser(userID);
                MessageBox.Show("User deleted successfully!");
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Please select a user to delete.");
            }
            txtUsername.Clear();
            txtPassword.Clear();
            if (txtRole.Items.Count > 0)
            {
                txtRole.SelectedIndex = -1; // Deselect any selection
            }

            dgvUsers.ClearSelection(); // Deselect any selected row
        }

        private void btnClearFeilds_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            if (txtRole.Items.Count > 0)
            {
                txtRole.SelectedIndex = -1; // Deselect any selection
            }

            dgvUsers.ClearSelection(); // Deselect any selected row
        }

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure a valid row is clicked
            {
                DataGridViewRow row = dgvUsers.Rows[e.RowIndex];

                txtUsername.Text = row.Cells["Username"].Value.ToString();
                txtRole.Text = row.Cells["Role"].Value.ToString();

                // 🔹 Fetch password from the database
                int userID = Convert.ToInt32(row.Cells["UserID"].Value);
                txtPassword.Text = GetUserPasswordFromDB(userID);
                txtPassword.UseSystemPasswordChar = true; // Ensure it's hidden by default
            }
        }

        private string GetUserPasswordFromDB(int userID)
        {
            string password = "";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT Password FROM Users WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                        password = result.ToString(); // Store the password
                }
            }

            return password;
        }


        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowPassword.Checked)
            {
                // Ask for Super Password
                string superPassword = "ahsan"; // 🔹 Change this to your actual super password
                string input = Microsoft.VisualBasic.Interaction.InputBox("Enter Super Password to view the password:",
                                                                           "Super Password Required",
                                                                           "",
                                                                           -1, -1);

                if (input == superPassword)
                {
                    txtPassword.UseSystemPasswordChar = false; // Show password
                }
                else
                {
                    MessageBox.Show("Incorrect Super Password!", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkShowPassword.Checked = false; // Uncheck if incorrect password
                }
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true; // Hide password
            }
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "")
            {
                MessageBox.Show("Please select a user first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string newPassword = "1234"; // Default password
            string Password = SecurityHelper.ComputeSHA256Hex(newPassword); // Hash it before storing

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "UPDATE Users SET Password = @Password WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Password", Password);
                    cmd.Parameters.AddWithValue("@UserID", txtUserID.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Password reset to '123456'. Ask the user to change it.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
