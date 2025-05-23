using System.ComponentModel.DataAnnotations;

namespace PetStoreProject.Helpers
{
    public class NotDefaultDateValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateOnly date)
            {
                return date != default;
            }
            return true;
        }
    }
}
