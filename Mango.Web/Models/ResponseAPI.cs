using System.Net;

namespace Mango.Web.Models
{
    public class ResponseAPI
    {
        public bool IsSuccess { get; set; }
        public object Result { get; set; }
        public string DisplayMessage { get; set; } = "";
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public HttpStatusCode StatusCode { get; set; }
    }
}
