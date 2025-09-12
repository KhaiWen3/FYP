using System.ComponentModel.DataAnnotations;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderSpecialty
    {
        [Key]
        public int ProviderId { get; set; }
        public int SpecialtyId { get; set; }

        // Navigation properties
        public ProviderDetails Provider { get; set; } = null!;
        public Specialty Specialty { get; set; } = null!;
    }
}
