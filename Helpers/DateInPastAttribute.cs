using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.Helpers
{
    public class DateInPastAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && value is string dateString)
            {
                // Định dạng ngày tháng mong đợi
                string[] formats = { "yyyy-MM-dd" };

                // Chuyển đổi chuỗi thành kiểu DateTime
                if (DateTime.TryParseExact(dateString, formats, null, System.Globalization.DateTimeStyles.None, out DateTime inputDate))
                {
                    // Kiểm tra xem ngày sinh có phải là ngày trong quá khứ không
                    if (inputDate >= DateTime.Today)
                    {
                        return new ValidationResult(ErrorMessage ?? "Ngày sinh phải là ngày trong quá khứ.");
                    }
                }
                else
                {
                    return new ValidationResult("Định dạng ngày tháng không hợp lệ.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
