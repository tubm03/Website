using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.ViewModels
{
    public class BookServiceViewModel
    {
        public int? OrderServiceId { get; set; }

        public int? CustomerId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên của bạn")]
        [StringLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại của bạn")]
        [RegularExpression("^\\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập lại.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sử dụng dịch vụ của bạn")]
        public string OrderDate { get; set; }

        [Precision(0)]
        public TimeOnly OrderTime { get; set; }

        public string? DateCreated { get; set; }

        public int ServiceOptionId { get; set; }

        public int ServiceId { get; set; }

        public string ServiceName { get; set; }

        public string PetType { get; set; }

        public string Weight { get; set; }

        public string Price { get; set; }

        [StringLength(250, ErrorMessage = "Ghi chú tối đa 250 ký tự")]
        public string? Message { get; set; }

        public string? Status { get; set; }

        public string? EmployeeName { get; set; }
    }
}
