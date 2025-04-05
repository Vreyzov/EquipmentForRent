using System;
using System.Windows;
using System.Windows.Controls;

namespace EquipmentForRent
{
    /// <summary>
    /// Логика взаимодействия для DateTimePickerWindow.xaml
    /// </summary>
    public partial class DateTimePickerWindow : Window
    {
        public DateTime? SelectedDate { get; set; }

        public DateTimePickerWindow(DateTime? initialDate)
        {
            InitializeComponent();
            SelectedDate = initialDate;
            Calendar.SelectedDate = initialDate; // Это теперь работает с Calendar

            // Устанавливаем минимальную дату для выбора: сегодня
            Calendar.DisplayDateStart = DateTime.Today;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную дату из Calendar и проверяем, что она не до сегодняшнего дня
            if (Calendar.SelectedDate < DateTime.Today)
            {
                MessageBox.Show("Нельзя выбрать дату раньше сегодняшнего дня.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SelectedDate = Calendar.SelectedDate;
            DialogResult = true; // Закрытие окна с подтверждением изменений
        }

        // Обработчик кнопки "Cancel" - отменяем изменения и закрываем окно
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Закрытие окна без изменений
        }

        // Дополнительная проверка при изменении выбранной даты
        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Calendar.SelectedDate < DateTime.Today)
            {
                MessageBox.Show("Нельзя выбрать дату раньше сегодняшнего дня.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Calendar.SelectedDate = DateTime.Today; // Сбросить выбранную дату на сегодняшнюю
            }
        }
    }
}
