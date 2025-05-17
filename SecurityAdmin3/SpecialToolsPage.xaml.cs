using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Collections.Generic;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class SpecialToolsPage : Page
    {
        public MainWindow _mainWindow;
        public List<SpecialTool> tools = new List<SpecialTool>();

        public SpecialToolsPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadToolsAsync();
        }

        private async void LoadToolsAsync()
        {
            try
            {
                tools.Clear();
                var query = "SELECT * FROM \"specialtools\" ORDER BY \"id\"";
                var result = await Database.GetDatabase().Select(query);
                foreach (var item in result)
                {
                    tools.Add(new SpecialTool(item));
                }
                ToolsGrid.ItemsSource = tools;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки спецсредств: {ex.Message}");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new MainPage());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new SpecialToolPage(_mainWindow, null));
        }

        private void ToolsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ToolsGrid.SelectedItem != null)
            {
                SpecialTool tool = (SpecialTool)ToolsGrid.SelectedItem;
                _mainWindow.mainFrame.Navigate(new SpecialToolPage(_mainWindow, tool));
            }
        }
    }
}