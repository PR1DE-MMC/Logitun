using Logitun.Core.Interfaces;
using Logitun.Shared.DTOs;
using Bogus;
using Microsoft.Extensions.Logging;
using Logitun.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Logitun.Core.Entities;

namespace Logitun.Infrastructure.Services
{
    public class MockDataService : IMockDataService
    {
        private readonly ITruckService _truckService;
        private readonly IMissionService _missionService;
        private readonly ILocationService _locationService;
        private readonly ITimeOffService _timeOffService;
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MockDataService> _logger;

        public MockDataService(
            ITruckService truckService,
            IMissionService missionService,
            ILocationService locationService,
            ITimeOffService timeOffService,
            IAuthService authService,
            ApplicationDbContext context,
            ILogger<MockDataService> logger)
        {
            _truckService = truckService;
            _missionService = missionService;
            _locationService = locationService;
            _timeOffService = timeOffService;
            _authService = authService;
            _context = context;
            _logger = logger;
        }

        public async Task GenerateMockDataAsync()
        {
            try
            {
                // Clean up existing data except admin user
                await CleanupDatabaseAsync();

                // Generate mock drivers
                var driverFaker = new Faker<AuthRegisterRequest>()
                    .RuleFor(u => u.Email, f => f.Internet.Email())
                    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                    .RuleFor(u => u.LastName, f => f.Name.LastName())
                    .RuleFor(u => u.Login, f => f.Internet.Email())
                    .RuleFor(u => u.Password, f => "Password123!")
                    .RuleFor(u => u.PhoneNumber, f => f.Random.Replace("########"))
                    .RuleFor(u => u.Cin, f => f.Random.Replace("########"))
                    .RuleFor(u => u.Birthdate, f => f.Date.Past(50, DateTime.UtcNow.AddYears(-18)).ToUniversalTime());

                var drivers = driverFaker.Generate(10);
                foreach (var driver in drivers)
                {
                    await _authService.RegisterAsync(driver);
                }

                // Generate mock trucks
                var truckFaker = new Faker<TruckDto>()
                    .RuleFor(t => t.PlateNumber, f => f.Random.Replace("???-###"))
                    .RuleFor(t => t.Model, f => f.Vehicle.Model())
                    .RuleFor(t => t.CapacityKg, f => f.Random.Number(1000, 5000))
                    .RuleFor(t => t.FuelType, f => f.PickRandom(new[] { "DIESEL", "PETROL", "ELECTRIC" }));

                var trucks = truckFaker.Generate(5);
                foreach (var truck in trucks)
                {
                    await _truckService.CreateAsync(truck);
                }

                // Generate mock locations
                var locationFaker = new Faker<LocationDto>()
                    .RuleFor(l => l.Name, f => f.Company.CompanyName())
                    .RuleFor(l => l.Address, f => f.Address.StreetAddress());

                var locations = locationFaker.Generate(10);
                foreach (var location in locations)
                {
                    await _locationService.CreateAsync(location);
                }

                // Get all trucks and locations for mission generation
                var allTrucks = (await _truckService.GetPagedAsync(1, 100)).Items;
                var allLocations = (await _locationService.GetPagedAsync(1, 100)).Items;
                var allDrivers = await _authService.GetAvailableDriversAsync();

                // Generate mock missions
                var missionFaker = new Faker<MissionDto>()
                    .RuleFor(m => m.TruckId, f => f.PickRandom(allTrucks).TruckId)
                    .RuleFor(m => m.DriverId, f => f.PickRandom(allDrivers).UserId)
                    .RuleFor(m => m.OriginLocationId, f => f.PickRandom(allLocations).LocationId)
                    .RuleFor(m => m.DestinationLocationId, f => f.PickRandom(allLocations).LocationId)
                    .RuleFor(m => m.StartDatetime, f => f.Date.Future().ToUniversalTime())
                    .RuleFor(m => m.EndDatetime, (f, m) => m.StartDatetime.AddHours(f.Random.Number(1, 24)))
                    .RuleFor(m => m.DistanceKm, f => f.Random.Number(50, 500))
                    .RuleFor(m => m.PayloadWeight, f => f.Random.Number(100, 2000))
                    .RuleFor(m => m.Status, f => f.PickRandom(new[] { "SCHEDULED", "IN_PROGRESS", "COMPLETED", "CANCELLED" }));

                var missions = missionFaker.Generate(20);
                foreach (var mission in missions)
                {
                    await _missionService.CreateAsync(mission);
                }

                // Generate mock time off requests
                var timeOffFaker = new Faker<TimeOffRequestDto>()
                    .RuleFor(t => t.DriverId, f => f.PickRandom(allDrivers).UserId)
                    .RuleFor(t => t.StartDate, f => f.Date.Future().ToUniversalTime())
                    .RuleFor(t => t.EndDate, (f, t) => t.StartDate.AddDays(f.Random.Number(1, 14)))
                    .RuleFor(t => t.Status, f => f.PickRandom(new[] { "PENDING", "APPROVED", "REJECTED" }));

                var timeOffRequests = timeOffFaker.Generate(10);
                foreach (var request in timeOffRequests)
                {
                    await _timeOffService.CreateAsync(request);
                }

                _logger.LogInformation("Mock data generated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating mock data");
                throw;
            }
        }

        private async Task CleanupDatabaseAsync()
        {
            try
            {
                // Get admin user's ID
                var adminUser = await _context.Set<User>()
                    .Include(u => u.Credentials)
                    .ThenInclude(c => c.Roles)
                    .FirstOrDefaultAsync(u => u.Credentials.Roles.Any(r => r.Name == "ROLE_ADMIN"));

                if (adminUser == null)
                {
                    throw new Exception("Admin user not found");
                }

                // Delete all data except admin user
                await _context.Database.ExecuteSqlRawAsync(@"
                    DELETE FROM time_off_requests;
                    DELETE FROM missions;
                    DELETE FROM trucks;
                    DELETE FROM locations;
                    DELETE FROM auth_user WHERE user_id != {0};
                    DELETE FROM auth_credentials WHERE credential_id != {1};
                    DELETE FROM auth_information WHERE information_id != {2};
                ", adminUser.UserId, adminUser.CredentialsId, adminUser.InformationId);

                _logger.LogInformation("Database cleaned up successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up database");
                throw;
            }
        }
    }
}