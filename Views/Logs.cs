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

namespace POS.Views
{
    public partial class Logs: Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["POSConnection"].ConnectionString;
        public Logs()
        {
            InitializeComponent();
            LoadLogs();
        }

        private void LoadLogs()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Users.Username, LoginLogs.LoginTime FROM LoginLogs INNER JOIN Users ON LoginLogs.UserID = Users.UserID";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridViewLogs.DataSource = dt;
            }
        }
    }
}
