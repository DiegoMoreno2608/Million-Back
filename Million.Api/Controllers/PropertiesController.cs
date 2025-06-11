using Microsoft.AspNetCore.Mvc;
using Million.Application.DTOs;
using Million.Application.Services;

namespace Million.Api.Controllers
{
    /// <summary>
    /// Controller for managing properties.
    /// Provides endpoints to filter, create, and retrieve property data and traces.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly PropertyService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesController"/> class.
        /// </summary>
        /// <param name="service">Service for property operations.</param>
        public PropertiesController(PropertyService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a filtered list of properties.
        /// </summary>
        /// <param name="name">Property name filter (optional).</param>
        /// <param name="address">Property address filter (optional).</param>
        /// <param name="minPrice">Minimum price filter (optional).</param>
        /// <param name="maxPrice">Maximum price filter (optional).</param>
        /// <returns>Returns a list of properties matching the filters.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? name, [FromQuery] string? address,
                                             [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var result = await _service.GetFilteredAsync(name, address, minPrice, maxPrice);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new property.
        /// </summary>
        /// <param name="dto">The property data transfer object.</param>
        /// <returns>Returns the created property data.</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Post([FromForm] PropertyDto dto)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.AddAsync(dto, baseUrl);
            return Ok(dto);
        }

        /// <summary>
        /// Retrieves the distinct names of all properties.
        /// </summary>
        /// <returns>Returns a list of property names.</returns>
        [HttpGet("names")]
        public async Task<IActionResult> GetPropertyNames()
        {
            var names = await _service.GetAllPropertyNamesAsync();
            return Ok(names.Distinct());
        }

        /// <summary>
        /// Creates a new property trace (e.g., for sales or changes).
        /// </summary>
        /// <param name="dto">The property trace data transfer object.</param>
        /// <returns>Returns a confirmation message.</returns>
        [HttpPost("createTrace")]
        public async Task<IActionResult> Create([FromBody] PropertyTraceDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok(new { message = "Trazabilidad registrada correctamente." });
        }

        /// <summary>
        /// Retrieves all property traces, optionally filtered by property ID.
        /// </summary>
        /// <param name="idProperty">The property ID to filter traces (optional).</param>
        /// <returns>Returns a list of property traces.</returns>
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll([FromQuery] string? idProperty)
        {
            var traces = await _service.GetAllAsync(idProperty);
            return Ok(traces);
        }
    }
}
