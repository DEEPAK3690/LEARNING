namespace MyWebApplication.Models.DIExamples
{
    public class TransientService : ITransientService
    {
        private readonly Guid _id;

        public TransientService()
        {
            _id = Guid.NewGuid();
            Console.WriteLine($"[TRANSIENT] Created with ID: {_id}");
        }

        public Guid GetId() => _id;

        public string GetLifetime() => "Transient - New instance created every time";
    }
}
