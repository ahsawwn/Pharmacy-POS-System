using POS.DataAccess;
using POS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace POS.Views
{
    public partial class Login: Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString;

        public Login()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string Password = SecurityHelper.ComputeSHA256Hex(txtPassword.Text);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT UserID, Role FROM Users WHERE Username = @Username AND Password = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", Password);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int userID = reader.GetInt32(0);
                    string role = reader.GetString(1);

                    // Log the login
                    LogLogin(userID);

                    // Open the appropriate form based on the role
                    if (role == "Admin")
                    {
                        AdminDashboard AdminDashboard = new AdminDashboard();
                        AdminDashboard.Show();
                    }
                    else if (role == "Salesman")
                    {
                        SalesDashboard SalesDashboard = new SalesDashboard();
                        SalesDashboard.Show();
                    }

                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password");
                }
            }
        }

        private void LogLogin(int userID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO LoginLogs (UserID, LoginTime) VALUES (@UserID, @LoginTime)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@LoginTime", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
