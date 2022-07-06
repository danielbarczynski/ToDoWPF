using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Models
{
    public class CategoryListingModel
    {
        [Key]
        public int CategoryListingId { get; set; }
        public string CategoryListingName { get; set; }
        public int NumberOfTasks { get; set; }
    }
}
