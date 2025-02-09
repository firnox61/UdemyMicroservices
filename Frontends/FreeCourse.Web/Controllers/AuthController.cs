using FreeCourse.Web.Models;

using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IUserService _userService;

        public AuthController(IIdentityService identityService, IUserService userService)
        {
            _identityService = identityService;
            _userService = userService;
        }

        public IActionResult SignIn()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SigninInput signinInput)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var response=await _identityService.SignIn(signinInput);
            if (!response.IsSuccessful)
            {
                response.Errors.ForEach(x =>
                {
                    ModelState.AddModelError(String.Empty, x);
                });
                return View();
            }
            //home controllerdaki index sayfasına gitsin
            return RedirectToAction(nameof(Index), "Home");
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignupInput signupInput)
        {
            if (!ModelState.IsValid)
            {
                return View("SignUp", signupInput); // View'i açıkça belirtiyoruz

            }
            var response=await _userService.UserSignUp(signupInput);

            if (!response.IsSuccessful)
            {
                ModelState.Clear();
                response.Errors.ForEach(x =>
                {
                    ModelState.AddModelError(String.Empty, x);
                });
                return View();
            }
            var signInInput = new SigninInput
            {
                Email = signupInput.Email,
                Password = signupInput.Password,
                IsRemember = true
            };
            var signInResponse= await _identityService.SignIn(signInInput);
            if (!signInResponse.IsSuccessful)
            {
                signInResponse.Errors.ForEach(x =>
                {
                    ModelState.AddModelError(String.Empty, x);
                });
                return View("SignUp",signupInput);
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _identityService.RevokeRefreshToken();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
