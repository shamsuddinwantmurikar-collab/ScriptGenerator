using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace ShamsGenScripts
{
    public class SqlStatementGenerator
    {
        // List of columns to always exclude from the generated INSERT statements
        private static readonly HashSet<string> ExcludedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "FellerID", "TestMap"
    };

        /// <summary>
        /// Generates conditional SQL INSERT statements for a given table, optionally filtering by site.
        /// </summary>
        /// <param name="conStr">The SQL Server connection string.</param>
        /// <param name="tableName">The name of the table to generate inserts for.</param>
        /// <param name="siteName">The site name to filter data by. Leave empty for all data (no filter).</param>
        /// <returns>A string containing the generated SQL INSERT statements or an error message.</returns>
        public static string GenerateInsertStatements(string conStr, string tableName, string siteName = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    conn.Open();
                    return BuildAndExecuteQuery(conn, tableName, siteName);
                }
            }
            catch (Exception ex)
            {
                // Catch and return major connection or query errors
                return $"-- ERROR in table '{tableName}': {ex.Message}";
            }
        }

        // --- Private Helper Methods ---

        private static string BuildAndExecuteQuery(SqlConnection conn, string tableName, string siteName)
        {
            bool isSiteTable = tableName.Contains("SITE", StringComparison.OrdinalIgnoreCase);
            string query;

            // 1. Determine the appropriate SELECT query based on site filter and table type
            if (!string.IsNullOrWhiteSpace(siteName))
            {
                if (isSiteTable)
                {
                    // Linked table (e.g., EM_Site_Buildings)
                    string baseTable = tableName.Replace("Site_", "", StringComparison.OrdinalIgnoreCase);
                    query = $@"SELECT t.*
                        FROM {tableName} t
                        INNER JOIN {baseTable} b ON t.ID_LIST = b.ID
                        INNER JOIN EM_SITES s ON t.ID_SITE = s.ID
                        WHERE s.SITENAME = @SiteName AND ISNULL(b.DELCODE, '') <> 'SYSTEM'";
                }
                else
                {
                    // Base table (e.g., EM_Buildings)
                    string siteTable = tableName.Insert(tableName.IndexOf('_') + 1, "Site_");
                    query = $@"SELECT t.*
                        FROM {tableName} t
                        INNER JOIN {siteTable} st ON t.ID = st.ID_LIST
                        INNER JOIN EM_SITES s ON st.ID_SITE = s.ID
                        WHERE s.SITENAME = @SiteName AND ISNULL(t.DELCODE, '') <> 'SYSTEM'";
                }
            }
            else
            {
                // No site filter, select all data
                query = $"SELECT * FROM {tableName}";
            }

            // 2. Execute the query
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (!string.IsNullOrWhiteSpace(siteName))
                    cmd.Parameters.AddWithValue("@SiteName", siteName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // 3. Process the results and generate SQL inserts
                    return ProcessDataReaderAndBuildInserts(reader, tableName, siteName, isSiteTable);
                }
            }
        }

        private static string ProcessDataReaderAndBuildInserts(SqlDataReader reader, string tableName, string siteName, bool isSiteTable)
        {
            StringBuilder sb = new StringBuilder();

            // 1. Get column names and filter out excluded ones
            var schema = reader.GetColumnSchema();
            var columnNames = new List<string>();

            foreach (var col in schema)
            {
                if (!ExcludedColumns.Contains(col.ColumnName))
                    columnNames.Add(col.ColumnName);
            }

            if (!reader.HasRows)
            {
                sb.AppendLine($"-- Table {tableName} has no data for site '{siteName}'.");
                return sb.ToString();
            }

            // Pre-calculate the index of the ID_SITE column if it's a site table
            int idSiteIndex = -1;
            if (isSiteTable)
            {
                idSiteIndex = columnNames.FindIndex(c => c.Equals("ID_SITE", StringComparison.OrdinalIgnoreCase));
            }


            // 2. Iterate through rows and generate statements
            while (reader.Read())
            {
                var values = new List<string>();
                string verificationValue = null;
                string idListValue = null;

                // Extract all data and capture required verification fields (NAME, ID_LIST)
                for (int i = 0; i < columnNames.Count; i++)
                {
                    string columnName = columnNames[i];
                    object value = reader.GetValue(i);
                    string formattedValue = FormatValueForSql(value);

                    values.Add(formattedValue);

                    // Capture values for verification check
                    if (!isSiteTable && columnName.Equals("NAME", StringComparison.OrdinalIgnoreCase))
                        verificationValue = formattedValue;
                    else if (isSiteTable && columnName.Equals("ID_LIST", StringComparison.OrdinalIgnoreCase))
                        idListValue = formattedValue;
                }

                // --- NEW LOGIC: Replace ID_SITE actual value with @SITEID parameter ---
                if (isSiteTable && idSiteIndex != -1)
                {
                    // The value at idSiteIndex is currently the hardcoded ID value (e.g., '5')
                    // We replace it with the SQL parameter name '@SITEID'
                    values[idSiteIndex] = "@SITEID";
                }
                // ---------------------------------------------------------------------

                // 3. Build the INSERT statement and verification check
                string columnsList = string.Join(",", columnNames);
                string valuesList = string.Join(", ", values);
                string insertStatement = $"INSERT INTO {tableName} ({columnsList}) VALUES ({valuesList});";
                string verificationCheck = "";

                if (insertStatement.Contains("SYSTEM", StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Skip rows containing 'SYSTEM'
                }

                if (isSiteTable)
                {
                    // Verification condition uses @SITEID (already correct)
                    if (idListValue != null)
                    {
                        // Insert statement now also uses @SITEID due to the new logic above
                        verificationCheck = $@"IF NOT EXISTS (SELECT 1 FROM {tableName} WHERE ID_LIST = {idListValue} AND ID_SITE = @SITEID)
BEGIN
{insertStatement}
END";
                    }
                }
                else
                {
                    // Verification for base table: check if NAME value exists (unchanged)
                    if (verificationValue != null)
                    {
                        verificationCheck = $@"IF NOT EXISTS (SELECT 1 FROM {tableName} WHERE NAME = {verificationValue})
BEGIN
{insertStatement}
END";
                    }
                }

                // Append the generated SQL block
                if (!string.IsNullOrWhiteSpace(verificationCheck))
                    sb.AppendLine(verificationCheck);
                else
                    sb.AppendLine(insertStatement); // Fallback if verification data wasn't found
            }

            return sb.ToString();
        }
        /// <summary>
        /// Formats a single C# object value into its proper SQL literal string representation.
        /// </summary>
        private static string FormatValueForSql(object value)
        {
            if (value == DBNull.Value)
                return "NULL";

            if (value is string || value is DateTime || value is Guid)
            {
                string escaped = value.ToString().Replace("'", "''");
                return $"'{escaped}'";
            }

            if (value is bool)
                return (bool)value ? "1" : "0";

            if (value is byte[] bytes)
            {
                string hex = BitConverter.ToString(bytes).Replace("-", "");
                return $"0x{hex}";
            }

            // For numbers, return the string representation directly
            return value.ToString();
        }
    }
}
