using Microsoft.AspNetCore.Hosting;
using Million.Application.DTOs;
using Million.Domain.Entities;
using Million.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Million.Application.Services
{
    /// <summary>
    /// Service for managing property owners.
    /// Handles creation, retrieval, and name listing of owners.
    /// </summary>
    public class OwnerService
    {
        private readonly IOwnerRepository _repository;
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerService"/> class.
        /// </summary>
        /// <param name="repository">Repository for owner data access.</param>
        /// <param name="env">Web hosting environment for file storage.</param>
        public OwnerService(IOwnerRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        /// <summary>
        /// Creates a new owner and saves the photo if provided.
        /// </summary>
        /// <param name="request">The owner creation request data.</param>
        /// <param name="baseUrl">The base URL for constructing the photo path.</param>
        /// <returns>Returns the generated owner ID.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the request is null.</exception>
        public async Task<string> CreateOwnerAsync(CreateOwnerRequestDTO request, string baseUrl)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var owner = new Owner
            {
                IdOwner = Guid.NewGuid().ToString(),
                Name = request.Name,
                Address = request.Address,
                Birthday = request.Birthday
            };

            if (request.Photo != null && request.Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", "Owners");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Photo.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Photo.CopyToAsync(fileStream);
                }

                owner.Photo = $"{baseUrl}/images/Owners/{fileName}";
            }

            await _repository.AddAsync(owner);
            return owner.IdOwner;
        }

        /// <summary>
        /// Retrieves all owners.
        /// </summary>
        /// <returns>Returns a list of all owners.</returns>
        public async Task<List<Owner>> GetAllAsync() => await _repository.GetAllAsync();

        /// <summary>
        /// Retrieves the names and IDs of all owners.
        /// </summary>
        /// <returns>Returns a list of owner name DTOs.</returns>
        public async Task<List<OwnerNameDto>> GetOwnerNamesAsync()
        {
            var owners = await _repository.GetAllAsync();
            return owners.Select(o => new OwnerNameDto
            {
                IdOwner = o.IdOwner,
                Name = o.Name
            }).ToList();
        }
    }

}
