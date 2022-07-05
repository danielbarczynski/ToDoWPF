using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ToDo.Database;
using ToDo.Models;

namespace ToDo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void DeleteCategory(object sender, RoutedEventArgs e)
        {
            //MainWindow window = new MainWindow();
            //using (AppDbContext appDbContext = new AppDbContext())
            //{

            //        var category = categoryList.DataContext as TaskModel;
            //        CategoryModel categoryModel = appDbContext.Tasks.Find(category.CategoryId);
            //        appDbContext.Categories.Remove(taskModel);
            //        appDbContext.SaveChanges();
            //        window.ReadCategory();
            //}
            MainWindow window = new MainWindow();
            window.DeleteCategory();
        }
    }
}
