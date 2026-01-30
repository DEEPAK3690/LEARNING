using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIP
{
    // Abstraction (Interface) - High-level modules will depend on this
    public interface IDataRepository
    {
        string GetData();
    }
    // Low-level module: Database Repository
    public class DatabaseRepository : IDataRepository
    {
        public string GetData()
        {
            return "Data from database";
        }
    }

    // Low-level module: File Repository (Alternative implementation)
    public class FileRepository : IDataRepository
    {
        public string GetData()
        {
            return "Data from file";
        }
    }

    // High-level module depends on the abstraction (IDataRepository) instead of a concrete class
    public class DataProcessor
    {
        private readonly IDataRepository _repository;

        // Dependency Injection (Passing dependency via constructor)
        public DataProcessor(IDataRepository repository)
        {
            _repository = repository;
        }

        public void ProcessData()
        {
            string data = _repository.GetData();
            Console.WriteLine("Processing: " + data);
        }
    }


}
