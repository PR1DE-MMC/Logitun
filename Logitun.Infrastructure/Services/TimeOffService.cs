using AutoMapper;
using AutoMapper.QueryableExtensions;
using Logitun.Core.Entities;
using Logitun.Core.Interfaces;
using Logitun.Infrastructure.Data;
using Logitun.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Logitun.Infrastructure.Services
{
    public class TimeOffRequestService : ITimeOffService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TimeOffRequestService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<TimeOffRequestDto>> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.TimeOffRequests.AsNoTracking();
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<TimeOffRequestDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<TimeOffRequestDto>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<TimeOffRequestDto> GetByIdAsync(int id)
        {
            var entity = await _context.TimeOffRequests.FindAsync(id);
            return entity == null ? null : _mapper.Map<TimeOffRequestDto>(entity);
        }

        public async Task<TimeOffRequestDto> CreateAsync(TimeOffRequestDto dto)
        {
            var entity = _mapper.Map<TimeOffRequest>(dto);
            _context.TimeOffRequests.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<TimeOffRequestDto>(entity);
        }

        public async Task<TimeOffRequestDto> UpdateAsync(int id, TimeOffRequestDto dto)
        {
            var existing = await _context.TimeOffRequests.FindAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _context.SaveChangesAsync();
            return _mapper.Map<TimeOffRequestDto>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.TimeOffRequests.FindAsync(id);
            if (entity == null) return false;

            _context.TimeOffRequests.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
