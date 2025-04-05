using System.IO;
using System.Windows;
using System.Windows.Controls;
using EquipmentForRent.Models;
using EquipmentForRent.Windows;
using Microsoft.Win32;
using System.Linq;

namespace EquipmentForRent.Pages
{
    public partial class ProfileClientPage : Page
    {
        private Client _currentClient;

        // Конструктор, принимающий объект клиента
        public ProfileClientPage(Client client)
        {
            InitializeComponent();
            _currentClient = client;
            LoadClientData();
        }

        // Метод для загрузки данных клиента в интерфейс
        private void LoadClientData()
        {
            using (var context = new EquipmentRentContext())
            {
                _currentClient = context.Clients.FirstOrDefault(c => c.ClientId == _currentClient.ClientId);

                if (_currentClient != null)
                {
                    ClientName.Text = $"{_currentClient.FirstName} {_currentClient.LastName}";
                    ClientEmail.Text = _currentClient.Email ?? "Email не указан";

                    // Отображение аватара
                    if (_currentClient.Avatar != null && _currentClient.Avatar.Length > 0)
                    {
                        AvatarImage.Source = ConvertToImage(_currentClient.Avatar);
                    }
                    else
                    {
                        var defaultImage = context.DefaultImages.FirstOrDefault(img => img.ImageName == "default_avatar");
                        if (defaultImage != null)
                        {
                            AvatarImage.Source = ConvertToImage(defaultImage.ImageData);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Клиент не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Обработчик кнопки редактирования профиля
        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditProfileClientWindow(_currentClient);

            // Подписываемся на событие обновления профиля
            editWindow.ProfileUpdated += updatedClient =>
            {
                _currentClient = (Client)updatedClient;
                LoadClientData();
            };

            editWindow.ShowDialog(); // Открываем окно
        }

        // Метод для смены аватара
        private void ChangeAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg;*.png)|*.jpg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                byte[] imageData = File.ReadAllBytes(openFileDialog.FileName);
                _currentClient.Avatar = imageData;

                using (var context = new EquipmentRentContext())
                {
                    var clientToUpdate = context.Clients.FirstOrDefault(c => c.ClientId == _currentClient.ClientId);
                    if (clientToUpdate != null)
                    {
                        clientToUpdate.Avatar = imageData;
                        context.SaveChanges();
                    }
                }

                AvatarImage.Source = ConvertToImage(imageData);
            }
        }

        // Метод для удаления аватара
        private void DeleteAvatar_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new EquipmentRentContext())
            {
                var clientToUpdate = context.Clients.FirstOrDefault(c => c.ClientId == _currentClient.ClientId);
                if (clientToUpdate != null)
                {
                    clientToUpdate.Avatar = null;
                    context.SaveChanges();
                }
            }

            _currentClient.Avatar = null;
            LoadClientData();
        }

        // Преобразование байтового массива в изображение
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

        // Обработчик кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new ClientHomePage(_currentClient));
            }
        }
    }
}
