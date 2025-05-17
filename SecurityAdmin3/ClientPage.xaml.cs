using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Npgsql;
using SecurityAdmin3.Objects;

namespace SecurityAdmin3
{
    public partial class ClientPage : Page
    {
        public MainWindow _mainWindow;
        public Client client;
        private bool isPhysicalPerson;

        public ClientPage(MainWindow mainWindow, Client client, bool? isPhysicalPerson = null)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.client = client;

            if (client != null)
            {
                this.client = client;
                this.isPhysicalPerson = client.IsPhysical;
            }
            else
            {
                this.isPhysicalPerson = isPhysicalPerson ?? true; 
            }

            InitializeForm();
        }

        private void InitializeForm()
        {
            if (client != null)
            {
                // Заполняем поля в зависимости от типа клиента
                //MessageBox.Show(Convert.ToString(isPhysicalPerson));

                if (isPhysicalPerson)
                {
                    var fizClient = client as ClientFizlico;
                    if (fizClient != null)
                    {
                        txtSurname.Text = fizClient.Surname;
                        txtName.Text = fizClient.Name;
                        txtPatronymic.Text = fizClient.Patronymic;
                        txtPasportSer.Text = fizClient.PasportSer.ToString();
                        txtPasportNum.Text = fizClient.PasportNum.ToString();
                        dtPasport.SelectedDate = fizClient.PasportData;
                        txtPasportVydan.Text = fizClient.PasportVydan;
                        txtPhone.Text = fizClient.Phone ?? string.Empty;
                        pageTitle.Text = $"Редактирование клиента ({client.Type})";
                    }
                }
                else
                {
                    var jurClient = client as ClientJurlico;
                    if (jurClient != null)
                    {
                        txtTitle.Text = jurClient.TITLE ?? string.Empty;
                        txtDogovorNum.Text = jurClient.Dogovor_num ?? string.Empty;
                        dtDogovorBegin.SelectedDate = jurClient.Dogovor_dt;
                        dtDogovorEnd.SelectedDate = jurClient.Dogovor_end;
                        txtPhone.Text = jurClient.Phone ?? string.Empty;
                        pageTitle.Text = $"Редактирование клиента ({client.Type})";
                    }
                }
                txtAdres.Text = client.Address ?? string.Empty;
                btnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                pageTitle.Text = isPhysicalPerson ? "Новый клиент (физ.лицо)" : "Новый клиент (юр.лицо)";
            }
            UpdateFieldsVisibility();
            (isPhysicalPerson ? txtSurname : txtTitle).Focus();
        }


        private void UpdateFieldsVisibility()
        {
            PhysicalFields.Visibility = isPhysicalPerson ? Visibility.Visible : Visibility.Collapsed;
            LegalFields.Visibility = isPhysicalPerson ? Visibility.Collapsed : Visibility.Visible;
        }

        private bool ValidateInput()
        {
            if (isPhysicalPerson)
            {
                if (string.IsNullOrWhiteSpace(txtSurname.Text) || txtSurname.Text.Trim().Length < 2)
                {
                    MessageBox.Show("Фамилия должна содержать минимум 2 символа");
                    txtSurname.Focus();
                    return false;
                }
                if (string.IsNullOrWhiteSpace(txtName.Text) || txtName.Text.Trim().Length < 2)
                {
                    MessageBox.Show("Имя должно содержать минимум 2 символа");
                    txtName.Focus();
                    return false;
                }
                if (!int.TryParse(txtPasportSer.Text, out _))
                {
                    MessageBox.Show("Серия паспорта должна быть числом");
                    txtPasportSer.Focus();
                    return false;
                }
                if (!int.TryParse(txtPasportNum.Text, out _))
                {
                    MessageBox.Show("Номер паспорта должен быть числом");
                    txtPasportNum.Focus();
                    return false;
                }
                if (!dtPasport.SelectedDate.HasValue)
                {
                    MessageBox.Show("Укажите дату выдачи паспорта");
                    dtPasport.Focus();
                    return false;
                }
                if (string.IsNullOrWhiteSpace(txtPasportVydan.Text) || txtPasportVydan.Text.Trim().Length < 5)
                {
                    MessageBox.Show("Укажите кем выдан паспорт (минимум 5 символов)");
                    txtPasportVydan.Focus();
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtTitle.Text) || txtTitle.Text.Trim().Length < 3)
                {
                    MessageBox.Show("Наименование должно содержать минимум 3 символа");
                    txtTitle.Focus();
                    return false;
                }
                if (string.IsNullOrWhiteSpace(txtDogovorNum.Text))
                {
                    MessageBox.Show("Укажите номер договора");
                    txtDogovorNum.Focus();
                    return false;
                }
                if (!dtDogovorBegin.SelectedDate.HasValue)
                {
                    MessageBox.Show("Укажите дату начала договора");
                    dtDogovorBegin.Focus();
                    return false;
                }
                if (!dtDogovorEnd.SelectedDate.HasValue)
                {
                    MessageBox.Show("Укажите дату окончания договора");
                    dtDogovorEnd.Focus();
                    return false;
                }
                if (dtDogovorEnd.SelectedDate.Value <= dtDogovorBegin.SelectedDate.Value)
                {
                    MessageBox.Show("Дата окончания должна быть позже даты начала");
                    dtDogovorEnd.Focus();
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(txtAdres.Text) || txtAdres.Text.Trim().Length < 5)
            {
                MessageBox.Show("Адрес должен содержать минимум 5 символов");
                txtAdres.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text) || txtPhone.Text.Trim().Length < 6)
            {
                MessageBox.Show("Телефон должен содержать минимум 6 символов");
                txtPhone.Focus();
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            string address = txtAdres.Text.Trim();
            string phone = txtPhone.Text.Trim();

            try
            {
                Database database = Database.GetDatabase();

                if (isPhysicalPerson)
                {
                    string surname = txtSurname.Text.Trim();
                    string name = txtName.Text.Trim();
                    string patronymic = txtPatronymic.Text.Trim();
                    int pasportSer = int.Parse(txtPasportSer.Text.Trim());
                    int pasportNum = int.Parse(txtPasportNum.Text.Trim());
                    DateTime pasportDate = dtPasport.SelectedDate.Value;
                    string pasportVydan = txtPasportVydan.Text.Trim();

                    if (client != null)
                    {
                        // Update existing physical client
                        string updateQuery = @"UPDATE ""clients"" SET 
                            ""SURNAME"" = $1, ""NAME"" = $2, ""PATRONYMIC"" = $3, 
                            ""ADRES"" = $4, ""PHONE"" = $5, ""PASPORT_SER"" = $6, 
                            ""PASPORT_NUM"" = $7, ""PASPORT_DT"" = $8, ""PASPORT_VYDAN"" = $9,
                            ""FIZ_LITSO"" = $10
                            WHERE ""ID"" = $11";

                        var parameters = new NpgsqlParameter[] {
                            new NpgsqlParameter { Value = surname },
                            new NpgsqlParameter { Value = name },
                            new NpgsqlParameter { Value = patronymic },
                            new NpgsqlParameter { Value = address },
                            new NpgsqlParameter { Value = phone },
                            new NpgsqlParameter { Value = pasportSer },
                            new NpgsqlParameter { Value = pasportNum },
                            new NpgsqlParameter { Value = pasportDate },
                            new NpgsqlParameter { Value = pasportVydan },
                            new NpgsqlParameter { Value = true },
                            new NpgsqlParameter { Value = client.Id }
                        };

                        await database.ExecuteNonQuery(updateQuery, parameters);
                        MessageBox.Show("Данные клиента успешно обновлены");
                    }
                    else
                    {
                        // Create new physical client
                        string insertQuery = @"INSERT INTO ""clients"" 
                            (""SURNAME"", ""NAME"", ""PATRONYMIC"", ""ADRES"", ""PHONE"", 
                            ""PASPORT_SER"", ""PASPORT_NUM"", ""PASPORT_DT"", ""PASPORT_VYDAN"", 
                            ""ID_TYPE"", ""FIZ_LITSO"") 
                            VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11)";

                        var parameters = new NpgsqlParameter[] {
                            new NpgsqlParameter { Value = surname },
                            new NpgsqlParameter { Value = name },
                            new NpgsqlParameter { Value = patronymic },
                            new NpgsqlParameter { Value = address },
                            new NpgsqlParameter { Value = phone },
                            new NpgsqlParameter { Value = pasportSer },
                            new NpgsqlParameter { Value = pasportNum },
                            new NpgsqlParameter { Value = pasportDate },
                            new NpgsqlParameter { Value = pasportVydan },
                            new NpgsqlParameter { Value = 1 },
                            new NpgsqlParameter { Value = true }
                        };

                        await database.ExecuteNonQuery(insertQuery, parameters);
                        MessageBox.Show("Клиент успешно добавлен");
                    }
                }
                else
                {
                    string title = txtTitle.Text.Trim();
                    string dogovorNum = txtDogovorNum.Text.Trim();
                    DateTime dogovorBegin = dtDogovorBegin.SelectedDate.Value;
                    DateTime dogovorEnd = dtDogovorEnd.SelectedDate.Value;

                    if (client != null)
                    {
                        // Update existing legal client
                        string updateQuery = @"UPDATE ""clients"" SET ""TITLE"" = $1, ""ADRES"" = $2, ""PHONE"" = $3, ""DOGOVOR_NUM"" = $4, ""DOGOVOR_DT"" = $5, ""DOGOVOR_END"" = $6, ""FIZ_LITSO"" = $7 WHERE ""ID"" = $8";
                        var parameters = new NpgsqlParameter[] {
                            new NpgsqlParameter { Value = title },
                            new NpgsqlParameter { Value = address },
                            new NpgsqlParameter { Value = phone },
                            new NpgsqlParameter { Value = dogovorNum },
                            new NpgsqlParameter { Value = dogovorBegin },
                            new NpgsqlParameter { Value = dogovorEnd },
                            new NpgsqlParameter { Value = false },
                            new NpgsqlParameter { Value = client.Id }
                        };

                        await database.ExecuteNonQuery(updateQuery, parameters);
                        MessageBox.Show("Данные клиента успешно обновлены");
                    }
                    else
                    {
                        // Create new legal client
                        string insertQuery = @"INSERT INTO ""clients"" (""TITLE"", ""ADRES"", ""PHONE"", ""DOGOVOR_NUM"", ""DOGOVOR_DT"", ""DOGOVOR_END"", ""ID_TYPE"", ""FIZ_LITSO"") VALUES ($1, $2, $3, $4, $5, $6, $7, $8)";
                        var parameters = new NpgsqlParameter[] {
                            new NpgsqlParameter { Value = title },
                            new NpgsqlParameter { Value = address },
                            new NpgsqlParameter { Value = phone },
                            new NpgsqlParameter { Value = dogovorNum },
                            new NpgsqlParameter { Value = dogovorBegin },
                            new NpgsqlParameter { Value = dogovorEnd },
                            new NpgsqlParameter { Value = 2 },
                            new NpgsqlParameter { Value = false }
                        };

                        await database.ExecuteNonQuery(insertQuery, parameters);
                        MessageBox.Show("Клиент успешно добавлен");
                    }
                }

                await _mainWindow.LoadClients();
                _mainWindow.mainFrame.Navigate(new ClientsPage(_mainWindow));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (client == null) return;

            if (MessageBox.Show("Вы действительно хотите удалить этого клиента?", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Database database = Database.GetDatabase();
                    string deleteQuery = @"DELETE FROM ""clients"" WHERE ""ID"" = $1";
                    var parameter = new NpgsqlParameter { Value = client.Id };
                    await database.ExecuteNonQuery(deleteQuery, parameter);
                    MessageBox.Show("Клиент успешно удален");
                    await _mainWindow.LoadClients();
                    _mainWindow.mainFrame.Navigate(new ClientsPage(_mainWindow));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении клиента: {ex.Message}");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainFrame.Navigate(new ClientsPage(_mainWindow));
        }
    }
}