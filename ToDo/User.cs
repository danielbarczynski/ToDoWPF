using System.ComponentModel.DataAnnotations;

namespace ToDo
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password{ get; set; }
    }
}