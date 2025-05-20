using Logitun.Core.Interfaces;
using Logitun.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Logitun.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MissionsController : ControllerBase
    {
        private readonly IMissionService _service;

        public MissionsController(IMissionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MissionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.MissionId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MissionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
