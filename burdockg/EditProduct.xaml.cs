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
using Microsoft.Win32;
using System.IO;

namespace burdockg
{
    /// <summary>
    /// Логика взаимодействия для EditProduct.xaml
    /// </summary>
    public partial class EditProduct : Window
    {
        private string selectedImagePath = null;
        private int productId; // To store the ID of the product being edited

        public EditProduct()
        {
            InitializeComponent();
        }

        // Constructor that accepts a product ID
        public EditProduct(int id)
        {
            InitializeComponent();
            productId = id;
            LoadProductData(id);
        }

        // Method to load product data based on ID
        private void LoadProductData(int id)
        {
            // Here you would load the product data from your database
            // For now, we'll just set some sample data
            productNameTextBox.Text = "Тестовый продукт " + id;
            productTypeComboBox.SelectedIndex = 0;
            materialsComboBox.SelectedIndex = 0;
            costTextBox.Text = "1000";

            // Load product image if available
            // For now, we'll just use the default image
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to home window
            home homeWindow = new home();
            homeWindow.Show();
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(productNameTextBox.Text) ||
                productTypeComboBox.SelectedItem == null ||
                materialsComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(costTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Try to parse cost as a number
            if (!decimal.TryParse(costTextBox.Text, out decimal cost))
            {
                MessageBox.Show("Стоимость должна быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Here you would update the product in your database
            MessageBox.Show("Изменения сохранены успешно!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Navigate back to home window
            home homeWindow = new home();
            homeWindow.Show();
            this.Close();
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Все файлы|*.*";
            openFileDialog.Title = "Выберите изображение продукта";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    selectedImagePath = openFileDialog.FileName;
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(selectedImagePath);
                    bitmap.EndInit();
                    productImage.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
