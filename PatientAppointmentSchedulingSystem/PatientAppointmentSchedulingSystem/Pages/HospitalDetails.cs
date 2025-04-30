using System.ComponentModel.DataAnnotations;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class HospitalDetails
    {
        public int HospitalId { get; set; }
        
        [Required]
        public string HospitalName { get; set; }

        [Required]
        public string HospitalDescription { get; set; }

        [Required]
        public int HospitalEstablishedYear { get; set; }

        [Required]
        public string HospitalType {get; set; }

        [Required]
        public string HospitalAdress { get; set; }

        [Required]
        [Url(ErrorMessage = "Must be a valid URL to the logo image")]
        [RegularExpression(@"\.(png|jpg|jpeg|gif)$",
        ErrorMessage = "Logo must be a PNG, JPG, JPEG, or GIF image")]
        public string HospitalLogo { get; set; }

        [Required]
        public string HospitalPrimaryContactFirstName { get; set; }

        [Required]
        public string HospitalPrimaryContactLastName { get; set; }

        [Required]
        public string HospitalPrimaryContactPosition { get; set; }

        [Required]
        public string HospitalPrimaryContactPhoneNum { get; set; }

        [Required]
        public string HospitalEmail { get; set; }

        [Required]
        public string HospitalPassword { get; set; }

        [Required]  
        public string HospitalServices { get; set; }

        [Required]
        public int HospitalBed {  get; set; }
    }
}
