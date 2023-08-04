using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public ActionResult<ApiResponse> ExtractTags(ExtractionRequest extractionRequest)
        {
            if(extractionRequest == null || string.IsNullOrEmpty(extractionRequest.FileUrl))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("File Url field cannot be empty");
                return _response;
            }
            bool isAvailable = VerifyFileUrl(extractionRequest.FileUrl);
            if (!isAvailable)
            {
                _response.StatusCode= HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("File Url is invalid");
                return _response;
            }
            bool isXML = CheckFileExtension(extractionRequest.FileUrl);
            if (!isXML)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("File extension must be .xml");
                return _response;
            }
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = extractionRequest.FileUrl;
            return _response;
        }
        private bool VerifyFileUrl(string fileUrl)
        {
            bool isAvailable = false;
            try
            {
                Uri fileUri = new Uri(fileUrl);
                if (fileUri.Scheme != Uri.UriSchemeHttp || fileUri.Scheme != Uri.UriSchemeHttps)
                {
                    if (fileUri.Scheme != "file")
                    {
                        return isAvailable;
                    }
                }
                isAvailable = true;
            }
            catch (UriFormatException) { }
            return isAvailable;
        }
        private bool CheckFileExtension(string fileUrl)
        {
            bool isXML = false;

            string ext = fileUrl.Substring(fileUrl.Length - 3);

            if(ext == "xml")
            {
                isXML = true;
            }
            return isXML;
        }
    }
}
