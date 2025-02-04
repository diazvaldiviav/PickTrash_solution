using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Dtos.TransportCategory;
using WebApplication1.Services.Interfaces;
using WebApplication1.Services.Implementations;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Driver")]
    public class TransportCategoryController : ControllerBase
    {
        private readonly ITransportCategory _transportCategoryService;
        private readonly ILogger<TransportCategoryController> _logger;

        public TransportCategoryController(
            ITransportCategory transportCategoryService,
            ILogger<TransportCategoryController> logger)
        {
            _transportCategoryService = transportCategoryService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransportCategoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _transportCategoryService.GetAllCategoriesAsync();

                if (!categories.Any())
                    return NotFound("No se encontraron categorías de transporte");

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las categorías de transporte");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
