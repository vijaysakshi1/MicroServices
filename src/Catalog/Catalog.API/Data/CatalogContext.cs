using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Catalog.API.Entities.Setings;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(ICatalogDatabseSettings catalogDatabseSettings)
        {
            var client = new MongoClient(catalogDatabseSettings.ConnectionString);
            var database = client.GetDatabase(catalogDatabseSettings.DatabaseName);

            Products = database.GetCollection<Product>(catalogDatabseSettings.CollectionName);
            CatalogContextSeed.SeedData(Products);
        }
        public IMongoCollection<Product> Products { get; }
    }
}
