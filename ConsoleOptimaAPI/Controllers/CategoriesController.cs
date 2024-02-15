using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using OptimaAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OptimaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        
        readonly ILogger<CategoriesController> _logger;
        private readonly ICategoriesRepository _categoriesRepository;

        public CategoriesController(ILogger<CategoriesController> logger, ICategoriesRepository categories)
        {
            _logger = logger;
            _categoriesRepository = categories;
        }

        [HttpGet(Name = "GetCategories")]
        public async Task<ActionResult<IEnumerable<CatRequest>>> Get()
        {
            try
            {
                var result = await _categoriesRepository.GetCategories();
                var createReq = new Models.CatRequest(result);
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
