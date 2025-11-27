using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace ShamsGenScripts
{
    public class SqlStatementGenerator
    {
        // List of columns to always exclude from the generated INSERT statements
        private static readonly HashSet<string> ExcludedColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            ColumnFellerId, ColumnTestMap
        };

        // Column name constants
        private const string ColumnFellerId = "FellerID";
        private const string ColumnTestMap = "TestMap";
        private const string ColumnIdSite = "ID_SITE";
        private const string ColumnIdList = "ID_LIST";
        private const string ColumnName = "NAME";
        private const string SystemDelCode = "SYSTEM";

        // Table name constants
        private const string TableEmSites = "EM_SITES";

        // Logging helper
        private static void Log(string message)
        {
            Debug.WriteLine($"[SqlStatementGenerator] {message}");
        }

        /// <summary>
        /// Generates conditional SQL INSERT statements for a given table, optionally filtering by site.
        /// </summary>
        /// <param name="conStr">The SQL Server connection string.</param>
        /// <param name="tableName">The name of the table to generate inserts for.</param>
        /// <param name="siteName">The site name to filter data by. Leave empty for all data (no filter).</param>
        /// <returns>A string containing the generated SQL INSERT statements or an error message.</returns>
        /// <summary>
        /// Generates conditional SQL INSERT statements for a given table, optionally filtering by site.
        /// </summary>
        public static string GenerateInsertStatements(string conStr, string tableName, string siteName = "")
        {
            try
            {
                using (var conn = new SqlConnection(conStr))
                {
                    conn.Open();
                    Log($"Connection opened for table: {tableName}, site: {siteName}");
                    return BuildAndExecuteQuery(conn, tableName, siteName);
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR in table '{tableName}': {ex}");
                return $"-- ERROR in table '{tableName}': {ex.Message}";
            }
        }

        // --- Private Helper Methods ---

        private static string BuildAndExecuteQuery(SqlConnection conn, string tableName, string siteName)
        {
            bool isSiteTable = IsSiteTable(tableName);
            string query = BuildSelectQuery(tableName, siteName, isSiteTable);
            Log($"Executing query: {query}");

            using (var cmd = new SqlCommand(query, conn))
            {
                if (!string.IsNullOrWhiteSpace(siteName))
                {
                    cmd.Parameters.AddWithValue("@SiteName", siteName);
                }
                using (var reader = cmd.ExecuteReader())
                {
                    return ProcessDataReaderAndBuildInserts(reader, tableName, siteName, isSiteTable);
                }
            }
        }

        // Helper to determine if table is a site table
        private static bool IsSiteTable(string tableName)
        {
            // Consider a table a site table if it contains "SITE" (case-insensitive)
            return tableName.Contains("SITE", StringComparison.OrdinalIgnoreCase);
        }

        // Helper to build SELECT query
        private static string BuildSelectQuery(string tableName, string siteName, bool isSiteTable)
        {
            if (!string.IsNullOrWhiteSpace(siteName))
            {
                if (isSiteTable)
                {
                    // Linked table (e.g., EM_Site_Buildings)
                    string baseTable = GetBaseTableName(tableName);
                    return $@"SELECT t.{ColumnIdList},t.{ColumnIdSite}
                        FROM {tableName} t
                        INNER JOIN {baseTable} b ON t.{ColumnIdList} = b.ID
                        INNER JOIN {TableEmSites} s ON t.{ColumnIdSite} = s.ID
                        WHERE s.SITENAME = @SiteName AND ISNULL(b.DELCODE, '') <> '{SystemDelCode}'";
                }
                else
                {
                    // Base table (e.g., EM_Buildings)
                    string siteTable = GetSiteTableName(tableName);
                    return $@"SELECT t.*
                        FROM {tableName} t
                        INNER JOIN {siteTable} st ON t.ID = st.{ColumnIdList}
                        INNER JOIN {TableEmSites} s ON st.{ColumnIdSite} = s.ID
                        WHERE s.SITENAME = @SiteName AND ISNULL(t.DELCODE, '') <> '{SystemDelCode}'";
                }
            }
            else
            {
                // No site filter, select all data
                return $"SELECT * FROM {tableName}";
            }
        }

        // Helper to get base table name from site table
        private static string GetBaseTableName(string tableName)
        {
            // Remove "Site_" prefix if present
            int idx = tableName.IndexOf("Site_", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                return tableName.Remove(idx, "Site_".Length);
            }
            return tableName;
        }

        // Helper to get site table name from base table
        private static string GetSiteTableName(string tableName)
        {
            int idx = tableName.IndexOf('_');
            if (idx >= 0)
            {
                return tableName.Insert(idx + 1, "Site_");
            }
            return tableName;
        }

        private static string ProcessDataReaderAndBuildInserts(SqlDataReader reader, string tableName, string siteName, bool isSiteTable)
        {
            var sb = new StringBuilder();
            var schema = reader.GetColumnSchema();
            var columnNames = new List<string>();

            // Filter out excluded columns
            foreach (var col in schema)
            {
                if (!ExcludedColumns.Contains(col.ColumnName))
                    columnNames.Add(col.ColumnName);
            }

            if (!reader.HasRows)
            {
                sb.AppendLine($"-- Table {tableName} has no data for site '{siteName}'.");
                Log($"No data found for table: {tableName}, site: {siteName}");
                return sb.ToString();
            }

            // Pre-calculate the index of the ID_SITE column if it's a site table
            int idSiteIndex = -1;
            if (isSiteTable)
            {
                idSiteIndex = columnNames.FindIndex(c => c.Equals(ColumnIdSite, StringComparison.OrdinalIgnoreCase));
            }

            int rowCount = 0;
            while (reader.Read())
            {
                var values = new List<string>();
                string verificationValue = null;
                string idListValue = null;

                for (int i = 0; i < columnNames.Count; i++)
                {
                    string columnName = columnNames[i];
                    object value = reader.GetValue(i);
                    string formattedValue = FormatValueForSql(value);
                    values.Add(formattedValue);

                    // Capture values for verification check
                    if (!isSiteTable && columnName.Equals(ColumnName, StringComparison.OrdinalIgnoreCase))
                        verificationValue = formattedValue;
                    else if (isSiteTable && columnName.Equals(ColumnIdList, StringComparison.OrdinalIgnoreCase))
                        idListValue = formattedValue;
                }

                // Replace ID_SITE actual value with @SITEID parameter
                if (isSiteTable && idSiteIndex != -1)
                {
                    values[idSiteIndex] = "@SITEID";
                }

                string columnsList = string.Join(",", columnNames);
                string valuesList = string.Join(", ", values);
                string insertStatement = $"INSERT INTO {tableName} ({columnsList}) VALUES ({valuesList});";
                string verificationCheck = "";

                // Skip rows containing 'SYSTEM' in DELCODE
                if (insertStatement.Contains($", '{SystemDelCode}'", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (isSiteTable)
                {
                    if (idListValue != null)
                    {
                        verificationCheck = $@"IF NOT EXISTS (SELECT 1 FROM {tableName} WHERE {ColumnIdList} = {idListValue} AND {ColumnIdSite} = @SITEID)
BEGIN
{insertStatement}
END";
                    }
                }
                else
                {
                    if (verificationValue != null)
                    {
                        verificationCheck = $@"IF NOT EXISTS (SELECT 1 FROM {tableName} WHERE {ColumnName} = {verificationValue})
BEGIN
{insertStatement}
END";
                    }
                }

                if (!string.IsNullOrWhiteSpace(verificationCheck))
                    sb.AppendLine(verificationCheck);
                else
                    sb.AppendLine(insertStatement);
                rowCount++;
            }
            Log($"Generated {rowCount} insert statements for table: {tableName}, site: {siteName}");
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
        // End of SqlStatementGenerator
    }
}
