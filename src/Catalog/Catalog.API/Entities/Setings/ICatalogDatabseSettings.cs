namespace Catalog.API.Entities.Setings
{
    public interface ICatalogDatabseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string CollectionName { get; set; }
    }
}
