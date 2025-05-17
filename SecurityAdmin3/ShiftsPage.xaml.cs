// SecurityAdmin3/ShiftsPage.xaml.cs
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Npgsql;
using SecurityAdmin3.Objects;
using System.Threading.Tasks;

namespace SecurityAdmin3
{
    public partial class ShiftsPage : Page
    {
        public MainWindow _mainWindow;
        public List<Shift> shifts = new List<Shift>();

        public ShiftsPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Loaded += ShiftsPage_Loaded;
        }

        private async void ShiftsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadShiftsAsync();
            calendar.DisplayDateStart = DateTime.Today.AddMonths(-3);
            calendar.DisplayDateEnd = DateTime.Today.AddMonths(3);
            calendar.SelectedDate = DateTime.Today;
        }

        private async Task LoadShiftsAsync()
        {
            try
            {
                shifts.Clear();
                var query = @"SELECT s.*, 
                     e.surname || ' ' || e.name || COALESCE(' ' || e.patronymic, '') as employeename,
                     c.""TITLE"" as clientname,
                     ct.""contractnumber""
                     FROM ""shifts"" s
                     JOIN ""employees"" e ON s.""employeeid"" = e.""id""
                     JOIN ""clients"" c ON s.""clientid"" = c.""ID""
                     JOIN ""contracts"" ct ON s.""contractid"" = ct.""id""
                     ORDER BY s.""date"", s.""timeinterval""";

                var result = await Database.GetDatabase().Select(query);
                foreach (var item in result)
                {
                    shifts.Add(new Shift(item));
                }

                UpdateCalendar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки графика дежурств: {ex.Message}");
            }
        }

        private void UpdateCalendar()
        {
            calendar.BlackoutDates.Clear();

            foreach (var shift in shifts)
            {
                // Можно добавить выделение дат с дежурствами, если нужно
                // calendar.BlackoutDates.Add(new CalendarDateRange(shift.Date));
            }

            UpdateShiftsList();
        }

        private void UpdateShiftsList()
        {
            if (calendar.SelectedDate.HasValue)
            {
                var selectedDate = calendar.SelectedDate.Value;
                shiftsListView.ItemsSource = shifts.FindAll(s => s.Date.Date == selectedDate.Date);
            }
        }

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateShiftsList();
        }

        private async void btnAddShift_Click(object sender, RoutedEventArgs e)
        {
            if (calendar.SelectedDate.HasValue)
            {
                var shiftDate = calendar.SelectedDate.Value;
                var shiftDialog = new ShiftEditDialog(_mainWindow, null, shiftDate);
                if (shiftDialog.ShowDialog() == true)
                {
                    await LoadShiftsAsync();
                }
            }
        }

        private async void btnEditShift_Click(object sender, RoutedEventArgs e)
        {
            if (shiftsListView.SelectedItem is Shift selectedShift)
            {
                var shiftDialog = new ShiftEditDialog(_mainWindow, selectedShift);
                if (shiftDialog.ShowDialog() == true)
                {
                    await LoadShiftsAsync();
                }
            }
            else
            {
                MessageBox.Show("Выберите дежурство для редактирования");
            }
        }

        private async void btnDeleteShift_Click(object sender, RoutedEventArgs e)
        {
            if (shiftsListView.SelectedItem is Shift selectedShift)
            {
                if (MessageBox.Show("Удалить выбранное дежурство?", "Подтверждение",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = "DELETE FROM \"shifts\" WHERE \"id\" = $1";
                        var parameter = new NpgsqlParameter { Value = selectedShift.Id };
                        await Database.GetDatabase().ExecuteNonQuery(query, parameter);
                        await LoadShiftsAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите дежурство для удаления");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new MainPage());
        }
    }

    public class Shift
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public string TimeInterval { get; set; }
        public int ContractId { get; set; }
        public string ContractNumber { get; set; }

        public Shift(Dictionary<string, object> shift)
        {
            Id = (int)shift["id"];
            ClientId = (int)shift["clientid"];
            ClientName = shift["clientname"].ToString();
            EmployeeId = (int)shift["employeeid"];
            EmployeeName = shift["employeename"].ToString();
            Date = (DateTime)shift["date"];
            TimeInterval = shift["timeinterval"].ToString();
            ContractId = (int)shift["contractid"];
            ContractNumber = shift["contractnumber"].ToString();
        }
    }
}