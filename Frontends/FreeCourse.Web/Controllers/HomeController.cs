using FreeCourse.Web.Exception;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FreeCourse.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICatalogService _catalogService;
        public HomeController(ILogger<HomeController> logger, ICatalogService catalogService)
        {
            _logger = logger;
            _catalogService = catalogService;
        }

        public async  Task<IActionResult> Index()
        {
            return View( await _catalogService.GetAllCourseAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction(nameof(Index));
            }

            var searchResults = await _catalogService.SearchCoursesAsync(query);
            return View("Index", searchResults);
        }
        public async Task<IActionResult> Detail(string id)
        {
            return View(await _catalogService.GetByCourseId(id));
        }
      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorFeature=HttpContext.Features.Get<IExceptionHandlerFeature>();
            //Hata bilgisi varsa (errorFeature != null) ve bu hata UnAuthorizeException türündeyse:
            //Kullanýcý oturumunu sonlandýrmak için AuthController.LogOut aksiyonuna yönlendirilir.
            if (errorFeature != null && errorFeature.Error is UnAuthorizeException)
            {
                return RedirectToAction(nameof(AuthController.LogOut), "Auth");
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            //Yukarýdaki kontrol saðlanmazsa, genel bir hata sayfasý döndürülür.
        }
    }
}
