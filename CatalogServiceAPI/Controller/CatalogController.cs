using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CatalogServiceAPI.Models;
using EnvironmentAPI.Models;

namespace CatalogServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogInterface _catalogService;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(ICatalogInterface catalogService, ILogger<CatalogController> logger)
        {
            _catalogService = catalogService;
            _logger = logger;
        }

        [HttpGet("{_id}")]
        public async Task<ActionResult<Catalog>> GetCatalog(Guid _id)
        {
            var catalog = await _catalogService.GetCatalog(_id);
            if (catalog == null)
            {
                return NotFound();
            }
            return catalog;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Catalog>>> GetCatalogList()
        {
            var catalogList = await _catalogService.GetCatalogList();
            if (catalogList == null) { throw new ApplicationException("The list is null"); };
            return Ok(catalogList);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> AddCatalog(Catalog catalog)
        {
            var _id = await _catalogService.AddCatalog(catalog);
            return CreatedAtAction(nameof(GetCatalog), new { catalog_id = _id }, _id);
        }

        [HttpPut("{_id}")]
        public async Task<IActionResult> UpdateCatalog(Guid id, Catalog catalog)
        {
            if (id != catalog._id)
            {
                return BadRequest();
            }

            var result = await _catalogService.UpdateCatalog(catalog);
            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{_id}")]
        public async Task<IActionResult> DeleteCatalog(Guid _id)
        {
            var result = await _catalogService.DeleteCatalog(_id);
            if (result == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
