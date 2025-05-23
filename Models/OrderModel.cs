namespace PetStoreProject.Models
{
    public class OrderModel
    {
        public int UserId { get; set; }

        public int pageIndex { get; set; }

        public int pageSize { get; set; }

        public string SortOrderId { get; set; }

        public string SortName { get; set; }

        public string SortTotalItems { get; set; }

        public string SortPrice { get; set; }

        public string SortDate { get; set; }
        public string SearchName { get; set; }

        public string SearchDate { get; set; }

        public string SearchOrderId { get; set; }

        public string SearchConsigneeName { get; set; }

        public string SortConsigneeName { get; set; }

    }
}
