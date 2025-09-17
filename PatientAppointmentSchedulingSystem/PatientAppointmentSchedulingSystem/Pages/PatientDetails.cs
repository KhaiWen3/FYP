using System.ComponentModel.DataAnnotations;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class PatientDetails
    {
        [Key]
        public int PatientId {  get; set; }

        [Required]
        public string PatientFirstName { get; set; }

        [Required]
        public string PatientLastName {  get; set; }

        [Required]
        [RegularExpression(@"^\+?[0-9]{9,15}$", ErrorMessage = "Phone number must be between 9 and 15 digits and can start with +")]
        public string PatientPhoneNum { get; set; }

        [Required]
        public int PatientAge { get; set; }

        [Required]
        public string PatientEmail { get; set; }

        [Required]
        public string PatientPassword { get; set; }
        
        //FK
        [Required]
        public int? InsuranceProviderId { get; set; }

        public string? PatientAppointment {  get; set; }

        //during PatientProfile update purpose
        public DateOnly? DateOfBirth {  get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        //public string? InsuranceProvider { get; set; }
        public string? BloodType { get; set; }
        public string? Allergies { get; set; }

        //One patient → one provider; one provider → many patients
        public InsuranceProvider? InsuranceProviders { get; set; }

        //public ICollection<InsuranceProvider> InsuranceProviders { get; set; } = new List<InsuranceProvider>();
    }
}
