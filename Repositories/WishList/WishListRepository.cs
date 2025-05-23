

using Microsoft.EntityFrameworkCore;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.WishList
{
	public class WishListRepository : IWishListRepository
	{
		private readonly PetStoreDBContext _dbContext;

		public WishListRepository(PetStoreDBContext dBContext) => _dbContext = dBContext;

		public List<WishListVM> wishListVMs(int customerId)
		{
			var list = GetListProductInWishList(customerId);
			foreach (var wl in list)
			{
				wl.price = GetPriceByDefault(wl.ProductId);
				wl.img_url = GetUrlImgByProductID(wl.ProductId);
			}
			return list;
		}

		public void DeleteFromWishList(int customerId, int productId)
		{
			var customer = _dbContext.Customers
									 .Include(c => c.Products)
									 .FirstOrDefault(c => c.CustomerId == customerId);

			if (customer == null)
			{
				throw new Exception("Customer not found.");
			}

			var product = customer.Products.FirstOrDefault(p => p.ProductId == productId);

			if (product == null)
			{
				throw new Exception("Product not found in the customer's wishlist.");
			}
			customer.Products.Remove(product);
			_dbContext.SaveChanges();
		}

		public List<WishListVM> GetListProductInWishList(int customerId)
		{
			return (from c in _dbContext.Customers
					where c.CustomerId == customerId
					from p in c.Products
					select new WishListVM
					{
						ProductId = p.ProductId,
						ProductName = p.Name,
						Brand = p.Brand.Name 
					}).ToList();
		}
		public string GetUrlImgByProductID(int productId)
		{
			var imageUrl = (from po in _dbContext.ProductOptions
							where po.ProductId == productId
							join img in _dbContext.Images on po.ImageId equals img.ImageId
							select img.ImageUrl  // Sử dụng thuộc tính đúng với tên được ánh xạ trong lớp
						   ).FirstOrDefault();
			return imageUrl;
		}

		public float GetPriceByDefault(int productID)
		{
			return (from p in _dbContext.ProductOptions
					where p.ProductId == productID
					select p.Price).FirstOrDefault();
		}
	}
}
