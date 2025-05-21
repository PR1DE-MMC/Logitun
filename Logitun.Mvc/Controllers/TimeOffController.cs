using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Logitun.Shared.DTOs;
using Logitun.Shared.Models;
using Microsoft.Extensions.Logging;
using Logitun.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Logitun.Mvc.Controllers
{
    [Authorize]
    public class TimeOffController : Controller
    {
        private readonly ITimeOffService _timeOffService;
        private readonly IAuthService _authService;
        private readonly ILogger<TimeOffController> _logger;

        public TimeOffController(
            ITimeOffService timeOffService,
            IAuthService authService,
            ILogger<TimeOffController> logger)
        {
            _timeOffService = timeOffService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Index action called with page {Page} and pageSize {PageSize}", page, pageSize);
            var timeOffRequests = await _timeOffService.GetPagedAsync(page, pageSize);
            await PopulateDriversDropdown();
            return View(timeOffRequests);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Create action called");
            await PopulateDriversDropdown();
            return View(new TimeOffRequestDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] TimeOffRequestDto timeOffRequest)
        {
            _logger.LogInformation("Create POST action called with time off request data");

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", errors);
                await PopulateDriversDropdown();
                return View(timeOffRequest);
            }

            try
            {
                // Convert dates to UTC
                timeOffRequest.StartDate = DateTime.SpecifyKind(timeOffRequest.StartDate, DateTimeKind.Utc);
                timeOffRequest.EndDate = DateTime.SpecifyKind(timeOffRequest.EndDate, DateTimeKind.Utc);

                _logger.LogInformation("Attempting to create time off request");
                var createdRequest = await _timeOffService.CreateAsync(timeOffRequest);
                _logger.LogInformation("Time off request created successfully with ID: {RequestId}", createdRequest.RequestId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating time off request: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error creating time off request: {ex.Message}");
                await PopulateDriversDropdown();
                return View(timeOffRequest);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Edit GET action called for time off request ID: {RequestId}", id);
            try
            {
                var request = await _timeOffService.GetByIdAsync(id);
                if (request == null)
                {
                    _logger.LogWarning("Time off request not found with ID: {RequestId}", id);
                    return NotFound();
                }

                await PopulateDriversDropdown();
                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving time off request for editing with ID: {RequestId}", id);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] TimeOffRequestDto timeOffRequest)
        {
            _logger.LogInformation("Edit POST action called for time off request ID: {RequestId}", id);

            if (id != timeOffRequest.RequestId)
            {
                _logger.LogWarning("ID mismatch: {Id} != {RequestId}", id, timeOffRequest.RequestId);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", errors);
                await PopulateDriversDropdown();
                return View(timeOffRequest);
            }

            try
            {
                // Convert dates to UTC
                timeOffRequest.StartDate = DateTime.SpecifyKind(timeOffRequest.StartDate, DateTimeKind.Utc);
                timeOffRequest.EndDate = DateTime.SpecifyKind(timeOffRequest.EndDate, DateTimeKind.Utc);

                _logger.LogInformation("Attempting to update time off request");
                var updatedRequest = await _timeOffService.UpdateAsync(id, timeOffRequest);
                if (updatedRequest == null)
                {
                    _logger.LogWarning("Time off request not found for update with ID: {RequestId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Time off request updated successfully with ID: {RequestId}", updatedRequest.RequestId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating time off request: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error updating time off request: {ex.Message}");
                await PopulateDriversDropdown();
                return View(timeOffRequest);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete action called for time off request ID: {RequestId}", id);
            try
            {
                var result = await _timeOffService.DeleteAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Time off request not found for deletion with ID: {RequestId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Time off request deleted successfully with ID: {RequestId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting time off request: {Message}", ex.Message);
                return NotFound();
            }
        }

        private async Task PopulateDriversDropdown()
        {
            var drivers = await _authService.GetAllDriversAsync();
            ViewData["Drivers"] = new SelectList(drivers.Select(d => new
            {
                d.UserId,
                Display = $"{d.FirstName} {d.LastName} ({d.Email})"
            }), "UserId", "Display");
        }
    }
}