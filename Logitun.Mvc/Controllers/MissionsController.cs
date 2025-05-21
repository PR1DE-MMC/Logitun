using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Logitun.Shared.DTOs;
using Logitun.Core.Interfaces;

namespace Logitun.Mvc.Controllers
{
    [Authorize]
    public class MissionsController : Controller
    {
        private readonly IMissionService _missionService;
        private readonly ITruckService _truckService;
        private readonly ILocationService _locationService;
        private readonly IAuthService _authService;
        private readonly ILogger<MissionsController> _logger;

        public MissionsController(
            IMissionService missionService,
            ITruckService truckService,
            ILocationService locationService,
            IAuthService authService,
            ILogger<MissionsController> logger)
        {
            _missionService = missionService;
            _truckService = truckService;
            _locationService = locationService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Index action called with page {Page} and pageSize {PageSize}", page, pageSize);
            var missions = await _missionService.GetPagedAsync(page, pageSize);

            // Get all trucks and drivers for lookup
            var trucksResult = await _truckService.GetPagedAsync(1, 100);
            var drivers = await _authService.GetAvailableDriversAsync();

            // Create lookup dictionaries for quick access
            ViewData["Trucks"] = trucksResult.Items.ToDictionary(t => t.TruckId, t => t.PlateNumber);
            ViewData["Drivers"] = drivers.ToDictionary(d => d.UserId, d => $"{d.FirstName} {d.LastName} ({d.Email})");

            return View(missions);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Create action called");

            await PopulateDropdowns();

            return View(new MissionDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] MissionDto mission)
        {
            _logger.LogInformation("Create POST action called for mission");

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", errors);

                // Only repopulate dropdowns when returning to the view
                await PopulateDropdowns();
                return View(mission);
            }

            try
            {
                _logger.LogInformation("Attempting to create mission with data: {@Mission}", mission);
                var createdMission = await _missionService.CreateAsync(mission);
                _logger.LogInformation("Mission created successfully with ID: {MissionId}", createdMission.MissionId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating mission: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error creating mission: {ex.Message}");

                // Only repopulate dropdowns when returning to the view
                await PopulateDropdowns();
                return View(mission);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Edit GET action called for mission ID: {MissionId}", id);
            try
            {
                var mission = await _missionService.GetByIdAsync(id);
                if (mission == null)
                {
                    _logger.LogWarning("Mission not found with ID: {MissionId}", id);
                    return NotFound();
                }

                await PopulateDropdowns();

                _logger.LogInformation("Retrieved mission for editing: ID={MissionId}", mission.MissionId);
                return View(mission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving mission for editing with ID: {MissionId}", id);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] MissionDto mission)
        {
            _logger.LogInformation("Edit POST action called for mission ID: {MissionId}", id);

            if (id != mission.MissionId)
            {
                _logger.LogWarning("ID mismatch: {Id} != {MissionId}", id, mission.MissionId);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", errors);

                // Only repopulate dropdowns when returning to the view
                await PopulateDropdowns();
                return View(mission);
            }

            try
            {
                _logger.LogInformation("Attempting to update mission: {@Mission}", mission);
                var updatedMission = await _missionService.UpdateAsync(id, mission);
                if (updatedMission == null)
                {
                    _logger.LogWarning("Mission not found for update with ID: {MissionId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Mission updated successfully with ID: {MissionId}", updatedMission.MissionId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating mission: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error updating mission: {ex.Message}");

                // Only repopulate dropdowns when returning to the view
                await PopulateDropdowns();
                return View(mission);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete action called for mission ID: {MissionId}", id);
            try
            {
                var deleted = await _missionService.DeleteAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("Mission not found for deletion with ID: {MissionId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Mission deleted successfully with ID: {MissionId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting mission: {Message}", ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task PopulateDropdowns()
        {
            // Get all trucks and locations for dropdowns
            var trucksResult = await _truckService.GetPagedAsync(1, 100);
            var locationsResult = await _locationService.GetPagedAsync(1, 100);
            var drivers = await _authService.GetAvailableDriversAsync();

            // Convert to SelectList for easier use in views
            ViewData["Trucks"] = new SelectList(trucksResult.Items, "TruckId", "PlateNumber");
            ViewData["Locations"] = new SelectList(locationsResult.Items, "LocationId", "Name");
            ViewData["Drivers"] = new SelectList(drivers.Select(d => new
            {
                d.UserId,
                Display = $"{d.FirstName} {d.LastName} ({d.Email})"
            }), "UserId", "Display");
        }
    }
}