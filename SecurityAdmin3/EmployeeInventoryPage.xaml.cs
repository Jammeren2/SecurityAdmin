using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Npgsql;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class EmployeeInventoryPage : Page
    {
        public MainWindow _mainWindow;
        public Employee Employee { get; set; }
        public string EmployeeName => $"{Employee.Surname} {Employee.Name} {Employee.Patronymic}";

        private List<Weapon> _allWeapons = new List<Weapon>();
        private List<SpecialTool> _allTools = new List<SpecialTool>();

        public EmployeeInventoryPage(MainWindow mainWindow, Employee employee)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Employee = employee;
            DataContext = this;
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                // Загрузка всего оружия
                var weaponsQuery = "SELECT * FROM \"weapons\" ORDER BY \"brand\"";
                var weaponsResult = await Database.GetDatabase().Select(weaponsQuery);
                _allWeapons = weaponsResult.Select(item => new Weapon(item)).ToList();

                // Загрузка оружия сотрудника
                var employeeWeaponsQuery = @"
                    SELECT w.* FROM ""weapons"" w
                    JOIN ""employeeweapons"" ew ON w.""id"" = ew.""weaponid""
                    WHERE ew.""employeeid"" = $1";
                var employeeWeaponsResult = await Database.GetDatabase().Select(employeeWeaponsQuery,
                    new NpgsqlParameter { Value = Employee.Id });
                Employee.Weapons = employeeWeaponsResult.Select(item => new Weapon(item)).ToList();

                WeaponsGrid.ItemsSource = Employee.Weapons;

                // Загрузка всех спецсредств
                var toolsQuery = "SELECT * FROM \"specialtools\" ORDER BY \"name\"";
                var toolsResult = await Database.GetDatabase().Select(toolsQuery);
                _allTools = toolsResult.Select(item => new SpecialTool(item)).ToList();

                // Загрузка спецсредств сотрудника
                var employeeToolsQuery = @"
                    SELECT st.* FROM ""specialtools"" st
                    JOIN ""employeetools"" et ON st.""id"" = et.""toolid""
                    WHERE et.""employeeid"" = $1";
                var employeeToolsResult = await Database.GetDatabase().Select(employeeToolsQuery,
                    new NpgsqlParameter { Value = Employee.Id });
                Employee.Tools = employeeToolsResult.Select(item => new SpecialTool(item)).ToList();

                ToolsGrid.ItemsSource = Employee.Tools;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private async void btnAddWeapon_Click(object sender, RoutedEventArgs e)
        {
            var availableWeapons = _allWeapons.Except(Employee.Weapons, new WeaponComparer()).ToList();

            var dialog = new SelectionDialog("Выберите оружие", availableWeapons);
            if (dialog.ShowDialog() == true && dialog.SelectedItem is Weapon selectedWeapon)
            {
                try
                {
                    string query = @"INSERT INTO ""employeeweapons"" (""employeeid"", ""weaponid"") VALUES ($1, $2)";
                    var parameters = new NpgsqlParameter[]
                    {
                        new NpgsqlParameter { Value = Employee.Id },
                        new NpgsqlParameter { Value = selectedWeapon.Id }
                    };

                    await Database.GetDatabase().ExecuteNonQuery(query, parameters);
                    Employee.Weapons.Add(selectedWeapon);
                    WeaponsGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении оружия: {ex.Message}");
                }
            }
        }

        private async void btnRemoveWeapon_Click(object sender, RoutedEventArgs e)
        {
            if (WeaponsGrid.SelectedItem is Weapon selectedWeapon)
            {
                if (MessageBox.Show("Вы действительно хотите удалить это оружие у сотрудника?",
                    "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = @"DELETE FROM ""employeeweapons"" 
                                        WHERE ""employeeid"" = $1 AND ""weaponid"" = $2";
                        var parameters = new NpgsqlParameter[]
                        {
                            new NpgsqlParameter { Value = Employee.Id },
                            new NpgsqlParameter { Value = selectedWeapon.Id }
                        };

                        await Database.GetDatabase().ExecuteNonQuery(query, parameters);
                        Employee.Weapons.Remove(selectedWeapon);
                        WeaponsGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении оружия: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите оружие для удаления");
            }
        }

        private async void btnAddTool_Click(object sender, RoutedEventArgs e)
        {
            var availableTools = _allTools.Except(Employee.Tools, new SpecialToolComparer()).ToList();
            var dialog = new SelectionDialog("Выберите спецсредство", availableTools);
            if (dialog.ShowDialog() == true && dialog.SelectedItem is SpecialTool selectedTool)
            {
                try
                {
                    string query = @"INSERT INTO ""employeetools"" (""employeeid"", ""toolid"") VALUES ($1, $2)";
                    var parameters = new NpgsqlParameter[] {
                new NpgsqlParameter { Value = Employee.Id },
                new NpgsqlParameter { Value = selectedTool.Id }
            };
                    await Database.GetDatabase().ExecuteNonQuery(query, parameters);
                    Employee.Tools.Add(selectedTool);
                    ToolsGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении спецсредства: {ex.Message}");
                }
            }
        }

        private async void btnRemoveTool_Click(object sender, RoutedEventArgs e)
        {
            if (ToolsGrid.SelectedItem is SpecialTool selectedTool)
            {
                if (MessageBox.Show("Вы действительно хотите удалить это спецсредство у сотрудника?",
                    "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = @"DELETE FROM ""employeetools"" 
                                        WHERE ""employeeid"" = $1 AND ""toolid"" = $2";
                        var parameters = new NpgsqlParameter[]
                        {
                            new NpgsqlParameter { Value = Employee.Id },
                            new NpgsqlParameter { Value = selectedTool.Id }
                        };

                        await Database.GetDatabase().ExecuteNonQuery(query, parameters);
                        Employee.Tools.Remove(selectedTool);
                        ToolsGrid.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении спецсредства: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите спецсредство для удаления");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new EmployeePage(_mainWindow, Employee));
        }
    }

    // Классы для сравнения объектов
    public class WeaponComparer : IEqualityComparer<Weapon>
    {
        public bool Equals(Weapon x, Weapon y) => x?.Id == y?.Id;
        public int GetHashCode(Weapon obj) => obj.Id.GetHashCode();
    }

    public class SpecialToolComparer : IEqualityComparer<SpecialTool>
    {
        public bool Equals(SpecialTool x, SpecialTool y) => x?.Id == y?.Id;
        public int GetHashCode(SpecialTool obj) => obj.Id.GetHashCode();
    }
}