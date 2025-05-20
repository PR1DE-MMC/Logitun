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
    public interface ITruckService
    {
        Task<PagedResult<TruckDto>> GetPagedAsync(int page, int pageSize);
        Task<TruckDto> GetByIdAsync(int id);
        Task<TruckDto> CreateAsync(TruckDto truckDto);
        Task<TruckDto> UpdateAsync(int id, TruckDto truckDto);
        Task<bool> DeleteAsync(int id);
    }
}
