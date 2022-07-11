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

            using (AppDbContext appDbContext = new AppDbContext())
            {
                if (!appDbContext.Categories.Any(x => x.CategoryName == "All"))
                {
                    appDbContext.Categories.Add(new CategoryModel() { CategoryName = "All" });
                    appDbContext.SaveChanges();
                }
            }

            ReadCategory();
            ReadTask();
        }

        public void CreateTask()
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                CategoryModel categoryModel = categoryList.SelectedItem as CategoryModel;
                var task = newTask.Text;
                try
                {
                    appDbContext.Tasks.Add(new TaskModel() { Name = task, CategoryModelId = categoryModel.CategoryId });
                    appDbContext.SaveChanges();
                    ReadCategory();
                    ReadTask();
                }
                catch (NullReferenceException)
                {

                }

                newTask.Clear();
            }
        }

        public void CreateCategory(object sender, RoutedEventArgs e)
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                var category = newTask.Text;
                appDbContext.Categories.Add(new CategoryModel() { CategoryName = category });
                appDbContext.SaveChanges();
                ReadCategory();
                ReadTask();
                newTask.Clear();
            }
        }

        private void newTask_KeyDown(object sender, KeyEventArgs e)
        {

            //else if (categoryList.SelectedItem != null)
            //{
            //    if (e.Key == Key.Return)
            //    {
            //        UpdateCategory();
            //        ReadCategory();
            //    }
            //}
            //else
            if (e.Key == Key.Return)
            {
                CreateTask();
                ReadCategory();
                ReadTask();
            }
        }

        public void ReadTask()
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                Tasks = appDbContext.Tasks.ToList();
                currentTasks.ItemsSource = Tasks;
                categoryList.SelectedIndex = 0;
            }
        }

        public void ReadCategory()
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                var cat = appDbContext.Categories.Select(x => new CategoryModel()
                { CategoryName = x.CategoryName, CategoryId = x.CategoryId, NumberOfTasks = x.Tasks.Count() });
                var catList = cat.ToList();
                int countedTasks = 0;

                foreach (var item in catList)
                {
                    countedTasks += item.NumberOfTasks;
                }

                var all = catList.Find(x => x.CategoryName == "All");
                all.NumberOfTasks = countedTasks;

                Categories = appDbContext.Categories.ToList();
                //currentCategories.ItemsSource = catList;
                categoryList.ItemsSource = catList;
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
                    ReadCategory();
                    ReadTask();
                }
            }
        }

        public void UpdateCategory()
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                CategoryModel categoryModel = categoryList.SelectedItem as CategoryModel;
                var category = newTask.Text;

                if (category != null)
                {
                    CategoryModel selectedCategory = appDbContext.Categories.Find(categoryModel.CategoryId);
                    selectedCategory.CategoryName = category;
                    appDbContext.SaveChanges();
                    newTask.Clear();
                    ReadCategory();
                    ReadTask();
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
                    ReadCategory();
                    ReadTask();
                }
            }
        }

        private void DeleteCategory(object sender, MouseButtonEventArgs e)
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                CategoryModel categoryModel = categoryList.SelectedItem as CategoryModel;
                CategoryModel selectedCategory = appDbContext.Categories.Find(categoryModel.CategoryId);
                appDbContext.Categories.Remove(selectedCategory);
                appDbContext.SaveChanges();
                ReadCategory();
                ReadTask();
            }
        }

        private void ShowTasks(object sender, SelectionChangedEventArgs e)
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                CategoryModel categoryModel = categoryList.SelectedItem as CategoryModel;
                var tasks = Tasks.AsEnumerable();

                try
                {
                    if (categoryModel.CategoryName == "All")
                    {
                        ReadTask();
                    }
                    else
                    {
                        tasks = appDbContext.Tasks.ToList().Where(x => x.CategoryModelId == categoryModel.CategoryId);
                        currentTasks.ItemsSource = tasks;
                    }
                }
                catch (NullReferenceException)
                {

                }


            }
        }

        private FrameworkElement FindByName(string name, FrameworkElement root)
        {
            Stack<FrameworkElement> tree = new Stack<FrameworkElement>();
            tree.Push(root);

            while (tree.Count > 0)
            {
                FrameworkElement current = tree.Pop();
                if (current.Name == name)
                    return current;

                int count = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < count; ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(current, i);
                    if (child is FrameworkElement)
                        tree.Push((FrameworkElement)child);
                }
            }

            return null;
        }

        private void changeTaskName(object sender, KeyEventArgs e)
        {
            object o = currentTasks.SelectedItem;
            ListViewItem lvi = (ListViewItem)currentTasks.ItemContainerGenerator.ContainerFromItem(o);

            TextBox textBox = FindByName("taskTextBox", lvi) as TextBox;
            TextBlock textBlock = FindByName("taskTextBlock", lvi) as TextBlock;
            CheckBox checkBox = FindByName("taskCheckBox", lvi) as CheckBox;

            textBox.Visibility = Visibility.Visible;
            textBlock.Visibility = Visibility.Hidden;
            checkBox.Visibility = Visibility.Collapsed;

            TaskModel taskModel = currentTasks.SelectedItem as TaskModel;

            if (e.Key == Key.F2)
            {
                using (AppDbContext appDbContext = new AppDbContext())
                {
                    TaskModel selectedTask = appDbContext.Tasks.Find(taskModel.Id);
                    textBox.Text = selectedTask.Name;
                }
            }

            else if (e.Key == Key.Return)
            {
                using (AppDbContext appDbContext = new AppDbContext())
                {
                    var updatedTask = textBox.Text;
                    TaskModel selectedTask = appDbContext.Tasks.Find(taskModel.Id);
                    selectedTask.Name = updatedTask;
                    appDbContext.SaveChanges();
                    ReadCategory();
                    ReadTask();
                }
            }
        }

        private void changeCategoryName(object sender, KeyEventArgs e)
        {
            object o = categoryList.SelectedItem;
            ListViewItem lvi = (ListViewItem)categoryList.ItemContainerGenerator.ContainerFromItem(o);

            TextBox textBox = FindByName("categoryTextBox", lvi) as TextBox;
            TextBlock textBlock = FindByName("categoryTextBlock", lvi) as TextBlock;

            textBox.Visibility = Visibility.Visible;
            textBlock.Visibility = Visibility.Hidden;

            CategoryModel categoryModel = categoryList.SelectedItem as CategoryModel;

            if (e.Key == Key.F2)
            {
                using (AppDbContext appDbContext = new AppDbContext())
                {
                    CategoryModel selectedCategory = appDbContext.Categories.Find(categoryModel.CategoryId);
                    textBox.Text = selectedCategory.CategoryName;
                }
            }

            else if (e.Key == Key.Return)
            {
                using (AppDbContext appDbContext = new AppDbContext())
                {
                    var updatedCateogry = textBox.Text;
                    CategoryModel selectedCategory = appDbContext.Categories.Find(categoryModel.CategoryId);
                    selectedCategory.CategoryName = updatedCateogry;
                    appDbContext.SaveChanges();
                    ReadCategory();
                    ReadTask();
                }
            }
        }
    }
}
