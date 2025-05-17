using System;
using System.Collections.Generic;
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

namespace SecurityAdmin3
{
    public partial class ClientsPage : Page
    {
        public MainWindow _mainWindow;

        public ClientsPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadClientsAsync();
        }

        private async void LoadClientsAsync()
        {
            await _mainWindow.LoadClients();
            Clients.ItemsSource = _mainWindow.clients;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new MainPage());
        }

        private void btnAddPhysical_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new ClientPage(_mainWindow, null, true));
        }

        private void btnAddLegal_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new ClientPage(_mainWindow, null, false));
        }

        private void Clients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid.SelectedItem != null)
            {
                Client client = (Client)dataGrid.SelectedItem;
                _mainWindow.mainFrame.Navigate(new ClientPage(_mainWindow, client));
            }
        }
    }
}