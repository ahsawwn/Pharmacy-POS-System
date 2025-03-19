using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace POS.DataAccess
{
    public class SupplierDataAccess
    {
        public List<Supplier> GetAllSuppliers()
        {
            List<Supplier> suppliers = new List<Supplier>();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT * FROM Suppliers";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            suppliers.Add(new Supplier
                            {
                                SupplierID = Convert.ToInt32(reader["SupplierID"]),
                                SupplierName = reader["SupplierName"].ToString(),
                                ContactNumber = reader["ContactNumber"].ToString(),
                                Address = reader["Address"].ToString(),
                                Balance = Convert.ToDecimal(reader["Balance"])
                            });
                        }
                    }
                }
            }
            return suppliers;
        }

        public bool AddSupplier(string name, string contact, string address, decimal balance)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "INSERT INTO Suppliers (SupplierName, ContactNumber, Address, Balance) VALUES (@Name, @Contact, @Address, @Balance)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Contact", contact);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Balance", balance);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateSupplier(int id, string name, string contact, string address, decimal balance)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "UPDATE Suppliers SET SupplierName = @Name, ContactNumber = @Contact, Address = @Address, Balance = @Balance WHERE SupplierID = @ID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Contact", contact);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Balance", balance);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteSupplier(int id)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "DELETE FROM Suppliers WHERE SupplierID = @ID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }

    public class Supplier
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public decimal Balance { get; set; }
    }
}
