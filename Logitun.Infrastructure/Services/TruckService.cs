using AutoMapper;
using AutoMapper.QueryableExtensions;
using Logitun.Core.Entities;
using Logitun.Core.Interfaces;
using Logitun.Infrastructure.Data;
using Logitun.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Logitun.Infrastructure.Services
{
    

    public class TruckService : ITruckService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TruckService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            var truck = await _context.Trucks.FindAsync(id);
            return truck == null ? null : _mapper.Map<TruckDto>(truck);
        }

        public async Task<TruckDto> CreateAsync(TruckDto truckDto)
        {
            var truck = _mapper.Map<Truck>(truckDto);
            _context.Trucks.Add(truck);
            await _context.SaveChangesAsync();
            return _mapper.Map<TruckDto>(truck);
        }

        public async Task<TruckDto> UpdateAsync(int id, TruckDto truckDto)
        {
            var existing = await _context.Trucks.FindAsync(id);
            if (existing == null) return null;

            _mapper.Map(truckDto, existing);
            await _context.SaveChangesAsync();
            return _mapper.Map<TruckDto>(existing);
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
