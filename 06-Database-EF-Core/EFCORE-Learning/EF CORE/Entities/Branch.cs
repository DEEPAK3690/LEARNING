using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_CORE.Entities
{
    public class Branch
    {
        [Key]
        public int Branch_Id { get; set; }
        [Required]
        public string? BranchName { get; set; }
        public string? Description { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        // Collection navigation property representing the students enrolled in the branch
        public ICollection<Student>? Students { get; set; }
    }
}
