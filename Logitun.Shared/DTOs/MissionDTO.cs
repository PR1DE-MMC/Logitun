using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logitun.Shared.DTOs
{
    public class MissionDto
    {
        public int MissionId { get; set; }
        public int? TruckId { get; set; }
        public Guid? DriverId { get; set; }
        public int? OriginLocationId { get; set; }
        public int? DestinationLocationId { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime? EndDatetime { get; set; }
        public decimal? DistanceKm { get; set; }
        public decimal? PayloadWeight { get; set; }
        public string Status { get; set; } = "SCHEDULED";
    }

}
