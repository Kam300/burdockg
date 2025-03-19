using System;
using System.Windows;
using System.Windows.Controls;

namespace burdockg
{
    public partial class menu : Window
    {
        public menu()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to Authorization
            Authorization authWindow = new Authorization();
            authWindow.Show();
            this.Hide();
        }

        private void ProductInfoButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to home window (product information)
            home homeWindow = new home();
            homeWindow.Show();
            this.Hide();
        }

        // Add other button click handlers as needed
        private void ProductionButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to production window
            MessageBox.Show("Переход к окну производства", "Навигация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void WarehouseButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to warehouse window
            MessageBox.Show("Переход к окну склада", "Навигация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to employees window
            MessageBox.Show("Переход к окну сотрудников", "Навигация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AgentsButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to agents window
            MessageBox.Show("Переход к окну агентов", "Навигация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ProfileImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Navigate to profile window
            profile profileWindow = new profile();
            profileWindow.Show();
            this.Hide();
        }
    }
}
