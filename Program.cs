using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using PetStoreProject.Areas.Admin.Service.Cloudinary;
using PetStoreProject.Helpers;
using PetStoreProject.Models;
using PetStoreProject.Repositories.Accounts;
using PetStoreProject.Repositories.Admin;
using PetStoreProject.Repositories.Attribute;
using PetStoreProject.Repositories.Brand;
using PetStoreProject.Repositories.Cart;
using PetStoreProject.Repositories.Category;
using PetStoreProject.Repositories.Checkout;
using PetStoreProject.Repositories.Consultion;
using PetStoreProject.Repositories.Customers;
using PetStoreProject.Repositories.Discount;
using PetStoreProject.Repositories.DiscountType;
using PetStoreProject.Repositories.District;
using PetStoreProject.Repositories.Employee;
using PetStoreProject.Repositories.FeedBack;
using PetStoreProject.Repositories.Image;
using PetStoreProject.Repositories.News;
using PetStoreProject.Repositories.Order;
using PetStoreProject.Repositories.OrderItem;
using PetStoreProject.Repositories.OrderService;
using PetStoreProject.Repositories.Product;
using PetStoreProject.Repositories.ProductCategory;
using PetStoreProject.Repositories.ProductOption;
using PetStoreProject.Repositories.Promotion;
using PetStoreProject.Repositories.ReturnRefund;
using PetStoreProject.Repositories.Service;
using PetStoreProject.Repositories.Shipper;
using PetStoreProject.Repositories.Size;
using PetStoreProject.Repositories.WishList;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PetStoreDBContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("PetStoreDBContext"),
    sqlOptions => sqlOptions.EnableRetryOnFailure())
);

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(30);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
        options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
        options.CallbackPath = "/signin-google";
        options.Events = new OAuthEvents
        {
            OnRemoteFailure = (context) =>
            {
                context.HandleResponse();
                context.Response.Redirect("/Account/GoogleFailure");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSingleton<EmailService>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<IWishListRepository, WishListRepository>();

builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<INewsRepository, NewsRepository>();

builder.Services.AddTransient<IProductRepository, ProductRepository>();

builder.Services.AddTransient<ICartRepository, CartRepository>();

builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();

builder.Services.AddTransient<IFeedbackRepository, FeedbackRepository>();

builder.Services.AddTransient<IProductCategoryRepository, ProductCategoryRepository>();

builder.Services.AddTransient<IBrandRepository, BrandRepository>();

builder.Services.AddTransient<IAttributeRepository, AttributeRepository>();

builder.Services.AddTransient<ISizeRepository, SizeRepository>();

builder.Services.AddTransient<IDiscountTypeRepository, DiscountTypeRepository>();

builder.Services.AddTransient<IDiscountRepository, DiscountRepository>();

builder.Services.AddTransient<IPromotionRepository, PromotionRepository>();

builder.Services.AddTransient<IProductOptionRepository, ProductOptionRepository>();

builder.Services.AddTransient<IImageRepository, ImageRepository>();

builder.Services.AddTransient<IServiceRepository, ServiceRepository>();

builder.Services.AddTransient<IOrderRepository, OrderRepository>();

builder.Services.AddTransient<IOrderServiceRepository, OrderServiceRepository>();

builder.Services.AddTransient<IAdminRepository, AdminRepository>();

builder.Services.AddTransient<IEmployeeRepository, EmployeeReporistory>();

builder.Services.AddTransient<IOrderItemRepository, OrderItemRepository>();

builder.Services.AddTransient<IConsultationRepository, ConsultationRepository>();

builder.Services.AddTransient<ICheckoutRepository, CheckoutRepository>();

builder.Services.AddTransient<ICloudinaryService, CloudinaryService>();

builder.Services.AddTransient<IShipperRepository, ShipperRepository>();

builder.Services.AddTransient<IReturnRefundRepository, ReturnRefundRepository>();

builder.Services.AddTransient<IShipperRepository, ShipperRepository>();

builder.Services.AddTransient<IDistrictRepository, DistrictRepository>();

builder.Services.AddSingleton(new CloudinaryDotNet.Cloudinary(new CloudinaryDotNet.Account(
        builder.Configuration.GetSection("Cloudinary:CloudName").Value,
        builder.Configuration.GetSection("Cloudinary:ApiKey").Value,
        builder.Configuration.GetSection("Cloudinary:ApiSecret").Value
    )));

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = new JobKey("OrderServiceJob");
    q.AddJob<ServiceRepository>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("OrderServiceJob-trigger")
        .WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "area",
    pattern: "{area:exists}/{controller}/{action}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"
    );

app.Run();
