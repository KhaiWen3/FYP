namespace PatientAppointmentSchedulingSystem.Pages
{
    public class Specialty
    {
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }

        // inverse nav so .WithMany(s => s.Doctors) compiles
        public ICollection<DoctorDetails> Doctors { get; set; } = new List<DoctorDetails>();


        // Navigation property
        public ICollection<ProviderSpecialty> ProviderSpecialties { get; set; } = new List<ProviderSpecialty>();
    }
}
