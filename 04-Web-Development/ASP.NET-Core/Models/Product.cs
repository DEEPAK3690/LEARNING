using System.ComponentModel.DataAnnotations;

namespace MyWebApplication.Models
{

   // Models are integral to controllers for binding incoming data, processing it, and returning responses.
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }
        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }
        [Required]
        public string Category { get; set; }
    }
}
