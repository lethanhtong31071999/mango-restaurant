using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<object> GetProducts()
        {
            try
            {
                var productDTOs = await _productRepo.GetProducts(tracked: false);
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = productDTOs;
            }
            catch (Exception ex)
            {
                _reponseDto.IsSuccess = false;
                _reponseDto.ErrorMessages.Add(ex.Message);
            }
            return _reponseDto;
        }

        [HttpGet]
        [Route("{productId}")]
        public async Task<object> GetProductById([FromRoute] int productId)
        {
            try
            {
                var productDTOs = await _productRepo.GetProductById(productId, tracked: false);
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = productDTOs;
            }
            catch (Exception ex)
            {
                _reponseDto.IsSuccess = false;
                _reponseDto.ErrorMessages.Add(ex.Message);
            }
            return _reponseDto;
        }

        [HttpPost]
        [Route("create")]
        public async Task<object> CreateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                var productDTOs = await _productRepo.CreateUpdateProduct(productDto);
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = productDTOs;
            }
            catch (Exception ex)
            {
                _reponseDto.IsSuccess = false;
                _reponseDto.ErrorMessages.Add(ex.Message);
            }
            return _reponseDto;
        }

        [HttpPost]
        [Route("update")]
        public async Task<object> UpdateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                var productDTOs = await _productRepo.CreateUpdateProduct(productDto);
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = productDTOs;
            }
            catch (Exception ex)
            {
                _reponseDto.IsSuccess = false;
                _reponseDto.ErrorMessages.Add(ex.Message);
            }
            return _reponseDto;
        }

        [HttpDelete]
        [Route("{productId}")]
        public async Task<object> DeleteProduct([FromRoute] int productId)
        {
            try
            {
                if (productId == 0) throw new Exception("Invalid product id");
                var productFromDb = await _productRepo.GetProductById(productId);
                if (productFromDb != null)
                {
                    _reponseDto.IsSuccess = await _productRepo.DeleteProduct(_mapper.Map<ProductDto>(productFromDb));
                }
                else
                {
                    throw new Exception("Not found");
                }
            }
            catch (Exception ex)
            {
                _reponseDto.IsSuccess = false;
                _reponseDto.ErrorMessages.Add(ex.Message);
            }
            return _reponseDto;
        }
    }
}
