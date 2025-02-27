namespace HoneyChef.Api.Persistence
{
    public interface IDatabaseSettings
    {
        string CollectionName { get; set; }
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
    }
}
