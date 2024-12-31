namespace FreeCourse.Web.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        //yiel ile dönecek ve ıenumeable olarak vericek
        public IEnumerable<string> GetUserProps()
        {
            yield return UserName;
            yield return Email;
            yield return City;
        }

    }
}
