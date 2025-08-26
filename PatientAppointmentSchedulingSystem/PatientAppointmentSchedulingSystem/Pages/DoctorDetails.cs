using System.ComponentModel.DataAnnotations;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class DoctorDetails
    {
        [Key]
        public int DoctorId { get; set; }

        //FK    
        public int ProviderId { get; set; }
        public int SpecialtyId { get; set; }

        [Required]
        public string DoctorFullName { get; set; }

        [Required]
        public string DoctorPhoneNum { get; set; }

        //[Required]
        //public string DoctorSpeciality { get; set; }

        [Required]
        public string DoctorMedicalService { get; set; }

        [Required]
        public string DoctorEmail { get; set; }
        
        [Required]
        public string DoctorPassword { get; set; }

        [Required]
        public string DoctorRoomNum { get; set; }

        [Required]
        public string DoctorLanguageSpoken { get; set; }

        public string? DoctorPhoto { get; set; }

        //public List<AvailabilitySlots> AvailabilitySlots { get; set; }
    }
}
