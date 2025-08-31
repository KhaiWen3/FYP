using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class AvailabilitySlots
    {
        [Key]
        public int SlotId { get; set; }

        [Required]
        public DateTime AptStartTime { get; set; }

        [Required]
        public DateTime AptEndTime { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string AppointmentType { get; set; } //video visit @ in-person visit

        [Required]
        public int AppointmentStatus { get; set; } = 0;

        // FK for Doctor
        [Required]
        public int DoctorId { get; set; }

        [ValidateNever]
        public DoctorDetails Doctor { get; set; }

        //[ForeignKey(nameof(DoctorId))]
        //public DoctorDetails Doctor { get; set; }

        // Foreign key for Patient (nullable, as a slot may not be booked yet)
        public int? PatientId { get; set; }

    }
}
