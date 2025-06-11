using Microsoft.Extensions.Options;
using Million.Domain.Entities;
using Million.Domain.Interfaces;
using Million.Infrastructure.Settings;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Million.Infrastructure.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly IMongoCollection<Property> _collectionProperty;
        private readonly IMongoCollection<PropertyImage> _collectionPropertyImage;
        private readonly IMongoCollection<PropertyTrace> _collectionPropertyTrace;

        public PropertyRepository(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collectionProperty = database.GetCollection<Property>("Properties");
            _collectionPropertyImage = database.GetCollection<PropertyImage>("PropertyImage");
            _collectionPropertyTrace = database.GetCollection<PropertyTrace>("PropertyTrace");
        }

        public async Task<List<Property>> GetPropertiesAsync(FilterDefinition<Property> filter)
        {
            return await _collectionProperty.Find(filter).ToListAsync();
        }
        public async Task<List<PropertyImage>> GetImagesByPropertyIdsAsync(List<string> propertyIds)
        {
            return await _collectionPropertyImage
                .Find(img => propertyIds.Contains(img.IdProperty))
                .ToListAsync();
        }
        public async Task AddAsync(Property property, PropertyImage propertyImage)
        {
            await _collectionProperty.InsertOneAsync(property);
            await _collectionPropertyImage.InsertOneAsync(propertyImage);

        }
        public async Task<List<Property>> GetAllNamesAsync()
        {
            return await _collectionProperty.Find(_ => true).ToListAsync();
        }

        public async Task InsertAsync(PropertyTrace propertyTrace)
        {
            await _collectionPropertyTrace.InsertOneAsync(propertyTrace);
        }
        public async Task<List<PropertyTrace>> GetAllAsync(string? idProperty)
        {
            if (!string.IsNullOrEmpty(idProperty))
            {
                var filter = Builders<PropertyTrace>.Filter.Eq(pt => pt.IdProperty, idProperty);
                return await _collectionPropertyTrace.Find(filter).ToListAsync();
            }

            return await _collectionPropertyTrace.Find(_ => true).ToListAsync();
        }
    }
}
