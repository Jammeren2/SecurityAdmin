using System.Windows;
using System.Windows.Controls;

namespace SecurityAdmin3
{
    public partial class SelectionDialog : Window
    {
        public object SelectedItem { get; private set; }

        public SelectionDialog(string title, System.Collections.IEnumerable items)
        {
            InitializeComponent();
            Title = title;
            ItemsListBox.ItemsSource = items;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem = ItemsListBox.SelectedItem;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ItemsListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ItemsListBox.SelectedItem != null)
            {
                SelectedItem = ItemsListBox.SelectedItem;
                DialogResult = true;
            }
        }
    }
}