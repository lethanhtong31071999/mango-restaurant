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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<object> GetProducts()
        {
            try
            {
                var productDTOs = await _productRepo.GetProducts(tracked: false);
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = productDTOs;
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
                var productDTO = await _productRepo.GetProductById(productId, tracked: false);
                if (productDTO == null)
                {
                    _reponseDto.IsSuccess = false;
                    _reponseDto.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_reponseDto);
                }
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = productDTO;
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
                var productDTO = await _productRepo.CreateUpdateProduct(productDto);
                await _productRepo.Save();
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = productDTO;
                return CreatedAtRoute(nameof(GetProductById), new {productId = productDTO.ProductId}, _reponseDto);
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
                var productDTOs = await _productRepo.CreateUpdateProduct(productDto);
                await _productRepo.Save();
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = productDTOs;
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
                var productFromDb = await _productRepo.GetProductById(productId);
                if (productFromDb != null)
                {
                    await _productRepo.DeleteProduct(_mapper.Map<ProductDto>(productFromDb));
                    await _productRepo.Save();
                    _reponseDto.IsSuccess = true;
                    _reponseDto.StatusCode=HttpStatusCode.OK;
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
