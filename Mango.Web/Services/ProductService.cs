using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IHttpClientFactory _client;
        public ProductService(IHttpClientFactory client) : base(client)
        {
            _client = client;
        }

        public async Task<ResponseAPI> CreateProductAsync(ProductDto productDto)
        {
            var request = new RequestAPI()
            {
                AccessToken = "",
                Data = productDto,
                Method = SD.ApiType.POST,
                Url = String.Format("{0}/create", SD.ProductAPIBase)
            };
            return await base.SendAsync(request);
        }

        public async Task<ResponseAPI> DeleteProductAsync(int productId)
        {
            var request = new RequestAPI()
            {
                AccessToken = "",
                Method = SD.ApiType.DELETE,
                Url = String.Format("{0}/{1}", SD.ProductAPIBase, productId)
            };
            return await base.SendAsync(request);
        }

        public async Task<ResponseAPI> GetAllProductsAsync(FilterProduct filter = null)
        {
            var url = SD.ProductAPIBase;
            var queryString = "";
            if(filter != null)
            {
                queryString = $"?page={filter.Page}&length={filter.Length}";
            }
            var request = new RequestAPI()
            {
                AccessToken = "",
                Method = SD.ApiType.GET,
                Url = url + queryString,
                Data = filter
            };
            return await base.SendAsync(request);
        }

        public async Task<ResponseAPI> GetProductByIdAsync(int productId)
        {
            var request = new RequestAPI()
            {
                AccessToken = "",
                Method = SD.ApiType.GET,
                Url = String.Format("{0}/{1}", SD.ProductAPIBase, productId)
            };
            return await base.SendAsync(request);
        }

        public async Task<ResponseAPI> UpdateProductAsync(ProductDto productDto)
        {
            var request = new RequestAPI()
            {
                AccessToken = "",
                Method = SD.ApiType.PUT,
                Data = productDto,
                Url = String.Format("{0}/Update", SD.ProductAPIBase)
            };
            return await base.SendAsync(request);
        }
        public async Task<ResponseAPI> CreateProductsByFileAsync(IFormFile file)
        {
            var request = new RequestAPI()
            {
                AccessToken = "",
                Method = SD.ApiType.POST,
                File = file,
                Url = String.Format("{0}/create-by-file", SD.ProductAPIBase)
            };
            return await base.SendAsync(request);
        }
    }
}
