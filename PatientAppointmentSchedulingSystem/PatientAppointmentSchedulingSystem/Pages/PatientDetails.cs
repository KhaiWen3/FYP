using System.ComponentModel.DataAnnotations;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class PatientDetails
    {
        public int PatientId {  get; set; }

        [Required]
        public string PatientFirstName { get; set; }

        [Required]
        public string PatientLastName {  get; set; }

        [Required]
        public string PatientPhoneNum { get; set; }

        [Required]
        public int PatientAge { get; set; }

        [Required]
        public string PatientEmail { get; set; }

        [Required]
        public string PatientPassword { get; set; }
        
    }
}
