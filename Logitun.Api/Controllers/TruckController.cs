using Logitun.Core.Interfaces;
using Logitun.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Logitun.Api.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class TrucksController : ControllerBase
    {
        private readonly ITruckService _truckService;

        public TrucksController(ITruckService truckService)
        {
            _truckService = truckService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10)
        {
            var result = await _truckService.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var truck = await _truckService.GetByIdAsync(id);
            if (truck == null) return NotFound();
            return Ok(truck);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TruckDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _truckService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.TruckId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TruckDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _truckService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _truckService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
