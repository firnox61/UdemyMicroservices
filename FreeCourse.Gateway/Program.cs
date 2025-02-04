using FreeCourse.Gateway.DelegateHandlers;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<TokenExchangeDelegateHandler>();
//private readonly IConfiguration Configuration;
//parametre belirttik ve isimlerini k�c�k harfle ald�k configuration i�in aayrl�ycaz
builder.Configuration.AddJsonFile($"configuration.{builder.Environment.EnvironmentName.ToLower()}.json");

builder.Services.AddAuthentication().AddJwtBearer("GatewayAuthenticationScheme", options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"];//buradan key do�rulamas� yapacak ve istedi�i datay� d�necek
    options.Audience = "resource_gateway";
    options.RequireHttpsMetadata = false;
});

builder.Services.AddOcelot().AddDelegatingHandler<TokenExchangeDelegateHandler>();
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
await app.UseOcelot();
app.Run();

