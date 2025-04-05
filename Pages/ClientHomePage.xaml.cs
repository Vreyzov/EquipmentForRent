using EquipmentForRent.Models;
using EquipmentForRent.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentForRent.Pages
{
    public partial class ClientHomePage : Page
    {
        private Client _loggedClient;
        private bool _notificationShown = false;
        private bool _reviewWindowShown = false;

        public ClientHomePage(Client loggedClient)
        {
            InitializeComponent();
            _loggedClient = loggedClient ?? throw new ArgumentNullException(nameof(loggedClient));

            WelcomeText.Text = $"Здравствуйте! {_loggedClient.FirstName} {_loggedClient.LastName}";

            // Подписываемся на событие Loaded (грузим данные при каждом входе)
            Loaded += ClientHomePage_Loaded;
        }

        private void ClientHomePage_Loaded(object sender, RoutedEventArgs e)
        {
            CheckExpiringRentals();
            CheckForPendingReviews();
        }

        private void CheckExpiringRentals()
        {
            if (_notificationShown) return;

            using (var context = new EquipmentRentContext())
            {
                var soonExpiringOrders = context.Orders
                    .Where(o => o.ClientId == _loggedClient.ClientId && o.EndDate.Date == DateTime.Today.AddDays(1))
                    .ToList();

                if (soonExpiringOrders.Any())
                {
                    MessageBox.Show("У вас есть аренда, срок которой истекает завтра!", "Напоминание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    _notificationShown = true;
                }
            }
        }

        private void CheckForPendingReviews()
        {
            if (_reviewWindowShown) return;

            using (var context = new EquipmentRentContext())
            {
                var completedOrders = context.Orders
                    .Where(o => o.ClientId == _loggedClient.ClientId && o.EndDate < DateTime.Now)
                    .ToList();

                foreach (var order in completedOrders)
                {
                    var existingReview = context.Reviews.FirstOrDefault(r => r.OrderId == order.OrderId);
                    if (existingReview != null) continue;

                    // Отзыв обязателен, поэтому блокируем страницу, пока его не оставят
                    ReviewWindow reviewWindow = new ReviewWindow(_loggedClient.ClientId, order.LessorId, order.OrderId);
                    reviewWindow.ShowDialog();
                    _reviewWindowShown = true;
                }
            }
        }

        private void ViewEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ClientEquipmentPage(_loggedClient.ClientId));
        }

        private void ViewMyOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ClientsOrdersPage(_loggedClient.ClientId));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginPage());
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ProfileClientPage(_loggedClient));
        }
    }
}
