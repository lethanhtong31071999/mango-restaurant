using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository.IRepository;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
                // Create
                await _productDb.AddAsync(product);
                return product;
            }
            // Update
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

        public async Task<PaginationResult<ProductDto>> GetProductsAsync(FilterProduct filter, bool tracked = true)
        {
            IQueryable<Product> query = tracked ? _productDb : _productDb.AsNoTracking();
            var result = new PaginationResult<ProductDto>()
            {
                Filter = filter,
                CurrentPage = filter.Page,
                TotalItems = query.Count()
            };

            // Name
            if (!String.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchName = filter.SearchTerm;
                query = _productDb.FromSqlRaw("Select * from Product WHERE name LIKE '%{searchName}%'");
            }

            // Category           

            // Sort
            if (!String.IsNullOrEmpty(filter.Sort))
            {
                filter.Sort.Split('+').ToList().ForEach(filter =>
                {
                    if (filter == SD.NAME_DESC)
                    {
                        query = query.OrderByDescending(x => x.Name);
                    }
                    else if (filter == SD.NAME_ASC)
                    {
                        query = query.OrderByDescending(x => x.Name).Reverse();
                    }
                    else if (filter == SD.PRICE_DESC)
                    {
                        query = query.OrderByDescending(x => x.Name);
                    }
                    else if (filter == SD.PRICE_ASC)
                    {
                        query = query.OrderByDescending(x => x.Name).Reverse();
                    }
                });
            }


            result.TotalFilteredItems = query.Count();
            result.TotalPages = (int)Math.Ceiling((double)result.TotalFilteredItems / filter.Length);
            query = query.Skip((filter.Page - 1) * filter.Length).Take(filter.Length);
            var productsFromDb = await query.ToListAsync<Product>();
            result.Data = _mapper.Map<List<ProductDto>>(productsFromDb);

            return result;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<List<Product>> CreateRangeProductsAsync(List<Product> products)
        {
            if (products.Count > 0)
            {
                await _productDb.AddRangeAsync(products);
                return products;
            }
            return null;
        }
    }
}
