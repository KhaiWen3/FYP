namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderSpecialty
    {
        public int ProviderId { get; set; }
        public int SpecialtyId { get; set; }

        // Navigation properties
        public ProviderDetails Provider { get; set; } = default!;
        public Specialty Specialty { get; set; } = default!;
    }
}
