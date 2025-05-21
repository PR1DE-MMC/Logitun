using Microsoft.AspNetCore.Mvc;
using Logitun.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

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

        public HomeController(
            ITruckService truckService,
            IMissionService missionService,
            ILocationService locationService,
            ITimeOffService timeOffService,
            ILogger<HomeController> logger)
        {
            _truckService = truckService;
            _missionService = missionService;
            _locationService = locationService;
            _timeOffService = timeOffService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get all missions and filter for SCHEDULED or IN_PROGRESS
                var missionsResult = await _missionService.GetPagedAsync(1, int.MaxValue);
                var activeMissions = missionsResult.Items.Count(m =>
                    m.Status == "SCHEDULED" || m.Status == "IN_PROGRESS");

                // Get all time off requests and filter for PENDING
                var timeOffResult = await _timeOffService.GetPagedAsync(1, int.MaxValue);
                var pendingTimeOffRequests = timeOffResult.Items.Count(r =>
                    r.Status == "PENDING");

                var dashboardData = new
                {
                    Trucks = (await _truckService.GetPagedAsync(1, 1)).TotalItems,
                    Missions = activeMissions,
                    Locations = (await _locationService.GetPagedAsync(1, 1)).TotalItems,
                    TimeOffRequests = pendingTimeOffRequests
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                return View(new { Trucks = 0, Missions = 0, Locations = 0, TimeOffRequests = 0 });
            }
        }
    }
}