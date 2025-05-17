using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using Npgsql;
using SecurityAdmin3;

namespace SecurityAdmin3.Objects
{
    internal class Database
    {
        private static Database? uniqueInstance;
        private string connectionString;
        private NpgsqlDataSource dataSource;

        private Database()
        {
            connectionString = $"Host={MainWindow.DB_IP}:{MainWindow.DB_PORT};Username={MainWindow.DB_USER};Password={MainWindow.DB_PASSWORD};Database={MainWindow.DB_NAME}";
            dataSource = NpgsqlDataSource.Create(connectionString);
        }

        public static Database GetDatabase()
        {
            if (uniqueInstance == null)
            {
                uniqueInstance = new Database();
            }
            return uniqueInstance;
        }

        public async Task<List<Dictionary<string, object>>> Select(string query, params NpgsqlParameter[] parameters)
        {
            try
            {
                List<Dictionary<string, object>> result;
                using (NpgsqlCommand command = dataSource.CreateCommand(query))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    List<Dictionary<string, object>> list2;
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            Dictionary<string, object> dictionary = new Dictionary<string, object>();
                            foreach (DataColumn column in dataTable.Columns)
                            {
                                dictionary[column.ColumnName] = row[column];
                            }
                            list.Add(dictionary);
                        }
                        list2 = list;
                    }
                    result = list2;
                }
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<int> ExecuteNonQuery(string query, params NpgsqlParameter[] parameters)
        {
            try
            {
                using NpgsqlCommand command = dataSource.CreateCommand(query);

                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}");
                return -1;
            }
        }
    }
}