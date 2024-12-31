using FreeCourse.Web.Handler;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services;
using FreeCourse.Web.Services.Interfaces;
using FreeCourses.Shared.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ClientSettings>(builder.Configuration.GetSection("ClientSettings"));
builder.Services.Configure<ServiceApiSettings>(builder.Configuration.GetSection("ServiceApiSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();
var serviceApiSettings = builder.Configuration.GetSection("ServiceApiSettings").Get<ServiceApiSettings>();

builder.Services.AddHttpClient<IClientCredentialTokenService, ClientCredentialTokenService>();

builder.Services.AddScoped<ResourceOwnerPasswordTokenHandler>();
builder.Services.AddScoped<ClientCredentialTokenHandler>();



//builder.Services.AddHttpClient<IMemoryCache, MemoryCache>();
builder.Services.AddHttpClient<IIdentityService, IdentityService>();
//builder.Services.AddAccessTokenManagement();


builder.Services.AddHttpClient<ICatalogService, CatalogService>(opt =>
{
    opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUrl}/{serviceApiSettings.Catalog.Path}");
}).AddHttpMessageHandler<ClientCredentialTokenHandler>();//token varse ekleyecek yoksa yenisini alýp ekleyecek



//Userservice içnde herhangi bir client kullanýldýðýnda  git bu hanlerý çalýþtýr diyoýruz
builder.Services.AddHttpClient<IUserService, UserService>(opt =>
{
    opt.BaseAddress = new Uri(serviceApiSettings.IdentityBaseUrl);
}).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();





builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie
    (CookieAuthenticationDefaults.AuthenticationScheme, opts =>
    {
        opts.LoginPath = "/Auth/SignIn";
        opts.ExpireTimeSpan = TimeSpan.FromDays(20);
        opts.SlidingExpiration = true;
        opts.Cookie.Name = "udemywebcookie";
    });


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
