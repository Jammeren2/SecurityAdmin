using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Npgsql;
using System.Collections.Generic;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class ContractPage : Page
    {
        public MainWindow _mainWindow;
        public Contract contract;
        private List<Client> clients = new List<Client>();
        private List<SecurityObject> objects = new List<SecurityObject>();

        public ContractPage(MainWindow mainWindow, Contract contract)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.contract = contract;
            LoadClientsAndObjectsAsync();
            InitializeForm();
        }

        private async void LoadClientsAndObjectsAsync()
        {
            try
            {
                // Загрузка клиентов
                if (clients.Count == 0)
                {
                    // Измененный запрос - убрали условие CASE WHEN, так как оно мешает отображению физ. лиц
                    var clientsQuery = @"SELECT ""ID"", 
                                CASE WHEN ""FIZ_LITSO"" = true 
                                     THEN ""SURNAME"" || ' ' || ""NAME"" || COALESCE(' ' || ""PATRONYMIC"", '') 
                                     ELSE ""TITLE"" 
                                END as ""TITLE"", 
                                ""FIZ_LITSO"" 
                                FROM ""clients"" 
                                ORDER BY ""TITLE""";

                    var clientsResult = await Database.GetDatabase().Select(clientsQuery);
                    clients.Clear();

                    foreach (var item in clientsResult)
                    {
                        bool isPhysical = (bool)item["FIZ_LITSO"];
                        Client client = isPhysical ?
                            (Client)new ClientFizlico(item) :
                            (Client)new ClientJurlico(item);
                        clients.Add(client);
                    }
                    cbClient.ItemsSource = clients;
                }

                // Загрузка объектов
                if (objects.Count == 0)
                {
                    var objectsQuery = "SELECT * FROM \"objects\" ORDER BY \"name\"";
                    var objectsResult = await Database.GetDatabase().Select(objectsQuery);
                    objects.Clear();
                    foreach (var item in objectsResult)
                    {
                        objects.Add(new SecurityObject(item));
                    }
                    cbObject.ItemsSource = objects;
                }

                // Инициализация формы после загрузки данных
                InitializeForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void InitializeForm()
        {
            if (contract != null)
            {
                pageTitle.Text = "Редактирование договора";
                txtContractNumber.Text = contract.ContractNumber;
                dpStartDate.SelectedDate = contract.StartDate;
                dpEndDate.SelectedDate = contract.EndDate;
                txtPrice.Text = contract.Price.ToString("N2");
                txtDescription.Text = contract.Description;
                txtAddress.Text = contract.Address;

                // Установка выбранного клиента
                if (contract.ClientId > 0)
                {
                    cbClient.SelectedValue = contract.ClientId;
                }

                // Установка выбранного объекта
                if (contract.ObjectId > 0)
                {
                    cbObject.SelectedValue = contract.ObjectId;
                }

                btnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                pageTitle.Text = "Новый договор";
                contract = new Contract();
                dpStartDate.SelectedDate = DateTime.Today;
                dpEndDate.SelectedDate = DateTime.Today.AddYears(1);
            }
            txtContractNumber.Focus();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtContractNumber.Text))
            {
                MessageBox.Show("Номер договора не может быть пустым");
                txtContractNumber.Focus();
                return false;
            }

            if (cbClient.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать клиента");
                cbClient.Focus();
                return false;
            }

            if (cbObject.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать объект");
                cbObject.Focus();
                return false;
            }

            if (dpStartDate.SelectedDate == null)
            {
                MessageBox.Show("Необходимо указать дату начала");
                dpStartDate.Focus();
                return false;
            }

            if (dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Необходимо указать дату окончания");
                dpEndDate.Focus();
                return false;
            }

            if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания");
                dpStartDate.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Некорректное значение стоимости");
                txtPrice.Focus();
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            string contractNumber = txtContractNumber.Text.Trim();
            int clientId = ((Client)cbClient.SelectedItem).Id;
            int objectId = ((SecurityObject)cbObject.SelectedItem).Id;
            DateTime startDate = dpStartDate.SelectedDate ?? DateTime.Today;
            DateTime endDate = dpEndDate.SelectedDate ?? DateTime.Today.AddYears(1);
            decimal price = decimal.Parse(txtPrice.Text);
            string description = txtDescription.Text.Trim();
            string address = txtAddress.Text.Trim();

            try
            {
                Database database = Database.GetDatabase();
                if (contract.Id > 0)
                {
                    // Update existing contract
                    string updateQuery = @"UPDATE ""contracts"" SET 
                                        ""contractnumber"" = $1, 
                                        ""ClientID"" = $2, 
                                        ""ObjectID"" = $3, 
                                        ""startdate"" = $4, 
                                        ""enddate"" = $5, 
                                        ""price"" = $6, 
                                        ""description"" = $7, 
                                        ""address"" = $8 
                                        WHERE ""id"" = $9";
                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = contractNumber },
                        new NpgsqlParameter { Value = clientId },
                        new NpgsqlParameter { Value = objectId },
                        new NpgsqlParameter { Value = startDate },
                        new NpgsqlParameter { Value = endDate },
                        new NpgsqlParameter { Value = price },
                        new NpgsqlParameter { Value = string.IsNullOrEmpty(description) ? DBNull.Value : (object)description },
                        new NpgsqlParameter { Value = string.IsNullOrEmpty(address) ? DBNull.Value : (object)address },
                        new NpgsqlParameter { Value = contract.Id }
                    };
                    await database.ExecuteNonQuery(updateQuery, parameters);
                    MessageBox.Show("Договор успешно обновлен");
                }
                else
                {
                    // Create new contract
                    string insertQuery = @"INSERT INTO ""contracts"" 
                                        (""contractnumber"", ""ClientID"", ""ObjectID"", ""startdate"", ""enddate"", ""price"", ""description"", ""address"") 
                                        VALUES ($1, $2, $3, $4, $5, $6, $7, $8)";
                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = contractNumber },
                        new NpgsqlParameter { Value = clientId },
                        new NpgsqlParameter { Value = objectId },
                        new NpgsqlParameter { Value = startDate },
                        new NpgsqlParameter { Value = endDate },
                        new NpgsqlParameter { Value = price },
                        new NpgsqlParameter { Value = string.IsNullOrEmpty(description) ? DBNull.Value : (object)description },
                        new NpgsqlParameter { Value = string.IsNullOrEmpty(address) ? DBNull.Value : (object)address }
                    };
                    await database.ExecuteNonQuery(insertQuery, parameters);
                    MessageBox.Show("Договор успешно добавлен");
                }
                _mainWindow.mainFrame.Navigate(new ContractsPage(_mainWindow));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (contract == null || contract.Id == 0) return;

            if (MessageBox.Show("Вы действительно хотите удалить этот договор?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Database database = Database.GetDatabase();
                    string deleteQuery = @"DELETE FROM ""contracts"" WHERE ""id"" = $1";
                    var parameter = new NpgsqlParameter { Value = contract.Id };
                    await database.ExecuteNonQuery(deleteQuery, parameter);
                    MessageBox.Show("Договор успешно удален");
                    _mainWindow.mainFrame.Navigate(new ContractsPage(_mainWindow));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении договора: {ex.Message}");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new ContractsPage(_mainWindow));
        }
    }
}