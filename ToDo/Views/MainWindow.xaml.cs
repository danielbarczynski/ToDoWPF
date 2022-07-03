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
using System.Windows.Threading;
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
        void timer_Tick(object sender, EventArgs e)
        {
            Today.Text = DateTime.Now.ToString("F");
        }
        public MainWindow()
        {
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
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

        public void DeleteTask(object sender, RoutedEventArgs e)
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                var checkbox = sender as CheckBox;
                
                if (checkbox != null)
                {
                    var task = checkbox.DataContext as TaskModel;
                    TaskModel taskModel = appDbContext.Tasks.Find(task.Id);
                    appDbContext.Tasks.Remove(taskModel);
                    appDbContext.SaveChanges();
                    ReadTask();
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
