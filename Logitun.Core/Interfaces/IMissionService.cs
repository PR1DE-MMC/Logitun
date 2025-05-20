using Logitun.Core.Entities;
using Logitun.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitun.Core.Interfaces
{
    public interface IMissionService
    {
        Task<PagedResult<MissionDto>> GetPagedAsync(int page, int pageSize);
        Task<MissionDto> GetByIdAsync(int id);
        Task<MissionDto> CreateAsync(MissionDto dto);
        Task<MissionDto> UpdateAsync(int id, MissionDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
