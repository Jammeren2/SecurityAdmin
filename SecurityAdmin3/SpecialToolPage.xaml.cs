using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Npgsql;
using System.Collections.Generic;
using SecurityAdmin3.Objects;
using System.Xml.Linq;
using System;

namespace SecurityAdmin3
{
    public partial class SpecialToolPage : Page
    {
        public MainWindow _mainWindow;
        public SpecialTool tool;

        public SpecialToolPage(MainWindow mainWindow, SpecialTool tool)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.tool = tool;
            InitializeForm();
        }

        private void InitializeForm()
        {
            if (tool != null)
            {
                pageTitle.Text = "Редактирование спецсредства";
                txtName.Text = tool.Name;
                txtNumber.Text = tool.Number;
                txtDescription.Text = tool.Description;
                btnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                pageTitle.Text = "Новое спецсредство";
                tool = new SpecialTool();
            }
            txtName.Focus();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Название не может быть пустым");
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNumber.Text))
            {
                MessageBox.Show("Номер не может быть пустым");
                txtNumber.Focus();
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            string name = txtName.Text.Trim();
            string number = txtNumber.Text.Trim();
            string description = txtDescription.Text.Trim();

            try
            {
                Database database = Database.GetDatabase();
                if (tool.Id > 0)
                {
                    // Update existing tool
                    string updateQuery = @"UPDATE ""specialtools"" SET ""name"" = $1, ""number"" = $2, ""description"" = $3 WHERE ""id"" = $4";
                    var parameters = new NpgsqlParameter[] {
                        new NpgsqlParameter { Value = name },
                        new NpgsqlParameter { Value = number },
                        new NpgsqlParameter { Value = string.IsNullOrEmpty(description) ? DBNull.Value : (object)description },
                        new NpgsqlParameter { Value = tool.Id }
                    };
                    await database.ExecuteNonQuery(updateQuery, parameters);
                    MessageBox.Show("Спецсредство успешно обновлено");
                }
                else
                {
                    // Create new tool
                    string insertQuery = @"INSERT INTO ""specialtools"" (""name"", ""number"", ""description"") VALUES ($1, $2, $3)";
                    var parameters = new NpgsqlParameter[] {
                        new NpgsqlParameter { Value = name },
                        new NpgsqlParameter { Value = number },
                        new NpgsqlParameter { Value = string.IsNullOrEmpty(description) ? DBNull.Value : (object)description }
                    };
                    await database.ExecuteNonQuery(insertQuery, parameters);
                    MessageBox.Show("Спецсредство успешно добавлено");
                }
                _mainWindow.mainFrame.Navigate(new SpecialToolsPage(_mainWindow));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (tool == null || tool.Id == 0) return;

            if (MessageBox.Show("Вы действительно хотите удалить это спецсредство?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Database database = Database.GetDatabase();
                    string deleteQuery = @"DELETE FROM ""specialtools"" WHERE ""id"" = $1";
                    var parameter = new NpgsqlParameter { Value = tool.Id };
                    await database.ExecuteNonQuery(deleteQuery, parameter);
                    MessageBox.Show("Спецсредство успешно удалено");
                    _mainWindow.mainFrame.Navigate(new SpecialToolsPage(_mainWindow));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении спецсредства: {ex.Message}");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new SpecialToolsPage(_mainWindow));
        }
    }
}