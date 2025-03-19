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
            // Here you would add authentication logic
            // For now, we'll just navigate to the menu window
            
            menu menuWindow = new menu();
            menuWindow.Show();
            this.Hide();
        }
    }
}
