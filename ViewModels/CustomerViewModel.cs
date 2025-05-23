using Microsoft.VisualBasic;
using PetStoreProject.Helpers;
using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.ViewModels
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên của bạn")]
        [StringLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự")]
        public string FullName { get; set; } = null!;

        public string? DoB { get; set; }

        public bool? Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại của bạn")]
        [RegularExpression("^\\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập lại.")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "Địa chỉ tối đa 250 ký tự")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email của bạn")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = null!;

        public int AccountId { get; set; }
    }
}
