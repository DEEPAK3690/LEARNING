using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRP
{
    class EmployeeService
    {
        public void AddEmployee(string name, string department)
        {
            Console.WriteLine($"Employee {name} added to {department}.");
        }

        public double CalculateSalary(int hoursWorked, double hourlyRate)
        {
            return hoursWorked * hourlyRate;
        }

        public void GenerateEmployeeReport()
        {
            Console.WriteLine("Generating Employee Report...");
        }
    }
}
