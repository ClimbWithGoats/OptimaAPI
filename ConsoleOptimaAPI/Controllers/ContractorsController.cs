using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Diagnostics;
//Kontrahenci
namespace OptimaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractorsController : ControllerBase
    {
      
        private readonly ILogger<ContractorsController> _logger;
        private readonly IContractorsRepository _contractorsRepository;

        public ContractorsController(ILogger<ContractorsController> logger, IContractorsRepository contractors)
        {
            _logger = logger;
            _contractorsRepository = contractors;
        }

        [HttpGet("GetContractors", Name = "GetContractors")]
        public async Task<ActionResult<ContractorRequest>> Get()
        {
            try
            {
                var result = await _contractorsRepository.GetContractors();
                var createReq = new Models.ContractorRequest(result);
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

        [HttpGet("GetSqlContractors",Name = "GetSQLContractors")]
        public async Task<ActionResult<ContractorSQLRequest>> GetSQLContractors()
        {
            try
            {
                var result = await _contractorsRepository.GetContractors();
                var createReq = new Models.ContractorSQLRequest(result);
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

        //[HttpGet(Name = "GetSqlContractors")]
        //public async Task<ActionResult<ContractorSQLRequest>> GetSqlo()
        //{
        //    try
        //    {
        //        var result = await _contractorsRepository.GetSQLContractors();
        //        var createReq = new Models.ContractorSQLRequest(result);
        //        _logger.LogInformation(((Request)createReq).Json);

        //        if (result == null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(createReq);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}


        [HttpGet("GetMini",Name = "GetMiniContractors")]
        public async Task<ActionResult<IEnumerable<ContractorRequest>>> GetMini()
        {
            try
            {
                var result = await _contractorsRepository.GetMiniInfoAboutContractors();

                var createReq = new Models.ContractorRequest(result);
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
