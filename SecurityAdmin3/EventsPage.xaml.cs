using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using SecurityAdmin3.Objects;
using System.Diagnostics.Tracing;

namespace SecurityAdmin3
{
    public partial class EventsPage : Page
    {
        public MainWindow _mainWindow;
        public List<Event> events = new List<Event>();

        public EventsPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadEventsAsync();
        }

        private async void LoadEventsAsync()
        {
            try
            {
                events.Clear();
                var query = "SELECT * FROM \"events\" ORDER BY \"id\"";
                var result = await Database.GetDatabase().Select(query);

                foreach (var item in result)
                {
                    events.Add(new Event(item));
                }

                EventsGrid.ItemsSource = events;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки мероприятий: {ex.Message}");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new MainPage());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new EventPage(_mainWindow, null));
        }

        private void EventsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EventsGrid.SelectedItem != null)
            {
                Event eventItem = (Event)EventsGrid.SelectedItem;
                _mainWindow.mainFrame.Navigate(new EventPage(_mainWindow, eventItem));
            }
        }
    }
}