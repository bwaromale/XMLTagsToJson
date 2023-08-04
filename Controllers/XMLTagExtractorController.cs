using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using XMLTAgsExtractor.Models;

namespace XMLTAgsExtractor.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class XMLTagExtractorController : ControllerBase
    {
        ApiResponse _response = new ApiResponse();
        [HttpPost]
        public ApiResponse ExtractTags(ExtractionRequest extractionRequest)
        {
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = extractionRequest.FileUrl;
            return _response;
        }
    }
}
