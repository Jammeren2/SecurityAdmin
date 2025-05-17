// SecurityAdmin3/EmployeePage.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Npgsql;
using System.Collections.Generic;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class EmployeePage : Page
    {
        public MainWindow _mainWindow;
        public Employee employee;
        private List<Position> positions = new List<Position>();

        public EmployeePage(MainWindow mainWindow, Employee employee)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.employee = employee;
            LoadPositionsAsync();
            InitializeForm();
        }

        private async void LoadPositionsAsync()
        {
            try
            {
                positions.Clear();
                var query = "SELECT * FROM \"positions\" ORDER BY \"name\"";
                var result = await Database.GetDatabase().Select(query);

                foreach (var item in result)
                {
                    positions.Add(new Position(item));
                }

                cbPosition.ItemsSource = positions;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки должностей: {ex.Message}");
            }
        }

        private void InitializeForm()
        {
            if (employee != null)
            {
                pageTitle.Text = "Редактирование сотрудника";
                txtSurname.Text = employee.Surname;
                txtName.Text = employee.Name;
                txtPatronymic.Text = employee.Patronymic;
                txtAddress.Text = employee.Address;

                foreach (Position position in cbPosition.Items)
                {
                    if (position.Id == employee.PositionId)
                    {
                        cbPosition.SelectedItem = position;
                        break;
                    }
                }

                cbWeaponPermit.IsChecked = employee.WeaponPermit;
                txtIdCard.Text = employee.IdCard;
                txtInn.Text = employee.Inn;
                txtPfr.Text = employee.Pfr;
                txtBonus.Text = employee.Bonus.ToString("0.00");
                dpHiredDate.SelectedDate = employee.HiredDate;
                dpFiredDate.SelectedDate = employee.FiredDate;
                txtLicense.Text = employee.License;
                txtLogin.Text = employee.Login;
                txtPassword.Password = employee.Password;

                btnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                pageTitle.Text = "Новый сотрудник";
                employee = new Employee();
                dpHiredDate.SelectedDate = DateTime.Today;
            }

            txtSurname.Focus();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtSurname.Text))
            {
                MessageBox.Show("Фамилия не может быть пустой");
                txtSurname.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Имя не может быть пустым");
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Адрес не может быть пустым");
                txtAddress.Focus();
                return false;
            }

            if (cbPosition.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать должность");
                cbPosition.Focus();
                return false;
            }

            if (dpHiredDate.SelectedDate == null)
            {
                MessageBox.Show("Необходимо указать дату приема");
                dpHiredDate.Focus();
                return false;
            }

            if (!decimal.TryParse(txtBonus.Text, out decimal bonus))
            {
                MessageBox.Show("Некорректное значение премии");
                txtBonus.Focus();
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            string surname = txtSurname.Text.Trim();
            string name = txtName.Text.Trim();
            string patronymic = string.IsNullOrWhiteSpace(txtPatronymic.Text) ? null : txtPatronymic.Text.Trim();
            string address = txtAddress.Text.Trim();
            int positionId = ((Position)cbPosition.SelectedItem).Id;
            bool weaponPermit = cbWeaponPermit.IsChecked ?? false;
            string idCard = string.IsNullOrWhiteSpace(txtIdCard.Text) ? null : txtIdCard.Text.Trim();
            string inn = string.IsNullOrWhiteSpace(txtInn.Text) ? null : txtInn.Text.Trim();
            string pfr = string.IsNullOrWhiteSpace(txtPfr.Text) ? null : txtPfr.Text.Trim();
            decimal bonus = decimal.Parse(txtBonus.Text);
            DateTime hiredDate = dpHiredDate.SelectedDate ?? DateTime.Today;
            DateTime? firedDate = dpFiredDate.SelectedDate;
            string license = string.IsNullOrWhiteSpace(txtLicense.Text) ? null : txtLicense.Text.Trim();
            string login = string.IsNullOrWhiteSpace(txtLogin.Text) ? null : txtLogin.Text.Trim();
            string password = string.IsNullOrWhiteSpace(txtPassword.Password) ? null : txtPassword.Password;

            try
            {
                Database database = Database.GetDatabase();

                if (employee.Id > 0)
                {
                    // Update existing employee
                    string updateQuery = @"UPDATE ""employees"" SET 
                                        ""surname"" = $1, ""name"" = $2, ""patronymic"" = $3, 
                                        ""address"" = $4, ""positionid"" = $5, ""weaponpermit"" = $6, 
                                        ""idcard"" = $7, ""inn"" = $8, ""pfr"" = $9, ""bonus"" = $10, 
                                        ""hireddate"" = $11, ""fireddate"" = $12, ""license"" = $13, 
                                        ""login"" = $14, ""password"" = $15 
                                        WHERE ""id"" = $16";

                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = surname },
                        new NpgsqlParameter { Value = name },
                        new NpgsqlParameter { Value = patronymic ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = address },
                        new NpgsqlParameter { Value = positionId },
                        new NpgsqlParameter { Value = weaponPermit },
                        new NpgsqlParameter { Value = idCard ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = inn ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = pfr ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = bonus },
                        new NpgsqlParameter { Value = hiredDate },
                        new NpgsqlParameter { Value = firedDate ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = license ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = login ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = password ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = employee.Id }
                    };

                    await database.ExecuteNonQuery(updateQuery, parameters);
                    MessageBox.Show("Сотрудник успешно обновлен");
                }
                else
                {
                    // Create new employee
                    string insertQuery = @"INSERT INTO ""employees"" 
                                        (""surname"", ""name"", ""patronymic"", ""address"", ""positionid"", 
                                        ""weaponpermit"", ""idcard"", ""inn"", ""pfr"", ""bonus"", 
                                        ""hireddate"", ""fireddate"", ""license"", ""login"", ""password"") 
                                        VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14, $15)";

                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = surname },
                        new NpgsqlParameter { Value = name },
                        new NpgsqlParameter { Value = patronymic ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = address },
                        new NpgsqlParameter { Value = positionId },
                        new NpgsqlParameter { Value = weaponPermit },
                        new NpgsqlParameter { Value = idCard ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = inn ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = pfr ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = bonus },
                        new NpgsqlParameter { Value = hiredDate },
                        new NpgsqlParameter { Value = firedDate ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = license ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = login ?? (object)DBNull.Value },
                        new NpgsqlParameter { Value = password ?? (object)DBNull.Value }
                    };

                    await database.ExecuteNonQuery(insertQuery, parameters);
                    MessageBox.Show("Сотрудник успешно добавлен");
                }

                _mainWindow.mainFrame.Navigate(new EmployeesPage(_mainWindow));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (employee == null || employee.Id == 0) return;

            if (MessageBox.Show("Вы действительно хотите удалить этого сотрудника?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Database database = Database.GetDatabase();
                    string deleteQuery = @"DELETE FROM ""employees"" WHERE ""id"" = $1";
                    var parameter = new NpgsqlParameter { Value = employee.Id };

                    await database.ExecuteNonQuery(deleteQuery, parameter);
                    MessageBox.Show("Сотрудник успешно удален");
                    _mainWindow.mainFrame.Navigate(new EmployeesPage(_mainWindow));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении сотрудника: {ex.Message}");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new EmployeesPage(_mainWindow));
        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            if (employee != null && employee.Id > 0)
            {
                _mainWindow.mainFrame.Navigate(new EmployeeInventoryPage(_mainWindow, employee));
            }
            else
            {
                MessageBox.Show("Сначала сохраните сотрудника, чтобы управлять инвентарем");
            }
        }
    }
}