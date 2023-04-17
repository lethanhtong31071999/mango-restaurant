using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private ResponseDto _reponseDto;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _reponseDto = new ResponseDto();
        }

        [HttpDelete]
        public async Task<object> DeleteProducts()
        {
            try
            {
                var products = await _productRepo.GetProductsAsync(tracked: false);
                _productRepo.DeleteRangeProducts(products.ToList<Product>());
                await _productRepo.SaveAsync();
                _reponseDto.IsSuccess = true;
                _reponseDto.StatusCode = HttpStatusCode.NoContent;
                return Ok(_reponseDto);
            }
            catch (Exception ex)
            {
                _reponseDto.IsSuccess = false;
                _reponseDto.ErrorMessages.Add(ex.Message);
                _reponseDto.StatusCode = HttpStatusCode.InternalServerError;
            }
            return _reponseDto;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<object> GetProducts()
        {
            try
            {
                var products = await _productRepo.GetProductsAsync(tracked: false);
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = _mapper.Map<List<ProductDto>>(products);
                _reponseDto.StatusCode = HttpStatusCode.OK;
                return Ok(_reponseDto);
            }
            catch (Exception ex)
            {
                _reponseDto.IsSuccess = false;
                _reponseDto.ErrorMessages.Add(ex.Message);
                _reponseDto.StatusCode = HttpStatusCode.InternalServerError;
            }
            return _reponseDto;
        }

        [HttpGet]
        [Route("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<object> GetProductById([FromRoute] int productId)
        {
            try
            {
                if (productId == 0)
                {
                    _reponseDto.IsSuccess = false;
                    _reponseDto.ErrorMessages.Add("Invalid product Id.");
                    _reponseDto.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_reponseDto);
                }
                var product = await _productRepo.GetProductByIdAsync(productId, tracked: false);
                if (product == null)
                {
                    _reponseDto.IsSuccess = false;
                    _reponseDto.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_reponseDto);
                }
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = _mapper.Map<ProductDto>(product);
                _reponseDto.StatusCode = HttpStatusCode.OK;
                return Ok(_reponseDto);
            }
            catch (Exception ex)
            {
                _reponseDto.IsSuccess = false;
                _reponseDto.ErrorMessages.Add(ex.Message);
                _reponseDto.StatusCode = HttpStatusCode.InternalServerError;
            }
            return _reponseDto;
        }

        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<object> CreateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                product = await _productRepo.CreateUpdateProductAsync(product);
                await _productRepo.SaveAsync();
                _reponseDto.Result = _mapper.Map<ProductDto>(product);
                _reponseDto.IsSuccess = true;
                return CreatedAtAction(nameof(GetProductById), new { productId = product.ProductId }, _reponseDto);
                return Ok(_reponseDto);
            }
            catch (Exception ex)
            {
                _reponseDto.IsSuccess = false;
                _reponseDto.ErrorMessages.Add(ex.Message);
                _reponseDto.StatusCode = HttpStatusCode.InternalServerError;
            }
            return _reponseDto;
        }

        [HttpPut]
        [Route("update")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<object> UpdateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                await _productRepo.CreateUpdateProductAsync(product);
                await _productRepo.SaveAsync();
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = _mapper.Map<ProductDto>(product);
                _reponseDto.StatusCode = HttpStatusCode.OK;
                return Ok(_reponseDto);
            }
            catch (Exception ex)
            {
                _reponseDto.IsSuccess = false;
                _reponseDto.ErrorMessages.Add(ex.Message);
                _reponseDto.StatusCode = HttpStatusCode.InternalServerError;
            }
            return _reponseDto;
        }

        [HttpDelete]
        [Route("{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<object> DeleteProduct([FromRoute] int productId)
        {
            try
            {
                if (productId == 0)
                {
                    _reponseDto.IsSuccess = false;
                    _reponseDto.ErrorMessages.Add("product id is not valid.");
                    _reponseDto.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_reponseDto);
                }
                var product = await _productRepo.GetProductByIdAsync(productId);
                if (product != null)
                {
                    _productRepo.DeleteProduct(product);
                    await _productRepo.SaveAsync();
                    _reponseDto.IsSuccess = true;
                    _reponseDto.StatusCode = HttpStatusCode.NoContent;
                    return Ok(_reponseDto);
                }
                else
                {
                    _reponseDto.IsSuccess = false;
                    _reponseDto.ErrorMessages.Add("product not found");
                    _reponseDto.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_reponseDto);
                }
            }
            catch (Exception ex)
            {
                _reponseDto.IsSuccess = false;
                _reponseDto.ErrorMessages.Add(ex.Message);
                _reponseDto.StatusCode = HttpStatusCode.InternalServerError;
            }
            return _reponseDto;
        }
    }
}
