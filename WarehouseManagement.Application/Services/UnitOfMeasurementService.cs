using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.Common.Exceptions;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Application.Services;

public class UnitOfMeasurementService : IUnitOfMeasurementService
{
    private readonly WarehouseDbContext _context;
    private readonly IMapper _mapper;
    
    public UnitOfMeasurementService(WarehouseDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<UnitOfMeasurementDto>> GetAllAsync(bool includeArchived = false)
    {
        var query = _context.UnitsOfMeasurement.AsQueryable();
        
        if (!includeArchived)
            query = query.Where(u => !u.IsArchived);
            
        var units = await query.OrderBy(u => u.Name).ToListAsync();
        return _mapper.Map<IEnumerable<UnitOfMeasurementDto>>(units);
    }
    
    public async Task<UnitOfMeasurementDto?> GetByIdAsync(int id)
    {
        var unit = await _context.UnitsOfMeasurement.FindAsync(id);
        return unit == null ? null : _mapper.Map<UnitOfMeasurementDto>(unit);
    }
    
    public async Task<UnitOfMeasurementDto> CreateAsync(CreateUnitOfMeasurementDto dto)
    {
        var exists = await _context.UnitsOfMeasurement
            .AnyAsync(u => u.Name == dto.Name && !u.IsArchived);
            
        if (exists)
            throw new DuplicateEntityException("Unit of Measurement", "name", dto.Name);
            
        var unit = _mapper.Map<UnitOfMeasurement>(dto);
        _context.UnitsOfMeasurement.Add(unit);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<UnitOfMeasurementDto>(unit);
    }
    
    public async Task<UnitOfMeasurementDto> UpdateAsync(int id, UpdateUnitOfMeasurementDto dto)
    {
        var unit = await _context.UnitsOfMeasurement.FindAsync(id);
        if (unit == null)
            throw new EntityNotFoundException("Unit of Measurement", id);
            
        var duplicateExists = await _context.UnitsOfMeasurement
            .AnyAsync(u => u.Name == dto.Name && u.Id != id && !u.IsArchived);
            
        if (duplicateExists)
            throw new DuplicateEntityException("Unit of Measurement", "name", dto.Name);
            
        _mapper.Map(dto, unit);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<UnitOfMeasurementDto>(unit);
    }
    
    public async Task<bool> ArchiveAsync(int id)
    {
        var unit = await _context.UnitsOfMeasurement.FindAsync(id);
        if (unit == null)
            throw new EntityNotFoundException("Unit of Measurement", id);
            
        unit.IsArchived = true;
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var unit = await _context.UnitsOfMeasurement
            .Include(u => u.Balances)
            .Include(u => u.ReceiptResources)
            .Include(u => u.ShipmentResources)
            .FirstOrDefaultAsync(u => u.Id == id);
            
        if (unit == null)
            throw new EntityNotFoundException("Unit of Measurement", id);
            
        if (unit.Balances.Any() || 
            unit.ReceiptResources.Any() || 
            unit.ShipmentResources.Any())
        {
            throw new EntityInUseException("Unit of Measurement");
        }
        
        _context.UnitsOfMeasurement.Remove(unit);
        await _context.SaveChangesAsync();
        return true;
    }
}