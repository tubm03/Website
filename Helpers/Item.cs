namespace PetStoreProject.Helpers
{
    public class Item
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public double? RatingValue { get; set; }

        public Item()
        {
        }

        public Item(Item item)
        {
            UserId = item.UserId;
            ProductId = item.ProductId;
            RatingValue = item.RatingValue;
        }

        public Item(int productId, int userId, double? ratingValue)
        {
            UserId = userId;
            ProductId = productId;
            RatingValue = ratingValue;
        }
    }
}
