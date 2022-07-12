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
using ToDo.Database;
using ToDo.Models;

namespace ToDo
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            var username = Username.Text;
            var password = Password.Password;

            using (AppDbContext context = new AppDbContext())
            {
                bool user = context.Users.Any(x => x.Username == username && x.Password == password);

                if (user)
                {
                    MainWindowNav();
                    Close();
                }
                else
                {
                    MessageBox.Show("Incorrect password or username.", "Error");
                }
            }
        }

        public void MainWindowNav()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        private void SignUpWindowClick(object sender, RoutedEventArgs e)
        {
            SingUpWindowNav();
            Close();
        }

        public void SingUpWindowNav()
        {
            Window singUp = new SignUp();
            singUp.Show();
        }
    }
}
