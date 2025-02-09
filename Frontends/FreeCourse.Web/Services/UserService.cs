using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using FreeCourses.Shared.Dtos;

namespace FreeCourse.Web.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserViewModel> GetUser()
        {
            return await _httpClient.GetFromJsonAsync<UserViewModel>("/api/user/getuser");
        }
        public async Task<Response<bool>> UserSignUp(SignupInput signupInput)
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/user/signup", signupInput);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                return Response<bool>.Fail("Boş yanıt alındı.", 400);
            }
            
            return Response<bool>.Success(200);

        }
    }
}
