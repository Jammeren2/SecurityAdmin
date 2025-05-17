using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SecurityAdmin3.Objects;
using Npgsql;

namespace SecurityAdmin3
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Title = "Авторизация";
            }
            txtLogin.Focus();
            txtPassword.Password = "admin";
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text) || txtLogin.Text.Trim().Length < 3)
            {
                MessageBox.Show("Логин должен содержать минимум 3 символа");
                txtLogin.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Password) || txtPassword.Password.Trim().Length < 3)
            {
                MessageBox.Show("Пароль должен содержать минимум 3 символа");
                txtPassword.Focus();
                return false;
            }
            return true;
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            try
            {
                var query = @"
            SELECT 
                e.""id"", 
                e.""login"", 
                e.""password"",
                p.""roleid""
            FROM 
                employees AS e
            JOIN 
                positions AS p ON e.""positionid"" = p.""id""
            WHERE 
                (p.""roleid"" = 1 OR p.""roleid"" = 2)
                AND e.""login"" = @login 
                AND e.""password"" = @password
        ";

                var parameters = new NpgsqlParameter[] {
            new NpgsqlParameter("@login", login),
            new NpgsqlParameter("@password", password)
        };

                var result = await Database.GetDatabase().Select(query, parameters);
                if (result != null && result.Count > 0)
                {
                    MainWindow.id_user = Convert.ToInt32(result[0]["id"]);
                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.Width = 900;
                        mainWindow.Height = 600;
                        mainWindow.mainFrame.Navigate(new MainPage());
                        _ = mainWindow.LoadClients();
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}");
            }
        }

        private void txtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }
    }
}