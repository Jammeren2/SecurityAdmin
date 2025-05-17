using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Npgsql;
using System.Collections.Generic;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class RolePage : Page
    {
        public MainWindow _mainWindow;
        public Role role;

        public RolePage(MainWindow mainWindow, Role role)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.role = role;
            InitializeForm();
        }

        private void InitializeForm()
        {
            if (role != null)
            {
                pageTitle.Text = "Редактирование роли";
                txtName.Text = role.Name;
                btnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                pageTitle.Text = "Новая роль";
                role = new Role();
            }

            txtName.Focus();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Название роли не может быть пустым");
                txtName.Focus();
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            string name = txtName.Text.Trim();

            try
            {
                Database database = Database.GetDatabase();

                if (role.Id > 0)
                {
                    // Update existing role
                    string updateQuery = @"UPDATE ""roles"" SET ""name"" = $1 WHERE ""id"" = $2";
                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = name },
                        new NpgsqlParameter { Value = role.Id }
                    };

                    await database.ExecuteNonQuery(updateQuery, parameters);
                    MessageBox.Show("Роль успешно обновлена");
                }
                else
                {
                    // Create new role
                    string insertQuery = @"INSERT INTO ""roles"" (""name"") VALUES ($1)";
                    var parameter = new NpgsqlParameter { Value = name };

                    await database.ExecuteNonQuery(insertQuery, parameter);
                    MessageBox.Show("Роль успешно добавлена");
                }

                _mainWindow.mainFrame.Navigate(new RolesPage(_mainWindow));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (role == null || role.Id == 0) return;

            if (MessageBox.Show("Вы действительно хотите удалить эту роль?", "Подтверждение удаления",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Database database = Database.GetDatabase();
                    string deleteQuery = @"DELETE FROM ""roles"" WHERE ""id"" = $1";
                    var parameter = new NpgsqlParameter { Value = role.Id };

                    await database.ExecuteNonQuery(deleteQuery, parameter);
                    MessageBox.Show("Роль успешно удалена");

                    _mainWindow.mainFrame.Navigate(new RolesPage(_mainWindow));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении роли: {ex.Message}");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new RolesPage(_mainWindow));
        }
    }
}