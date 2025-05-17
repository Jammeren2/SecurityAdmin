using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Npgsql;
using System.Collections.Generic;
using SecurityAdmin3.Objects;
using System;

namespace SecurityAdmin3
{
    public partial class PositionPage : Page
    {
        public MainWindow _mainWindow;
        public Position position;
        public List<Role> roles = new List<Role>();

        public PositionPage(MainWindow mainWindow, Position position)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.position = position;
            LoadRolesAsync();
            InitializeForm();
        }

        private async void LoadRolesAsync()
        {
            try
            {
                roles.Clear();
                var query = "SELECT * FROM roles ORDER BY id";
                var result = await Database.GetDatabase().Select(query);
                foreach (var item in result)
                {
                    roles.Add(new Role(item));
                }
                cmbRoles.ItemsSource = roles;

                if (position != null && position.RoleId > 0)
                {
                    cmbRoles.SelectedValue = position.RoleId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ролей: {ex.Message}");
            }
        }

        private void InitializeForm()
        {
            if (position != null)
            {
                pageTitle.Text = "Редактирование должности";
                txtName.Text = position.Name;
                txtSalary.Text = position.Salary.ToString();

                foreach (Role role in cmbRoles.Items)
                {
                    if (role.Id == position.RoleId)
                    {
                        cmbRoles.SelectedItem = role;
                        break;
                    }
                }

                btnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                pageTitle.Text = "Новая должность";
                position = new Position();
            }
            txtName.Focus();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Название должности не может быть пустым");
                txtName.Focus();
                return false;
            }

            if (!decimal.TryParse(txtSalary.Text, out decimal salary) || salary <= 0)
            {
                MessageBox.Show("Укажите корректную зарплату");
                txtSalary.Focus();
                return false;
            }

            if (cmbRoles.SelectedItem == null)
            {
                MessageBox.Show("Выберите роль");
                cmbRoles.Focus();
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            string name = txtName.Text.Trim();
            decimal salary = decimal.Parse(txtSalary.Text);
            int roleId = ((Role)cmbRoles.SelectedItem).Id;

            try
            {
                Database database = Database.GetDatabase();
                if (position.Id > 0)
                {
                    // Update existing position
                    string updateQuery = @"UPDATE positions SET name = $1, salary = $2, roleid = $3 WHERE id = $4";
                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = name },
                        new NpgsqlParameter { Value = salary },
                        new NpgsqlParameter { Value = roleId },
                        new NpgsqlParameter { Value = position.Id }
                    };
                    await database.ExecuteNonQuery(updateQuery, parameters);
                    MessageBox.Show("Должность успешно обновлена");
                }
                else
                {
                    // Create new position
                    string insertQuery = @"INSERT INTO positions (name, salary, roleid) VALUES ($1, $2, $3)";
                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = name },
                        new NpgsqlParameter { Value = salary },
                        new NpgsqlParameter { Value = roleId }
                    };
                    await database.ExecuteNonQuery(insertQuery, parameters);
                    MessageBox.Show("Должность успешно добавлена");
                }
                _mainWindow.mainFrame.Navigate(new PositionsPage(_mainWindow));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (position == null || position.Id == 0) return;

            if (MessageBox.Show("Вы действительно хотите удалить эту должность?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Database database = Database.GetDatabase();
                    string deleteQuery = @"DELETE FROM positions WHERE id = $1";
                    var parameter = new NpgsqlParameter { Value = position.Id };
                    await database.ExecuteNonQuery(deleteQuery, parameter);
                    MessageBox.Show("Должность успешно удалена");
                    _mainWindow.mainFrame.Navigate(new PositionsPage(_mainWindow));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении должности: {ex.Message}");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new PositionsPage(_mainWindow));
        }
    }
}