using System.Net;

namespace XMLTAgsExtractor.Models
{
    public class ApiResponse
    {
        public ApiResponse()
        {
            ErrorMessages = new string[] { };
        }
        public bool IsSuccess { get; set; } = false;
        public HttpStatusCode StatusCode { get; set; }
        public string[] ErrorMessages { get; set; }
        public object Result { get; set; }

    }
}
