using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Collections.Generic;
using SecurityAdmin3.Objects;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using Npgsql;
using System;
using System.IO; 
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace SecurityAdmin3
{
    public partial class ContractsPage : Page
    {
        public MainWindow _mainWindow;
        public List<Contract> contracts = new List<Contract>();

        public ContractsPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadContractsAsync();
        }

        private async void LoadContractsAsync()
        {
            try
            {
                contracts.Clear();
                var query = @"SELECT c.*, 
                     CASE 
                         WHEN cl.""FIZ_LITSO"" = true THEN cl.""SURNAME"" || ' ' || cl.""NAME"" || COALESCE(' ' || cl.""PATRONYMIC"", '')
                         ELSE cl.""TITLE""
                     END as clientname, 
                     o.""name"" as objectname 
                     FROM ""contracts"" c 
                     JOIN ""clients"" cl ON c.""ClientID"" = cl.""ID"" 
                     JOIN ""objects"" o ON c.""ObjectID"" = o.""id"" 
                     ORDER BY c.""startdate"" DESC";

                var result = await Database.GetDatabase().Select(query);
                foreach (var item in result)
                {
                    contracts.Add(new Contract(item));
                }
                ContractsGrid.ItemsSource = contracts;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки договоров: {ex.Message}");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new MainPage());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new ContractPage(_mainWindow, null));
        }

        private void ContractsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ContractsGrid.SelectedItem != null)
            {
                Contract contract = (Contract)ContractsGrid.SelectedItem;
                _mainWindow.mainFrame.Navigate(new ContractPage(_mainWindow, contract));
            }
        }

        private async void btnGenerateContract_Click(object sender, RoutedEventArgs e)
        {
            if (ContractsGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите договор для формирования документа");
                return;
            }

            Contract selectedContract = (Contract)ContractsGrid.SelectedItem;

            try
            {
                // Загружаем список сотрудников
                var employeesQuery = @"SELECT e.*, p.name as positionname FROM ""employees"" e 
                             LEFT JOIN ""positions"" p ON e.positionid = p.id 
                             WHERE e.""fireddate"" IS NULL 
                             ORDER BY e.""surname"", e.""name""";
                var employeesResult = await Database.GetDatabase().Select(employeesQuery);
                var employees = new List<Employee>();
                foreach (var item in employeesResult)
                {
                    employees.Add(new Employee(item));
                }

                if (employees.Count == 0)
                {
                    MessageBox.Show("Нет доступных сотрудников для выбора");
                    return;
                }

                // Показываем диалог выбора сотрудника
                var dialog = new SelectionDialog2(employees);
                if (dialog.ShowDialog() == true && dialog.SelectedEmployee != null)
                {
                    // Получаем выбранного сотрудника
                    Employee selectedEmployee = dialog.SelectedEmployee;

                    // Get template path
                    string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dogovor.docx");
                    // Create a copy of the template
                    string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        $"Договор_{selectedContract.ContractNumber}_{DateTime.Now:yyyyMMddHHmmss}.docx");
                    File.Copy(templatePath, outputPath, true);

                    // Open the document and replace placeholders
                    using (WordprocessingDocument doc = WordprocessingDocument.Open(outputPath, true))
                    {
                        // Get client details
                        var clientQuery = @"SELECT * FROM ""clients"" WHERE ""ID"" = $1";
                        var clientResult = await Database.GetDatabase().Select(clientQuery,
                            new NpgsqlParameter { Value = selectedContract.ClientId });

                        if (clientResult == null || clientResult.Count == 0)
                        {
                            MessageBox.Show("Клиент не найден в базе данных");
                            return;
                        }

                        Client client;
                        if (clientResult[0]["FIZ_LITSO"].Equals(true))
                        {
                            client = new ClientFizlico(clientResult[0]);
                        }
                        else
                        {
                            client = new ClientJurlico(clientResult[0]);
                        }

                        ReplacePlaceholder(doc.MainDocumentPart.Document, "NUM", selectedContract.ContractNumber);
                        ReplacePlaceholder(doc.MainDocumentPart.Document, "DAY", DateTime.Now.Day.ToString());
                        ReplacePlaceholder(doc.MainDocumentPart.Document, "MONTH", DateTime.Now.Month.ToString());
                        ReplacePlaceholder(doc.MainDocumentPart.Document, "YYYY", DateTime.Now.Year.ToString());
                        ReplacePlaceholder(doc.MainDocumentPart.Document, "ZAKAZ", client.TITLE);

                        // Используем ФИО выбранного сотрудника
                        string employeeName = $"{selectedEmployee.Surname} {selectedEmployee.Name} {selectedEmployee.Patronymic}";
                        ReplacePlaceholder(doc.MainDocumentPart.Document, "FAM", employeeName);
                    }

                    MessageBox.Show($"Договор успешно сформирован: {outputPath}");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = outputPath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании договора: {ex.Message}");
            }
        }


        private void ReplacePlaceholder(OpenXmlElement element, string placeholder, string value)
        {
            foreach (var run in element.Descendants<Run>())
            {
                var texts = run.Elements<Text>().ToList();
                foreach (var text in texts)
                {
                    if (text.Text.Contains(placeholder))
                    {
                        var parts = text.Text.Split(new[] { placeholder }, StringSplitOptions.None);

                        text.Remove();

                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(parts[i]))
                            {
                                run.AppendChild(new Text(parts[i]) { Space = SpaceProcessingModeValues.Preserve });
                            }

                            if (i < parts.Length - 1)
                            {
                                run.AppendChild(new Text(value) { Space = SpaceProcessingModeValues.Preserve });
                            }
                        }
                    }
                }
            }
        }
    }
}