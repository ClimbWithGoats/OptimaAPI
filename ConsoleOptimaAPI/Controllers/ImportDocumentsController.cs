using OptimaAPI.Interfaces;
using OptimaAPI.Models;
using OptimaAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OptimaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportDocumentsController : ControllerBase
    {
        private readonly ILogger<ImportDocumentsController> _logger;
        private readonly IImportDocumentsRepository _ImportDocumentsRepository;

        public ImportDocumentsController(ILogger<ImportDocumentsController> logger, IImportDocumentsRepository ImportDocuments)
        {
            _logger = logger;
            _ImportDocumentsRepository = ImportDocuments;
        }

        [HttpGet("RunTest", Name = "TestingRunProcces")]
        public async Task<ActionResult<IEnumerable<DocRequest>>> GetTestingRunProccesResult()
        {
            try
            {
                var result = await _ImportDocumentsRepository.TestingRunProcces();

                var createReq = new Models.ImpDocRequest(result);
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
        [HttpGet("PostPZ", Name = "PostPZRequest")]
        public async Task<ActionResult<IEnumerable<Request>>> HandlePostPZRequestResult()
        {
            try
            {
                var result = await _ImportDocumentsRepository.PostPZ();
                var createReq = new Models.ImpDocRequest(result);
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
        [HttpGet("PostFZ", Name = "PostFZRequest")]
        public async Task<ActionResult<IEnumerable<Request>>> HandlePostFZRequestResult()
        {
            try
            {
                var result = await _ImportDocumentsRepository.PostFZ();
                var createReq = new Models.ImpDocRequest(result);
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
        [HttpGet("PostWZ", Name = "PostWZRequest")]
        public async Task<ActionResult<IEnumerable<Request>>> HandlePostWZRequestResult()
        {
            try
            {
                var result = await _ImportDocumentsRepository.PostWZ();
                var createReq = new Models.ImpDocRequest(result);
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
        [HttpGet("PostFA", Name = "PostFARequest")]
        public async Task<ActionResult<IEnumerable<Request>>> HandlePostFARequestResult()
        {
            try
            {
                var result = await _ImportDocumentsRepository.PostFA();
                var createReq = new Models.ImpDocRequest(result);
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
        [HttpGet("PutDisrepancies", Name = "PutWmsDiscrepancies")]
        public async Task<ActionResult<IEnumerable<Request>>> HandlePutWmsDiscrepanciesResult()
        {
            try
            {
                var result = await _ImportDocumentsRepository.PutWMSDiscrepancies();
                var createReq = new Models.ImpDocRequest(result);
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
        [HttpGet("PutStatus", Name = "PutWmsStatusRequest")]
        public async Task<ActionResult<IEnumerable<Request>>> HandlePutWmsStatusRequestResult()
        {
            try
            {
                var result = await _ImportDocumentsRepository.PutWMSStatus();
                var createReq = new Models.ImpDocRequest(result);
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
        [HttpGet("PutSignature", Name = "PutWmsStatusSignature")]
        public async Task<ActionResult<IEnumerable<Request>>> HandlePutWmsStatusSignatureResult()
        {
            try
            {
                var result = await _ImportDocumentsRepository.PutWMSSignature();
                var createReq = new Models.ImpDocRequest(result);
                _logger.LogInformation(((Request)createReq) .Json);

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
