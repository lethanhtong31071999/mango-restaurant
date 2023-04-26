using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductAdminController : Controller
    {
        private readonly IProductService _productService;
        public ProductAdminController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetProducts()
        {
            var filter = new FilterProduct()
            {
                Length = Convert.ToInt32(Request.Form["length"].FirstOrDefault()),
                Page = Convert.ToInt32(Request.Form["start"].FirstOrDefault()) / Convert.ToInt32(Request.Form["length"].FirstOrDefault()) + 1,
                SearchTerm = Request.Form["search[value]"].FirstOrDefault()
            };
            var res = await _productService.GetAllProductsAsync(filter);
            if (!res.IsSuccess)
            {
                return null;
            }
            PaginationResult<ProductDto> pagination = JsonConvert.DeserializeObject<PaginationResult<ProductDto>>(res.Result.ToString());

            return Json(new
            {
                data = pagination.Data,
                recordsFiltered = pagination.TotalFilteredItems,
                recordsTotal = pagination.TotalItems,
                draw = 1,
            });
        }

        [HttpGet]
        public async Task<IActionResult> UpsertProduct([FromRoute] int id)
        {
            if (id == 0)
            {
                // Create
                return View(new ProductDto());
            }
            // Update
            var res = await _productService.GetProductByIdAsync(id);
            var model = JsonConvert.DeserializeObject<ProductDto>(res.Result.ToString());
            if (model == null) return RedirectToAction("Error", "Home");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto productDto)
        {
            if (productDto == null || !ModelState.IsValid) { return RedirectToAction("Error", "Home"); }
            var res = await _productService.CreateProductAsync(productDto);
            if (res.IsSuccess)
            {
                TempData["success"] = "Upserted successfully";
            }
            return RedirectToAction("Index", "ProductAdmin");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (id == 0) return RedirectToAction("Error", "Home");
            var res = await _productService.GetProductByIdAsync(id);
            var model = JsonConvert.DeserializeObject<ProductDto>(res.Result.ToString());
            if (model == null) return RedirectToAction("Error", "Home");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProductById([FromRoute] int id, bool isTrash = true)
        {
            if (id == 0) return RedirectToAction("Error", "Home");
            var res = await _productService.DeleteProductAsync(id);
            return res.IsSuccess ? RedirectToAction("Index", "ProductAdmin") : RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductDto productDto)
        {
            if (productDto == null || !ModelState.IsValid) { return RedirectToAction("Error", "Home"); }
            var res = await _productService.UpdateProductAsync(productDto);
            if (res.IsSuccess)
            {
                TempData["success"] = "Upserted successfully";
            }
            return RedirectToAction("Index", "ProductAdmin");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductsByFile(IFormFile file)
        {
            if (file != null)
            {
                await _productService.CreateProductsByFileAsync(file);
                return RedirectToAction("Index", "ProductAdmin");
            }
            return null;
        }
    }
}
