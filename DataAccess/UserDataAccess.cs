using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using POS.Helpers; // Import SecurityHelper

namespace POS.DataAccess
{
    public class UserDataAccess
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString;
        public DataTable GetUsers()
        {
            DataTable dtUsers = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT UserID, Username, Role FROM Users";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dtUsers);
                }
            }
            return dtUsers;
        }

        public void AddUser(string username, string password, string role)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string Password = SecurityHelper.ComputeSHA256Hex(password);
                string query = "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    cmd.Parameters.AddWithValue("@Role", role);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateUser(int userID, string username, string password, string role)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string Password = SecurityHelper.ComputeSHA256Hex(password);
                string query = "UPDATE Users SET Username = @Username, Password = @Password, Role = @Role WHERE UserID = @UserID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    cmd.Parameters.AddWithValue("@Role", role);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteUser(int userID)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // First, delete logs related to this user
                string deleteLogsQuery = "DELETE FROM LoginLogs WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(deleteLogsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.ExecuteNonQuery();
                }

                // Now, delete the user
                string deleteUserQuery = "DELETE FROM Users WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(deleteUserQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
