using AutoMapper;
using AutoMapper.QueryableExtensions;
using Logitun.Core.Entities;
using Logitun.Core.Interfaces;
using Logitun.Infrastructure.Data;
using Logitun.Shared.DTOs;
using Logitun.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitun.Infrastructure.Services
{
    

    public class TruckService : ITruckService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TruckService> _logger;

        public TruckService(ApplicationDbContext context, IMapper mapper, ILogger<TruckService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<TruckDto>> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Trucks.AsNoTracking();
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<TruckDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<TruckDto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<TruckDto> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving truck with ID: {TruckId}", id);
                var truck = await _context.Trucks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TruckId == id);

                if (truck == null)
                {
                    _logger.LogWarning("Truck not found with ID: {TruckId}", id);
                    return null;
                }

                var truckDto = _mapper.Map<TruckDto>(truck);
                _logger.LogInformation("Successfully retrieved truck with ID: {TruckId}, PlateNumber: {PlateNumber}", 
                    truckDto.TruckId, truckDto.PlateNumber);
                return truckDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving truck with ID: {TruckId}", id);
                throw;
            }
        }

        public async Task<TruckDto> CreateAsync(TruckDto truckDto)
        {
            try
            {
                _logger.LogInformation("Creating truck with PlateNumber={PlateNumber}, Model={Model}, FuelType={FuelType}", 
                    truckDto.PlateNumber, 
                    truckDto.Model, 
                    truckDto.FuelType);

                var truck = _mapper.Map<Truck>(truckDto);
                _context.Trucks.Add(truck);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Truck created successfully with ID: {TruckId}", truck.TruckId);
                return _mapper.Map<TruckDto>(truck);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating truck: {Message}", ex.Message);
                throw new Exception($"Error creating truck: {ex.Message}", ex);
            }
        }

        public async Task<TruckDto> UpdateAsync(int id, TruckDto truckDto)
        {
            try
            {
                _logger.LogInformation("Updating truck with ID: {TruckId}", id);
                var existing = await _context.Trucks
                    .FirstOrDefaultAsync(t => t.TruckId == id);

                if (existing == null)
                {
                    _logger.LogWarning("Truck not found for update with ID: {TruckId}", id);
                    return null;
                }

                _logger.LogInformation("Found existing truck: PlateNumber={PlateNumber}, Model={Model}", 
                    existing.PlateNumber, existing.Model);

                _mapper.Map(truckDto, existing);
                await _context.SaveChangesAsync();

                var updatedDto = _mapper.Map<TruckDto>(existing);
                _logger.LogInformation("Successfully updated truck with ID: {TruckId}", updatedDto.TruckId);
                return updatedDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating truck with ID: {TruckId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var truck = await _context.Trucks.FindAsync(id);
            if (truck == null) return false;

            _context.Trucks.Remove(truck);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
