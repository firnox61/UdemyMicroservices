using FreeCourse.Web.Helpers;
using FreeCourse.Web.Models;
using FreeCourse.Web.Models.Catalogs;
using FreeCourse.Web.Services.Interfaces;
using FreeCourses.Shared.Dtos;

namespace FreeCourse.Web.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IPhotoStockService _photoStockService;
        private readonly PhotoHelper _photoHelper;

        public CatalogService(HttpClient httpClient, IPhotoStockService photoStockService, PhotoHelper photoHelper)
        {
            _httpClient = httpClient;
            _photoStockService = photoStockService;
            _photoHelper = photoHelper;
        }
        public async Task<List<CourseViewModel>> SearchCoursesAsync(string query)
        {
            var response=await _httpClient.GetAsync($"courses/search?query={query}");
            if (!response.IsSuccessStatusCode)
            {
                return new List<CourseViewModel>();
            }
            var responseSuccess=await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();
            if(responseSuccess.Data !=null)
            {
                responseSuccess.Data.ForEach(x =>
                {
                    x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
                });
            }
            return responseSuccess.Data ?? new List<CourseViewModel>();
        }
        public async Task<bool> CreateCourseAsync(CourseCreateInput courseCreateInput)
        {
            var resultPhoto = await _photoStockService.UploadPhoto(courseCreateInput.PhotoFormfile);
            if (resultPhoto != null)
            {
                courseCreateInput.Picture = resultPhoto.Url;
            }
           // var response = await _httpClient.GetAsync("courses");
            var response=await _httpClient.PostAsJsonAsync<CourseCreateInput>("courses",courseCreateInput);
            return response.IsSuccessStatusCode;
          
        }

        public async Task<bool> DeleteCourseAsync(string courseId)
        {
            var response = await _httpClient.DeleteAsync($"courses/{courseId}");
            return response.IsSuccessStatusCode;    
        }

        public async Task<List<CategoryViewModel>> GetAllCategoryAsync()
        {
            var response = await _httpClient.GetAsync("categories");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var responseSuccess=await response.Content.ReadFromJsonAsync<Response<List<CategoryViewModel>>>();
            return responseSuccess.Data;
        }

        public async Task<List<CourseViewModel>> GetAllCourseAsync()
        {
            //http://localhost:5000/services/catalog/courses
            var response = await _httpClient.GetAsync("courses");
            if(!response.IsSuccessStatusCode)
            {
                return null;
            }
            var responseSuccess=await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();
            responseSuccess.Data.ForEach(x =>
            {
                x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
            });
            return responseSuccess.Data;
        }

        public async Task<List<CourseViewModel>> GetAllCourseByUserIdAsync(string userId)
        {
            //[controller]/GetAllByUserId/{userId}
            var response = await _httpClient.GetAsync($"courses/GetAllByUserId/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();
            responseSuccess.Data.ForEach(x =>
            {
                x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture);
            });
            return responseSuccess.Data;
        }

        public async Task<CourseViewModel> GetByCourseId(string courseId)
        {
            //[controller]/GetAllByUserId/{id}
            var response = await _httpClient.GetAsync($"courses/{courseId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<CourseViewModel>>();
            responseSuccess.Data.StockPictureUrl=_photoHelper.GetPhotoStockUrl(responseSuccess.Data.Picture);
            return responseSuccess.Data;
        }

       

        public async Task<bool> UpdateCourseAsync(CourseUpdateInput courseUpdateInput)
        {
            var resultPhoto = await _photoStockService.UploadPhoto(courseUpdateInput.PhotoFormfile);
            if (resultPhoto != null)
            {
                await _photoStockService.DeletePhoto(courseUpdateInput.Picture);
                courseUpdateInput.Picture = resultPhoto.Url;
            }
            var response = await _httpClient.PutAsJsonAsync<CourseUpdateInput>("courses", courseUpdateInput);
            return response.IsSuccessStatusCode;
        }
    }
}
