using Microsoft.AspNetCore.Mvc;
using Logitun.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Logitun.Shared.Models;
using Logitun.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logitun.Mvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITruckService _truckService;
        private readonly IMissionService _missionService;
        private readonly ILocationService _locationService;
        private readonly ITimeOffService _timeOffService;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public HomeController(
            ITruckService truckService,
            IMissionService missionService,
            ILocationService locationService,
            ITimeOffService timeOffService,
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            IAuthService authService)
        {
            _truckService = truckService;
            _missionService = missionService;
            _locationService = locationService;
            _timeOffService = timeOffService;
            _logger = logger;
            _context = context;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get all missions with truck and driver information
                var missionsResult = await _missionService.GetPagedAsync(1, int.MaxValue);
                var activeMissions = missionsResult.Items.Where(m =>
                    m.Status == "SCHEDULED" || m.Status == "IN_PROGRESS").ToList();

                // Get all trucks and drivers for lookup
                var trucksResult = await _truckService.GetPagedAsync(1, 100);
                var drivers = await _authService.GetAvailableDriversAsync();

                // Create lookup dictionaries for quick access
                ViewData["Trucks"] = trucksResult.Items.ToDictionary(t => t.TruckId, t => t.PlateNumber);
                ViewData["Drivers"] = drivers.ToDictionary(d => d.UserId, d => $"{d.FirstName} {d.LastName} ({d.Email})");

                // Get all time off requests and filter for PENDING
                var timeOffResult = await _timeOffService.GetPagedAsync(1, int.MaxValue);
                var pendingTimeOffRequests = timeOffResult.Items.Count(r =>
                    r.Status == "PENDING");

                var dashboardData = new
                {
                    Trucks = (await _truckService.GetPagedAsync(1, 1)).TotalItems,
                    Missions = activeMissions.Count,
                    Locations = (await _locationService.GetPagedAsync(1, 1)).TotalItems,
                    TimeOffRequests = pendingTimeOffRequests,
                    ActiveMissions = new PagedResult<Shared.DTOs.MissionDto>
                    {
                        Items = activeMissions,
                        TotalItems = activeMissions.Count,
                        Page = 1,
                        PageSize = activeMissions.Count
                    }
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                return View(new
                {
                    Trucks = 0,
                    Missions = 0,
                    Locations = 0,
                    TimeOffRequests = 0,
                    ActiveMissions = new PagedResult<Shared.DTOs.MissionDto>
                    {
                        Items = new List<Shared.DTOs.MissionDto>(),
                        TotalItems = 0,
                        Page = 1,
                        PageSize = 0
                    }
                });
            }
        }
    }
}