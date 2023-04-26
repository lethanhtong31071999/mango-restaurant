using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ExcelDataReader;
using OfficeOpenXml;
using System.Drawing;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly IWebHostEnvironment _env;
        private ResponseDto _reponseDto;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepo, IMapper mapper, IWebHostEnvironment env)
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _reponseDto = new ResponseDto();
            _env = env;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductDto>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<object> GetProducts([FromQuery]FilterProduct filter)
        {
            try
            {
                if (filter == null)
                {
                    filter = new FilterProduct();
                }
                var paginatedProducts = await _productRepo.GetProductsAsync(filter, tracked: false);
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = paginatedProducts;
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

        [HttpGet]
        [Route("get-template-file")]
        public object GetTemplateProductExcelFile()
        {
            try
            {
                using var package = new ExcelPackage();
                {
                    // Excel
                    var worksheet = package.Workbook.Worksheets.Add(SD.SheetNameProduct);
                    worksheet.DefaultColWidth = 10;
                    worksheet.DefaultRowHeight = 20;
                    worksheet.Cells["A1"].Value = "Name";
                    worksheet.Cells["B1"].Value = "Description";
                    worksheet.Cells["C1"].Value = "Price";
                    worksheet.Cells["D1"].Value = "Category Name";
                    worksheet.Cells["E1"].Value = "Image Url";

                    // Style
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["B1"].Style.Font.Bold = true;
                    worksheet.Cells["C1"].Style.Font.Bold = true;
                    worksheet.Cells["D1"].Style.Font.Bold = true;
                    worksheet.Cells["E1"].Style.Font.Bold = true;

                    worksheet.Cells["A2"].Style.Numberformat.Format = "@";
                    worksheet.Cells["B2"].Style.Numberformat.Format = "@";
                    worksheet.Cells["C2"].Style.Numberformat.Format = "@";
                    worksheet.Cells["D2"].Style.Numberformat.Format = "@";
                    worksheet.Cells["E2"].Style.Numberformat.Format = "@";


                    // Save the Excel package to a file
                    byte[] fileContents = package.GetAsByteArray();
                    return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "product_template.xlsx");
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
        [Route("create-by-file")]
        public async Task<object> CreateProductByFile([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new Exception("No file uploaded");
                }
                var productDTOs = new List<ProductDto>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var excelPackage = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[SD.SheetNameProduct];
                        if(worksheet == null )
                        {
                            _reponseDto.IsSuccess = false;
                            _reponseDto.ErrorMessages.Add("Cannot find the same name sheet.");
                            _reponseDto.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(_reponseDto);
                        }
                        else if(worksheet.Dimension.End.Row > 2)
                        {
                            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                            {
                                productDTOs.Add(new ProductDto()
                                {
                                    Name = worksheet.Cells[row, 1].Value.ToString(),
                                    Description = worksheet.Cells[row, 2].Value.ToString(),
                                    Price = double.Parse(worksheet.Cells[row, 3].Value.ToString()),
                                    CategoryName = worksheet.Cells[row, 4].ToString(),
                                    ImageUrl = worksheet.Cells[row, 5].ToString()
                                });
                            }
                        }
                    }
                }
                var productsFromDba = await _productRepo.CreateRangeProductsAsync(_mapper.Map<List<Product>>(productDTOs));
                await _productRepo.SaveAsync();
                _reponseDto.IsSuccess = true;
                _reponseDto.Result = _mapper.Map<List<ProductDto>>(productsFromDba);
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

        [HttpPut]
        [Route("Update")]
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
