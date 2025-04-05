using EquipmentForRent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using EquipmentForRent.Windows;

namespace EquipmentForRent.Pages
{
    public partial class LessorEquipmentPage : Page
    {
        private int _lessorId; // ID арендодателя

        public LessorEquipmentPage(int lessorId)
        {
            InitializeComponent();
            _lessorId = lessorId;
            LoadEquipmentData();
        }

        public void LoadEquipmentData()
        {
            string connectionString = "Server=PC;Database=EquipmentRent;TrustServerCertificate=True;Trusted_Connection=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT EquipmentID, Name, PricePerDay FROM Equipment WHERE LessorID = @LessorID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@LessorID", _lessorId);

                SqlDataReader reader = command.ExecuteReader();

                var equipmentList = new List<Equipment>();
                while (reader.Read())
                {
                    equipmentList.Add(new Equipment
                    {
                        EquipmentId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        PricePerDay = reader.GetDecimal(2)
                    });
                }

                EquipmentListBox.ItemsSource = equipmentList;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NewEquipmentWindow newEquipmentWindow = new NewEquipmentWindow(this);
            newEquipmentWindow.ShowDialog();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEquipment = EquipmentListBox.SelectedItem as Equipment;
            if (selectedEquipment == null)
            {
                MessageBox.Show("Пожалуйста, выберите оборудование для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditEquipmentWindow editEquipmentWindow = new EditEquipmentWindow(selectedEquipment);
            if (editEquipmentWindow.ShowDialog() == true)
            {
                LoadEquipmentData();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEquipment = EquipmentListBox.SelectedItem as Equipment;
            if (selectedEquipment != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить это оборудование?", "Удалить", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    string connectionString = "Server=PC;Database=EquipmentRent;TrustServerCertificate=True;Trusted_Connection=True;";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Equipment WHERE EquipmentID = @EquipmentID";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@EquipmentID", selectedEquipment.EquipmentId);
                        command.ExecuteNonQuery();
                    }

                    LoadEquipmentData();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите оборудование для удаления.");
            }
        }
    }
}
