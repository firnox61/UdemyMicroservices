using FreeCourse.Web.Models;
using FreeCourses.Shared.Dtos;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserViewModel> GetUser();
        Task<Response<bool>> UserSignUp(SignupInput signupInput);
    }
}
