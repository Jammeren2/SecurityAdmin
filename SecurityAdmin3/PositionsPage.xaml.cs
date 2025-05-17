using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Collections.Generic;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class PositionsPage : Page
    {
        public MainWindow _mainWindow;
        public List<Position> positions = new List<Position>();

        public PositionsPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadPositionsAsync();
        }

        private async void LoadPositionsAsync()
        {
            try
            {
                positions.Clear();
                var query = @"SELECT p.*, r.name as rolename 
                            FROM positions p 
                            JOIN roles r ON p.roleid = r.id 
                            ORDER BY p.id";
                var result = await Database.GetDatabase().Select(query);

                foreach (var item in result)
                {
                    positions.Add(new Position(item));
                }

                PositionsGrid.ItemsSource = positions;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки должностей: {ex.Message}");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new MainPage());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new PositionPage(_mainWindow, null));
        }

        private void PositionsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PositionsGrid.SelectedItem != null)
            {
                Position position = (Position)PositionsGrid.SelectedItem;
                _mainWindow.mainFrame.Navigate(new PositionPage(_mainWindow, position));
            }
        }
    }
}