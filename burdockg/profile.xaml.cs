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
using Npgsql;

namespace burdockg
{
    /// <summary>
    /// Логика взаимодействия для profile.xaml
    /// </summary>
    public partial class profile : Window
    {
        private string _connectionString = "Host=localhost;Port=5432;Database=лопух;Username=postgres;Password=00000000;";
        private int _userId; // Store the current user ID

        public profile(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadProfileData();
        }

        // Default constructor for backward compatibility
        public profile()
        {
            InitializeComponent();
            // If no user ID is provided, try to get it from the current session
            // For now, we'll use a default value of 1
            _userId = 1;
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    
                    // Based on your query, the table name is "Users" in public schema
                    using (var cmd = new NpgsqlCommand(
                        "SELECT u.\"ID\", u.\"LastName\", u.\"FirstName\", u.\"MiddleName\", u.\"Login\", u.\"Role\" " +
                        "FROM public.\"Users\" u " +
                        "WHERE u.\"ID\" = @userId", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", _userId);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Set the text boxes with data from the database
                                loginTextBox.Text = reader["Login"].ToString();
                                lastNameTextBox.Text = reader["LastName"].ToString();
                                firstNameTextBox.Text = reader["FirstName"].ToString();
                                middleNameTextBox.Text = reader["MiddleName"].ToString();
                                roleTextBox.Text = reader["Role"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Пользователь не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных профиля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Also update the SaveButton_Click method to use the correct table name
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    
                    using (var cmd = new NpgsqlCommand(
                        "UPDATE public.\"Users\" SET \"LastName\" = @lastName, \"FirstName\" = @firstName, \"MiddleName\" = @middleName " +
                        "WHERE \"ID\" = @userId", conn))
                    {
                        cmd.Parameters.AddWithValue("@lastName", lastNameTextBox.Text);
                        cmd.Parameters.AddWithValue("@firstName", firstNameTextBox.Text);
                        cmd.Parameters.AddWithValue("@middleName", middleNameTextBox.Text);
                        cmd.Parameters.AddWithValue("@userId", _userId);
                        
                        int rowsAffected = cmd.ExecuteNonQuery();
                        
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные профиля успешно обновлены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить данные профиля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных профиля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to menu
            menu menuWindow = new menu();
            menuWindow.Show();
            this.Hide();
        }
    }
}
