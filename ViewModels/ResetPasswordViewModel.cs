using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.ViewModels
{
    public class ResetPasswordViewModel
    {
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Vui lòng nhập mật khẩu độ dài từ 8 đến 20 ký tự.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mật khẩu xác nhận không được để trống")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Vui lòng nhập mật khẩu độ dài từ 8 đến 20 ký tự.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
