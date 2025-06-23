using Microsoft.EntityFrameworkCore;
using VivaApiProject.Data.Migrations;
using VivaApiProject.Handlers;
using VivaApiProject.Mapping;
using VivaApiProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.builder.Services.AddMemoryCache(); // for later caching

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache(); // for later caching

builder.Services.AddScoped<ICountryCacheHandler, CountryCacheHandler>();
builder.Services.AddScoped<CountryMapper>();
builder.Services.AddHttpClient<ICountryService, CountryService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.MapControllers();

app.Run();
