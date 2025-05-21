using Microsoft.AspNetCore.Mvc;
using Logitun.Core.Interfaces;

namespace Logitun.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MockDataController : ControllerBase
    {
        private readonly IMockDataService _mockDataService;
        private readonly ILogger<MockDataController> _logger;

        public MockDataController(
            IMockDataService mockDataService,
            ILogger<MockDataController> logger)
        {
            _mockDataService = mockDataService;
            _logger = logger;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateMockData()
        {
            try
            {
                await _mockDataService.GenerateMockDataAsync();
                return Ok(new { message = "Mock data generated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating mock data");
                return StatusCode(500, new { error = "Error generating mock data", message = ex.Message });
            }
        }
    }
}