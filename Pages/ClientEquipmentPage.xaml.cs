using EquipmentForRent.Models;
using EquipmentForRent.Windows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EquipmentForRent.Pages
{
    public partial class ClientEquipmentPage : Page
    {
        private int _clientId;

        public ClientEquipmentPage(int clientId)
        {
            InitializeComponent();
            _clientId = clientId;
            LoadEquipment();
        }

        private void LoadEquipment(string searchQuery = "", decimal? minPrice = null, decimal? maxPrice = null)
        {
            using (var context = new EquipmentRentContext())
            {
                var equipmentList = context.Equipment
                                           .Include(e => e.Lessor)
                                           .ToList();

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    equipmentList = equipmentList
                        .Where(e => e.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (minPrice.HasValue)
                {
                    equipmentList = equipmentList.Where(e => e.PricePerDay >= minPrice.Value).ToList();
                }

                if (maxPrice.HasValue)
                {
                    equipmentList = equipmentList.Where(e => e.PricePerDay <= maxPrice.Value).ToList();
                }

                byte[] defaultAvatar = context.DefaultImages
                                              .Where(img => img.ImageName == "default_avatar")
                                              .Select(img => img.ImageData)
                                              .FirstOrDefault();

                var equipmentWithImages = equipmentList.Select(e => new EquipmentDisplayModel
                {
                    Name = e.Name,
                    PricePerDay = e.PricePerDay,
                    Lessor = e.Lessor,
                    AvatarImage = ConvertBytesToBitmapImage(e.Lessor.Avatar ?? defaultAvatar)
                }).ToList();

                EquipmentListBox.ItemsSource = equipmentWithImages;
            }
        }

        private BitmapImage ConvertBytesToBitmapImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;

            var bitmap = new BitmapImage();
            using (var stream = new System.IO.MemoryStream(imageData))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            return bitmap;
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentListBox.SelectedItem is EquipmentDisplayModel selectedEquipment)
            {
                var selectedLessorEquipment = selectedEquipment.Lessor.Equipment.FirstOrDefault();
                if (selectedLessorEquipment != null)
                {
                    var newOrderWindow = new NewOrderWindow(_clientId, selectedLessorEquipment);
                    newOrderWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Оборудование не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите оборудование для оформления заказа.",
                                 "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClientHomePage(GetClientById(_clientId)));
        }

        private Client GetClientById(int clientId)
        {
            using (var context = new EquipmentRentContext())
            {
                return context.Clients.FirstOrDefault(c => c.ClientId == clientId);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = SearchTextBox.Text;
            decimal? minPrice = null;
            decimal? maxPrice = null;

            if (decimal.TryParse(MinPriceTextBox.Text, out var minPriceValue))
            {
                minPrice = minPriceValue;
            }

            if (decimal.TryParse(MaxPriceTextBox.Text, out var maxPriceValue))
            {
                maxPrice = maxPriceValue;
            }

            LoadEquipment(searchQuery, minPrice, maxPrice);
        }
    }

    public class EquipmentDisplayModel
    {
        public string Name { get; set; }
        public decimal PricePerDay { get; set; }
        public Lessor Lessor { get; set; }
        public BitmapImage AvatarImage { get; set; }
    }
}
