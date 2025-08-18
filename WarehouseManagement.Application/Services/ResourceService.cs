using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.Common.Exceptions;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Application.Services;

public class ResourceService : IResourceService
{
    private readonly WarehouseDbContext _context;
    private readonly IMapper _mapper;
    
    public ResourceService(WarehouseDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<ResourceDto>> GetAllAsync(bool includeArchived = false)
    {
        var query = _context.Resources.AsQueryable();
        
        if (!includeArchived)
            query = query.Where(r => !r.IsArchived);
            
        var resources = await query.OrderBy(r => r.Name).ToListAsync();
        return _mapper.Map<IEnumerable<ResourceDto>>(resources);
    }
    
    public async Task<ResourceDto?> GetByIdAsync(int id)
    {
        var resource = await _context.Resources.FindAsync(id);
        return resource == null ? null : _mapper.Map<ResourceDto>(resource);
    }
    
    public async Task<ResourceDto> CreateAsync(CreateResourceDto dto)
    {
        var exists = await _context.Resources
            .AnyAsync(r => r.Name == dto.Name && !r.IsArchived);
            
        if (exists)
            throw new DuplicateEntityException("Resource", "name", dto.Name);
            
        var resource = _mapper.Map<Resource>(dto);
        _context.Resources.Add(resource);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<ResourceDto>(resource);
    }
    
    public async Task<ResourceDto> UpdateAsync(int id, UpdateResourceDto dto)
    {
        var resource = await _context.Resources.FindAsync(id);
        if (resource == null)
            throw new EntityNotFoundException("Resource", id);
            
        var duplicateExists = await _context.Resources
            .AnyAsync(r => r.Name == dto.Name && r.Id != id && !r.IsArchived);
            
        if (duplicateExists)
            throw new DuplicateEntityException("Resource", "name", dto.Name);
            
        _mapper.Map(dto, resource);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<ResourceDto>(resource);
    }
    
    public async Task<bool> ArchiveAsync(int id)
    {
        var resource = await _context.Resources.FindAsync(id);
        if (resource == null)
            throw new EntityNotFoundException("Resource", id);
            
        resource.IsArchived = true;
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var resource = await _context.Resources
            .Include(r => r.Balances)
            .Include(r => r.ReceiptResources)
            .Include(r => r.ShipmentResources)
            .FirstOrDefaultAsync(r => r.Id == id);
            
        if (resource == null)
            throw new EntityNotFoundException("Resource", id);
            
        if (resource.Balances.Any() || 
            resource.ReceiptResources.Any() || 
            resource.ShipmentResources.Any())
        {
            throw new EntityInUseException("Resource");
        }
        
        _context.Resources.Remove(resource);
        await _context.SaveChangesAsync();
        return true;
    }
}