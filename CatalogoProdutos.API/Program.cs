using Catalogo.Application.Interfaces;
using Catalogo.Application.Mappings;
using Catalogo.Application.Services;
using Catalogo.Core.Interfaces;
using Catalogo.Infrastructure.Data.Context;
using Catalogo.Infrastructure.Repositories;
using CatalogoProdutos.API.Middleware;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore; // <--- PASSO 1: ADICIONE ESTE USING
using FluentValidation; // <--- PASSO 1: ADICIONE ESTE USING
using Catalogo.Application.Validators;

// ...e outros que o EF e o AutoMapper precisam



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CategoryCreateDTOValidator>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

// builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString,
        b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();


builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IHybridCacheService, HybridCacheService>();

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();


builder.Services.AddStackExchangeRedisCache(options =>
{
    // A string de conex�o mais segura para usar o Redis do Docker
    options.Configuration = "localhost:6379";
    // Voc� pode usar um prefixo para as chaves no Redis (opcional)
    options.InstanceName = "CatalogoAPI_";
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapControllers();

app.Run();
