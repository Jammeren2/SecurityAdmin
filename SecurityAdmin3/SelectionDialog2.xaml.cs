// SelectionDialog.xaml.cs
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class SelectionDialog2 : Window
    {
        public Employee SelectedEmployee { get; private set; }

        public SelectionDialog2(List<Employee> employees)
        {
            InitializeComponent();
            EmployeesListBox.ItemsSource = employees;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedEmployee = EmployeesListBox.SelectedItem as Employee;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}