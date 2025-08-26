namespace PatientAppointmentSchedulingSystem.Pages
{
    public class Specialty
    {
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }

        // Navigation property
        public ICollection<ProviderSpecialty> ProviderSpecialties { get; set; } = new List<ProviderSpecialty>();
    }
}
