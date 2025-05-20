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
    public interface ILocationService
    {
        Task<PagedResult<LocationDto>> GetPagedAsync(int page, int pageSize);
        Task<LocationDto> GetByIdAsync(int id);
        Task<LocationDto> CreateAsync(LocationDto dto);
        Task<LocationDto> UpdateAsync(int id, LocationDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
