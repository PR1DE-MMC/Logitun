using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Logitun.Shared.DTOs;
using Logitun.Shared.Models;
using Microsoft.Extensions.Logging;
using Logitun.Core.Interfaces;

namespace Logitun.Mvc.Controllers
{
    [Authorize]
    public class TrucksController : Controller
    {
        private readonly ITruckService _truckService;
        private readonly ILogger<TrucksController> _logger;

        public TrucksController(ITruckService truckService, ILogger<TrucksController> logger)
        {
            _truckService = truckService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Index action called with page {Page} and pageSize {PageSize}", page, pageSize);
            var trucks = await _truckService.GetPagedAsync(page, pageSize);
            return View(trucks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("Create action called");
            return View(new TruckDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] TruckDto truck)
        {
            _logger.LogInformation("Create POST action called with truck: PlateNumber={PlateNumber}, Model={Model}, FuelType={FuelType}, CapacityKg={CapacityKg}, ManufactureYear={ManufactureYear}", 
                truck.PlateNumber, 
                truck.Model, 
                truck.FuelType, 
                truck.CapacityKg, 
                truck.ManufactureYear);

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", errors);
                return View(truck);
            }

            try
            {
                _logger.LogInformation("Attempting to create truck");
                var createdTruck = await _truckService.CreateAsync(truck);
                _logger.LogInformation("Truck created successfully with ID: {TruckId}", createdTruck.TruckId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating truck: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error creating truck: {ex.Message}");
                return View(truck);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Edit GET action called for truck ID: {TruckId}", id);
            try
            {
                var truck = await _truckService.GetByIdAsync(id);
                if (truck == null)
                {
                    _logger.LogWarning("Truck not found with ID: {TruckId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Retrieved truck for editing: PlateNumber={PlateNumber}, Model={Model}, FuelType={FuelType}", 
                    truck.PlateNumber, truck.Model, truck.FuelType);
                return View(truck);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving truck for editing with ID: {TruckId}", id);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] TruckDto truck)
        {
            _logger.LogInformation("Edit POST action called for truck ID: {TruckId} with data: PlateNumber={PlateNumber}, Model={Model}, FuelType={FuelType}", 
                id, truck.PlateNumber, truck.Model, truck.FuelType);

            if (id != truck.TruckId)
            {
                _logger.LogWarning("ID mismatch: {Id} != {TruckId}", id, truck.TruckId);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", errors);
                return View(truck);
            }

            try
            {
                _logger.LogInformation("Attempting to update truck");
                var updatedTruck = await _truckService.UpdateAsync(id, truck);
                if (updatedTruck == null)
                {
                    _logger.LogWarning("Truck not found for update with ID: {TruckId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Truck updated successfully with ID: {TruckId}", updatedTruck.TruckId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating truck: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error updating truck: {ex.Message}");
                return View(truck);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Delete action called for truck ID: {TruckId}", id);
            try
            {
                var result = await _truckService.DeleteAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Truck not found for deletion with ID: {TruckId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Truck deleted successfully with ID: {TruckId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting truck: {Message}", ex.Message);
                return NotFound();
            }
        }
    }
} 