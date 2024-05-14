using CatalogAPI.Service;
using EnvironmentAPI.Models;
using MongoDB.Driver;

namespace CatalogServiceAPI.Models
{
    public interface ICatalogInterface
    {
        Task<Catalog?> GetCatalog(Guid catalogID);
        Task<IEnumerable<Catalog>?> GetCatalogList();
        Task<Guid> AddCatalog(Catalog catalog);
        Task<long> UpdateCatalog(Catalog catalog);
        Task<long> DeleteCatalog(Guid catalogID);
    }

    public class CatalogMongoDBService : ICatalogInterface
    {
        private readonly ILogger<CatalogMongoDBService> _logger;
        private readonly IMongoCollection<Catalog> _catalogCollection;

        public CatalogMongoDBService(ILogger<CatalogMongoDBService> logger, MongoDBContext dbContext, IConfiguration configuration)
        {
            var collectionName = configuration["collectionName"];
            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ApplicationException("CatalogCollectionName is not configured.");
            }

            _logger = logger;
            _catalogCollection = dbContext.GetCollection<Catalog>(collectionName);
            _logger.LogInformation($"Collection name: {collectionName}");
        }

        public async Task<Catalog?> GetCatalog(Guid catalogID)
        {
            var filter = Builders<Catalog>.Filter.Eq(x => x.CatalogID, catalogID);
            return await _catalogCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Catalog>?> GetCatalogList()
        {
            return await _catalogCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Guid> AddCatalog(Catalog catalog)
        {
            catalog.CatalogID = Guid.NewGuid();
            await _catalogCollection.InsertOneAsync(catalog);
            return catalog.CatalogID;
        }

        public async Task<long> UpdateCatalog(Catalog catalog)
        {
            var filter = Builders<Catalog>.Filter.Eq(x => x.CatalogID, catalog.CatalogID);
            var result = await _catalogCollection.ReplaceOneAsync(filter, catalog);
            return result.ModifiedCount;
        }

        public async Task<long> DeleteCatalog(Guid catalogID)
        {
            var filter = Builders<Catalog>.Filter.Eq(x => x.CatalogID, catalogID);
            var result = await _catalogCollection.DeleteOneAsync(filter);
            return result.DeletedCount;
        }
    }
}
