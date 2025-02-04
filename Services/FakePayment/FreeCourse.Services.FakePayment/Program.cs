using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {//Default Port: 5672;
        cfg.Host(builder.Configuration["RabbitMQUrl"], "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });
    });
});
//builder.Services.AddMassTransitHostedService(); kullaným dýþý diyor bakacaðýz
// Add services to the container.
var requreAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();//bize id lazým identity koruma altýna almak için

builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(requreAuthorizePolicy));
});// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
//JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"];//buradan key doðrulamasý yapacak ve istediði datayý dönecek
    options.Audience = "resource_payment";
    options.RequireHttpsMetadata = false;
});
builder.Services.AddHttpContextAccessor();

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
