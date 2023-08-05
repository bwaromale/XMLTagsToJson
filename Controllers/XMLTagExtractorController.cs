using Microsoft.AspNetCore.Mvc;
using System.Net;
using XMLTAgsExtractor.Models;
using XMLTAgsExtractor.Services;

namespace XMLTAgsExtractor.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class XMLTagExtractorController : ControllerBase
    {
        ApiResponse _response = new ApiResponse();
        private readonly IExtraction _extractionService;

        public XMLTagExtractorController(IExtraction extractionService)
        {
            _extractionService = extractionService;
        }

        [HttpPost]
        public ActionResult<ApiResponse> ExtractTags(ExtractionRequest extractionRequest)
        {
            if (extractionRequest == null || string.IsNullOrEmpty(extractionRequest.FileUrl))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("File Url field cannot be empty");
                return _response;
            }
            bool isAvailable = _extractionService.VerifyFileUrl(extractionRequest.FileUrl);
            if (!isAvailable)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("File Url is invalid");
                return _response;
            }
            bool isXML = _extractionService.CheckFileExtension(extractionRequest.FileUrl);
            if (!isXML)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("File extension must be .xml");
                return _response;
            }
            List<string> extractedTags = _extractionService.ExtractTags(extractionRequest.FileUrl);

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = extractedTags;
            return _response;
        }

        [HttpPost]
        public ActionResult<ApiResponse> ExtractTagsWithData(ExtractionRequest extractionRequest)
        {
            bool isAvailable = _extractionService.VerifyFileUrl(extractionRequest.FileUrl);
            if (!isAvailable)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("File Url is invalid");
                return _response;
            }
            bool isXML = _extractionService.CheckFileExtension(extractionRequest.FileUrl);
            if (!isXML)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("File extension must be .xml");
                return _response;
            }

            Dictionary<string, object> extractedData = _extractionService.ExtractTagsandTagContent(extractionRequest.FileUrl);
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = extractedData;
            return _response;
        }   



    }
}