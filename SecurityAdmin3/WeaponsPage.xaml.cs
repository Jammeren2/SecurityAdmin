using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Collections.Generic;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class WeaponsPage : Page
    {
        public MainWindow _mainWindow;
        public List<Weapon> weapons = new List<Weapon>();

        public WeaponsPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadWeaponsAsync();
        }

        private async void LoadWeaponsAsync()
        {
            try
            {
                weapons.Clear();
                var query = "SELECT * FROM \"weapons\" ORDER BY \"id\"";
                var result = await Database.GetDatabase().Select(query);
                foreach (var item in result)
                {
                    weapons.Add(new Weapon(item));
                }
                WeaponsGrid.ItemsSource = weapons;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки оружия: {ex.Message}");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new MainPage());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new WeaponPage(_mainWindow, null));
        }

        private void WeaponsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WeaponsGrid.SelectedItem != null)
            {
                Weapon weapon = (Weapon)WeaponsGrid.SelectedItem;
                _mainWindow.mainFrame.Navigate(new WeaponPage(_mainWindow, weapon));
            }
        }
    }
}