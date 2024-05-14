using EnvironmentAPI.Service;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace EnvironmentAPI.Models
{
    public interface IUserInterface
    {
        Task<Catalog?> GetCatalog(Guid catalog_id);
        Task<IEnumerable<Catalog>?> GetCatalogList();
        Task<Guid> AddCatalog(Catalog catalog);
        Task<long> UpdateCatalog(Catalog catalog);
        Task<long> DeleteCatalog(Guid catalog_id);
    }
    public class CatalogMongoDBService : ICatalogInterface
    {
        private readonly ILogger<CatalogMongoDBService> _logger;
        private readonly IMongoCollection<Catalog> _CatalogCollection;

        public CatalogMongoDBService(ILogger<CatalogMongoDBService> logger, MongoDBContext dbContext, IConfiguration configuration)
        {
            var collectionName = configuration["collectionName"];
            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ApplicationException("CatalogCollectionName is not configured.");
            }

            _logger = logger;
            _CatalogCollection = dbContext.GetCollection<Catalog>(collectionName);  
            _logger.LogInformation($"Collection name: {collectionName}");
        }

        public async Task<Catalog?> GetCatalog(Guid catalog_id)
        {
            var filter = Builders<Catalog>.Filter.Eq(x => x._id, catalog_id);
            return await _catalogCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Catalog>?> GetUserList()
        {
            return await _catalogCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Guid> AddCatalog(Catalog catalog)
        {
            catalog._id = Guid.NewGuid();
            await _catalogCollection.InsertOneAsync(catalog);
            return catalog._id;
        }

        public async Task<long> UpdateCatalog(Catalog catalog)
        {
            var filter = Builders<Catalog>.Filter.Eq(x => x._id, catalog._id);
            var result = await _catalogCollection.ReplaceOneAsync(filter, catalog);
            return result.ModifiedCount;
        }

        public async Task<long> Deletecatalog(Guid catalog_id)
        {
            var filter = Builders<Catalog>.Filter.Eq(x => x._id, catalog_id);
            var result = await _catalogCollection.DeleteOneAsync(filter);
            return result.DeletedCount;
        }
    }
}
