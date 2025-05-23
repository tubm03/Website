using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.ViewModels
{
    public class ChangePasswordViewModel
    {
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Vui lòng nhập mật khẩu có độ dài từ 8 đến 20 ký tự.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu xác nhận không được để trống")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Vui lòng nhập mật khẩu có độ dài từ 8 đến 20 ký tự.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp với mật khẩu mới")]
        public string ConfirmPassword { get; set; }
    }
}
