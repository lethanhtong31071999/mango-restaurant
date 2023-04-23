using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        protected ResponseAPI res;
        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new List<ProductDto>();
            res = await _productService.GetAllProductsAsync();
            if(res == null || !res.IsSuccess )
            {
                TempData["error"] = "Cannot get products!";
                return View(model);
            }
            // Temporarily place full items -> we change after having the best seller
            var pagination = JsonConvert.DeserializeObject<PaginationResult<ProductDto>>(res.Result.ToString());
            model = pagination.Data;
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }
    }
}