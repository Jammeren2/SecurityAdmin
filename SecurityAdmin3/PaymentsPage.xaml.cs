using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Collections.Generic;
using SecurityAdmin3.Objects;
using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

using Npgsql;
using System.Linq;

namespace SecurityAdmin3
{
    public partial class PaymentsPage : Page
    {
        public MainWindow _mainWindow;
        public List<Payment> payments = new List<Payment>();

        public PaymentsPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadPaymentsAsync();
        }

        private async void LoadPaymentsAsync()
        {
            try
            {
                payments.Clear();
                var query = @"SELECT p.*, 
                     CASE 
                         WHEN c.""FIZ_LITSO"" = true THEN c.""SURNAME"" || ' ' || c.""NAME"" || COALESCE(' ' || c.""PATRONYMIC"", '') 
                         ELSE c.""TITLE"" 
                     END as clientname, 
                     ct.""contractnumber"" 
                     FROM ""payments"" p 
                     JOIN ""clients"" c ON p.""clientid"" = c.""ID"" 
                     JOIN ""contracts"" ct ON p.""contractid"" = ct.""id"" 
                     ORDER BY p.""paymentdate"" DESC";
                var result = await Database.GetDatabase().Select(query);
                foreach (var item in result)
                {
                    payments.Add(new Payment(item));
                }
                PaymentsGrid.ItemsSource = payments;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки платежей: {ex.Message}");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new MainPage());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new PaymentPage(_mainWindow, null));
        }

        private void PaymentsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PaymentsGrid.SelectedItem != null)
            {
                Payment payment = (Payment)PaymentsGrid.SelectedItem;
                _mainWindow.mainFrame.Navigate(new PaymentPage(_mainWindow, payment));
            }
        }

        private async void btnGenerateInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentsGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите платеж для формирования счета");
                return;
            }

            Payment selectedPayment = (Payment)PaymentsGrid.SelectedItem;

            try
            {
                // Get template path
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schet.docx");

                // Create a copy of the template
                string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Счет_{selectedPayment.Id}_{DateTime.Now:yyyyMMddHHmmss}.docx");
                File.Copy(templatePath, outputPath, true);

                // Open the document and replace placeholders
                using (WordprocessingDocument doc = WordprocessingDocument.Open(outputPath, true))
                {
                    var body = doc.MainDocumentPart.Document.Body;

                    // Get contract details
                    var contractQuery = @"SELECT * FROM ""contracts"" WHERE ""id"" = $1";
                    var contractResult = await Database.GetDatabase().Select(contractQuery, new NpgsqlParameter { Value = selectedPayment.ContractId });
                    var contract = new Contract(contractResult[0]);

                    // Get client details
                    var clientQuery = @"SELECT * FROM ""clients"" WHERE ""ID"" = $1";
                    var clientResult = await Database.GetDatabase().Select(clientQuery, new NpgsqlParameter { Value = selectedPayment.ClientId });

                    Client client;
                    if (clientResult[0]["FIZ_LITSO"].Equals(true))
                    {
                        client = new ClientFizlico(clientResult[0]);
                    }
                    else
                    {
                        client = new ClientJurlico(clientResult[0]);
                    }

                    // Calculate tax (assuming 20% VAT)
                    decimal tax = selectedPayment.Amount * 0.2m;

                    // Replace placeholders
                    // Вместо ReplacePlaceholder(body, ...) используйте:
                    ReplacePlaceholder(doc.MainDocumentPart.Document, "@@NUM", selectedPayment.Id.ToString());
                    ReplacePlaceholder(doc.MainDocumentPart.Document, "DAY", selectedPayment.PaymentDate.Day.ToString());
                    ReplacePlaceholder(doc.MainDocumentPart.Document, "MONTH", selectedPayment.PaymentDate.Month.ToString());
                    ReplacePlaceholder(doc.MainDocumentPart.Document, "YEAR", selectedPayment.PaymentDate.Year.ToString());
                    ReplacePlaceholder(doc.MainDocumentPart.Document, "@@CLIENT", client.TITLE);
                    ReplacePlaceholder(doc.MainDocumentPart.Document, "@@TITLE", "Предоставление охранных услуг осуществляется в соответствии со статьёй 113.1 Кодекса Российской Федерации об административных правонарушениях.");
                    ReplacePlaceholder(doc.MainDocumentPart.Document, "@@COST", selectedPayment.Amount.ToString("N2"));
                    ReplacePlaceholder(doc.MainDocumentPart.Document, "@@TAX", tax.ToString("N2"));
                    ReplacePlaceholder(doc.MainDocumentPart.Document, "COST", selectedPayment.Amount.ToString("N2"));


                }

                MessageBox.Show($"Счет успешно сформирован: {outputPath}");

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = outputPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании счета: {ex.Message}");
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
                        // Разбиваем текст на части перед и после плейсхолдера
                        var parts = text.Text.Split(new[] { placeholder }, StringSplitOptions.None);

                        // Удаляем старый текст
                        text.Remove();

                        // Добавляем новые текстовые элементы с сохранением форматирования
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