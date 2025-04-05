using EquipmentForRent.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentForRent.Windows
{
    public partial class EditProfileLessorWindow : Window
    {
        private Lessor _lessor;
        private bool _isPasswordVisible = false;

        public Action<object> ProfileUpdated { get; internal set; }

        public EditProfileLessorWindow(Lessor lessor)
        {
            InitializeComponent();
            LoadLessorData(lessor); // Загружаем актуальные данные
        }

        /// <summary>
        /// Метод для загрузки актуальных данных арендодателя
        /// </summary>
        private void LoadLessorData(Lessor lessor)
        {
            using (var context = new EquipmentRentContext())
            {
                // Загружаем арендодателя из базы, чтобы избежать кэширования старых данных
                _lessor = context.Lessors.FirstOrDefault(l => l.LessorId == lessor.LessorId);

                if (_lessor != null)
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
                    FirstNameTextBox.Text = _lessor.FirstName ?? "";
                    LastNameTextBox.Text = _lessor.LastName ?? "";
                    PhoneTextBox.Text = _lessor.Phone ?? "";
                    EmailTextBox.Text = _lessor.Email ?? "";
                    LoginTextBox.Text = _lessor.Login ?? "";

                    PasswordBox.Password = _lessor.Password ?? "";
                    PasswordTextBox.Text = _lessor.Password ?? "";
                }
                else
                {
                    MessageBox.Show("Ошибка загрузки данных арендодателя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new EquipmentRentContext())
            {
                var lessor = context.Lessors.FirstOrDefault(l => l.LessorId == _lessor.LessorId);

                if (lessor != null)
                {
                    lessor.FirstName = FirstNameTextBox.Text;
                    lessor.LastName = LastNameTextBox.Text;
                    lessor.Phone = PhoneTextBox.Text;
                    lessor.Email = EmailTextBox.Text;
                    lessor.Login = LoginTextBox.Text;

                    string newPassword = _isPasswordVisible ? PasswordTextBox.Text : PasswordBox.Password;
                    if (!string.IsNullOrEmpty(newPassword) && newPassword != _lessor.Password)
                    {
                        lessor.Password = newPassword;
                    }

                    context.SaveChanges();
                    MessageBox.Show("Данные успешно обновлены!");

                    this.Close(); // Закрываем окно после сохранения
                }
                else
                {
                    MessageBox.Show("Ошибка: арендодатель не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
