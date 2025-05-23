using Microsoft.AspNetCore.Mvc;
using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Controllers
{
	public class FeedbackController : Controller
	{
		private readonly PetStoreDBContext _context;

		public FeedbackController(PetStoreDBContext context)
		{
			_context = context;
		}

		[HttpPost]
		public IActionResult Submit()
		{
			int pid = 0;
			int sid = 0;
            bool isProductId = Int32.TryParse(Request.Form["pid"], out pid);
			bool isServiceId = Int32.TryParse(Request.Form["sid"], out sid);

            Feedback feedback = new Feedback();
			if(isProductId)
			{
				feedback.ProductId = pid;
			} else
			{
				feedback.ServiceId = sid;
			}
			feedback.Name = Request.Form["FullName"];
			feedback.Phone = Request.Form["PhoneNumber"];
			feedback.Email = Request.Form["Email"];
			feedback.Rating = Int32.Parse(Request.Form["rating"]);
			feedback.Content = Request.Form["Content"];
            feedback.Status = false;
            feedback.DateCreated = DateTime.Now;
			_context.Feedbacks.Add(feedback);
			_context.SaveChanges();

            if (isProductId)
            {
                return RedirectToAction("Detail", "Product", new { productId = pid});

            }
            else
            {
                return RedirectToAction("Detail", "Service", new { serviceId = sid });
            }

		}

		[HttpPost]
		public JsonResult CheckOrderProduct(string phoneNumber, int pid)
		{
			var listProductOptionIdOrder = (from o in _context.Orders
											where o.Phone == phoneNumber
											join ot in _context.OrderItems on o.OrderId equals ot.OrderId
											select ot.ProductOptionId);

			var listPOId = _context.ProductOptions
								   .Where(po => po.ProductId == pid)
								   .Select(po => po.ProductOptionId);

			bool haveOrdered = listProductOptionIdOrder.Any(i => listPOId.Contains(i));

			if (haveOrdered)
				return Json(new { status = "success" });
			else
				return Json(new { status = "error", message = "Bạn không thể comment vì bạn chưa mua hàng." });
		}

        [HttpPost]
        public JsonResult CheckUsedService(string phoneNumber, int pid)
        {
            var listServiceOptionIdOrder = (from o in _context.OrderServices
                                            where o.Phone == phoneNumber
                                            select o.ServiceOptionId).ToList();

            var listSOId = _context.ServiceOptions
                                   .Where(po => po.ServiceId == pid)
                                   .Select(po => po.ServiceOptionId).ToList();

			foreach(int i  in listSOId)
			{
				int x = i;
			}


            bool haveOrdered = listServiceOptionIdOrder.Any(i => listSOId.Contains(i));

            if (haveOrdered)
                return Json(new { status = "success" });
            else
                return Json(new { status = "error", message = "Bạn không thể comment vì bạn chưa sử dụng dịch vụ." });
        }
    }
}
