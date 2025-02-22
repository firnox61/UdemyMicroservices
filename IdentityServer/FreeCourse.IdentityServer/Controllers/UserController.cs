﻿using FreeCourse.IdentityServer.Data;
using FreeCourse.IdentityServer.Dtos;
using FreeCourse.IdentityServer.Models;
using FreeCourses.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace FreeCourse.IdentityServer.Controllers
{
    [Authorize(LocalApi.PolicyName)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext )
        {
            _userManager = userManager;
            _context = applicationDbContext;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignUp(SignupDto signupDto)
        {
            var user = new ApplicationUser
            {
                UserName = signupDto.UserName,
                Email = signupDto.Email,
                City = signupDto.City,
            };
            var result=await _userManager.CreateAsync(user,signupDto.Password);
            await _context.SaveChangesAsync();
            if (!result.Succeeded)
            {
                return BadRequest(Response<NoContent>.Fail(result.Errors.Select(x=>x.Description).ToList(),400));
            }
            return Ok(new { success = true , message = "Kullanıcı başarıyla oluşturuldu." });
           // return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userIdClaim=User.Claims.FirstOrDefault(x=>x.Type==JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null) return BadRequest();

            var user=await _userManager.FindByIdAsync(userIdClaim.Value);
            if (user == null) return BadRequest();

            return Ok(new {Id=user.Id, UserName=user.UserName, Email=user.Email, City=user.City});
        }
    }
}
