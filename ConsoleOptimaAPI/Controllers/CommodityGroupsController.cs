using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using OptimaAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OptimaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommodityGroupsController : ControllerBase
    {
        private readonly ILogger<CommodityGroupsController> _logger;
        private readonly ICommodityGoupsRepository _commodityGoupsRepository;

        public CommodityGroupsController(ILogger<CommodityGroupsController> logger, ICommodityGoupsRepository commodityGoups)
        {
            _logger = logger;
            _commodityGoupsRepository = commodityGoups;
        }

        [HttpGet(Name = "GetCommoditygGoups")]
        public async Task<ActionResult<CommRequest>> Get()
        {
            try
            {
                var result = await _commodityGoupsRepository.GetCommodityGroups();
                var createReq = new Models.CommRequest(result);
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
