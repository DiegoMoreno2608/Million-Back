using Microsoft.AspNetCore.Hosting;
using Million.Application.DTOs;
using Million.Domain.Entities;
using Million.Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
namespace Million.Application.Services
{
    /// <summary>
    /// Service for managing properties.
    /// Handles filtering, creation, image management, and property traceability.
    /// </summary>
    public class PropertyService
    {
        private readonly IPropertyRepository _repository;
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyService"/> class.
        /// </summary>
        /// <param name="repository">Repository for property data access.</param>
        /// <param name="env">Web hosting environment for file storage.</param>
        public PropertyService(IPropertyRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        /// <summary>
        /// Retrieves a filtered list of properties, including their images.
        /// </summary>
        /// <param name="name">Property name filter (optional).</param>
        /// <param name="address">Property address filter (optional).</param>
        /// <param name="minPrice">Minimum price filter (optional).</param>
        /// <param name="maxPrice">Maximum price filter (optional).</param>
        /// <returns>Returns a list of property DTOs matching the filters.</returns>
        public async Task<List<PropertyDto>> GetFilteredAsync(string? name, string? address, decimal? minPrice, decimal? maxPrice)
        {
            var filters = new List<FilterDefinition<Property>>();

            if (!string.IsNullOrWhiteSpace(name))
                filters.Add(Builders<Property>.Filter.Regex(p => p.Name, new BsonRegularExpression(name, "i")));

            if (!string.IsNullOrWhiteSpace(address))
                filters.Add(Builders<Property>.Filter.Regex(p => p.Address, new BsonRegularExpression(address, "i")));

            if (minPrice.HasValue)
                filters.Add(Builders<Property>.Filter.Gte(p => p.Price, minPrice.Value));

            if (maxPrice.HasValue)
                filters.Add(Builders<Property>.Filter.Lte(p => p.Price, maxPrice.Value));

            var filter = filters.Any()
                ? Builders<Property>.Filter.And(filters)
                : Builders<Property>.Filter.Empty;

            var properties = await _repository.GetPropertiesAsync(filter);
            var ids = properties.Select(p => p.IdProperty).ToList();

            var images = await _repository.GetImagesByPropertyIdsAsync(ids);

            var dtos = properties.Select(p =>
            {
                var image = images.FirstOrDefault(i => i.IdProperty == p.IdProperty);
                return new PropertyDto
                {
                    IdProperty = p.IdProperty,
                    Name = p.Name,
                    Address = p.Address,
                    Price = p.Price,
                    CodeInternal = p.CodeInternal,
                    Year = p.Year,
                    IdOwner = p.IdOwner,
                    FileUrl = image?.File
                };
            }).ToList();

            return dtos;
        }

        /// <summary>
        /// Adds a new property and saves its image if provided.
        /// </summary>
        /// <param name="dto">The property data transfer object.</param>
        /// <param name="baseUrl">The base URL for constructing the image path.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddAsync(PropertyDto dto, string baseUrl)
        {
            var property = new Property
            {
                IdProperty = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Address = dto.Address,
                Price = dto.Price,
                CodeInternal = dto.CodeInternal,
                Year = dto.Year,
                IdOwner = dto.IdOwner
            };

            var propertyImage = new PropertyImage
            {
                IdPropertyImage = Guid.NewGuid().ToString(),
                Enabled = true,
                IdProperty = property.IdProperty
            };
            if (dto.File != null && dto.File.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", "Properties");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(fileStream);
                }

                propertyImage.File = $"{baseUrl}/images/Properties/{fileName}";
            }

            await _repository.AddAsync(property, propertyImage);
        }

        /// <summary>
        /// Retrieves the names of all properties.
        /// </summary>
        /// <returns>Returns a list of property names.</returns>
        public async Task<List<string>> GetAllPropertyNamesAsync()
        {
            var properties = await _repository.GetAllNamesAsync();
            return properties.Select(p => p.Name).ToList();
        }

        /// <summary>
        /// Creates a new property trace (e.g., for sales or changes).
        /// </summary>
        /// <param name="dto">The property trace data transfer object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CreateAsync(PropertyTraceDto dto)
        {
            var propertyTrace = new PropertyTrace
            {
                IdPropertyTrace = Guid.NewGuid().ToString(),
                IdProperty = dto.IdProperty,
                DateSale = dto.DateSale,
                Name = dto.Name,
                Value = dto.Value,
                Tax = dto.Tax
            };

            await _repository.InsertAsync(propertyTrace);
        }

        /// <summary>
        /// Retrieves all property traces, optionally filtered by property ID.
        /// </summary>
        /// <param name="idProperty">The property ID to filter traces (optional).</param>
        /// <returns>Returns a list of property traces.</returns>
        public async Task<List<PropertyTrace>> GetAllAsync(string? idProperty)
        {
            return await _repository.GetAllAsync(idProperty);
        }
    }
}
