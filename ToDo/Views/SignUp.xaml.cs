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
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            var username = Username.Text;
            var password = Password.Password;

            using (AppDbContext appDbContext = new AppDbContext())
            {
                if (username.Length >= 3 && password.Length >= 8)
                {
                    appDbContext.Users.Add(new User { Username = username, Password = password });
                    appDbContext.SaveChanges();
                    MessageBox.Show("Successfully created account.", "Information");
                    LoginWindowNav();
                    Close();
                }

                else
                {
                    MessageBox.Show("Username: minmum 3 characters\nPassword: minimum 8 characters.", "Error");
                }
            }         
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
