using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PetStoreProject.Helpers;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.Repositories.Product;
using PetStoreProject.ViewModels;
using System.Net;

namespace PetStoreProject.Controllers
{
    [Route("[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _product;
        private readonly ICustomerRepository _customer;
        private readonly IAccountRepository _account;

        public ProductController(IProductRepository product, ICustomerRepository customer, IAccountRepository account)
        {
            _product = product;
            _customer = customer;
            _account = account;
        }
        public IActionResult Search(string key)
        {
            if (key.IsNullOrEmpty())
            {
                SearchResultViewModel list = new SearchResultViewModel
                {
                    TotalResults = 0,
                    Results = new List<SearchViewModel>()
                };
                return View(list);
            }

            int cusomerID = GetCustomerId();
            if (cusomerID != -1)
            {
                List<int> listPID = _product.GetProductIDInWishList(GetCustomerId());
                ViewData["listPID"] = listPID;
            }
            else
            {
                ViewData["listPID"] = null;
            }
            ViewData["key"] = key;
            SearchResultViewModel listSearch = _product.GetListProductsByKeyWords(key, 1);
            return View(listSearch);
        }

        [HttpPost]
        public IActionResult LoadMoreSearch(string key, int page)
        {
            List<int> listPID = _product.GetProductIDInWishList(GetCustomerId());
            SearchResultViewModel listSearch = _product.GetListProductsByKeyWords(key, page);
            return new JsonResult(new
            {
                wishlist = listPID,
                listResult = listSearch.Results
            });
        }

        public int GetCustomerId()
        {
            var email = HttpContext.Session.GetString("userEmail");
            if (email != null)
            {
                var roles = _account.GetUserRoles(email);
                if (roles == "Customer")
                {
                    var customerID = _customer.GetCustomerId(email);
                    return customerID;
                }
            }
            return -1;
        }

        [HttpPost]
        public IActionResult ToggleFavorite(int productId)
        {
            int cusomerID = GetCustomerId();
            if (cusomerID == -1)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Content("Bạn cần đăng nhập để thực hiện thao tác này.");
            }
            var favoriteList = _product.GetProductIDInWishList(GetCustomerId());

            if (favoriteList.Contains(productId))
            {
                _product.RemoveFromFavorites(GetCustomerId(), productId);
            }
            else
            {
                _product.AddToFavorites(GetCustomerId(), productId);
            }
            return Json(new { success = true });
        }

        [HttpGet("{productId}")]
        public ActionResult Detail(int productId)
        {
            var customerId = GetCustomerId();
            var product_detail = _product.GetDetail(productId);

            ViewData["product_detail"] = product_detail;
            ViewData["related_products"] = _product.getRelatedProduct(productId, customerId);
            ViewData["listPID"] = _product.GetProductIDInWishList(customerId);
            ViewData["listFeedback"] = _product.GetListFeedBack(productId);

            return View();
        }

        [HttpPost]
        public ActionResult quickPreview(int productId)
        {
            var product_detail = _product.GetDetail(productId);
            return Json(product_detail);
        }

        [HttpGet("/Product/List")]
        public async Task<ActionResult> ListProduct(string CategoryId, int? productCateId, int? pageSize, int? page)
        {
            List<int> categoryIds = null;
            string url = "";

            int customerId = GetCustomerId();
            var productDetails = await _product.GetProductDetail(categoryIds, 0, customerId); // thay doi
            var totalItems = productDetails.Count();
            var pageIndex = page ?? 1;
            var _pageSize = pageSize ?? 21;
            var numberPage = Math.Ceiling((float)totalItems / _pageSize);
            List<int> listPID = _product.GetProductIDInWishList(GetCustomerId());

            ViewData["listPID"] = listPID;
            ViewBag.totalItems = totalItems;
            ViewBag.currentPage = pageIndex;
            ViewBag.pageSize = _pageSize;
            ViewBag.numberPage = numberPage;
            ViewBag.title = url;

            ViewBag.Sizes = _product.GetSizesByCategoryIdsAndProductCateId(categoryIds, 0);

            if (productDetails.Count != 0)
            {
                var priceMax = productDetails.SelectMany(p => p.productOption).Max(po => po.price);
                var priceMin = productDetails.SelectMany(p => p.productOption).Min(po => po.price);
                ViewBag.priceMin = priceMin;
                ViewBag.priceMax = priceMax;
            }

            return View("Product", PaginatedList<ProductDetailViewModel>.Create(productDetails, pageIndex, _pageSize));
        }

        [HttpPost]
        public async Task<ActionResult> ListProduct(string url, int? pageSize, int? page, List<string>? selectedBrands, string? selectedSort, int priceMin,
            int priceMax, List<string>? selectedColors, List<string>? selectedSizes, List<string>? selectedStatus, List<string>? selectedPromotion)
        {
            List<int> cateId = new List<int>();
            int productCateId = 0;
            var urlSplit = url.Split("/").ToList();

            switch (urlSplit[2])
            {
                case "DogFood":
                    cateId = new List<int> { 1, 3 };
                    break;
                case "CatFood":
                    cateId = new List<int> { 1, 4 };
                    break;
                case "DogAccessory":
                    cateId = new List<int> { 2, 5 };
                    break;
                case "CatAccessory":
                    cateId = new List<int> { 2, 6 };
                    break;
                case "foods":
                    cateId = [1];
                    break;
                case "accessories":
                    cateId = [2];
                    break;
            }
            if (urlSplit.Count > 3 && urlSplit[3] != null)
            {
                productCateId = int.Parse(urlSplit[3]);
            }
            var customerId = GetCustomerId();
            List<ProductDetailViewModel> productDetails = await _product.GetProductDetail(cateId, productCateId, customerId);

            //Filter brand
            if (!selectedBrands.IsNullOrEmpty())
                productDetails = productDetails
                    .Where(p => selectedBrands
                    .Contains(p.Brand) && (p.productOption[0].price >= priceMin && p.productOption[0].price <= priceMax))
                    .ToList();
            else
                productDetails = productDetails
                    .Where(p => p.productOption[0].price >= priceMin && p.productOption[0].price <= priceMax)
                    .ToList();

            if (productDetails != null)
            {
                // Filter size
                if (!selectedSizes.IsNullOrEmpty())
                    productDetails = productDetails
                        .Where(p => p.productOption
                    .Any(po => po != null && selectedSizes
                    .Any(size => po.size.Name != null && po.size.Name
                    .Contains(size))))
                        .ToList();

                // Filter color
                if (!selectedColors.IsNullOrEmpty())
                    productDetails = productDetails
                        .Where(p => p.productOption
                        .Any(po => po != null && selectedColors
                        .Any(color => po.attribute.Name != null && po.attribute.Name
                        .Contains(color))))
                        .ToList();
            }


            if (!selectedSort.IsNullOrEmpty())
            {
                switch (selectedSort)
                {
                    case "ProductNameAZ":
                        productDetails = productDetails.OrderBy(p => p.Name).ToList();
                        break;
                    case "ProductNameZA":
                        productDetails = productDetails.OrderByDescending(p => p.Name).ToList();
                        break;
                    case "PriceLowToHight":
                        productDetails = productDetails.OrderBy(p => p.productOption[0].price).ToList();
                        break;
                    case "PriceHightToLow":
                        productDetails = productDetails.OrderByDescending(p => p.productOption[0].price).ToList();
                        break;
                }
            }

            //Filter isSoldOut
            if (!selectedStatus.IsNullOrEmpty())
            {
                if (selectedStatus.Count == 1 && selectedStatus[0] == "False")
                {
                    productDetails = productDetails
                     .Where(p => p.productOption != null && p.productOption
                     .Any(po => po != null && po.IsSoldOut == false))
                     .ToList();
                }
                else if (selectedStatus.Count == 1 && selectedStatus[0] == "True")
                {
                    productDetails = productDetails
                    .Where(p => p.productOption != null && p.productOption
                    .All(po => po != null && po.IsSoldOut == true))
                    .ToList();
                }
            }

            if (!selectedPromotion.IsNullOrEmpty())
            {
                if (selectedPromotion.Count == 1 && selectedPromotion[0] == "True")
                {
                    productDetails = productDetails.Where(p => p.Promotion != null && p.Promotion.Status == true).ToList();
                }
                else if (selectedPromotion.Count == 1 && selectedPromotion[0] == "False")
                {
                    productDetails = productDetails.Where(p => p.Promotion == null).ToList();
                }
            }
            var _pageSize = pageSize ?? 21;
            var pageIndex = page ?? 1;
            var totalItems = productDetails.Count();
            var numberPage = Math.Ceiling((float)totalItems / _pageSize);
            var productDetail = productDetails.Skip((pageIndex - 1) * _pageSize).Take(_pageSize).ToList();
            List<int> listPID = _product.GetProductIDInWishList(GetCustomerId());

            TempData["listPID"] = listPID;
            TempData["pageSize"] = _pageSize;
            TempData["totalItems"] = totalItems;
            TempData["currentPage"] = pageIndex;
            return new JsonResult(new
            {
                wishlist = listPID,
                Data = productDetail,
                CurrentPage = pageIndex,
                NumberPage = numberPage,
                PageSize = _pageSize,
            });

        }

    }
}
