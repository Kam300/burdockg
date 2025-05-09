﻿using System;
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
            var dbHelper = new DatabaseHelper();

            // Проверка совпадения паролей
            if (passwordBox.Password != confirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            // Регистрация через хранимую процедуру
            bool success = dbHelper.RegisterUser(
                lastNameTextBox.Text,
                firstNameTextBox.Text,
                middleNameTextBox.Text,
                loginTextBox.Text,
                passwordBox.Password,
                roleComboBox.SelectedItem.ToString()
            );

            if (success)
            {
                MessageBox.Show("Регистрация успешна!");
                // Переход на главный экран
            }
            else
            {
                MessageBox.Show("Ошибка регистрации. Возможно, логин занят.");
            }
        }
    }
}
