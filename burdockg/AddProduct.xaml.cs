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
    /// Логика взаимодействия для AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Window
    {
        private string selectedImagePath = null;

        public AddProduct()
        {
            InitializeComponent();
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
            // Here you would add code to save the product data
            MessageBox.Show("Продукт успешно сохранен!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            
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
