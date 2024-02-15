using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using OptimaAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OptimaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
  
        private readonly ILogger<ResourcesController> _logger;
        private readonly IResourcesRepository _resourcesRepository;

        public ResourcesController(ILogger<ResourcesController> logger, IResourcesRepository resources)
        {
            _logger = logger;
            _resourcesRepository = resources;
        }

        [HttpGet(Name = "GetResources")]
        public async Task<ActionResult<IEnumerable<RessourcesRequest>>> Get()
        {
            try
            {
                var result = await _resourcesRepository.GetResources();
                var createReq = new Models.RessourcesRequest(result);
                _logger.LogInformation(((Request)createReq).Json);

                if (result == null)
                {
                    return NotFound();
                }
                return Ok(createReq);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
