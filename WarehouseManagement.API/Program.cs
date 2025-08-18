using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Infrastructure.Data;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using WarehouseManagement.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateResourceValidator>();

builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<WarehouseManagement.Domain.Entities.Resource, WarehouseManagement.Application.DTOs.ResourceDto>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.CreateResourceDto,
        WarehouseManagement.Domain.Entities.Resource>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.UpdateResourceDto,
        WarehouseManagement.Domain.Entities.Resource>();

    cfg.CreateMap<WarehouseManagement.Domain.Entities.UnitOfMeasurement,
        WarehouseManagement.Application.DTOs.UnitOfMeasurementDto>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.CreateUnitOfMeasurementDto,
        WarehouseManagement.Domain.Entities.UnitOfMeasurement>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.UpdateUnitOfMeasurementDto,
        WarehouseManagement.Domain.Entities.UnitOfMeasurement>();

    cfg.CreateMap<WarehouseManagement.Domain.Entities.Client, WarehouseManagement.Application.DTOs.ClientDto>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.CreateClientDto, WarehouseManagement.Domain.Entities.Client>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.UpdateClientDto, WarehouseManagement.Domain.Entities.Client>();

    cfg.CreateMap<WarehouseManagement.Domain.Entities.Balance, WarehouseManagement.Application.DTOs.BalanceDto>()
        .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Resource.Name))
        .ForMember(dest => dest.UnitOfMeasurementName, opt => opt.MapFrom(src => src.UnitOfMeasurement.Name));

    cfg.CreateMap<WarehouseManagement.Domain.Entities.ReceiptDocument,
        WarehouseManagement.Application.DTOs.ReceiptDocumentDto>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.CreateReceiptDocumentDto,
        WarehouseManagement.Domain.Entities.ReceiptDocument>();
    cfg.CreateMap<WarehouseManagement.Domain.Entities.ReceiptResource,
        WarehouseManagement.Application.DTOs.ReceiptResourceDto>();

    cfg.CreateMap<WarehouseManagement.Domain.Entities.ShipmentDocument,
        WarehouseManagement.Application.DTOs.ShipmentDocumentDto>();
    cfg.CreateMap<WarehouseManagement.Application.DTOs.CreateShipmentDocumentDto,
        WarehouseManagement.Domain.Entities.ShipmentDocument>();
    cfg.CreateMap<WarehouseManagement.Domain.Entities.ShipmentResource,
        WarehouseManagement.Application.DTOs.ShipmentResourceDto>();
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IUnitOfMeasurementService, UnitOfMeasurementService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IReceiptDocumentService, ReceiptDocumentService>();
builder.Services.AddScoped<IShipmentDocumentService, ShipmentDocumentService>();

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