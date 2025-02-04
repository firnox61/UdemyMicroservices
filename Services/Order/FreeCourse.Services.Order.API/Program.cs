using FreeCourse.Services.Order.Application.Consumer;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourses.Shared.Services;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

var builder = WebApplication.CreateBuilder(args);





builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateOrderMessageCommandConsumer>();
    x.AddConsumer<CourseNameChangedEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {//Default Port: 5672;
        cfg.Host(builder.Configuration["RabbitMQUrl"], "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });
        cfg.ReceiveEndpoint("create-order-service", e =>
        {
            e.ConfigureConsumer<CreateOrderMessageCommandConsumer>(context);
        });
        cfg.ReceiveEndpoint("course-name-changed-event-order-service", e =>
        {
            e.ConfigureConsumer<CourseNameChangedEventConsumer>(context);
        });
    });
});





var requreAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();//bize id laz�m identity koruma alt�na almak i�in
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
//JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"];//buradan key do�rulamas� yapacak ve istedi�i datay� d�necek
    options.Audience = "resource_order";
    options.RequireHttpsMetadata = false;
});

// Add services to the container.
builder.Services.AddDbContext<OrderDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), configure =>
    {
        configure.MigrationsAssembly("FreeCourse.Services.Order.Infrastructure");
    });
});
builder.Services.AddMediatR(typeof(FreeCourse.Services.Order.Application.Handlers.CreateOrderCommandHandler).Assembly);
builder.Services.AddHttpContextAccessor();//altdaki bunu kulland��� i�in ekledik
builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(requreAuthorizePolicy));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var orderDbContext = serviceProvider.GetRequiredService<OrderDbContext>();
    orderDbContext.Database.Migrate();

}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
