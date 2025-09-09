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

        public string? Logo { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ContactNum { get; set; }

        public int? BedCount {  get; set; }

        public ICollection<ProviderSpecialty> ProviderSpecialties { get; set; } = new List<ProviderSpecialty>();

    }
}
