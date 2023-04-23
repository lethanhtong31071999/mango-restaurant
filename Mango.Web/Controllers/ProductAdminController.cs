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
    }
}
