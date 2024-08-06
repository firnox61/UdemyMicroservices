using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
//private readonly IConfiguration Configuration;
//parametre belirttik ve isimlerini kücük harfle aldýk configuration için aayrlýycaz
builder.Configuration.AddJsonFile($"configuration.{builder.Environment.EnvironmentName.ToLower()}.json");

builder.Services.AddAuthentication().AddJwtBearer("GatewayAuthenticationScheme", options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"];//buradan key doðrulamasý yapacak ve istediði datayý dönecek
    options.Audience = "resource_gateway";
    options.RequireHttpsMetadata = false;
});

builder.Services.AddOcelot();
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
await app.UseOcelot();
app.Run();

