using System;
using System.Configuration;
using System.Data.SqlClient;

namespace POS.DataAccess
{
    public static class DatabaseConnection
    {
        private static string connectionString;

        static DatabaseConnection()
        {
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["POSConnection"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Database connection string is missing in App.config.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading connection string: {ex.Message}");
            }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // Method to update connection string dynamically at runtime
        public static void SetConnectionString(string newConnectionString)
        {
            if (!string.IsNullOrEmpty(newConnectionString))
            {
                connectionString = newConnectionString;
            }
            else
            {
                throw new ArgumentException("Connection string cannot be empty.");
            }
        }
    }
}
