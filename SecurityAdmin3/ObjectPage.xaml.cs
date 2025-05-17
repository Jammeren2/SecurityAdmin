using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Npgsql;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class ObjectPage : Page
    {
        public MainWindow _mainWindow;
        public SecurityObject securityObject;

        public ObjectPage(MainWindow mainWindow, SecurityObject obj)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.securityObject = obj;
            InitializeForm();
        }

        private void InitializeForm()
        {
            if (securityObject != null)
            {
                pageTitle.Text = "Редактирование объекта";
                txtName.Text = securityObject.Name;
                txtCoefficient.Text = securityObject.Coefficient.ToString();
                txtPricePerHour.Text = securityObject.PricePerHour.ToString();
                btnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                pageTitle.Text = "Новый объект";
                securityObject = new SecurityObject();
            }
            txtName.Focus();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Название объекта не может быть пустым");
                txtName.Focus();
                return false;
            }

            if (!decimal.TryParse(txtCoefficient.Text, out decimal coefficient) || coefficient < 0 || coefficient > 999.99m)
            {
                MessageBox.Show("Коэффициент должен быть числом от 0 до 999.99");
                txtCoefficient.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPricePerHour.Text, out decimal pricePerHour) || pricePerHour < 0 || pricePerHour > 99999999.99m)
            {
                MessageBox.Show("Цена за час должна быть числом от 0 до 99,999,999.99");
                txtPricePerHour.Focus();
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            string name = txtName.Text.Trim();
            decimal coefficient = decimal.Parse(txtCoefficient.Text);
            decimal pricePerHour = decimal.Parse(txtPricePerHour.Text);

            try
            {
                Database database = Database.GetDatabase();
                if (securityObject.Id > 0)
                {
                    // Update existing object
                    string updateQuery = @"UPDATE ""objects"" SET ""name"" = $1, ""coefficient"" = $2, ""priceperhour"" = $3 WHERE ""id"" = $4";
                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = name },
                        new NpgsqlParameter { Value = coefficient },
                        new NpgsqlParameter { Value = pricePerHour },
                        new NpgsqlParameter { Value = securityObject.Id }
                    };
                    await database.ExecuteNonQuery(updateQuery, parameters);
                    MessageBox.Show("Объект успешно обновлен");
                }
                else
                {
                    // Create new object
                    string insertQuery = @"INSERT INTO ""objects"" (""name"", ""coefficient"", ""priceperhour"") VALUES ($1, $2, $3)";
                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = name },
                        new NpgsqlParameter { Value = coefficient },
                        new NpgsqlParameter { Value = pricePerHour }
                    };
                    await database.ExecuteNonQuery(insertQuery, parameters);
                    MessageBox.Show("Объект успешно добавлен");
                }
                _mainWindow.mainFrame.Navigate(new ObjectsPage(_mainWindow));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (securityObject == null || securityObject.Id == 0) return;

            if (MessageBox.Show("Вы действительно хотите удалить этот объект?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Database database = Database.GetDatabase();
                    string deleteQuery = @"DELETE FROM ""objects"" WHERE ""id"" = $1";
                    var parameter = new NpgsqlParameter { Value = securityObject.Id };
                    await database.ExecuteNonQuery(deleteQuery, parameter);
                    MessageBox.Show("Объект успешно удален");
                    _mainWindow.mainFrame.Navigate(new ObjectsPage(_mainWindow));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении объекта: {ex.Message}");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new ObjectsPage(_mainWindow));
        }
    }
}