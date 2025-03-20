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
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void RegistrationClick(object sender, RoutedEventArgs e)
        {
            Registration taskWindow = new Registration();
            
            taskWindow.Show();
            this.Hide();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var dbHelper = new DatabaseHelper();

            // Авторизация через хранимую процедуру
            var (userId, role) = dbHelper.AuthenticateUser(
                loginTextBox.Text,
                passwordBox.Password
            );

            if (userId != -1)
            {

                MessageBox.Show($"Добро пожаловать! Ваша роль: {role}");
                // Открытие главного окна в зависимости от роли
                menu taskWindow = new menu();

                taskWindow.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
        }
    }
}
