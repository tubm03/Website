using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Cart
{
	public class CartRepository : ICartRepository
	{
		private readonly PetStoreDBContext _context;

		public CartRepository(PetStoreDBContext context)
		{
			_context = context;
		}

		public CartItemViewModel GetCartItemVM(int productOptionId, int quantity)
		{
			var cartItem = (from po in _context.ProductOptions
							join p in _context.Products on po.ProductId equals p.ProductId
							join s in _context.Sizes on po.SizeId equals s.SizeId
							join i in _context.Images on po.ImageId equals i.ImageId
							where po.ProductOptionId == productOptionId
							select new CartItemViewModel()
							{
								ProductOptionId = productOptionId,
								Name = p.Name,
								Size = new PetStoreProject.Models.Size()
								{
									SizeId = s.SizeId,
									Name = s.Name
								},
								Price = po.Price,
								ImgUrl = i.ImageUrl,
								Quantity = quantity,
								ProductId = p.ProductId,
								isDeleted = p.IsDelete,
								QuantityInStock = po.Quantity ?? 0
							}).FirstOrDefault();
			return cartItem;
		}

		public List<CartItemViewModel> GetListCartItemsVM(int customerId)
		{
			var listCartItems = from ca in _context.CartItems
								join po in _context.ProductOptions on ca.ProductOptionId equals po.ProductOptionId
								join p in _context.Products on po.ProductId equals p.ProductId
								join s in _context.Sizes on po.SizeId equals s.SizeId
								join i in _context.Images on po.ImageId equals i.ImageId
								where ca.CustomerId == customerId && p.IsDelete == false
								select new CartItemViewModel()
								{
									ProductId = p.ProductId,
									ProductOptionId = ca.ProductOptionId,
									Name = p.Name,
									Size = new PetStoreProject.Models.Size()
									{
										SizeId = s.SizeId,
										Name = s.Name
									},
									Price = po.Price,
									Quantity = ca.Quantity,
									ImgUrl = i.ImageUrl,
									QuantityInStock = po.Quantity ?? 0
								};
			return listCartItems.ToList<CartItemViewModel>();
		}

		public void AddItemsToCart(int productOptionId, int quantity, int customerID)
		{
			var cartItem = new CartItem
			{
				ProductOptionId = productOptionId,
				Quantity = quantity,
				CustomerId = customerID
			};

			_context.CartItems.Add(cartItem);

			_context.SaveChanges();
		}

		public void UpdateQuantityToCartItem(int productOptionId, int quantity, int customerID)
		{
			var cartItem = (from c in _context.CartItems
							where c.ProductOptionId == productOptionId && c.CustomerId == customerID
							select c).FirstOrDefault();

			cartItem.Quantity += quantity;

			_context.SaveChanges();
		}

		public void UpdateNewCartItem(int oldProductOptionId, int newProductOptionId, int quantity, int customerID)
		{
			var cartItem = (from c in _context.CartItems
							where c.ProductOptionId == oldProductOptionId && c.CustomerId == customerID
							select c).FirstOrDefault();

			cartItem.ProductOptionId = newProductOptionId;

			cartItem.Quantity = quantity;

			_context.SaveChanges();
		}

		public void DeleteCartItem(int productOptionId, int customer)
		{
			var cartItem = (from c in _context.CartItems
							where c.ProductOptionId == productOptionId && c.CustomerId == customer
							select c).FirstOrDefault();

			_context.CartItems.Remove(cartItem);

			_context.SaveChanges();
		}

		public bool isExistProductOption(int newProductOptionID, int customerID)
		{
			var cartItem = (from c in _context.CartItems
							where c.CustomerId == customerID && newProductOptionID == c.ProductOptionId
							select c).FirstOrDefault();

			if (cartItem != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public CartItemViewModel? findCartItemViewModel(int productOptionId, int customerID)
		{
			var cartItem = from c in _context.CartItems
						   join po in _context.ProductOptions on c.ProductOptionId equals po.ProductOptionId
						   join p in _context.Products on po.ProductId equals p.ProductId
						   join b in _context.Brands on p.BrandId equals b.BrandId
						   join s in _context.Sizes on po.SizeId equals s.SizeId
						   join a in _context.Attributes on po.AttributeId equals a.AttributeId
						   join i in _context.Images on po.ImageId equals i.ImageId
						   where c.ProductOptionId == productOptionId && c.CustomerId == customerID
						   select new CartItemViewModel
						   {
							   ProductId = p.ProductId,
							   ProductOptionId = c.ProductOptionId,
							   Name = p.Name,
							   Attribute = new PetStoreProject.Models.Attribute()
							   {
								   AttributeId = a.AttributeId,
								   Name = a.Name
							   },
							   Size = new PetStoreProject.Models.Size()
							   {
								   SizeId = s.SizeId,
								   Name = s.Name
							   },
							   Price = po.Price,
							   Quantity = c.Quantity,
							   ImgUrl = i.ImageUrl,
							   QuantityInStock = po.Quantity ?? 0
						   };

			return cartItem.FirstOrDefault();
		}

		public Models.Promotion GetItemPromotion(int itemId)
		{
			var productId = _context.ProductOptions.Find(itemId).ProductId;

			var product = _context.Products.Find(productId);

			var now = DateTime.Now;
			var promotions = (from p in _context.Promotions
							  where (p.BrandId == 0 || p.BrandId == product.BrandId)
								 && (p.ProductCateId == 0 || p.ProductCateId == product.ProductCateId)
								 && p.Status == true
							  select p).ToList();

			if (promotions.Count != 0)
			{
				int max = 0;
				Models.Promotion p = new Models.Promotion();
				foreach (var item in promotions)
				{
					if (item.Value > max && (DateTime.Parse(item.StartDate) <= now && DateTime.Parse(item.EndDate) >= now))
					{
						max = (int)item.Value;
						p = item;
					}
				}
				return p;
			}
			return null;
		}

	}
}
