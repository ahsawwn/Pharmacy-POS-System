using POS.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Helpers;

namespace POS.Views
{
    public partial class ProductDefinition : Form
    {

        public ProductDefinition()
        {
            InitializeComponent();
            LoadDropdowns();
            LoadLineItems();
            GetNextProductID();
            LoadProducts();
            LoadComboBoxes();
        }

        private void ProductDefinition_Load(object sender, EventArgs e)
        {
            LoadDropdowns();
            LoadLineItems();
            GetNextProductID();
            LoadProducts();
            LoadComboBoxes();
        }

        private void ClearFields()
        {
            txtProductID.Clear();
            txtProductName.Clear();
            cmbCategory.SelectedIndex = -1;
            cmbManufacturer.SelectedIndex = -1;
            cmbSupplier.SelectedIndex = -1;
            cmbGeneric.SelectedIndex = -1;
            cmbLineItem.SelectedIndex = -1;
            txtPurchaseFactor.Clear();
            txtPurchasePrice.Clear();
            txtRetailPrice.Clear();

            txtProductID.Text = GetNextProductID().ToString(); // Auto-generate Product ID
        }

        private void LoadDropdowns()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // Load Manufacturers
                string query = "SELECT ManufacturerID, ManufacturerName FROM Manufacturers";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    List<ComboboxItem> manufacturerList = new List<ComboboxItem>();
                    AutoCompleteStringCollection manufacturerCollection = new AutoCompleteStringCollection();

                    while (reader.Read())
                    {
                        string manufacturerName = reader["ManufacturerName"].ToString();
                        manufacturerList.Add(new ComboboxItem { Text = manufacturerName, Value = reader["ManufacturerID"] });
                        manufacturerCollection.Add(manufacturerName);
                    }

                    cmbManufacturer.DataSource = manufacturerList;
                    cmbManufacturer.DisplayMember = "Text";
                    cmbManufacturer.ValueMember = "Value";

                    cmbManufacturer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cmbManufacturer.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    cmbManufacturer.AutoCompleteCustomSource = manufacturerCollection;
                }

                // Load Generics
                query = "SELECT GenericID, GenericName FROM Generics";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    List<ComboboxItem> genericList = new List<ComboboxItem>();
                    AutoCompleteStringCollection genericCollection = new AutoCompleteStringCollection();

                    while (reader.Read())
                    {
                        string genericName = reader["GenericName"].ToString();
                        genericList.Add(new ComboboxItem { Text = genericName, Value = reader["GenericID"] });
                        genericCollection.Add(genericName);
                    }

                    cmbGeneric.DataSource = genericList;
                    cmbGeneric.DisplayMember = "Text";
                    cmbGeneric.ValueMember = "Value";

                    cmbGeneric.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cmbGeneric.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    cmbGeneric.AutoCompleteCustomSource = genericCollection;
                }

                // Load Categories
                query = "SELECT CategoryID, CategoryName FROM Categories";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    List<ComboboxItem> categoryList = new List<ComboboxItem>();
                    AutoCompleteStringCollection categoryCollection = new AutoCompleteStringCollection();

                    while (reader.Read())
                    {
                        string categoryName = reader["CategoryName"].ToString();
                        categoryList.Add(new ComboboxItem { Text = categoryName, Value = reader["CategoryID"] });
                        categoryCollection.Add(categoryName);
                    }

                    cmbCategory.DataSource = categoryList;
                    cmbCategory.DisplayMember = "Text";
                    cmbCategory.ValueMember = "Value";

                    cmbCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cmbCategory.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    cmbCategory.AutoCompleteCustomSource = categoryCollection;
                }

                // Load Suppliers
                query = "SELECT SupplierID, SupplierName FROM Suppliers";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    List<ComboboxItem> supplierList = new List<ComboboxItem>();
                    AutoCompleteStringCollection supplierCollection = new AutoCompleteStringCollection();

                    while (reader.Read())
                    {
                        string supplierName = reader["SupplierName"].ToString();
                        supplierList.Add(new ComboboxItem { Text = supplierName, Value = reader["SupplierID"] });
                        supplierCollection.Add(supplierName);
                    }

                    cmbSupplier.DataSource = supplierList;
                    cmbSupplier.DisplayMember = "Text";
                    cmbSupplier.ValueMember = "Value";

                    cmbSupplier.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cmbSupplier.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    cmbSupplier.AutoCompleteCustomSource = supplierCollection;
                }
            }
        }


        private void LoadLineItems()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT LineItemID, LineItemName FROM LineItems";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open(); // ✅ Open connection before executing the query
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cmbLineItem.Items.Clear();

                        while (reader.Read())
                        {
                            cmbLineItem.Items.Add(new ComboboxItem
                            {
                                Text = reader["LineItemName"].ToString(),
                                Value = Convert.ToInt32(reader["LineItemID"])
                            });
                        }
                    }
                }
            }
        }

        private int GetNextProductID()
        {
            int nextID = 1; // Default if no products exist

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT ISNULL(MAX(ProductID), 0) + 1 FROM Products";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    nextID = Convert.ToInt32(cmd.ExecuteScalar()); // Get the next available ID
                }
            }

            return nextID;
        }

        public void LoadProducts()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"SELECT 
                    p.ProductID, p.ProductName, 
                    c.CategoryName, 
                    g.GenericName, 
                    m.ManufacturerName, 
                    s.SupplierName, 
                    l.LineItemName,
                    p.PurchasePrice, p.RetailPrice, p.PurchaseFactor
                FROM Products p
                LEFT JOIN LineItems l ON l.LineItemID = l.LineItemID
                LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                LEFT JOIN Generics g ON p.GenericID = g.GenericID
                LEFT JOIN Manufacturers m ON p.ManufacturerID = m.ManufacturerID
                LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvProducts.DataSource = dt;
            }
        }

        private void LoadComboBoxes()
        {
            // Load Category ComboBox
            cmbCategory.DataSource = null; // Reset DataSource before setting a new one
            cmbCategory.DataSource = ProductDataAccess.GetCategories();
            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryID";
            cmbCategory.SelectedIndex = -1;

            // Load Generic ComboBox
            cmbGeneric.DataSource = null;
            cmbGeneric.DataSource = ProductDataAccess.GetGenerics();
            cmbGeneric.DisplayMember = "GenericName";
            cmbGeneric.ValueMember = "GenericID";
            cmbGeneric.SelectedIndex = -1;

            // Load Manufacturer ComboBox
            cmbManufacturer.DataSource = null;
            cmbManufacturer.DataSource = ProductDataAccess.GetManufacturers();
            cmbManufacturer.DisplayMember = "ManufacturerName";
            cmbManufacturer.ValueMember = "ManufacturerID";
            cmbManufacturer.SelectedIndex = -1;

            // Load Supplier ComboBox
            cmbSupplier.DataSource = null;
            cmbSupplier.DataSource = ProductDataAccess.GetSuppliers();
            cmbSupplier.DisplayMember = "SupplierName";
            cmbSupplier.ValueMember = "SupplierID";
            cmbSupplier.SelectedIndex = -1;

            // Load LineItem ComboBox (manually populated, not data-bound)
            cmbLineItem.DataSource = null;  // Reset DataSource before modifying Items
            cmbLineItem.Items.Clear();
            cmbLineItem.Items.Add("Medicine");
            cmbLineItem.Items.Add("Non-Medicine");
            cmbLineItem.SelectedIndex = -1;
        }









        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text) || cmbManufacturer.SelectedIndex == -1 ||
    cmbGeneric.SelectedIndex == -1 || cmbCategory.SelectedIndex == -1 ||
    cmbSupplier.SelectedIndex == -1 || cmbLineItem.SelectedIndex == -1 ||
    string.IsNullOrWhiteSpace(txtPurchaseFactor.Text) || string.IsNullOrWhiteSpace(lblPurchasePrice.Text) ||
    string.IsNullOrWhiteSpace(lblRetailPrice.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int productID;
            if (!int.TryParse(txtProductID.Text, out productID))
            {
                productID = GetNextProductID(); // Get the next available ID if empty
                txtProductID.Text = productID.ToString(); // Show it in the text box
            }
            decimal purchasePrice, retailPrice;

            // Try parsing purchase price, show error if invalid
            if (!decimal.TryParse(txtPurchasePrice.Text, out purchasePrice))
            {
                MessageBox.Show("Invalid Purchase Price. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Try parsing retail price, show error if invalid
            if (!decimal.TryParse(txtRetailPrice.Text, out retailPrice))
            {
                MessageBox.Show("Invalid Retail Price. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "INSERT INTO Products (ProductName, ManufacturerID, GenericID, CategoryID, SupplierID, LineItemID, PurchaseFactor, PurchasePrice, RetailPrice) " +
                               "VALUES (@ProductName, @ManufacturerID, @GenericID, @CategoryID, @SupplierID, @LineItemID, @PurchaseFactor, @PurchasePrice, @RetailPrice)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    //cmd.Parameters.AddWithValue("@ProductID", productID);
                    cmd.Parameters.AddWithValue("@ProductName", txtProductName.Text);
                    cmd.Parameters.AddWithValue("@ManufacturerID", (cmbManufacturer.SelectedItem as ComboboxItem)?.Value ?? 0);
                    cmd.Parameters.AddWithValue("@GenericID", (cmbGeneric.SelectedItem as ComboboxItem)?.Value ?? 0);
                    cmd.Parameters.AddWithValue("@CategoryID", (cmbCategory.SelectedItem as ComboboxItem)?.Value ?? 0);
                    cmd.Parameters.AddWithValue("@SupplierID", (cmbSupplier.SelectedItem as ComboboxItem)?.Value ?? 0);
                    cmd.Parameters.AddWithValue("@LineItemID", (cmbLineItem.SelectedItem as ComboboxItem)?.Value ?? 0);
                    cmd.Parameters.AddWithValue("@PurchaseFactor", Convert.ToInt32(txtPurchaseFactor.Text));
                    cmd.Parameters.AddWithValue("@PurchasePrice", purchasePrice);
                    cmd.Parameters.AddWithValue("@RetailPrice", retailPrice);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Product saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadProducts(); // Refresh the product list
            ClearFields(); // Clear the form
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductID.Text))
            {
                MessageBox.Show("Please select a product to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "UPDATE Products SET ProductName = @ProductName, ManufacturerID = @ManufacturerID, GenericID = @GenericID, " +
                               "CategoryID = @CategoryID, SupplierID = @SupplierID, LineItemID = @LineItemID, " +
                               "PurchaseFactor = @PurchaseFactor, PurchasePrice = @PurchasePrice, RetailPrice = @RetailPrice " +
                               "WHERE ProductID = @ProductID";

                decimal purchasePrice, retailPrice;

                // Try parsing purchase price, show error if invalid
                if (!decimal.TryParse(txtPurchasePrice.Text, out purchasePrice))
                {
                    MessageBox.Show("Invalid Purchase Price. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Try parsing retail price, show error if invalid
                if (!decimal.TryParse(txtRetailPrice.Text, out retailPrice))
                {
                    MessageBox.Show("Invalid Retail Price. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", txtProductID.Text);
                    cmd.Parameters.AddWithValue("@ProductName", txtProductName.Text);
                    cmd.Parameters.AddWithValue("@ManufacturerID", (cmbManufacturer.SelectedItem as ComboboxItem)?.Value ?? 0);
                    cmd.Parameters.AddWithValue("@GenericID", (cmbGeneric.SelectedItem as ComboboxItem)?.Value ?? 0);
                    cmd.Parameters.AddWithValue("@CategoryID", (cmbCategory.SelectedItem as ComboboxItem)?.Value ?? 0);
                    cmd.Parameters.AddWithValue("@SupplierID", (cmbSupplier.SelectedItem as ComboboxItem)?.Value ?? 0);
                    cmd.Parameters.AddWithValue("@LineItemID", (cmbLineItem.SelectedItem as ComboboxItem)?.Value ?? 0);
                    cmd.Parameters.AddWithValue("@PurchaseFactor", Convert.ToInt32(txtPurchaseFactor.Text));
                    cmd.Parameters.AddWithValue("@PurchasePrice", purchasePrice);
                    cmd.Parameters.AddWithValue("@RetailPrice", retailPrice);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadProducts(); // Refresh product list
            ClearFields(); // Clear form
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductID.Text))
            {
                MessageBox.Show("Please select a product to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
                return;

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "DELETE FROM Products WHERE ProductID = @ProductID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", txtProductID.Text);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadProducts(); // Refresh product list
            ClearFields(); // Clear form
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvProducts.Rows[e.RowIndex];

                    txtProductID.Text = row.Cells["ProductID"].Value?.ToString() ?? "";
                    txtProductName.Text = row.Cells["ProductName"].Value?.ToString() ?? "";
                    txtPurchasePrice.Text = row.Cells["PurchasePrice"].Value?.ToString() ?? "0.00";
                    txtRetailPrice.Text = row.Cells["RetailPrice"].Value?.ToString() ?? "0.00";
                    txtPurchaseFactor.Text = row.Cells["PurchaseFactor"].Value?.ToString() ?? "1";

                    // Set ComboBoxes using names
                    cmbCategory.SelectedIndex = cmbCategory.FindStringExact(row.Cells["CategoryName"].Value?.ToString() ?? "");
                    cmbGeneric.SelectedIndex = cmbGeneric.FindStringExact(row.Cells["GenericName"].Value?.ToString() ?? "");
                    cmbManufacturer.SelectedIndex = cmbManufacturer.FindStringExact(row.Cells["ManufacturerName"].Value?.ToString() ?? "");
                    cmbSupplier.SelectedIndex = cmbSupplier.FindStringExact(row.Cells["SupplierName"].Value?.ToString() ?? "");

                    // Set LineItem selection
                    cmbLineItem.SelectedIndex = cmbLineItem.FindStringExact(row.Cells["LineItemName"].Value?.ToString() ?? "");

                   // btnSave.Text = "Update";
                   // btnDelete.Enabled = true;
                }
            }
        }
    }
}


