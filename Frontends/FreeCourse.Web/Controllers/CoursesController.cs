using FreeCourse.Web.Services.Interfaces;
using FreeCourses.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ICatalogService _catalogService;
        //burada ID almak için
        private readonly ISharedIdentityService _sharedIdentityService;

        public CoursesController(ICatalogService catalogService, ISharedIdentityService sharedIdentityService)
        {
            _catalogService = catalogService;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _catalogService.GetAllCourseByUserIdAsync(_sharedIdentityService.GetUserId));
        }
    }
}
