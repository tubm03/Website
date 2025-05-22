using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Checkout
{
    public interface ICheckoutRepository
    {
        public Task<string> ProcessCheckOut(CheckoutViewModel checkoutModel, int? customerId, string email);
    }
}
