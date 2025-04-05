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
using System.Windows.Shapes;

namespace EquipmentForRent.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditEquipmentWindow.xaml
    /// </summary>
    public partial class EditEquipmentWindow : Window
    {
        private Equipment _equipment;
        public EditEquipmentWindow(Equipment equipment)
        {
            InitializeComponent();
            _equipment = equipment;

            // Заполнение полей окна данными выбранного оборудования
            NameTextBox.Text = _equipment.Name;
            PriceTextBox.Text = _equipment.PricePerDay.ToString();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
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

                // Обновление данных оборудования
                _equipment.Name = name;
                _equipment.PricePerDay = price;

                using (var db = new EquipmentRentContext())
                {
                    var equipmentToUpdate = db.Equipment.FirstOrDefault(eq => eq.EquipmentId == _equipment.EquipmentId);
                    if (equipmentToUpdate != null)
                    {
                        equipmentToUpdate.Name = _equipment.Name;
                        equipmentToUpdate.PricePerDay = _equipment.PricePerDay;
                        db.SaveChanges();
                    }
                }

                // Сообщение об успешном сохранении
                MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}


