namespace MyWebApplication.Models.DIExamples
{
    public class SingletonService : ISingletonService
    {
        private readonly Guid _id;

        public SingletonService()
        {
            _id = Guid.NewGuid();
            Console.WriteLine($"[SINGLETON] Created with ID: {_id}");
        }

        public Guid GetId() => _id;

        public string GetLifetime() => "Singleton - Single instance for entire application lifetime";
    }
}
