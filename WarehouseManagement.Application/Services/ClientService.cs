using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Application.Common.Exceptions;
using WarehouseManagement.Application.DTOs;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Application.Services;

public class ClientService : IClientService
{
    private readonly WarehouseDbContext _context;
    private readonly IMapper _mapper;

    public ClientService(WarehouseDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClientDto>> GetAllAsync(bool includeArchived = false)
    {
        var query = _context.Clients.AsQueryable();

        if (!includeArchived)
            query = query.Where(c => !c.IsArchived);

        var clients = await query.OrderBy(c => c.Name).ToListAsync();
        return _mapper.Map<IEnumerable<ClientDto>>(clients);
    }

    public async Task<ClientDto?> GetByIdAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        return client == null ? null : _mapper.Map<ClientDto>(client);
    }

    public async Task<ClientDto> CreateAsync(CreateClientDto dto)
    {
        var exists = await _context.Clients
            .AnyAsync(c => c.Name == dto.Name && !c.IsArchived);

        if (exists)
            throw new DuplicateEntityException("Client", "name", dto.Name);

        var client = _mapper.Map<Client>(dto);
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return _mapper.Map<ClientDto>(client);
    }

    public async Task<ClientDto> UpdateAsync(int id, UpdateClientDto dto)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            throw new EntityNotFoundException("Client", id);

        var duplicateExists = await _context.Clients
            .AnyAsync(c => c.Name == dto.Name && c.Id != id && !c.IsArchived);

        if (duplicateExists)
            throw new DuplicateEntityException("Client", "name", dto.Name);

        _mapper.Map(dto, client);
        await _context.SaveChangesAsync();

        return _mapper.Map<ClientDto>(client);
    }

    public async Task<bool> ArchiveAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            throw new EntityNotFoundException("Client", id);

        client.IsArchived = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var client = await _context.Clients
            .Include(c => c.ShipmentDocuments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            throw new EntityNotFoundException("Client", id);

        if (client.ShipmentDocuments.Any())
        {
            throw new EntityInUseException("Client");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }
}