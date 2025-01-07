using FreeCourse.Web.Models.PhotoStocks;
using FreeCourse.Web.Services.Interfaces;
using FreeCourses.Shared.Dtos;

namespace FreeCourse.Web.Services
{
    public class PhotoStockService : IPhotoStockService
    {
        private readonly HttpClient _httpClient;

        public PhotoStockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> DeletePhoto(string photoUrl)
        {
            var response= await _httpClient.DeleteAsync($"photos?photoUrl={photoUrl}");
            return response.IsSuccessStatusCode;
        }

        public async Task<PhotoViewModel> UploadPhoto(IFormFile photo)
        {
            if(photo == null ||photo.Length<=0)
            {
                return null;
            }
            //örnek dosya 515535351.jpg
            var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(photo.FileName)}";
            using var ms=new MemoryStream();
            photo.CopyToAsync(ms);
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new ByteArrayContent(ms.ToArray()),"photo",randomFileName);
            //hangi controllera göndereceğimizii belirtiyoruz
            var response = await _httpClient.PostAsync("photos", multipartContent);
            if(!response.IsSuccessStatusCode)
            {
                return null;
            }
            var responseSuccess= await response.Content.ReadFromJsonAsync<Response<PhotoViewModel>>();
            return responseSuccess.Data;


        }
    }
}
