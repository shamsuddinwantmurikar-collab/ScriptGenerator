using Microsoft.Data.SqlClient;
using ShamsGenScripts;
using System.Diagnostics;
using System.Text;

namespace ELTApp
{
    public partial class frmGenerateScript : Form
    {
        public string connectionString;// = "Server=IE-0024\\SQLEXPRESS;Database=VAL_EM;Trusted_Connection=True;TrustServerCertificate=True;";

        public frmGenerateScript()
        {
            InitializeComponent();
        }

        private string BuildConnectionString()
        {
            string server = txtServerName.Text.Trim();
            string database = txtDbName.Text.Trim();
            string user = txtUserName.Text.Trim();
            string password = txtPassword.Text.Trim();


            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database))
            {
                MessageBox.Show("Please enter both Server Name and Database Name.", "Missing Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            // Use either Trusted Connection or SQL Authentication
            if (chkTrusted.Checked)
            {
                return $"Server={server};Database={database};Trusted_Connection=True;TrustServerCertificate=True;";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter both User Name and Password for SQL Authentication.", "Missing Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                return $"Server={server};Database={database};User ID={user};Password={password};TrustServerCertificate=True;";
            }
        }

        private void LoadTableNames(string query)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();


                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // If textbox is empty, use wildcard to show all tables
                        string filterValue = string.IsNullOrWhiteSpace(txtFilter.Text) ? "%" : $"%{txtFilter.Text.Trim()}%";

                        cmd.Parameters.AddWithValue("@filter", filterValue);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            checkedListBoxTables.Items.Clear();
                            while (reader.Read())
                            {
                                checkedListBoxTables.Items.Add(reader.GetString(0).ToUpper());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load tables: {ex.Message}");
            }
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedTables = new List<string>();

                // Loop through all checked items
                foreach (var item in checkedListBoxTables.CheckedItems)
                {
                    string tableName = item.ToString();

                    // Check if this is a "Site" table
                    if (tableName.Contains("Site", StringComparison.OrdinalIgnoreCase))
                    {
                        // Generate base table name (remove "Site")
                        string baseTableName = tableName.Replace("Site", "", StringComparison.OrdinalIgnoreCase)
                                                        .Replace("__", "_") // clean double underscores if any
                                                        .TrimEnd('_');      // remove trailing underscore if exists

                        // Add the base table first (even if not selected)
                        if (!selectedTables.Contains(baseTableName, StringComparer.OrdinalIgnoreCase))
                            selectedTables.Add(baseTableName);

                        // Then add the Site table
                        if (!selectedTables.Contains(tableName, StringComparer.OrdinalIgnoreCase))
                            selectedTables.Add(tableName);
                    }
                    else
                    {
                        // Non-site table: just add it normally
                        if (!selectedTables.Contains(tableName, StringComparer.OrdinalIgnoreCase))
                            selectedTables.Add(tableName);

                        // Try to find a corresponding "Site" table in the selection
                        string siteVariant = tableName.Insert(tableName.LastIndexOf('_') + 1, "Site_");

                        foreach (var chkItem in checkedListBoxTables.CheckedItems)
                        {
                            string chkName = chkItem.ToString();
                            if (chkName.Equals(siteVariant, StringComparison.OrdinalIgnoreCase))
                            {
                                if (!selectedTables.Contains(chkName, StringComparer.OrdinalIgnoreCase))
                                    selectedTables.Add(chkName);
                            }
                        }
                    }
                }



                if (selectedTables.Count == 0)
                {
                    MessageBox.Show("Please select at least one table.");
                    return;
                }

                StringBuilder allInserts = new StringBuilder();

                foreach (var table in selectedTables)
                {
                    allInserts.AppendLine($"-- ==== INSERTS FOR TABLE: {table} ====");
                    string script = SqlStatementGenerator.GenerateInsertStatements(connectionString, table, txtSiteFilter.Text);
                    allInserts.AppendLine(script);
                    allInserts.AppendLine();
                }

                txtInsert.Text = allInserts.ToString();

                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GeneratedSQL"
);
                Directory.CreateDirectory(folderPath); // creates if missing

                string filePath = Path.Combine(folderPath, "InsertScripts.sql");
                File.WriteAllText(filePath, allInserts.ToString());

                lblMessage.Text = $"Successfully Saved to :\n{filePath}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            string query = @"
                        SELECT TABLE_NAME 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_TYPE = 'BASE TABLE'
                        and TABLE_NAME like @filter
                        ORDER BY TABLE_NAME;";
            connectionString = BuildConnectionString();
            if (connectionString == null || connectionString.Length == 0)
                return;
            LoadTableNames(query);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtInsert.Clear();
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = chkSelectAll.Checked;

            // Loop through all items in the CheckedListBox
            for (int i = 0; i < checkedListBoxTables.Items.Count; i++)
            {
                checkedListBoxTables.SetItemChecked(i, isChecked);
            }
        }

        private void checkedListBoxTables_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Delay evaluation until after the item is checked/unchecked
            this.BeginInvoke((Action)(() =>
            {
                bool allChecked = checkedListBoxTables.CheckedItems.Count == checkedListBoxTables.Items.Count;
                chkSelectAll.Checked = allChecked;
            }));
        }
    }
}
