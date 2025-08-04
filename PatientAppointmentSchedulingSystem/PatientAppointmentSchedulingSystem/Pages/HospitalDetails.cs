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
        public string HospitalContactNum { get; set; }

        [Required]
        public int HospitalEstablishedYear { get; set; }

        [Required]
        [RegularExpression(@"^(public|private|university|nonprofit|specialized)$", ErrorMessage = "Invalid hospital type.")]
        public string HospitalType { get; set; }

        [Required]
        public string HospitalAddress { get; set; }

        [Required]
        public string HospitalState { get; set; }

        [Required]
        [FileExtensions(Extensions = "jpg,jpeg,png,gif", ErrorMessage = "Logo must be an image file")]
        public string HospitalLogo { get; set; }

        [Required]
        public string HospitalEmail { get; set; }

        [Required]
        public string HospitalPassword { get; set; }

        [Required]  
        public string HospitalServices { get; set; }
        //emergency or cardiology and so on

        [Required]
        public int HospitalBedCount {  get; set; }
    }
}
