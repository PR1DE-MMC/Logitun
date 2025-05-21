using System;
using System.ComponentModel.DataAnnotations;

namespace Logitun.Shared.DTOs
{
    public class TimeOffRequestDto
    {
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Please select a driver")]
        public Guid? DriverId { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(PENDING|APPROVED|REJECTED)$", ErrorMessage = "Status must be PENDING, APPROVED, or REJECTED")]
        public string Status { get; set; }
    }
}
