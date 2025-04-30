using System.ComponentModel.DataAnnotations;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class DoctorDetails
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        public string DoctorName { get; set; }

        [Required]
        public string DoctorPhoneNum { get; set; }

        [Required]
        public string DoctorSpeciality { get; set; }

        [Required]
        public string DoctorMedicalService { get; set; }

        [Required]
        public string DoctorEmail { get; set; }
        
        [Required]
        public string DoctorPassword { get; set; }

        //public List<AvailabilitySlots> AvailabilitySlots { get; set; }
    }
}
