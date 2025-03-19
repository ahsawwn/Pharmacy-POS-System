using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Views;

namespace POS.Views
{
    public partial class AdminDashboard: Form
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            Logs logs = new Logs();
            logs.Show();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            Login login = new Login();
            login.Show();
        }

        private void btnManageUsers_Click(object sender, EventArgs e)
        {
            UserManagement UserManagement = new UserManagement();
            UserManagement.Show();
        }

        private void btnManageProducts_Click(object sender, EventArgs e)
        {
            ProductDefinition productDefinition = new ProductDefinition();
            productDefinition.Show();
        }
    }
}
