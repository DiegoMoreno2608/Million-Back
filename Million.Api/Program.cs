using Microsoft.Extensions.Options;
using Million.Application.Services;
using Million.Domain.Interfaces;
using Million.Infrastructure.Repositories;
using Million.Infrastructure.Settings;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Configuración MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Inyección de dependencias
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<PropertyService>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<OwnerService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt => opt.AddPolicy("AllowAll", policy => { policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod(); }));
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("AllowAll");    
app.MapControllers();
app.UseStaticFiles();

app.Run();

