using System.ComponentModel.DataAnnotations;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class InsuranceProvider
    {
        [Key]
        public int InsuranceProviderId { get; set; }

        [Required]
        public string InsuranceProviderName { get; set; }

        public string? InsuranceProviderDesc { get; set; }

        [Required]
        public string InsuranceProviderHotline { get; set; }

        [Required]
        public string InsuranceProviderWebsite { get; set; }

        public ICollection<PatientDetails> Patients { get; set; } = new List<PatientDetails>();
    }
}
