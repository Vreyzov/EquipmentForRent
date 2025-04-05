using EquipmentForRent.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentForRent.Pages
{
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Сбор данных с формы
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string phone = PhoneTextBox.Text;
            string email = EmailTextBox.Text;
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            // Валидация данных (можно дополнить проверками на пустые значения, корректность)
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Регистрация клиента или арендодателя
            if (ClientRadioButton.IsChecked == true)
            {
                RegisterClient(firstName, lastName, phone, email, login, password);
            }
            else if (LessorRadioButton.IsChecked == true)
            {
                RegisterLessor(firstName, lastName, phone, email, login, password);
            }
        }

        private void RegisterClient(string firstName, string lastName, string phone, string email, string login, string password)
        {
            // Подключение к базе данных
            using (SqlConnection connection = new SqlConnection("Server=PC;Database=EquipmentRent;TrustServerCertificate=True;Trusted_Connection=True;"))
            {
                connection.Open();

                string query = "INSERT INTO Client (FirstName, LastName, Phone, Email, Login, Password) VALUES (@FirstName, @LastName, @Phone, @Email, @Login, @Password)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);

                    // Выполнение запроса
                    command.ExecuteNonQuery();
                }
            }

            // Показать сообщение об успехе
            MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Перенаправить на страницу авторизации
            NavigationService.Navigate(new LoginPage());
        }

        private void RegisterLessor(string firstName, string lastName, string phone, string email, string login, string password)
        {
            // Подключение к базе данных
            using (SqlConnection connection = new SqlConnection("Server=PC;Database=EquipmentRent;TrustServerCertificate=True;Trusted_Connection=True;"))
            {
                connection.Open();

                string query = "INSERT INTO Lessor (FirstName, LastName, Phone, Email, Login, Password) VALUES (@FirstName, @LastName, @Phone, @Email, @Login, @Password)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);

                    // Выполнение запроса
                    command.ExecuteNonQuery();
                }
            }

            // Показать сообщение об успехе
            MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Перенаправить на страницу авторизации
            NavigationService.Navigate(new LoginPage());
        }

        private void BackRegistration_ButtonClick(object sender, RoutedEventArgs e)
        {
            // Возврат на страницу авторизации
            NavigationService.Navigate(new LoginPage());
        }
    }
}