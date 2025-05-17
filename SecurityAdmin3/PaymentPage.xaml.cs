// SecurityAdmin3/PaymentPage.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Npgsql;
using System.Collections.Generic;
using SecurityAdmin3.Objects;
using System.Threading.Tasks;

namespace SecurityAdmin3
{
    public partial class PaymentPage : Page
    {
        public MainWindow _mainWindow;
        public Payment payment;
        private List<Client> clients = new List<Client>();
        private List<Contract> contracts = new List<Contract>();

        public PaymentPage(MainWindow mainWindow, Payment payment)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.payment = payment;
            LoadClientsAsync();
            InitializeForm();
        }

        private async void LoadClientsAsync()
        {
            try
            {
                clients.Clear();
                var query = @"SELECT ""ID"", 
                     CASE 
                         WHEN ""FIZ_LITSO"" = true THEN ""SURNAME"" || ' ' || ""NAME"" || COALESCE(' ' || ""PATRONYMIC"", '') 
                         ELSE ""TITLE"" 
                     END as ""TITLE"", 
                     ""FIZ_LITSO"" 
                     FROM ""clients"" 
                     ORDER BY ""TITLE""";
                var result = await Database.GetDatabase().Select(query);
                foreach (var item in result)
                {
                    bool isPhysical = (bool)item["FIZ_LITSO"];
                    Client client = isPhysical ? (Client)new ClientFizlico(item) : (Client)new ClientJurlico(item);
                    clients.Add(client);
                }
                cbClient.ItemsSource = clients;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}");
            }
        }

        private async Task LoadContractsForClient(int clientId)
        {
            try
            {
                contracts.Clear();
                var query = @"SELECT * FROM ""contracts"" WHERE ""ClientID"" = $1 ORDER BY ""contractnumber""";
                var parameters = new NpgsqlParameter[] {
            new NpgsqlParameter { Value = clientId }
        };

                var result = await Database.GetDatabase().Select(query, parameters);

                foreach (var item in result)
                {
                    contracts.Add(new Contract(item));
                }

                cbContract.ItemsSource = contracts;
                cbContract.IsEnabled = contracts.Count > 0;

                if (contracts.Count == 0)
                {
                    MessageBox.Show("У выбранного клиента нет договоров");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки договоров: {ex.Message}");
            }
        }

        private async void InitializeForm()
        {
            if (payment != null)
            {
                pageTitle.Text = "Редактирование платежа";
                txtAmount.Text = payment.Amount.ToString("N2");
                dpPaymentDate.SelectedDate = payment.PaymentDate;

                // First load the client
                foreach (Client client in cbClient.Items)
                {
                    if (client.Id == payment.ClientId)
                    {
                        cbClient.SelectedItem = client;
                        // Now load contracts for this client
                        await LoadContractsForClient(client.Id);
                        break;
                    }
                }

                btnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                pageTitle.Text = "Новый платеж";
                payment = new Payment();
                dpPaymentDate.SelectedDate = DateTime.Today;
            }

            cbClient.Focus();
        }

        private bool ValidateInput()
        {


            if (cbClient.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать клиента");
                cbClient.Focus();
                return false;
            }

            if (cbContract.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать договор");
                cbContract.Focus();
                return false;
            }

            if (dpPaymentDate.SelectedDate == null)
            {
                MessageBox.Show("Необходимо указать дату платежа");
                dpPaymentDate.Focus();
                return false;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Некорректное значение суммы");
                txtAmount.Focus();
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            int clientId = ((Client)cbClient.SelectedItem).Id;
            int contractId = ((Contract)cbContract.SelectedItem).Id;
            decimal amount = decimal.Parse(txtAmount.Text);
            DateTime paymentDate = dpPaymentDate.SelectedDate ?? DateTime.Today;

            try
            {
                Database database = Database.GetDatabase();

                if (payment.Id > 0)
                {
                    // Update existing payment
                    string updateQuery = @"UPDATE ""payments"" SET ""clientid"" = $1, ""contractid"" = $2, ""amount"" = $3, ""paymentdate"" = $4 WHERE ""id"" = $5";
                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = clientId },
                        new NpgsqlParameter { Value = contractId },
                        new NpgsqlParameter { Value = amount },
                        new NpgsqlParameter { Value = paymentDate },
                        new NpgsqlParameter { Value = payment.Id }
                    };

                    await database.ExecuteNonQuery(updateQuery, parameters);
                    MessageBox.Show("Платеж успешно обновлен");
                }
                else
                {
                    // Create new payment
                    string insertQuery = @"INSERT INTO ""payments"" (""clientid"", ""contractid"", ""amount"", ""paymentdate"") VALUES ($1, $2, $3, $4)";
                    MessageBox.Show(insertQuery);
                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = clientId },
                        new NpgsqlParameter { Value = contractId },
                        new NpgsqlParameter { Value = amount },
                        new NpgsqlParameter { Value = paymentDate }
                    };
                    MessageBox.Show(Convert.ToString(parameters));
                    await database.ExecuteNonQuery(insertQuery, parameters);
                    MessageBox.Show("Платеж успешно добавлен");
                }

                _mainWindow.mainFrame.Navigate(new PaymentsPage(_mainWindow));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (payment == null || payment.Id == 0) return;

            if (MessageBox.Show("Вы действительно хотите удалить этот платеж?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Database database = Database.GetDatabase();
                    string deleteQuery = @"DELETE FROM ""payments"" WHERE ""id"" = $1";
                    var parameter = new NpgsqlParameter { Value = payment.Id };

                    await database.ExecuteNonQuery(deleteQuery, parameter);
                    MessageBox.Show("Платеж успешно удален");

                    _mainWindow.mainFrame.Navigate(new PaymentsPage(_mainWindow));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении платежа: {ex.Message}");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new PaymentsPage(_mainWindow));
        }

        private async void cbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbClient.SelectedItem is Client selectedClient)
            {
                await LoadContractsForClient(selectedClient.Id);

                if (payment != null && payment.Id > 0)
                {
                    foreach (Contract contract in cbContract.Items)
                    {
                        if (contract.Id == payment.ContractId)
                        {
                            cbContract.SelectedItem = contract;
                            break;
                        }
                    }
                }
                else
                {
                    cbContract.SelectedItem = null;
                }
            }
        }
    }
}