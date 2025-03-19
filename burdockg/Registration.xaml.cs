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

namespace burdockg
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
            
            // Initialize the role combobox
            roleComboBox.Items.Add("Администратор");
            roleComboBox.Items.Add("Менеджер");
            roleComboBox.Items.Add("Клиент");
            roleComboBox.SelectedIndex = 2; // Default to Client
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Authorization taskWindow = new Authorization();
            taskWindow.Show();
            this.Hide();
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            // Here you would add registration validation logic
            
            // Check if passwords match
            if (passwordBox.Password != confirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // Check if all fields are filled
            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(firstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(loginTextBox.Text) ||
                string.IsNullOrWhiteSpace(passwordBox.Password) ||
                roleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            // Registration successful, navigate to home
            MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            
            home homeWindow = new home();
            homeWindow.Show();
            this.Hide();
        }
    }
}
