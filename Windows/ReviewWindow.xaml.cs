using EquipmentForRent.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace EquipmentForRent.Windows
{
    public partial class ReviewWindow : Window
    {
        private int _clientId;
        private int _lessorId;
        private int _orderId;

        public ReviewWindow(int clientId, int lessorId, int orderId)
        {
            InitializeComponent();
            _clientId = clientId;
            _lessorId = lessorId;
            _orderId = orderId;
            LoadReviewData();
        }

        private void LoadReviewData()
        {
            using (var context = new EquipmentRentContext())
            {
                var order = context.Orders.FirstOrDefault(o => o.OrderId == _orderId);
                if (order == null)
                {
                    MessageBox.Show("Заказ не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                var lessor = context.Lessors.FirstOrDefault(l => l.LessorId == _lessorId);
                if (lessor != null)
                {
                    LessorNameTextBlock.Text = $"{lessor.FirstName} {lessor.LastName}";
                    LoadLessorAvatar(lessor.Avatar);
                }

                var equipment = context.Equipment.FirstOrDefault(e => e.Orders.Any(o => o.OrderId == _orderId));
                if (equipment != null)
                {
                    EquipmentNameTextBlock.Text = equipment.Name;
                }
            }
        }

        private void LoadLessorAvatar(byte[] avatarData)
        {
            if (avatarData != null && avatarData.Length > 0)
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(avatarData))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                }
                LessorAvatarImage.Source = bitmap;
            }
            else
            {
                LessorAvatarImage.Source = new BitmapImage(new Uri("/Images/default_avatar.png", UriKind.Relative));
            }
        }

        private void SubmitReview_Click(object sender, RoutedEventArgs e)
        {
            if (RatingComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите рейтинг!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int rating = RatingComboBox.SelectedIndex + 1;
            string reviewText = ReviewTextBox.Text.Trim();

            using (var context = new EquipmentRentContext())
            {
                var review = new Review
                {
                    ClientId = _clientId,
                    LessorId = _lessorId,
                    OrderId = _orderId,
                    Rating = rating,
                    ReviewText = string.IsNullOrEmpty(reviewText) ? null : reviewText,
                    CreatedAt = DateTime.Now
                };

                context.Reviews.Add(review);
                context.SaveChanges();
            }

            MessageBox.Show("Спасибо за ваш отзыв!", "Отзыв отправлен", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
