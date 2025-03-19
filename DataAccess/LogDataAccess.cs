using System;
using System.Data;
using System.Data.SqlClient;

namespace POS.DataAccess
{
    public class LogDataAccess
    {
        public bool InsertLoginLog(int userID, string username, string role)
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    string query = @"
                        INSERT INTO LoginLogs (UserID, Username, Role, LoginTime) 
                        VALUES (@UserID, @Username, @Role, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                        cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 50).Value = role;

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0; // Return true if insertion is successful
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InsertLoginLog: {ex.Message}");
                return false;
            }
        }

        public bool InsertLogoutLog(int userID)
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    string query = @"
                        UPDATE LoginLogs 
                        SET LogoutTime = GETDATE() 
                        WHERE UserID = @UserID AND LogoutTime IS NULL";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0; // Return true if update is successful
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in InsertLogoutLog: {ex.Message}");
                return false;
            }
        }
    }
}
