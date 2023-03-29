using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;
        public ProductRepository(IMapper mapper, ApplicationDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }
        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            if(product.ProductId != 0)
            {
                // Update
                 _db.Update(product);
            } else
            {
                // Create
                await _db.Products.AddAsync(product);
            }
            await _db.SaveChangesAsync();
            return  _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            var productFromDba = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if(productFromDba != null)
            {
                _db.Remove(productFromDba);
                _db.SaveChanges();
                return true;
            }
            return true;
        }

        public async Task<ProductDto> GetProductById(int productId)
        {
            var productFromDba = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            return _mapper.Map<ProductDto>(productFromDba);
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var products = await _db.Products.ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}
