using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Vitalia.Infrastructure.Data;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.Application.Interfaces.Services;
using Vitalia.Application.Services;
using Vitalia.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vitalia",
        Version = "v1",
        Description =
            "API da plataforma Vitalia para gerenciamento de produtos, pets,"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var connectionString = builder.Configuration.GetConnectionString("OracleConnection")
                       ?? throw new InvalidOperationException(
                           "A connection string 'OracleConnection' não foi encontrada."
                       );

builder.Services.AddDbContext<VitaliaDbContext>(options =>
    options.UseOracle(connectionString));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Vitalia v1"
        );

        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();