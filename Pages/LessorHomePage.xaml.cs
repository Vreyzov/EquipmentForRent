using EquipmentForRent.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EquipmentForRent.Pages
{
    /// <summary>
    /// Логика взаимодействия для LessorHomePage.xaml
    /// </summary>
    public partial class LessorHomePage : Page
    {
        private Lessor _loggedLessor;

        public LessorHomePage(Lessor loggedLessor)
        {
            InitializeComponent();
            _loggedLessor = loggedLessor;
            WelcomeText.Text = $"Здравствуйте! {_loggedLessor.FirstName} {_loggedLessor.LastName}";
        }

        private void ViewMyEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LessorEquipmentPage(_loggedLessor.LessorId));
        }

        private void ViewMyOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LessorOrdersPage(_loggedLessor.LessorId));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Возвращаемся на страницу авторизации
            NavigationService.Navigate(new LoginPage());
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу профиля арендодателя
            NavigationService.Navigate(new ProfileLessorPage(_loggedLessor));
        }
    }
}
