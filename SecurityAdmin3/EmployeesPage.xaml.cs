// SecurityAdmin3/EmployeesPage.xaml.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Collections.Generic;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class EmployeesPage : Page
    {
        public MainWindow _mainWindow;
        public List<Employee> employees = new List<Employee>();

        public EmployeesPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadEmployeesAsync();
        }

        private async void LoadEmployeesAsync()
        {
            try
            {
                employees.Clear();
                var query = @"SELECT e.*, p.name as positionname 
                            FROM ""employees"" e 
                            LEFT JOIN ""positions"" p ON e.positionid = p.id 
                            ORDER BY e.""surname"", e.""name""";
                var result = await Database.GetDatabase().Select(query);

                foreach (var item in result)
                {
                    employees.Add(new Employee(item));
                }

                EmployeesGrid.ItemsSource = employees;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new MainPage());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new EmployeePage(_mainWindow, null));
        }

        private void EmployeesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EmployeesGrid.SelectedItem != null)
            {
                Employee employee = (Employee)EmployeesGrid.SelectedItem;
                _mainWindow.mainFrame.Navigate(new EmployeePage(_mainWindow, employee));
            }
        }
    }
}