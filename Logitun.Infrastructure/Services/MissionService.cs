using AutoMapper;
using AutoMapper.QueryableExtensions;
using Logitun.Core.Entities;
using Logitun.Core.Interfaces;
using Logitun.Infrastructure.Data;
using Logitun.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Logitun.Infrastructure.Services
{
    public class MissionService : IMissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MissionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<MissionDto>> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Missions
                .Include(m => m.OriginLocation)
                .Include(m => m.DestinationLocation)
                .AsNoTracking();

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<MissionDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<MissionDto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<MissionDto> GetByIdAsync(int id)
        {
            var mission = await _context.Missions
                .Include(m => m.OriginLocation)
                .Include(m => m.DestinationLocation)
                .FirstOrDefaultAsync(m => m.MissionId == id);

            return mission == null ? null : _mapper.Map<MissionDto>(mission);
        }

        public async Task<MissionDto> CreateAsync(MissionDto dto)
        {
            var mission = _mapper.Map<Mission>(dto);
            _context.Missions.Add(mission);
            await _context.SaveChangesAsync();
            return _mapper.Map<MissionDto>(mission);
        }

        public async Task<MissionDto> UpdateAsync(int id, MissionDto dto)
        {
            var existing = await _context.Missions.FindAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _context.SaveChangesAsync();
            return _mapper.Map<MissionDto>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var mission = await _context.Missions.FindAsync(id);
            if (mission == null) return false;

            _context.Missions.Remove(mission);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
