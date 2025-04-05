using EquipmentForRent.Models;
using EquipmentForRent.Pages;
using System;
using System.Linq;
using System.Windows;

namespace EquipmentForRent.Windows
{
    public partial class NewOrderWindow : Window
    {
        private Equipment _selectedEquipment; // Используем Equipment, если это ваша модель
        private int _clientId;

        public NewOrderWindow(int clientId, Equipment selectedEquipment)
        {
            InitializeComponent();
            _clientId = clientId;
            _selectedEquipment = selectedEquipment;  // Инициализируем _selectedEquipment

            // Инициализация данных
            EquipmentName.Text = selectedEquipment.Name;
            PricePerDay.Text = selectedEquipment.PricePerDay.ToString("C2");
            EndDatePicker.SelectedDate = DateTime.Now.AddDays(1); // По умолчанию завтрашний день
        }


        private void ConfirmOrderButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? endDate = EndDatePicker.SelectedDate;

            if (!endDate.HasValue || endDate <= DateTime.Now)
            {
                MessageBox.Show("Дата окончания аренды должна быть позже сегодняшнего дня.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Расчет количества дней аренды, включая начальную и конечную даты
            int totalDays = (endDate.Value - DateTime.Now.Date).Days + 1;

            // Проверка на корректное количество дней
            if (totalDays <= 0)
            {
                MessageBox.Show("Дата окончания аренды должна быть позже сегодняшнего дня.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Расчет общей стоимости
            decimal totalCost = _selectedEquipment.PricePerDay * totalDays;

            var newOrder = new Order
            {
                ClientId = _clientId,
                EquipmentId = _selectedEquipment.EquipmentId, // Используем _selectedEquipment для получения EquipmentId
                LessorId = _selectedEquipment.LessorId, // Убедитесь, что LessorId корректно передается
                StartDate = DateTime.Now,
                EndDate = endDate.Value,
                TotalCost = totalCost
            };

            using (var context = new EquipmentRentContext())
            {
                context.Orders.Add(newOrder);
                context.SaveChanges();
            }

            MessageBox.Show($"Заказ успешно оформлен!\nИтоговая стоимость аренды: {totalCost:C2}",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }



        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();  // Закрытие окна оформления заказа
        }
    }
}
