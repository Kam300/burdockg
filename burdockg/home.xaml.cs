using System;
using System.Windows;
using System.Windows.Controls;

namespace burdockg
{
    public partial class home : Window
    {
        public home()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to AddProduct window
            AddProduct addProductWindow = new AddProduct();
            addProductWindow.Show();
            this.Hide();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to menu
            menu menuWindow = new menu();
            menuWindow.Show();
            this.Hide();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the product ID from the clicked item
            // For now, we'll just use a sample ID of 1
            int productId = 1;
            
            // Navigate to edit product window
            EditProduct editProductWindow = new EditProduct(productId);
            editProductWindow.Show();
            this.Hide();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Show confirmation dialog
            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить этот продукт?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Here you would add code to delete the product
                MessageBox.Show("Продукт успешно удален!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
