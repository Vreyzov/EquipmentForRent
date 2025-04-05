using System.IO;
using System.Windows;
using System.Windows.Controls;
using EquipmentForRent.Models;
using EquipmentForRent.Windows;
using Microsoft.Win32;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EquipmentForRent.Pages
{
    public partial class ProfileLessorPage : Page
    {
        private Lessor _currentLessor;

        public ProfileLessorPage(Lessor lessor)
        {
            InitializeComponent();
            _currentLessor = lessor;
            LoadLessorData();
        }

        private void LoadLessorData()
        {
            using (var context = new EquipmentRentContext())
            {
                _currentLessor = context.Lessors.FirstOrDefault(l => l.LessorId == _currentLessor.LessorId);

                if (_currentLessor != null)
                {
                    LessorName.Text = $"{_currentLessor.FirstName} {_currentLessor.LastName}";
                    LessorEmail.Text = _currentLessor.Email ?? "Email не указан";

                    if (_currentLessor.Avatar != null && _currentLessor.Avatar.Length > 0)
                    {
                        AvatarImage.Source = ConvertToImage(_currentLessor.Avatar);
                    }
                    else
                    {
                        var defaultImage = context.DefaultImages.FirstOrDefault(img => img.ImageName == "default_avatar");
                        if (defaultImage != null)
                        {
                            AvatarImage.Source = ConvertToImage(defaultImage.ImageData);
                        }
                    }

                    // Загрузка отзывов
                    LoadReviews();
                    // Загрузка рейтинга
                    DisplayRating();
                }
                else
                {
                    MessageBox.Show("Арендодатель не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadReviews()
        {
            using (var context = new EquipmentRentContext())
            {
                var reviews = context.Reviews
                    .Where(r => r.LessorId == _currentLessor.LessorId)
                    .OrderByDescending(r => r.CreatedAt)
                    .Include(r => r.Client)
                    .ToList();

                ReviewsPanel.Children.Clear();

                foreach (var review in reviews)
                {
                    var reviewTextBlock = new TextBlock
                    {
                        Text = $"{review.Client?.FirstName ?? "Неизвестно"} {review.Client?.LastName ?? "Неизвестно"} " +
                               $"({review.Rating} ★)\n{review.ReviewText ?? "Отзыв не оставлен"}",
                        Margin = new Thickness(0, 5, 0, 5),
                        TextWrapping = TextWrapping.Wrap
                    };

                    ReviewsPanel.Children.Add(reviewTextBlock);
                }
            }
        }

        private void DisplayRating()
        {
            using (var context = new EquipmentRentContext())
            {
                var reviews = context.Reviews.Where(r => r.LessorId == _currentLessor.LessorId).ToList();
                if (reviews.Any())
                {
                    double averageRating = reviews.Average(r => r.Rating);
                    LessorRating.Text = $"Средний рейтинг: {averageRating:F1} ★";
                }
                else
                {
                    LessorRating.Text = "Нет отзывов.";
                }
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditProfileLessorWindow(_currentLessor);
            editWindow.ProfileUpdated += updatedLessor =>
            {
                _currentLessor = (Lessor)updatedLessor;
                LoadLessorData();
            };
            editWindow.ShowDialog();
        }

        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg;*.png)|*.jpg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                byte[] imageData = File.ReadAllBytes(openFileDialog.FileName);
                _currentLessor.Avatar = imageData;

                using (var context = new EquipmentRentContext())
                {
                    var lessorToUpdate = context.Lessors.FirstOrDefault(l => l.LessorId == _currentLessor.LessorId);
                    if (lessorToUpdate != null)
                    {
                        lessorToUpdate.Avatar = imageData;
                        context.SaveChanges();
                    }
                }

                AvatarImage.Source = ConvertToImage(imageData);
            }
        }

        private void DeleteAvatar_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new EquipmentRentContext())
            {
                var lessorToUpdate = context.Lessors.FirstOrDefault(l => l.LessorId == _currentLessor.LessorId);
                if (lessorToUpdate != null)
                {
                    lessorToUpdate.Avatar = null;
                    context.SaveChanges();
                }
            }

            _currentLessor.Avatar = null;
            LoadLessorData();
        }

        private void LeaveReview_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new EquipmentRentContext())
            {
                var order = context.Orders
                    .Where(o => o.LessorId == _currentLessor.LessorId)
                    .OrderByDescending(o => o.EndDate)
                    .FirstOrDefault();

                if (order == null)
                {
                    MessageBox.Show("У вас нет завершенных заказов, по которым можно оставить отзыв.",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var reviewWindow = new ReviewWindow(order.ClientId, _currentLessor.LessorId, order.OrderId);
                reviewWindow.ShowDialog();

                LoadLessorData();
            }
        }

        private System.Windows.Media.ImageSource ConvertToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            using (var stream = new MemoryStream(imageData))
            {
                var image = new System.Windows.Media.Imaging.BitmapImage();
                image.BeginInit();
                image.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                return image;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new LessorHomePage(_currentLessor));
            }
        }
    }
}
