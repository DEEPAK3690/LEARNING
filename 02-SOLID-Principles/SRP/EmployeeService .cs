using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRP
{
    // 1. Class responsible for report generation
    public class ReportGenerator
    {
        public string GenerateReport(string content)
        {
            return "Report: " + content;
        }
    }

    // 2. Class responsible for file handling
    public class ReportSaver
    {
        public void SaveToFile(string content)
        {
            File.WriteAllText("report.txt", content);
        }
    }

    // 3. Class responsible for email handling
    public class EmailSender
    {
        public void SendEmail(string email, string content)
        {
            Console.WriteLine($"Email sent to {email} with content: {content}");
        }
    }

    // 4. Coordinator class (optional)
    public class ReportService
    {
        private readonly ReportGenerator _generator;
        private readonly ReportSaver _saver;
        private readonly EmailSender _emailSender;

        public ReportService()
        {
            _generator = new ReportGenerator();
            _saver = new ReportSaver();
            _emailSender = new EmailSender();
        }

        public void ProcessReport(string content, string email)
        {
            string report = _generator.GenerateReport(content);
            _saver.SaveToFile(report);
            _emailSender.SendEmail(email, report);
        }
    }
}
