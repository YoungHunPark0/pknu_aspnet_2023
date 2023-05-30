using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Security.Permissions;

namespace aspnet02_boardapp.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "이메일주소는 필수입니다.")]
        [EmailAddress]
        [DisplayName("이메일주소")]
        public string Email { get; set; }

        [Required(ErrorMessage = "패스워드가 확인은 필수입니다.")]
        [DataType(DataType.Password)]
        [DisplayName("패스워드 확인")]
        public string Password { get; set; }

        [DisplayName("메일주소기억")]
        public bool RememberMe { get; set; }
    }
}
