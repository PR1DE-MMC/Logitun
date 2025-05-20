using Microsoft.AspNetCore.Mvc;
using Logitun.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Logitun.Mvc.Controllers
{
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
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var dashboardData = new
                {
                    Trucks = (await _truckService.GetPagedAsync(1, 1)).TotalItems,
                    Missions = (await _missionService.GetPagedAsync(1, 1)).TotalItems,
                    Locations = (await _locationService.GetPagedAsync(1, 1)).TotalItems,
                    TimeOffRequests = (await _timeOffService.GetPagedAsync(1, 1)).TotalItems
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