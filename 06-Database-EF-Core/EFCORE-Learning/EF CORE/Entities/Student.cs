using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_CORE.Entities
{
    [Table("tb_Students")]
    public class Student
    {
        [Key]//used to denaote as primary key
        public int Students { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }

        [Column("EmailAddress")]
        [MaxLength(100)]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime EnrollmentDate { get; set; }

        [NotMapped]
        public int? customid { get; set; }

        // Navigation property representing the Branch the student is enrolled in
        public virtual Branch? Branch { get; set; }
    }
}
