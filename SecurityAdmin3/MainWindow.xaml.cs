using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Npgsql;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class MainWindow : Window
    {
        public static string DB_IP = "90.189.209.31";
        public static string DB_PORT = "5432";
        public static string DB_USER = "postgres";
        public static string DB_PASSWORD = "asdasdasd";
        public static string DB_NAME = "security";
        public static int id_user = 0;

        public List<Client> clients = new List<Client>();

        public List<Payment> payments = new List<Payment>();

        public async Task LoadPayments()
        {
            try
            {
                payments.Clear();
                var query = @"SELECT p.*, 
                     CASE WHEN c.""FIZ_LITSO"" = true 
                          THEN c.""SURNAME"" || ' ' || c.""NAME"" || COALESCE(' ' || c.""PATRONYMIC"", '') 
                          ELSE c.""TITLE"" END as clientname, 
                     ct.""contractnumber"" 
                     FROM ""payments"" p 
                     JOIN ""clients"" c ON p.""clientid"" = c.""ID"" 
                     JOIN ""contracts"" ct ON p.""contractid"" = ct.""id"" 
                     WHERE p.""paymentdate"" BETWEEN $1 AND $2
                     ORDER BY p.""paymentdate"" DESC";

                var parameters = new NpgsqlParameter[] {
            new NpgsqlParameter { Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) },
            new NpgsqlParameter { Value = DateTime.Now }
        };

                var result = await Database.GetDatabase().Select(query, parameters);
                foreach (var item in result)
                {
                    payments.Add(new Payment(item));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки платежей: {ex.Message}");
            }
        }

        public async Task LoadClients()
        {
            try
            {
                clients.Clear();

                // Load all clients from unified table
                var query = "SELECT * FROM \"clients\" ORDER BY \"ID\"";
                var result = await Database.GetDatabase().Select(query);

                foreach (var item in result)
                {
                    bool isPhysical = (bool)item["FIZ_LITSO"];
                    if (isPhysical)
                    {
                        clients.Add(new ClientFizlico(item));
                    }
                    else
                    {
                        clients.Add(new ClientJurlico(item));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}");
            }
        }

        public void Load()
        {
            LoadClients().Wait();
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new LoginPage());
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}