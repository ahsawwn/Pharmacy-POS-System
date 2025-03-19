using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace POS.DataAccess
{
    public class GenericDataAccess
    {
        public List<Generic> GetAllGenerics()
        {
            List<Generic> generics = new List<Generic>();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT * FROM Generics";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            generics.Add(new Generic
                            {
                                GenericID = Convert.ToInt32(reader["GenericID"]),
                                GenericName = reader["GenericName"].ToString(),
                                Description = reader["Description"].ToString()
                            });
                        }
                    }
                }
            }
            return generics;
        }

        public bool AddGeneric(string genericName, string description)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "INSERT INTO Generics (GenericName, Description) VALUES (@GenericName, @Description)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GenericName", genericName);
                    cmd.Parameters.AddWithValue("@Description", description);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateGeneric(int genericId, string genericName, string description)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "UPDATE Generics SET GenericName = @GenericName, Description = @Description WHERE GenericID = @GenericID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GenericID", genericId);
                    cmd.Parameters.AddWithValue("@GenericName", genericName);
                    cmd.Parameters.AddWithValue("@Description", description);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteGeneric(int genericId)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "DELETE FROM Generics WHERE GenericID = @GenericID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GenericID", genericId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }

    public class Generic
    {
        public int GenericID { get; set; }
        public string GenericName { get; set; }
        public string Description { get; set; }
    }
}
