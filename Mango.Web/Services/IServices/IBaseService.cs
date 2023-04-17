using Mango.Web.Models;

namespace Mango.Web.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        public ResponseAPI ResponseModel { get; set; }
        public Task<ResponseAPI> SendAsync(RequestAPI request);
    }
}
