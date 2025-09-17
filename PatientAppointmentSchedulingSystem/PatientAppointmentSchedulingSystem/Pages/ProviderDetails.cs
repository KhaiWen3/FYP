using System.ComponentModel.DataAnnotations;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderDetails
    {
        [Key]
        public int ProviderId { get; set; }
        [Required]
        public string ProviderType { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [RegularExpression(@"^(Private|Public|Government|NonProfit)$",
            ErrorMessage = "Invalid ownership type.")]
        public string OwnershipType { get; set; }


        [Required]
        public string Address { get; set; }

        [Required]
        public string State { get; set; }

        public string? ProviderLogo { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [RegularExpression(@"^\+?[0-9]{9,15}$", ErrorMessage = "Phone number must be between 9 and 15 digits and can start with +")]
        public string ContactNum { get; set; }

        public int? BedCount {  get; set; }

        public ICollection<ProviderSpecialty> ProviderSpecialties { get; set; } = new List<ProviderSpecialty>();
        public ICollection<DoctorDetails> Doctors { get; set; } = new List<DoctorDetails>();


    }
}
