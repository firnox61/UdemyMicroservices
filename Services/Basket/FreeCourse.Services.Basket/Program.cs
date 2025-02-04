using FreeCourse.Services.Basket.Consumer;
using FreeCourse.Services.Basket.Services;
using FreeCourse.Services.Basket.Settings;
using FreeCourses.Shared.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);


var requreAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();//bize id laz�m identity koruma alt�na almak i�in






builder.Services.AddMassTransit(x =>
{
   
    x.AddConsumer<BasketCourseNameChangedEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {//Default Port: 5672;
        cfg.Host(builder.Configuration["RabbitMQUrl"], "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });
       
        cfg.ReceiveEndpoint("course-name-changed-event-basket-service", e =>
        {
            e.ConfigureConsumer<BasketCourseNameChangedEventConsumer>(context);
        });
    });
});


//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");


builder.Services.AddControllers(opt=>
{
    opt.Filters.Add(new AuthorizeFilter(requreAuthorizePolicy));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
//JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"];//buradan key do�rulamas� yapacak ve istedi�i datay� d�necek
    options.Audience = "resource_basket";
    options.RequireHttpsMetadata = false;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();
builder.Services.AddScoped<IBasketService, BasketService>();
// Add services to the container.
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));
builder.Services.AddSingleton<RedisService>(sp =>
{
    var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    var redis = new RedisService(redisSettings.Host, redisSettings.Port);
    redis.Connect();
    return redis;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
