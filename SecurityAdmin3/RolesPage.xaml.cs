using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Collections.Generic;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class RolesPage : Page
    {
        public MainWindow _mainWindow;
        public List<Role> roles = new List<Role>();

        public RolesPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadRolesAsync();
        }

        private async void LoadRolesAsync()
        {
            try
            {
                roles.Clear();
                var query = "SELECT * FROM \"roles\" ORDER BY \"id\"";
                var result = await Database.GetDatabase().Select(query);

                foreach (var item in result)
                {
                    roles.Add(new Role(item));
                }

                RolesGrid.ItemsSource = roles;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ролей: {ex.Message}");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new MainPage());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new RolePage(_mainWindow, null));
        }

        private void RolesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (RolesGrid.SelectedItem != null)
            {
                Role role = (Role)RolesGrid.SelectedItem;
                _mainWindow.mainFrame.Navigate(new RolePage(_mainWindow, role));
            }
        }
    }
}