using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Services.Catalog.Settings;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Core.Configuration;
using Nest;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter());//hepisne authorize vermek i�in burdan yapt�k
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServerURL"];//buradan key do�rulamas� yapacak ve istedi�i datay� d�necek
    options.Audience = "resource_catalog";
    options.RequireHttpsMetadata = false;
});
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICourseService, CourseService>();
//builder.Services.AddAutoMapper(typeof(StartupBase));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
//herhangi bir clas�n constructurunda IDatabaseSettings ge�ti�im anda o bize dolu bir DatabaseSettings vericek ve DatabaseSettings den okuyor olucak
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddSingleton<IDatabaseSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
});
// Elasticsearch ayarlar�n� yap�land�r ve DI container'a ekle
var settings = new Nest.ConnectionSettings(new Uri("http://localhost:9200"))
    .DefaultIndex("courses");
var client = new ElasticClient(settings);
builder.Services.AddHostedService<ElasticsearchSyncHostedService>();

builder.Services.AddSingleton<IElasticClient>(client); // DI ile y�net

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    var categoryService = serviceProvider.GetRequiredService<ICategoryService>();

    if (!(await categoryService.GetAllAsync()).Data.Any())
    {
        await categoryService.CreateAsync(new CategoryDto { Name = "Asp.net Core Kursu" });
        await categoryService.CreateAsync(new CategoryDto { Name = "Asp.net Core API Kursu" });
    }
}



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
