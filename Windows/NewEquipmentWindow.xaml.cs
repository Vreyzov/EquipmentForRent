using EquipmentForRent.Pages;
using Microsoft.Data.SqlClient;
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
using System.Windows.Shapes;

namespace EquipmentForRent.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewEquipmentWindow.xaml
    /// </summary>
    public partial class NewEquipmentWindow : Window
    {
        private LessorEquipmentPage _parentPage;
        public NewEquipmentWindow(LessorEquipmentPage parentPage)
        {
            InitializeComponent();
            _parentPage = parentPage;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string priceText = PriceTextBox.Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(priceText) || !decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            if (price <= 0)
            {
                MessageBox.Show("Цена за день должна быть больше нуля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Добавление нового оборудования в базу данных
            string connectionString = "Server=PC;Database=EquipmentRent;TrustServerCertificate=True;Trusted_Connection=True;"; // Укажите строку подключения
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Equipment (Name, PricePerDay, LessorID) VALUES (@Name, @PricePerDay, @LessorID)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@PricePerDay", price);
                command.Parameters.AddWithValue("@LessorID", 1); // Пример ID арендатора
                command.ExecuteNonQuery();
            }

            // Отображаем всплывающее окно с уведомлением
            MessageBox.Show("Успешно сохранено!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

            // Обновляем список оборудования в родительской странице
            _parentPage.LoadEquipmentData();

            // Закрытие окна после сохранения
            this.Close();
        }



        // Обработчик кнопки "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

