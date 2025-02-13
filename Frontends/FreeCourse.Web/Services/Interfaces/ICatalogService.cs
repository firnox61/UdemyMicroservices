using FreeCourse.Web.Models.Catalogs;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface ICatalogService
    {
        //gelen json veriyi bir view modelle hatırlamamız lazım
        Task<List<CourseViewModel>> GetAllCourseAsync();
        Task<List<CategoryViewModel>> GetAllCategoryAsync();
        Task<List<CourseViewModel>> GetAllCourseByUserIdAsync(string userId);

        Task<bool> CreateCourseAsync(CourseCreateInput courseCreateInput);
        Task<bool> UpdateCourseAsync(CourseUpdateInput courseUpdateInput);
        Task<CourseViewModel> GetByCourseId(string courseId);
        Task<bool> DeleteCourseAsync(string courseId);
        Task<List<CourseViewModel>> SearchCoursesAsync(string query);

    }
}
