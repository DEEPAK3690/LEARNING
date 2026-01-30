using System.ComponentModel.DataAnnotations;

namespace MyWebApplication.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Position { get; set; }
        [Range(18, 65)]
        public int Age { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
