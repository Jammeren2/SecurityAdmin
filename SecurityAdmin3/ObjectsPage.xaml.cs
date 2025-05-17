using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Collections.Generic;
using SecurityAdmin3.Objects;
using System.Security.AccessControl;

namespace SecurityAdmin3
{
    public partial class ObjectsPage : Page
    {
        public MainWindow _mainWindow;
        public List<SecurityObject> objects = new List<SecurityObject>();

        public ObjectsPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadObjectsAsync();
        }

        private async void LoadObjectsAsync()
        {
            try
            {
                objects.Clear();
                var query = "SELECT * FROM \"objects\" ORDER BY \"id\"";
                var result = await Database.GetDatabase().Select(query);
                foreach (var item in result)
                {
                    objects.Add(new SecurityObject(item));
                }
                ObjectsGrid.ItemsSource = objects;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки объектов: {ex.Message}");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new MainPage());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new ObjectPage(_mainWindow, null));
        }

        private void ObjectsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ObjectsGrid.SelectedItem != null)
            {
                SecurityObject obj = (SecurityObject)ObjectsGrid.SelectedItem;
                _mainWindow.mainFrame.Navigate(new ObjectPage(_mainWindow, obj));
            }
        }
    }
}