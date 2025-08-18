using AutoMapper;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Resource, ResourceDto>();
        CreateMap<CreateResourceDto, Resource>();
        CreateMap<UpdateResourceDto, Resource>();
        
        CreateMap<UnitOfMeasurement, UnitOfMeasurementDto>();
        CreateMap<CreateUnitOfMeasurementDto, UnitOfMeasurement>();
        CreateMap<UpdateUnitOfMeasurementDto, UnitOfMeasurement>();
        
        CreateMap<Client, ClientDto>();
        CreateMap<CreateClientDto, Client>();
        CreateMap<UpdateClientDto, Client>();
    }
}