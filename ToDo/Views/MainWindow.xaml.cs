using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ToDo.Database;
using ToDo.Models;

// Z celów edukacyjych postanowiłem stworzyć tą aplikację bez wzorca MVVM oraz serwisów, by mieć porównanie do innego projektu i lepiej zrozumieć znaczenie ww. rzeczy. Także nie jest to najczystszy kod. Nie wprowadzałem też asynchroniczności ponieważ nie jest w tej aplikacji na razie potrzebna. Jest też kilka funkcji, które mam jeszcze w planach zaimplementować (np. powiązanie użytkownika z zadaniami), ale ponieważ terminy mnie gonią (m.in. chciałbym jeszcze dokończyć niektóre zadania z labów), a czasu mam niewiele, wrzucam projekt w obecnej formie. Jeśli starczy mi czasu, to wrzucę go jeszcze raz z nowymi funkcjami. 

namespace ToDo
{
    public partial class MainWindow : Window
    {
        public List<TaskModel> Tasks { get; private set; }
        public List<CategoryModel> Categories { get; private set; }

        public MainWindow()
        {
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += TimerTick;
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

        //------------------------------------------------------------------------------------------
        // CUSTOM METHODS
        //------------------------------------------------------------------------------------------

        void TimerTick(object sender, EventArgs e)
        {
            Today.Text = DateTime.Now.ToString("F");
        }

        FrameworkElement FindByName(string name, FrameworkElement root)
        {
            Stack<FrameworkElement> tree = new Stack<FrameworkElement>();
            tree.Push(root);

            while (tree.Count > 0)
            {
                FrameworkElement current = tree.Pop();

                if (current.Name == name)
                {
                    return current;
                }

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

        void NewTaskText(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox.Text == "Your new task/category...")
            {
                textBox.Text = "";
            }
        }

        void Help(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Press F2 to modify task/category and Esc to escape.\nDouble click on category to delete it.", "Help");
        }

        //------------------------------------------------------------------------------------------
        // TASK CRUD
        //------------------------------------------------------------------------------------------

        void CreateTask(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                using (AppDbContext appDbContext = new AppDbContext())
                {
                    CategoryModel categoryModel = categoryList.SelectedItem as CategoryModel;
                    var selectedIndex = categoryList.SelectedIndex;
                    var task = newTask.Text;


                    try
                    {
                        if (categoryModel.CategoryName == "All")
                        {
                            MessageBoxResult result = MessageBox.Show
                            ("You cannot add tasks to the \"All\" category.", "Error");
                        }

                        else if (task.Length > 0)
                        {
                            appDbContext.Tasks.Add(new TaskModel() { Name = task, CategoryModelId = categoryModel.CategoryId });
                            appDbContext.SaveChanges();
                            Tasks = appDbContext.Tasks.Where(x => x.CategoryModelId == categoryModel.CategoryId).ToList();
                            currentTasks.ItemsSource = Tasks;
                            ReadCategory();
                            newTask.Clear();
                        }
                    }

                    catch (NullReferenceException)
                    {

                    }

                    categoryList.SelectedIndex = selectedIndex;
                }
            }
        }

        void ReadTask()
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                Tasks = appDbContext.Tasks.ToList();
                currentTasks.ItemsSource = Tasks;
                categoryList.SelectedIndex = 0;
            }
        }

        void UpdateTask(object sender, KeyEventArgs e)
        {
            object o = currentTasks.SelectedItem;
            ListViewItem lvi = (ListViewItem)currentTasks.ItemContainerGenerator.ContainerFromItem(o);
            TextBox textBox = FindByName("taskTextBox", lvi) as TextBox;
            TextBlock textBlock = FindByName("taskTextBlock", lvi) as TextBlock;
            CheckBox checkBox = FindByName("taskCheckBox", lvi) as CheckBox;
            TaskModel taskModel = currentTasks.SelectedItem as TaskModel;
            var selectedIndex = categoryList.SelectedIndex;

            if (e.Key == Key.F2)
            {
                textBox.Visibility = Visibility.Visible;
                textBlock.Visibility = Visibility.Hidden;
                checkBox.Visibility = Visibility.Collapsed;

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
                    CategoryModel categoryModel = categoryList.SelectedItem as CategoryModel;
                    var updatedTask = textBox.Text;

                    if (updatedTask.Length > 0)
                    {
                        TaskModel selectedTask = appDbContext.Tasks.Find(taskModel.Id);
                        selectedTask.Name = updatedTask;
                        appDbContext.SaveChanges();
                        Tasks = appDbContext.Tasks.Where(x => x.CategoryModelId == categoryModel.CategoryId).ToList();
                        currentTasks.ItemsSource = Tasks;
                        ReadCategory();
                    }
                }
            }

            else if (e.Key == Key.Escape)
            {
                textBox.Visibility = Visibility.Collapsed;
                textBlock.Visibility = Visibility.Visible;
            }

            categoryList.SelectedIndex = selectedIndex;
        }

        void DeleteTask(object sender, RoutedEventArgs e)
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                CategoryModel categoryModel = categoryList.SelectedItem as CategoryModel;
                var selectedIndex = categoryList.SelectedIndex;
                var checkbox = sender as CheckBox;

                if (checkbox != null)
                {
                    var task = checkbox.DataContext as TaskModel;
                    TaskModel taskModel = appDbContext.Tasks.Find(task.Id);
                    appDbContext.Tasks.Remove(taskModel);
                    appDbContext.SaveChanges();
                    Tasks = appDbContext.Tasks.Where(x => x.CategoryModelId == categoryModel.CategoryId).ToList();
                    currentTasks.ItemsSource = Tasks;
                    ReadCategory();
                }

                categoryList.SelectedIndex = selectedIndex;
            }
        }

        void ShowTasks(object sender, SelectionChangedEventArgs e)
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

        //------------------------------------------------------------------------------------------
        // CATEGORY CRUD
        //------------------------------------------------------------------------------------------

        void CreateCategory(object sender, RoutedEventArgs e)
        {
            using (AppDbContext appDbContext = new AppDbContext())
            {
                var category = newTask.Text;
                if (category.Length > 0)
                {
                    appDbContext.Categories.Add(new CategoryModel() { CategoryName = category });
                    appDbContext.SaveChanges();
                    ReadCategory();
                    newTask.Clear();
                }
            }
        }

        void ReadCategory()
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
                categoryList.ItemsSource = catList;
            }
        }

        void UpdateCategory(object sender, KeyEventArgs e)
        {
            object o = categoryList.SelectedItem;
            var selectedIndex = categoryList.SelectedIndex;
            ListViewItem lvi = (ListViewItem)categoryList.ItemContainerGenerator.ContainerFromItem(o);

            TextBox textBox = FindByName("categoryTextBox", lvi) as TextBox;
            TextBlock textBlock = FindByName("categoryTextBlock", lvi) as TextBlock;
            CategoryModel categoryModel = categoryList.SelectedItem as CategoryModel;

            if (e.Key == Key.F2)
            {
                if (categoryModel.CategoryName == "All")
                {
                    MessageBox.Show("You cannot modify this category.", "Error");
                }

                else
                {
                    textBox.Visibility = Visibility.Visible;
                    textBlock.Visibility = Visibility.Hidden;


                    using (AppDbContext appDbContext = new AppDbContext())
                    {
                        CategoryModel selectedCategory = appDbContext.Categories.Find(categoryModel.CategoryId);
                        textBox.Text = selectedCategory.CategoryName;
                    }
                }
            }

            else if (e.Key == Key.Return)
            {
                using (AppDbContext appDbContext = new AppDbContext())
                {
                    var updatedCateogry = textBox.Text;

                    if (updatedCateogry.Length > 0)
                    {
                        CategoryModel selectedCategory = appDbContext.Categories.Find(categoryModel.CategoryId);
                        selectedCategory.CategoryName = updatedCateogry;
                        appDbContext.SaveChanges();
                        ReadCategory();
                    }

                    categoryList.SelectedIndex = selectedIndex;
                }
            }

            else if (e.Key == Key.Escape)
            {
                textBox.Visibility = Visibility.Collapsed;
                textBlock.Visibility = Visibility.Visible;
            }
        }

        void DeleteCategory(object sender, MouseButtonEventArgs e)
        {

            using (AppDbContext appDbContext = new AppDbContext())
            {
                CategoryModel categoryModel = categoryList.SelectedItem as CategoryModel;
                CategoryModel selectedCategory = appDbContext.Categories.Find(categoryModel.CategoryId);

                if (selectedCategory.CategoryName == "All")
                {
                    MessageBox.Show("You cannot delete this category.", "Error");
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show
                    ("Are you sure?", $"Delete Category {categoryModel.CategoryName}", MessageBoxButton.OKCancel,
                    MessageBoxImage.Information, MessageBoxResult.OK);

                    if (result == MessageBoxResult.OK)
                    {
                        appDbContext.Categories.Remove(selectedCategory);
                        appDbContext.SaveChanges();
                        ReadCategory();
                        ReadTask();
                    }
                }

            }
        }
    }
}
