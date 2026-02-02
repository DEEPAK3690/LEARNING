namespace MyWebApplication.Models.DIExamples
{
    public class ScopedService : IScopedService
    {
        private readonly Guid _id;

        public ScopedService()
        {
            _id = Guid.NewGuid();
            Console.WriteLine($"[SCOPED] Created with ID: {_id}");
        }

        public Guid GetId() => _id;

        public string GetLifetime() => "Scoped - One instance per HTTP request";
    }
}
