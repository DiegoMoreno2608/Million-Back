
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Million.Domain.Entities;
using Million.Domain.Interfaces;
using Million.Infrastructure.Settings;
using MongoDB.Driver;

namespace Million.Infrastructure.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IMongoCollection<Owner> _owners;

        public OwnerRepository(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _owners = database.GetCollection<Owner>("Owner");
        }

        public async Task AddAsync(Owner owner)
        {
            await _owners.InsertOneAsync(owner);
        }

        public async Task<List<Owner>> GetAllAsync()
        {
            return await _owners.Find(_ => true).ToListAsync();
        }
    }
}
