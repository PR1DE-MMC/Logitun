using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Logitun.Shared.DTOs;
using Logitun.Shared.Models;
using Microsoft.Extensions.Logging;
using Logitun.Core.Interfaces;
using Logitun.Mvc.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Logitun.Mvc.Controllers
{
    [Authorize]
    public class MissionsController : Controller
    {
        private readonly IMissionService _missionService;
        private readonly ITruckService _truckService;
        private readonly ILocationService _locationService;
        private readonly ILogger<MissionsController> _logger;

        public MissionsController(
            IMissionService missionService,
            ITruckService truckService,
            ILocationService locationService,
            ILogger<MissionsController> logger)
        {
            _missionService = missionService;
            _truckService = truckService;
            _locationService = locationService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Index action called with page {Page} and pageSize {PageSize}", page, pageSize);
            var missions = await _missionService.GetPagedAsync(page, pageSize);
            return View(missions);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Create action called");
            var trucksResult = await _truckService.GetPagedAsync(1, 100);
            var locationsResult = await _locationService.GetPagedAsync(1, 100);
             _logger.LogInformation(trucksResult.Items[0].PlateNumber);
            var viewModel = new MissionCreateViewModel
            {
                Mission = new MissionDto(),
                AvailableTrucks = trucksResult.Items,
                AvailableLocations = locationsResult.Items
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] MissionCreateViewModel viewModel)
        {
            _logger.LogInformation("Create POST action called for mission");

            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Model state is invalid. Errors: {Errors}", errors);
                
                var trucksResult = await _truckService.GetPagedAsync(1, 100);
                var locationsResult = await _locationService.GetPagedAsync(1, 100);
                
                viewModel.AvailableTrucks = trucksResult.Items.ToList();
                viewModel.AvailableLocations = locationsResult.Items.ToList();
                return View(viewModel);
            }

            try
            {
                _logger.LogInformation("Attempting to create mission");
                var createdMission = await _missionService.CreateAsync(viewModel.Mission);
                _logger.LogInformation("Mission created successfully with ID: {MissionId}", createdMission.MissionId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating mission: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error creating mission: {ex.Message}");
                
                var trucksResult = await _truckService.GetPagedAsync(1, 100);
                var locationsResult = await _locationService.GetPagedAsync(1, 100);
                
                viewModel.AvailableTrucks = trucksResult.Items.ToList();
                viewModel.AvailableLocations = locationsResult.Items.ToList();
                return View(viewModel);
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

                var viewModel = new MissionEditViewModel
                {
                    Mission = mission,
                    AvailableTrucks = (await _truckService.GetPagedAsync(1, 100)).Items.ToList(),
                    AvailableLocations = (await _locationService.GetPagedAsync(1, 100)).Items.ToList()
                };

                _logger.LogInformation("Retrieved mission for editing: ID={MissionId}", mission.MissionId);
                return View(viewModel);
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
                
                var viewModel = new MissionEditViewModel
                {
                    Mission = mission,
                    AvailableTrucks = (await _truckService.GetPagedAsync(1, 100)).Items.ToList(),
                    AvailableLocations = (await _locationService.GetPagedAsync(1, 100)).Items.ToList()
                };
                return View(viewModel);
            }

            try
            {
                _logger.LogInformation("Attempting to update mission");
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
                
                var viewModel = new MissionEditViewModel
                {
                    Mission = mission,
                    AvailableTrucks = (await _truckService.GetPagedAsync(1, 100)).Items.ToList(),
                    AvailableLocations = (await _locationService.GetPagedAsync(1, 100)).Items.ToList()
                };
                return View(viewModel);
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
    }
} 