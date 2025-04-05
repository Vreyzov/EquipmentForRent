using EquipmentForRent.Models;
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

namespace EquipmentForRent.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            // Очистка сообщений об ошибках
            LoginErrorMessage.Text = "";
            PasswordErrorMessage.Text = "";
            GeneralErrorMessage.Text = "";

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                GeneralErrorMessage.Text = "Логин и пароль обязательны!";
                return;
            }

            using (var context = new EquipmentRentContext())
            {
                var client = context.Clients.FirstOrDefault(c => c.Login == login && c.Password == password);
                var lessor = context.Lessors.FirstOrDefault(l => l.Login == login && l.Password == password);

                if (client != null)
                {
                    NavigationService.Navigate(new ClientHomePage(client));
                }
                else if (lessor != null)
                {
                    NavigationService.Navigate(new LessorHomePage(lessor));
                }
                else
                {
                    GeneralErrorMessage.Text = "Неверный логин или пароль!";
                }
            }
        }
                private void GoToRegisterPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }
    }
}
