using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên của bạn")]
        [DisplayName("Họ và Tên")]
        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email của bạn")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại của bạn")]
        [RegularExpression("^\\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập lại.")]
        [DisplayName("Số Điện Thoại")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu của bạn")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Vui lòng nhập mật khẩu có độ dài từ 8 đến 20 ký tự.")]
        [DisplayName("Mật Khẩu")]
        [DataType(DataType.Password)]
        public string? Password { get; set; } 

        //public string Address { get; set; }
    }
}
