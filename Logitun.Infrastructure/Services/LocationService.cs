using AutoMapper;
using AutoMapper.QueryableExtensions;
using Logitun.Infrastructure.Data;
using Logitun.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Logitun.Core.Interfaces;
using Logitun.Core.Entities;

namespace Logitun.Infrastructure.Services
{
    public class LocationService : ILocationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LocationService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<LocationDto>> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Locations.AsNoTracking();
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<LocationDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<LocationDto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<LocationDto> GetByIdAsync(int id)
        {
            var entity = await _context.Locations.FindAsync(id);
            return entity == null ? null : _mapper.Map<LocationDto>(entity);
        }

        public async Task<LocationDto> CreateAsync(LocationDto dto)
        {
            var entity = _mapper.Map<Location>(dto);
            _context.Locations.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<LocationDto>(entity);
        }

        public async Task<LocationDto> UpdateAsync(int id, LocationDto dto)
        {
            var existing = await _context.Locations.FindAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _context.SaveChangesAsync();
            return _mapper.Map<LocationDto>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Locations.FindAsync(id);
            if (entity == null) return false;

            _context.Locations.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
