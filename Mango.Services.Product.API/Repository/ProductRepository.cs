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
        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _productDb.AddAsync(product);
            return productDto;
        }

        public async Task<bool> DeleteProduct(ProductDto product)
        {
            try
            {
                _productDb.Remove(_mapper.Map<Product>(product));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ProductDto> GetProductById(int productId, bool tracked = true)
        {
            IQueryable<Product> query = tracked ? _productDb : _productDb.AsNoTracking();
            var productFromDb = await query.FirstOrDefaultAsync(x => x.ProductId == productId);
            return _mapper.Map<ProductDto>(productFromDb);
        }

        public async Task<IEnumerable<ProductDto>> GetProducts(bool tracked = true)
        {
            IQueryable<Product> query = tracked ? _productDb : _productDb.AsNoTracking();
            var productsFromDb = await query.ToListAsync<Product>();
            return _mapper.Map<IEnumerable<ProductDto>>(productsFromDb);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
