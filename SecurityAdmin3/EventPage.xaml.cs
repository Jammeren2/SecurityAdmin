using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;
using Npgsql;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class EventPage : Page
    {
        public MainWindow _mainWindow;
        public Event eventItem;

        public EventPage(MainWindow mainWindow, Event eventItem)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.eventItem = eventItem;
            InitializeForm();
        }

        private void InitializeForm()
        {
            if (eventItem != null)
            {
                pageTitle.Text = "Редактирование мероприятия";
                txtName.Text = eventItem.Name;
                txtCoefficient.Text = eventItem.Coefficient.ToString();
                txtPricePerHour.Text = eventItem.PricePerHour.ToString();
                btnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                pageTitle.Text = "Новое мероприятие";
                eventItem = new Event();
            }
            txtName.Focus();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Название мероприятия не может быть пустым");
                txtName.Focus();
                return false;
            }

            if (!decimal.TryParse(txtCoefficient.Text, out decimal coefficient) ||
                coefficient < 0 || coefficient > 999.99m)
            {
                MessageBox.Show("Коэффициент должен быть числом от 0 до 999.99");
                txtCoefficient.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPricePerHour.Text, out decimal pricePerHour) ||
                pricePerHour < 0 || pricePerHour > 99999999.99m)
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

            if (!decimal.TryParse(txtCoefficient.Text, out decimal coefficient))
            {
                MessageBox.Show("Некорректное значение коэффициента");
                return;
            }

            if (!decimal.TryParse(txtPricePerHour.Text, out decimal pricePerHour))
            {
                MessageBox.Show("Некорректное значение цены за час");
                return;
            }

            try
            {
                Database database = Database.GetDatabase();

                if (eventItem.Id > 0)
                {
                    // Update existing event
                    string updateQuery = @"UPDATE ""events"" SET ""name"" = $1, ""coefficient"" = $2, ""priceperhour"" = $3 WHERE ""id"" = $4";
                    var parameters = new NpgsqlParameter[] {
                new NpgsqlParameter { Value = name },
                new NpgsqlParameter { Value = coefficient },
                new NpgsqlParameter { Value = pricePerHour },
                new NpgsqlParameter { Value = eventItem.Id }
            };

                    await database.ExecuteNonQuery(updateQuery, parameters);
                    MessageBox.Show("Мероприятие успешно обновлено");
                }
                else
                {
                    // Create new event
                    string insertQuery = @"INSERT INTO ""events"" (""name"", ""coefficient"", ""priceperhour"") VALUES ($1, $2, $3)";
                    var parameters = new NpgsqlParameter[] {
                new NpgsqlParameter { Value = name },
                new NpgsqlParameter { Value = coefficient },
                new NpgsqlParameter { Value = pricePerHour }
            };

                    await database.ExecuteNonQuery(insertQuery, parameters);
                    MessageBox.Show("Мероприятие успешно добавлено");
                }

                _mainWindow.mainFrame.Navigate(new EventsPage(_mainWindow));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (eventItem == null || eventItem.Id == 0) return;

            if (MessageBox.Show("Вы действительно хотите удалить это мероприятие?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Database database = Database.GetDatabase();
                    string deleteQuery = @"DELETE FROM ""events"" WHERE ""id"" = $1";
                    var parameter = new NpgsqlParameter { Value = eventItem.Id };

                    await database.ExecuteNonQuery(deleteQuery, parameter);
                    MessageBox.Show("Мероприятие успешно удалено");

                    _mainWindow.mainFrame.Navigate(new EventsPage(_mainWindow));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении мероприятия: {ex.Message}");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new EventsPage(_mainWindow));
        }
    }
}