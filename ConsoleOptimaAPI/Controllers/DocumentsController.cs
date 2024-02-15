using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using OptimaAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace OptimaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {


        private readonly ILogger<DocumentsController> _logger;
        private readonly IDocumentsRepository _DocumentsRepository;

        public DocumentsController(ILogger<DocumentsController> logger, IDocumentsRepository Documents)
        {
            _logger = logger;
            _DocumentsRepository = Documents;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocRequest>>> Get()
        {
            try
            {
                var result = await _DocumentsRepository.GetDocuments();
                var createReq = new Models.DocRequest(result, "");
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


        [SwaggerOperation(Summary = "Faktura zakupu", Description = "301000")]
        [HttpGet("FZ")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetFZ()
        {
            try
            {
                //Trzeba uzupełnić
                var result = await _DocumentsRepository.GetFZDocuments();
                var createReq = new Models.DocRequest(result, "FZ");
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
        [SwaggerOperation(Summary = "Faktura sprzedaży", Description = "302000")]
        [HttpGet("FA")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetFA()
        {
            try
            {
                var result = await _DocumentsRepository.GetFADocuments();
                var createReq = new Models.DocRequest(result, "FA");
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


        [SwaggerOperation(Summary = "Faktura sprzedaży z paragonu", Description = "302006")]
        [HttpGet("FPA")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetFPA()
        {
            try
            {
                var result = await _DocumentsRepository.GetFPADocuments();
                var createReq = new Models.DocRequest(result, "FPA");
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
        [SwaggerOperation(Summary = "Faktura sprzedaży z RO", Description = "302008")]
        [HttpGet("FROD")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetFROD()
        {
            try
            {
                var result = await _DocumentsRepository.GetFRODocuments();
                var createReq = new Models.DocRequest(result, "FROD");
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
        [SwaggerOperation(Summary = "Faktura sprzedaży z pro-formy", Description = "302009")]
        [HttpGet("FPF")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetFPF()
        {
            try
            {
                var result = await _DocumentsRepository.GetFPFDocuments();
                var createReq = new Models.DocRequest(result, "FPF");
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
        [SwaggerOperation(Summary = "Przychód wewnętrzny", Description = "303000")]
        [HttpGet("PW")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetPW()
        {
            try
            {
                var result = await _DocumentsRepository.GetPWDocuments();
                var createReq = new Models.DocRequest(result, "PW");
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
        [SwaggerOperation(Summary = "Rozchód wewnętrzny", Description = "304000")]
        [HttpGet("RW")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetRW()
        {
            try
            {
                var result = await _DocumentsRepository.GetRWDocuments();
                var createReq = new Models.DocRequest(result, "RW");
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
        [SwaggerOperation(Summary = "paragon", Description = "305000")]
        [HttpGet("PA")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetPA()
        {
            try
            {
                var result = await _DocumentsRepository.GetPADocuments();
                var createReq = new Models.DocRequest(result, "PA");
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
        [SwaggerOperation(Summary = "Paragon sprzedaży przekształcony z WZ", Description = "305004")]
        [HttpGet("PAWZ")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetPAWZ()
        {
            try
            {
                var result = await _DocumentsRepository.GetPAWZDocuments();
                var createReq = new Models.DocRequest(result, "PAWZ");
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
        [SwaggerOperation(Summary = "Paragon sprzedaży przekształcony z RO", Description = "305008")]
        [HttpGet("PARO")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetPARO()
        {
            try
            {
                var result = await _DocumentsRepository.GetPAROocuments();
                var createReq = new Models.DocRequest(result, "PARO");
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
        [SwaggerOperation(Summary = "Wydanie zewnętrzne", Description = "306000")]
        [HttpGet("WZ")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetWZ()
        {
            try
            {
                var result = await _DocumentsRepository.GetWZDocuments();
                var createReq = new Models.DocRequest(result, "WZ");
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
        [SwaggerOperation(Summary = "przyjęcie zewnętrzne", Description = "307000")]
        [HttpGet("PZ")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetPZ()
        {
            try
            {

                var result = await _DocumentsRepository.GetPZDocuments();
                var createReq = new Models.DocRequest(result, "PZ");
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
        [SwaggerOperation(Summary = "Przyjęcie zewnętrzne powstałe z zamówienia dostawcy", Description = "307010")]
        [HttpGet("PZZD")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetPZZD()
        {
            try
            {
                var result = await _DocumentsRepository.GetPZZDDocuments();
                var createReq = new Models.DocRequest(result, "PZZD");
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
        [SwaggerOperation(Summary = "Rezerwacja odbiorcy", Description = "308000")]
        [HttpGet("RO")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetRO()
        {
            try
            {
                var result = await _DocumentsRepository.GetRODocuments();
                var createReq = new Models.DocRequest(result, "RO");
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
        [SwaggerOperation(Summary = "Rezerwacja odbiorcy przeksztalcona z PF", Description = "308011")]
        [HttpGet("ROPF")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetROPF()
        {
            try
            {
                var result = await _DocumentsRepository.GetROPFDocuments();
                var createReq = new Models.DocRequest(result, "ROPF");
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
        [SwaggerOperation(Summary = "Zamówienie dostawcy", Description = "309000")]
        [HttpGet("ZD")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetZD()
        {
            try
            {
                var result = await _DocumentsRepository.GetZDDocuments();
                var createReq = new Models.DocRequest(result, "ZD");
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
        [SwaggerOperation(Summary = "Rezerwacja odbiorcy przeksztalcona z PF", Description = "309308")]
        [HttpGet("ZROD")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetZROD()
        {
            try
            {
                var result = await _DocumentsRepository.GetZDRODocuments();
                var createReq = new Models.DocRequest(result, "ZROD");
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
        [SwaggerOperation(Summary = " Przesuniecie MM", Description = "312000")]
        [HttpGet("MM")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetMM()
        {
            try
            {
                var result = await _DocumentsRepository.GetMMDocuments();
                var createReq = new Models.DocRequest(result, "MM");
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
