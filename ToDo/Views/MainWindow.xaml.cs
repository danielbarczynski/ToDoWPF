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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToDo.Database;
using ToDo.Models;

namespace ToDo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<TaskModel> Tasks { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            ReadTask();
        }

        public void CreateTask()
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                var task = newTask.Text;
                appDbContext.Tasks.Add(new TaskModel() { Name = task });
                appDbContext.SaveChanges();
            }
        }

        private void newTask_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CreateTask();
                ReadTask();
            }
        }

        public void ReadTask()
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                Tasks = appDbContext.Tasks.ToList();
                currentTasks.ItemsSource = Tasks;
            }
        }
    }
}
