namespace MyWebApplication.Models.DIExamples
{
    public interface ITransientService
    {
        Guid GetId();
        string GetLifetime();
    }
}
