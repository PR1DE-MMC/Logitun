using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Logitun.Shared.DTOs;
using Logitun.Shared.Models;
using Microsoft.Extensions.Logging;
using Logitun.Core.Interfaces;

namespace Logitun.Mvc.Controllers
{
    [Authorize]
    public class LocationsController : Controller
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(ILocationService locationService, ILogger<LocationsController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Index action called with page {Page} and pageSize {PageSize}", page, pageSize);
            var locations = await _locationService.GetPagedAsync(page, pageSize);
            return View(locations);
        }

        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("Create action called");
            return View(new LocationDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] LocationDto location)
        {
            _logger.LogInformation("Create POST action called with location data");

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", errors);
                return View(location);
            }

            try
            {
                _logger.LogInformation("Attempting to create location");
                var createdLocation = await _locationService.CreateAsync(location);
                _logger.LogInformation("Location created successfully with ID: {LocationId}", createdLocation.LocationId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating location: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error creating location: {ex.Message}");
                return View(location);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Edit GET action called for location ID: {LocationId}", id);
            try
            {
                var location = await _locationService.GetByIdAsync(id);
                if (location == null)
                {
                    _logger.LogWarning("Location not found with ID: {LocationId}", id);
                    return NotFound();
                }

                return View(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving location for editing with ID: {LocationId}", id);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] LocationDto location)
        {
            _logger.LogInformation("Edit POST action called for location ID: {LocationId}", id);

            if (id != location.LocationId)
            {
                _logger.LogWarning("ID mismatch: {Id} != {LocationId}", id, location.LocationId);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", errors);
                return View(location);
            }

            try
            {
                _logger.LogInformation("Attempting to update location");
                var updatedLocation = await _locationService.UpdateAsync(id, location);
                if (updatedLocation == null)
                {
                    _logger.LogWarning("Location not found for update with ID: {LocationId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Location updated successfully with ID: {LocationId}", updatedLocation.LocationId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error updating location: {ex.Message}");
                return View(location);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete action called for location ID: {LocationId}", id);
            try
            {
                var result = await _locationService.DeleteAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Location not found for deletion with ID: {LocationId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Location deleted successfully with ID: {LocationId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting location: {Message}", ex.Message);
                return NotFound();
            }
        }
    }
}