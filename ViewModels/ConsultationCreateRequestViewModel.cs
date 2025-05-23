using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.ViewModels
{
    public class ConsultationCreateRequestViewModel
    {
        public int ConsultionId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên của bạn")]
        [DisplayName("Họ và Tên")]
        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email của bạn")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại của bạn")]
        [RegularExpression("^\\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập lại.")]
        [DisplayName("Số Điện Thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập yêu cầu của bạn")]
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự")]
        public string Content { get; set; }
    }
}
