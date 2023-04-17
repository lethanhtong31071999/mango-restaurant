using static Mango.Web.SD;

namespace Mango.Web.Models
{
    public class RequestAPI
    {
        public ApiType Method { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public string? AccessToken { get; set; } = null;
        public object Data { get; set; }
    }
}
