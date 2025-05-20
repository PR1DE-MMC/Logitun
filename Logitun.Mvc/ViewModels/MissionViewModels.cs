using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Logitun.Shared.DTOs;
using Logitun.Shared.Models;

namespace Logitun.Mvc.ViewModels
{
    public class MissionCreateViewModel
    {
        public int? TruckId { get; set; }
        public Guid? DriverId { get; set; }
        public int? OriginLocationId { get; set; }
        public int? DestinationLocationId { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime? EndDatetime { get; set; }
        public decimal? DistanceKm { get; set; }
        public decimal? PayloadWeight { get; set; }
        public string Status { get; set; } = "SCHEDULED";
        
        public List<TruckDto> AvailableTrucks { get; set; } = new();
        public List<UserDto> AvailableDrivers { get; set; } = new();
        public List<LocationDto> AvailableLocations { get; set; } = new();

        public MissionDto Mission
        {
            get => new()
            {
                TruckId = TruckId,
                DriverId = DriverId,
                OriginLocationId = OriginLocationId,
                DestinationLocationId = DestinationLocationId,
                StartDatetime = StartDatetime,
                EndDatetime = EndDatetime,
                DistanceKm = DistanceKm,
                PayloadWeight = PayloadWeight,
                Status = Status
            };
            set
            {
                if (value != null)
                {
                    TruckId = value.TruckId;
                    DriverId = value.DriverId;
                    OriginLocationId = value.OriginLocationId;
                    DestinationLocationId = value.DestinationLocationId;
                    StartDatetime = value.StartDatetime;
                    EndDatetime = value.EndDatetime;
                    DistanceKm = value.DistanceKm;
                    PayloadWeight = value.PayloadWeight;
                    Status = value.Status;
                }
            }
        }
    }

    public class MissionEditViewModel
    {
        public MissionDto Mission { get; set; } = new();
        
        public List<TruckDto> AvailableTrucks { get; set; } = new();
        public List<UserDto> AvailableDrivers { get; set; } = new();
        public List<LocationDto> AvailableLocations { get; set; } = new();
    }
} 