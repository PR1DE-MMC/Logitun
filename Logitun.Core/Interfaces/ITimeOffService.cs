using Logitun.Core.Entities;
using Logitun.Shared.DTOs;
using Logitun.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitun.Core.Interfaces
{
    public interface ITimeOffService
    {
        Task<PagedResult<TimeOffRequestDto>> GetPagedAsync(int page, int pageSize);
        Task<TimeOffRequestDto> GetByIdAsync(int id);
        Task<TimeOffRequestDto> CreateAsync(TimeOffRequestDto dto);
        Task<TimeOffRequestDto> UpdateAsync(int id, TimeOffRequestDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
