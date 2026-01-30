using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EF_CORE.Entities
{
    public class EFCoreDbContext : DbContext
    {
        // The OnConfiguring method allows us to configure the DbContext options,
        // such as specifying the database provider and connection string.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            // Display the generated SQL queries in the Console window
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);

            // Configuring the connection string to use a SQL Server database.
            // UseSqlServer is an extension method that configures the context to connect to a SQL Server database.
            //optionsBuilder.UseSqlServer(@"Server=LAPTOP-316VKNS5;Database=EFCoreDB1;Trusted_Connection=True;TrustServerCertificate=True;");
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-TRSDUK0\DEMOSERVER;Database=EFCoreDB1;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        // DbSet<Student> corresponds to the Students table in the database.
        // It allows EF Core to track and manage Student entities.
        public DbSet<Student> Students { get; set; }

        // DbSet<Branch> corresponds to the Branches table in the database.
        // It allows EF Core to track and manage Branch entities.
        public DbSet<Branch> Branches { get; set; }
    }
}
