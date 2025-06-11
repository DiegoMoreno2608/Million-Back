using Microsoft.AspNetCore.Mvc;
using Million.Application.DTOs;
using Million.Application.Services;
using Million.Domain.Interfaces;

namespace Million.Api.Controllers
{
    /// <summary>
    /// Controller for managing property owners.
    /// Provides endpoints to create owners, retrieve all owners, and get owner names.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OwnersController : ControllerBase
    {
        private readonly OwnerService _ownerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnersController"/> class.
        /// </summary>
        /// <param name="ownerService">Service for owner operations.</param>
        public OwnersController(OwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        /// <summary>
        /// Creates a new owner.
        /// </summary>
        /// <param name="request">The owner creation request data.</param>
        /// <returns>Returns the ID of the created owner.</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateOwnerRequestDTO request)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}"; 
            var id = await _ownerService.CreateOwnerAsync(request, baseUrl);
            return Ok(new { id });
        }

        /// <summary>
        /// Retrieves all owners.
        /// </summary>
        /// <returns>Returns a list of all owners.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var owners = await _ownerService.GetAllAsync();
            return Ok(owners);
        }

        /// <summary>
        /// Retrieves the names of all owners.
        /// </summary>
        /// <returns>Returns a list of owner names.</returns>
        [HttpGet("names")]
        public async Task<IActionResult> GetOwnerNames()
        {
            var names = await _ownerService.GetOwnerNamesAsync();
            return Ok(names);
        }
    }
}