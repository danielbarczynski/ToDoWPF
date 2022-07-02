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

namespace ToDo
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            MainWindowNav();
            Close();
        }

        public void MainWindowNav()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        public void LoginWindowClick(object sender, RoutedEventArgs e)
        {
            LoginWindowNav();
            Close();
        }

        private void LoginWindowNav()
        {
            Window login = new Login();
            login.Show();
        }
    }
}
