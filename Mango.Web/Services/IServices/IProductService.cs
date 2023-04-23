using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Services.IServices
{
    public interface IProductService
    {
        Task<ResponseAPI> GetAllProductsAsync(FilterProduct filter = null);
        Task<ResponseAPI> GetProductByIdAsync(int productId);
        Task<ResponseAPI> CreateProductAsync(ProductDto productDto);
        Task<ResponseAPI> UpdateProductAsync(ProductDto productDto);
        Task<ResponseAPI> DeleteProductAsync(int productId);
    }
}
