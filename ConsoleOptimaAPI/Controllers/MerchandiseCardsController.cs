using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using OptimaAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//KartyTowarowe
namespace OptimaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchandiseCardsController : ControllerBase
    {
 
        private readonly ILogger<MerchandiseCardsController> _logger;
        private readonly IMerchandiseCardsRepository _merchandiseCardsRepository;


        public MerchandiseCardsController(ILogger<MerchandiseCardsController> logger, IMerchandiseCardsRepository merchandiseCardsRepository)
        {
            _logger = logger;
            _merchandiseCardsRepository = merchandiseCardsRepository;
        }

        [HttpGet(Name = "GetMerchandiseCards")]
        public async Task<ActionResult<IEnumerable<MerchCardsRequest>>> Get()
        {
            try
            {
                var result = await _merchandiseCardsRepository.GetMerchandiseCards();
                var createReq = new Models.MerchCardsRequest(result);
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
