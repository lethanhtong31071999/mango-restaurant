using AutoMapper;
using Mango.Services.ProductAPI;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Repository;
using Mango.Services.ProductAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("AzureConnection"))
);

// Injection dependency
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Excel: EPPLUS
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
