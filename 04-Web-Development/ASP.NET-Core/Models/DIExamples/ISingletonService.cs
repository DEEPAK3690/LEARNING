namespace MyWebApplication.Models.DIExamples
{
    public interface ISingletonService
    {
        Guid GetId();
        string GetLifetime();
    }
}
