namespace MyWebApplication.Models.DIExamples
{
    public interface IScopedService
    {
        Guid GetId();
        string GetLifetime();
    }
}
