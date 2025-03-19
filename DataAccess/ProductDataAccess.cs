using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace POS.DataAccess
{
    public class ProductDataAccess
    {
        public bool AddProduct(string name, int genericID, int manufacturerID, int supplierID, int categoryID,
                               int purchaseFactor, decimal purchasePrice, decimal retailPrice, int stockQuantity,
                               DateTime expiryDate, string batchNumber, string barcode)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"INSERT INTO Products (Name, GenericID, ManufacturerID, SupplierID, CategoryID, 
                                 PurchaseFactor, PurchasePrice, RetailPrice, StockQuantity, ExpiryDate, BatchNumber, Barcode, CreatedAt) 
                                 VALUES (@Name, @GenericID, @ManufacturerID, @SupplierID, @CategoryID, @PurchaseFactor, 
                                 @PurchasePrice, @RetailPrice, @StockQuantity, @ExpiryDate, @BatchNumber, @Barcode, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@GenericID", genericID);
                    cmd.Parameters.AddWithValue("@ManufacturerID", manufacturerID);
                    cmd.Parameters.AddWithValue("@SupplierID", supplierID);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("@PurchaseFactor", purchaseFactor);
                    cmd.Parameters.AddWithValue("@PurchasePrice", purchasePrice);
                    cmd.Parameters.AddWithValue("@RetailPrice", retailPrice);
                    cmd.Parameters.AddWithValue("@StockQuantity", stockQuantity);
                    cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate);
                    cmd.Parameters.AddWithValue("@BatchNumber", batchNumber);
                    cmd.Parameters.AddWithValue("@Barcode", barcode);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public DataTable GetProducts()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"SELECT P.ProductID, P.Name, G.GenericName, M.Name AS Manufacturer, S.Name AS Supplier, 
                                 C.CategoryName, P.PurchaseFactor, P.PurchasePrice, P.RetailPrice, P.StockQuantity, 
                                 P.ExpiryDate, P.BatchNumber, P.Barcode, P.CreatedAt
                                 FROM Products P
                                 JOIN Generics G ON P.GenericID = G.GenericID
                                 JOIN Manufacturers M ON P.ManufacturerID = M.ManufacturerID
                                 JOIN Suppliers S ON P.SupplierID = S.SupplierID
                                 JOIN Categories C ON P.CategoryID = C.CategoryID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public bool UpdateProduct(int productID, string name, int genericID, int manufacturerID, int supplierID, int categoryID,
                                  int purchaseFactor, decimal purchasePrice, decimal retailPrice, int stockQuantity,
                                  DateTime expiryDate, string batchNumber, string barcode)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"UPDATE Products 
                                 SET Name = @Name, GenericID = @GenericID, ManufacturerID = @ManufacturerID, SupplierID = @SupplierID, 
                                 CategoryID = @CategoryID, PurchaseFactor = @PurchaseFactor, PurchasePrice = @PurchasePrice, 
                                 RetailPrice = @RetailPrice, StockQuantity = @StockQuantity, ExpiryDate = @ExpiryDate, 
                                 BatchNumber = @BatchNumber, Barcode = @Barcode
                                 WHERE ProductID = @ProductID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productID);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@GenericID", genericID);
                    cmd.Parameters.AddWithValue("@ManufacturerID", manufacturerID);
                    cmd.Parameters.AddWithValue("@SupplierID", supplierID);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("@PurchaseFactor", purchaseFactor);
                    cmd.Parameters.AddWithValue("@PurchasePrice", purchasePrice);
                    cmd.Parameters.AddWithValue("@RetailPrice", retailPrice);
                    cmd.Parameters.AddWithValue("@StockQuantity", stockQuantity);
                    cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate);
                    cmd.Parameters.AddWithValue("@BatchNumber", batchNumber);
                    cmd.Parameters.AddWithValue("@Barcode", barcode);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteProduct(int productID)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "DELETE FROM Products WHERE ProductID = @ProductID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productID);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    
    public static DataTable GetCategories()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT CategoryID, CategoryName FROM Categories";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static DataTable GetGenerics()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT GenericID, GenericName FROM Generics";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static DataTable GetManufacturers()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT ManufacturerID, ManufacturerName FROM Manufacturers";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static DataTable GetSuppliers()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT SupplierID, SupplierName FROM Suppliers";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
    }
