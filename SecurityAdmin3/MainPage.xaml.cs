using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class MainPage : Page
    {
        private MainWindow _mainWindow;

        public MainPage()
        {
            InitializeComponent();
            _mainWindow = Application.Current.MainWindow as MainWindow;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new ClientsPage(_mainWindow));
        }

        private void PositionsButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new PositionsPage(_mainWindow));
        }

        private void RolesButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new RolesPage(_mainWindow));
        }

        private void EventsButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new EventsPage(_mainWindow));
        }

        private void SpecialEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new SpecialToolsPage(_mainWindow));
        }

        private void ObjectsButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new ObjectsPage(_mainWindow));
        }

        private void WeaponsButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new WeaponsPage(_mainWindow));
        }

        private void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new EmployeesPage(_mainWindow));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new ContractsPage(_mainWindow));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new PaymentsPage(_mainWindow));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new PaymentPage(_mainWindow, null));
        }

        private async void GenerateFinancialReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _mainWindow.LoadPayments();

                decimal totalAmount = 0m;
                foreach (Payment payment in _mainWindow.payments)
                {
                    totalAmount += payment.Amount;
                }

                if (!File.Exists("blank.xlsx"))
                {
                    MessageBox.Show("Шаблон blank.xlsx не найден в папке приложения");
                    return;
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo("blank.xlsx")))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["стр.1_ф.0710002"];
                    if (worksheet == null)
                    {
                        MessageBox.Show("Лист 'стр.1_ф.0710002' не найден в шаблоне");
                        return;
                    }

                    worksheet.Cells["CD7"].Value = "Дата " + DateTime.Now.ToShortDateString();
                    worksheet.Cells["C29"].Value = DateTime.Now.Day;
                    worksheet.Cells["J29"].Value = DateTime.Now.Month;
                    worksheet.Cells["BN17"].Value = totalAmount;
                    worksheet.Cells["BP22"].Value = totalAmount * 0.13m; // 13% tax
                    worksheet.Cells["BN23"].Value = totalAmount - (totalAmount * 0.13m);

                    string reportFileName = $"Финансовый отчет {DateTime.Now:yyyy-MM-dd HH-mm-ss}.xlsx";
                    excelPackage.SaveAs(new FileInfo(reportFileName));

                    Process.Start(new ProcessStartInfo(reportFileName)
                    {
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}");
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new ShiftsPage(_mainWindow));
        }
    }
}
