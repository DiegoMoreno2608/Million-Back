using Million.Domain.Entities;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Million.Domain.Interfaces
{
    public interface IPropertyRepository
    {
        Task<List<Property>> GetPropertiesAsync(FilterDefinition<Property> filter);
        Task<List<PropertyImage>> GetImagesByPropertyIdsAsync(List<string> propertyIds);

        Task AddAsync(Property property, PropertyImage propertyImage);
        Task<List<Property>> GetAllNamesAsync();
        Task InsertAsync(PropertyTrace propertyTrace);
        Task<List<PropertyTrace>> GetAllAsync(string? idProperty);

    }
}
