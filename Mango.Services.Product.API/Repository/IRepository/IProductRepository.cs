using Mango.Services.ProductAPI.Models.Dto;
using System.Diagnostics;

namespace Mango.Services.ProductAPI.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<ProductDto> GetProductById(int productId, bool tracked = true);
        Task<IEnumerable<ProductDto>> GetProducts(bool tracked = true);
        Task<ProductDto> CreateUpdateProduct(ProductDto productDto);
        Task<bool> DeleteProduct(ProductDto productDto);
        Task Save();
    }
}
