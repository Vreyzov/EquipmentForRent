using EquipmentForRent.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace EquipmentForRent.Pages
{
    public partial class LessorOrdersPage : Page
    {
        private int _lessorID;

        public LessorOrdersPage(int lessorID)
        {
            InitializeComponent();
            _lessorID = lessorID;
            LoadOrdersData();
        }

        private void LoadOrdersData()
        {
            using (var db = new EquipmentRentContext())
            {
                // Выгружаем все заказы, относящиеся к текущему арендодателю
                var orders = db.Orders
                               .Where(o => o.LessorId == _lessorID)
                               .Select(o => new
                               {
                                   o.OrderId,
                                   ClientName = o.Client.FirstName + " " + o.Client.LastName,
                                   EquipmentName = o.Equipment.Name,
                                   o.StartDate,
                                   o.EndDate,
                                   o.TotalCost
                               }).ToList();

                // Привязываем данные к ListBox
                OrdersListBox.ItemsSource = orders;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выделенные строки
            var selectedOrder = OrdersListBox.SelectedItem as dynamic;

            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить заказ?",
                                         "Подтверждение удаления",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (var db = new EquipmentRentContext())
                {
                    int orderId = selectedOrder.OrderId;
                    var dbOrder = db.Orders.FirstOrDefault(o => o.OrderId == orderId);
                    if (dbOrder != null)
                    {
                        db.Orders.Remove(dbOrder);
                    }

                    db.SaveChanges();
                }

                MessageBox.Show("Заказ успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем ListBox
                LoadOrdersData();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на домашнюю страницу арендодателя
            NavigationService.GoBack();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранный заказ
            var selectedOrder = OrdersListBox.SelectedItem as dynamic;

            if (selectedOrder == null)
            {
                MessageBox.Show("Пожалуйста, выберите заказ для печати.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создаем документ для печати
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument document = new FlowDocument
                {
                    PagePadding = new Thickness(50),
                    FontSize = 14,
                    FontFamily = new FontFamily("Segoe UI")
                };

                // Добавляем заголовок
                Paragraph header = new Paragraph(new Bold(new Run("Информация о заказе")))
                {
                    FontSize = 18,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                document.Blocks.Add(header);

                // Добавляем информацию о заказе
                document.Blocks.Add(new Paragraph(new Run($"Клиент: {selectedOrder.ClientName}")));
                document.Blocks.Add(new Paragraph(new Run($"Оборудование: {selectedOrder.EquipmentName}")));
                document.Blocks.Add(new Paragraph(new Run($"Начало аренды: {selectedOrder.StartDate:dd.MM.yyyy}")));
                document.Blocks.Add(new Paragraph(new Run($"Конец аренды: {selectedOrder.EndDate:dd.MM.yyyy}")));
                document.Blocks.Add(new Paragraph(new Run($"Итоговая стоимость с учетом: {selectedOrder.TotalCost:C}")));

                // Печать документа
                IDocumentPaginatorSource idpSource = document;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Печать заказа");
            }
        }
    }
}
