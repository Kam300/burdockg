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
    /// Логика взаимодействия для profile.xaml
    /// </summary>
    public partial class profile : Window
    {
        public profile()
        {
            InitializeComponent();
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            // Here you would load the user's profile data
            // For now, we'll just set some sample data
            lastNameTextBox.Text = "Иванов";
            firstNameTextBox.Text = "Иван";
            middleNameTextBox.Text = "Иванович";
            roleTextBox.Text = "Администратор";
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
