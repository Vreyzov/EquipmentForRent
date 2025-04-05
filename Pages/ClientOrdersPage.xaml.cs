using EquipmentForRent.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace EquipmentForRent.Pages
{
    public partial class ClientsOrdersPage : Page
    {
        private int _clientId;
        private Client _loggedClient;

        public ClientsOrdersPage(int clientId)
        {
            InitializeComponent();
            _clientId = clientId;
            _loggedClient = GetClientById(_clientId);  // Загружаем клиента по его ID
            LoadOrders();
        }

        private void LoadOrders()
        {
            using (var context = new EquipmentRentContext())
            {
                var orders = context.Orders
                                    .Where(o => o.ClientId == _clientId)
                                    .Include(o => o.Equipment)  // Включаем информацию об оборудовании
                                    .Include(o => o.Lessor)     // Включаем информацию о арендодателе
                                    .ToList();

                // Привязываем данные к ListBox
                OrdersListBox.ItemsSource = orders;
            }
        }

        // Обработчик кнопки "Продлить аренду"
        private void ExtendRentalButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбран ли заказ в ListBox
            if (OrdersListBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите заказ для продления аренды.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;  // Выход из метода, если заказ не выбран
            }

            // Если заказ выбран, продолжаем выполнение
            if (OrdersListBox.SelectedItem is Order selectedOrder)
            {
                using (var context = new EquipmentRentContext())
                {
                    var order = context.Orders
                                       .Include(o => o.Equipment)
                                       .Include(o => o.Lessor)  // Включаем информацию о арендодателе
                                       .FirstOrDefault(o => o.OrderId == selectedOrder.OrderId);

                    if (order != null)
                    {
                        // Открываем окно для выбора новой даты окончания аренды
                        var newEndDateWindow = new DateTimePickerWindow(order.EndDate);

                        if (newEndDateWindow.ShowDialog() == true)  // Если нажата кнопка "OK"
                        {
                            DateTime? newEndDate = newEndDateWindow.SelectedDate;

                            // Проверяем, что новая дата не раньше даты начала аренды
                            if (newEndDate.HasValue && newEndDate.Value < order.StartDate)
                            {
                                MessageBox.Show("Дата окончания не может быть раньше даты начала", "Недопустимая дата", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            // Если дата окончания изменена
                            if (newEndDate.HasValue && newEndDate.Value != order.EndDate)
                            {
                                // Перерасчитываем стоимость аренды
                                int rentalDays = (newEndDate.Value - order.StartDate).Days;
                                if (rentalDays <= 0)
                                {
                                    MessageBox.Show("Дата окончания должна быть позже даты начала.", "Неверная дата.", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                double dailyRate = (double)order.Equipment.PricePerDay;
                                double newTotalCost = rentalDays * dailyRate;

                                // Обновляем дату окончания аренды и пересчитанную стоимость
                                order.EndDate = newEndDate.Value;
                                order.TotalCost = (decimal)newTotalCost;

                                // Обновляем заказ в базе данных
                                context.Orders.Update(order);
                                context.SaveChanges();

                                // Перезагружаем список заказов
                                LoadOrders();
                            }
                        }
                    }
                }
            }
        }

        // Метод для получения клиента по ID
        private Client GetClientById(int clientId)
        {
            using (var context = new EquipmentRentContext())
            {
                return context.Clients.FirstOrDefault(c => c.ClientId == clientId);
            }
        }

        // Обработчик кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClientHomePage(_loggedClient));  // Навигация на главную страницу клиента
        }
    }
}
