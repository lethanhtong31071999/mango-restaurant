using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository.IRepository;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMapper _mapper;
        private readonly DbSet<Product> _productDb;
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
            _productDb = db.Products;
        }
        public async Task<Product> CreateUpdateProductAsync(Product product)
        {
            if (product.ProductId == 0)
            {
                await _productDb.AddAsync(product);
                return product;
            }
            _productDb.Update(product);
            return product;
        }

        public bool DeleteProduct(Product product)
        {
            try
            {
                _productDb.Remove(product);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteRangeProducts(List<Product> products)
        {
            try
            {
                _productDb.RemoveRange(products);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Product> GetProductByIdAsync(int productId, bool tracked = true)
        {
            IQueryable<Product> query = tracked ? _productDb : _productDb.AsNoTracking();
            var productFromDb = await query.FirstOrDefaultAsync(x => x.ProductId == productId);
            return productFromDb;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(bool tracked = true)
        {
            IQueryable<Product> query = tracked ? _productDb : _productDb.AsNoTracking();
            var productsFromDb = await query.ToListAsync<Product>();
            return productsFromDb;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
