using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using System.Diagnostics;

namespace Mango.Services.ProductAPI.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIdAsync(int productId, bool tracked = true);
        Task<IEnumerable<Product>> GetProductsAsync(bool tracked = true);
        Task<Product> CreateUpdateProductAsync(Product product);
        bool DeleteProduct(Product product);
        bool DeleteRangeProducts(List<Product> products);
        Task SaveAsync();
    }
}
