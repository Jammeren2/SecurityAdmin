// SecurityAdmin3/ShiftEditDialog.xaml.cs
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ControlzEx.Standard;
using DocumentFormat.OpenXml.VariantTypes;
using Npgsql;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class ShiftEditDialog : Window
    {
        private MainWindow _mainWindow;
        private Shift _shift;
        private DateTime _shiftDate;
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<Contract> Contracts { get; set; } = new List<Contract>();

        public ShiftEditDialog(MainWindow mainWindow, Shift shift, DateTime? shiftDate = null)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _shift = shift;
            _shiftDate = shiftDate ?? DateTime.Today;

            if (_shift == null)
            {
                Title = "Новое дежурство";
                dpDate.SelectedDate = _shiftDate;
            }
            else
            {
                Title = "Редактирование дежурства";
                dpDate.SelectedDate = _shift.Date;
                cbTimeInterval.Text = _shift.TimeInterval;
            }

            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                // Load employees
                var empQuery = "SELECT * FROM \"employees\" ORDER BY \"surname\", \"name\"";
                var empResult = await Database.GetDatabase().Select(empQuery);
                Employees.Clear();
                foreach (var item in empResult)
                {
                    Employees.Add(new Employee(item));
                }
                cbEmployee.ItemsSource = Employees;

                // Load clients
                var clientQuery = "SELECT * FROM \"clients\" ORDER BY \"TITLE\"";
                var clientResult = await Database.GetDatabase().Select(clientQuery);
                Clients.Clear();
                foreach (var item in clientResult)
                {
                    bool isPhysical = item["FIZ_LITSO"] is bool && (bool)item["FIZ_LITSO"];
                    if (isPhysical)
                    {
                        Clients.Add(new ClientFizlico(item));
                    }
                    else
                    {
                        Clients.Add(new ClientJurlico(item));
                    }
                }
                cbClient.ItemsSource = Clients;

                // Load contracts
                var contractQuery = "SELECT * FROM \"contracts\" ORDER BY \"contractnumber\"";
                var contractResult = await Database.GetDatabase().Select(contractQuery);
                Contracts.Clear();
                foreach (var item in contractResult)
                {
                    Contracts.Add(new SecurityAdmin3.Objects.Contract(item));
                }

                cbContract.ItemsSource = Contracts;

                if (_shift != null)
                {
                    cbEmployee.SelectedValue = _shift.EmployeeId;
                    cbClient.SelectedValue = _shift.ClientId;
                    cbContract.SelectedValue = _shift.ContractId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            if (dpDate.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату дежурства");
                return false;
            }

            if (string.IsNullOrWhiteSpace(cbTimeInterval.Text))
            {
                MessageBox.Show("Укажите временной интервал");
                return false;
            }

            if (cbEmployee.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника");
                return false;
            }

            if (cbClient.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента");
                return false;
            }

            if (cbContract.SelectedItem == null)
            {
                MessageBox.Show("Выберите договор");
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            DateTime date = dpDate.SelectedDate.Value;
            string timeInterval = cbTimeInterval.Text;
            int employeeId = ((Employee)cbEmployee.SelectedItem).Id;
            int clientId = ((Client)cbClient.SelectedItem).Id;
            int contractId = ((Contract)cbContract.SelectedItem).Id;

            try
            {
                if (_shift == null)
                {
                    // Create new shift
                    string query = @"INSERT INTO ""shifts"" 
                                   (""clientid"", ""employeeid"", ""date"", ""timeinterval"", ""contractid"") 
                                   VALUES ($1, $2, $3, $4, $5)";

                    var parameters = new NpgsqlParameter[] {
                        new NpgsqlParameter { Value = clientId },
                        new NpgsqlParameter { Value = employeeId },
                        new NpgsqlParameter { Value = date },
                        new NpgsqlParameter { Value = timeInterval },
                        new NpgsqlParameter { Value = contractId }
                    };

                    await Database.GetDatabase().ExecuteNonQuery(query, parameters);
                }
                else
                {
                    // Update existing shift
                    string query = @"UPDATE ""shifts"" SET 
                                   ""clientid"" = $1, 
                                   ""employeeid"" = $2, 
                                   ""date"" = $3, 
                                   ""timeinterval"" = $4, 
                                   ""contractid"" = $5 
                                   WHERE ""id"" = $6";

                    var parameters = new NpgsqlParameter[] {
                        new NpgsqlParameter { Value = clientId },
                        new NpgsqlParameter { Value = employeeId },
                        new NpgsqlParameter { Value = date },
                        new NpgsqlParameter { Value = timeInterval },
                        new NpgsqlParameter { Value = contractId },
                        new NpgsqlParameter { Value = _shift.Id }
                    };

                    await Database.GetDatabase().ExecuteNonQuery(query, parameters);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}