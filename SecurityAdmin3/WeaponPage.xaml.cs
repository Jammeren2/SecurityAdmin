using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Npgsql;
using System.Collections.Generic;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class WeaponPage : Page
    {
        public MainWindow _mainWindow;
        public Weapon weapon;

        public WeaponPage(MainWindow mainWindow, Weapon weapon)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.weapon = weapon;
            InitializeForm();
        }

        private void InitializeForm()
        {
            if (weapon != null)
            {
                pageTitle.Text = "Редактирование оружия";
                txtBrand.Text = weapon.Brand;
                txtRegNumber.Text = weapon.RegNumber;
                btnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                pageTitle.Text = "Новое оружие";
                weapon = new Weapon();
            }
            txtBrand.Focus();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtBrand.Text))
            {
                MessageBox.Show("Марка оружия не может быть пустой");
                txtBrand.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtRegNumber.Text))
            {
                MessageBox.Show("Регистрационный номер не может быть пустым");
                txtRegNumber.Focus();
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            string brand = txtBrand.Text.Trim();
            string regNumber = txtRegNumber.Text.Trim();

            try
            {
                Database database = Database.GetDatabase();
                if (weapon.Id > 0)
                {
                    // Update existing weapon
                    string updateQuery = @"UPDATE ""weapons"" SET ""brand"" = $1, ""regnumber"" = $2 WHERE ""id"" = $3";
                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = brand },
                        new NpgsqlParameter { Value = regNumber },
                        new NpgsqlParameter { Value = weapon.Id }
                    };
                    await database.ExecuteNonQuery(updateQuery, parameters);
                    MessageBox.Show("Оружие успешно обновлено");
                }
                else
                {
                    // Create new weapon
                    string insertQuery = @"INSERT INTO ""weapons"" (""brand"", ""regnumber"") VALUES ($1, $2)";
                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = brand },
                        new NpgsqlParameter { Value = regNumber }
                    };
                    await database.ExecuteNonQuery(insertQuery, parameters);
                    MessageBox.Show("Оружие успешно добавлено");
                }
                _mainWindow.mainFrame.Navigate(new WeaponsPage(_mainWindow));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (weapon == null || weapon.Id == 0) return;

            if (MessageBox.Show("Вы действительно хотите удалить это оружие?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Database database = Database.GetDatabase();
                    string deleteQuery = @"DELETE FROM ""weapons"" WHERE ""id"" = $1";
                    var parameter = new NpgsqlParameter { Value = weapon.Id };
                    await database.ExecuteNonQuery(deleteQuery, parameter);
                    MessageBox.Show("Оружие успешно удалено");
                    _mainWindow.mainFrame.Navigate(new WeaponsPage(_mainWindow));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении оружия: {ex.Message}");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new WeaponsPage(_mainWindow));
        }
    }
}