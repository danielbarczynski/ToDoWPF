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
        public List<CategoryModel> Categories { get; private set; }

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
            ReadCategory();
        }

        public void CreateTask()
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                CategoryModel categoryModel = currentCategories.SelectedItem as CategoryModel;
                var task = newTask.Text; 
                appDbContext.Tasks.Add(new TaskModel() { Name = task, CategoryModelId = categoryModel.CategoryId});
                appDbContext.SaveChanges(); 
                newTask.Clear();
            }
        }

        public void CreateCategory()
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                var category = newTask.Text;
                appDbContext.Categories.Add(new CategoryModel() { CategoryName = category });
                appDbContext.SaveChanges();
                newTask.Clear();
            }
        }

        private void newTask_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentTasks.SelectedItem != null)
            {
                if (e.Key == Key.Return)
                {
                    UpdateTask();
                    ReadTask();
                }
            }

            else if (e.Key == Key.Return)
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

        public void ReadCategory()
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                Categories = appDbContext.Categories.ToList();
                currentCategories.ItemsSource = Categories;
                categoryList.ItemsSource = Categories;
            }
        }
        public void UpdateTask()
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                TaskModel taskModel = currentTasks.SelectedItem as TaskModel;
                var task = newTask.Text;

                if (task != null)
                {
                    TaskModel selectedTask = appDbContext.Tasks.Find(taskModel.Id);
                    selectedTask.Name = task;
                    appDbContext.SaveChanges();
                    newTask.Clear();
                }
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
    }
}
