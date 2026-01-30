using System;
namespace SRP
{
    class Program
    {
        static void Main(string[] args)
        {
            //The Single Responsibility Principle(SRP) is one of the five SOLID principles of object-oriented design and development, which aims to make software more understandable, flexible, and maintainable. 

            //The SRP states that a class should have only one reason to change, meaning it should have only one job or responsibility.

            ReportService reportService = new ReportService();
            reportService.ProcessReport("Report content", "gK4oF@example.com");

        }
    }
}
