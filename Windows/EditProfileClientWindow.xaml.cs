using EquipmentForRent.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentForRent.Windows
{
    public partial class EditProfileClientWindow : Window
    {
        private Client _client;
        private bool _isPasswordVisible = false;

        public Action<object> ProfileUpdated { get; internal set; }

        public EditProfileClientWindow(Client client)
        {
            InitializeComponent();
            LoadClientData(client); // Загружаем актуальные данные
        }

        /// <summary>
        /// Метод для загрузки актуальных данных клиента
        /// </summary>
        private void LoadClientData(Client client)
        {
            using (var context = new EquipmentRentContext())
            {
                // Загружаем клиента из базы, чтобы избежать кэширования старых данных
                _client = context.Clients.FirstOrDefault(c => c.ClientId == client.ClientId);

                if (_client != null)
                {
                    // Очистка полей перед заполнением новыми данными
                    FirstNameTextBox.Clear();
                    LastNameTextBox.Clear();
                    PhoneTextBox.Clear();
                    EmailTextBox.Clear();
                    LoginTextBox.Clear();
                    PasswordBox.Clear();
                    PasswordTextBox.Clear();

                    // Заполняем актуальными данными
                    FirstNameTextBox.Text = _client.FirstName ?? "";
                    LastNameTextBox.Text = _client.LastName ?? "";
                    PhoneTextBox.Text = _client.Phone ?? "";
                    EmailTextBox.Text = _client.Email ?? "";
                    LoginTextBox.Text = _client.Login ?? "";

                    PasswordBox.Password = _client.Password ?? "";
                    PasswordTextBox.Text = _client.Password ?? "";
                }
                else
                {
                    MessageBox.Show("Ошибка загрузки данных клиента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new EquipmentRentContext())
            {
                var client = context.Clients.FirstOrDefault(c => c.ClientId == _client.ClientId);

                if (client != null)
                {
                    client.FirstName = FirstNameTextBox.Text;
                    client.LastName = LastNameTextBox.Text;
                    client.Phone = PhoneTextBox.Text;
                    client.Email = EmailTextBox.Text;
                    client.Login = LoginTextBox.Text;

                    string newPassword = _isPasswordVisible ? PasswordTextBox.Text : PasswordBox.Password;
                    if (!string.IsNullOrEmpty(newPassword) && newPassword != _client.Password)
                    {
                        client.Password = newPassword;
                    }

                    context.SaveChanges();
                    MessageBox.Show("Данные успешно обновлены!");

                    this.Close(); // Закрываем окно после сохранения
                }
                else
                {
                    MessageBox.Show("Ошибка: клиент не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!_isPasswordVisible)
            {
                PasswordTextBox.Text = PasswordBox.Password;
            }
        }
    }
}
