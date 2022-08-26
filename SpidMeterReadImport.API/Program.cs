global using Microsoft.EntityFrameworkCore;
global using SpidMeterReadImport.DAL.DataContext;
using SpidMeterReadImport.Service.Imports;
using SpidMeterReadImport.Service.Imports.Interfaces;
using SpidMeterReadImport.Service.Interfaces;
using SpidMeterReadImport.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(typeof(DataContext));
builder.Services.AddScoped<ISpidMeterReadCsvImporter, SpidMeterReadCsvImporter>();
builder.Services.AddScoped<ISpidMeterReadService, SpidMeterReadService>();

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
