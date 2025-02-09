using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models
{
    public class SignupInput
    {
        [Required]
        [Display(Name = "Kullanıcı Adınız")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Emaiil adresiniz")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Şehriniz")]
        public string City { get; set; }
        [Required]
        [Display(Name = "Şifreniz")]
        public string Password { get; set; }
       
    }
}
