using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        protected ResponseDto _response;
        private IProductRepository _productRepo;
        public ProductController(IProductRepository productRepository)
        {
            _productRepo = productRepository;
            this._response = new ResponseDto();
        }

        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            try
            {
               var productDtos = await _productRepo.GetProducts();
                _response.Result = productDtos;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
            }
            return _response;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ResponseDto> Get(int id)
        {
            try
            {
                var productDtos = await _productRepo.GetProductById(id);
                _response.Result = productDtos;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
            }
            return _response;
        }
    }
}
