using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models
{
    public class SigninInput
    {//label etiketleri
        [Required]
        [Display(Name ="Emaiil adresiniz")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Şifreniz")]
        public string Password { get; set; }

        [Display(Name = "Beni hatırla")]
        public bool IsRemember { get; set; }
    }
}
