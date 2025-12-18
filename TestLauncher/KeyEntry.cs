using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

namespace TestLauncher
{
    public partial class KeyEntry : Form
    {
        private string loginKey;
        private string latestAvailableVersion;

        public string LoginKey => loginKey;

        public KeyEntry()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            loginKey = txtKeyString.Text.Trim();
            if (string.IsNullOrEmpty(loginKey))
            {
                MessageBox.Show("Please enter a valid key.", "Invalid Key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
