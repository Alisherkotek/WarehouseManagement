using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Infrastructure.Data;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<WarehouseManagement.Domain.Entities.Resource, WarehouseManagement.Application.DTOs.ResourceDto>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.CreateResourceDto, WarehouseManagement.Domain.Entities.Resource>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.UpdateResourceDto, WarehouseManagement.Domain.Entities.Resource>();
    
    cfg.CreateMap<WarehouseManagement.Domain.Entities.UnitOfMeasurement, WarehouseManagement.Application.DTOs.UnitOfMeasurementDto>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.CreateUnitOfMeasurementDto, WarehouseManagement.Domain.Entities.UnitOfMeasurement>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.UpdateUnitOfMeasurementDto, WarehouseManagement.Domain.Entities.UnitOfMeasurement>();
    
    cfg.CreateMap<WarehouseManagement.Domain.Entities.Client, WarehouseManagement.Application.DTOs.ClientDto>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.CreateClientDto, WarehouseManagement.Domain.Entities.Client>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.UpdateClientDto, WarehouseManagement.Domain.Entities.Client>();
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IUnitOfMeasurementService, UnitOfMeasurementService>();
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7020", "http://localhost:7020")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("BlazorPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();