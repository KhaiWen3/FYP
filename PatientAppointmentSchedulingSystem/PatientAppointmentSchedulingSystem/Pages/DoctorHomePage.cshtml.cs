using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class DoctorHomePageModel : PageModel
    {
        public void OnGet()
        {
            // Get doctor's email
            var doctorEmail = User.FindFirstValue(ClaimTypes.Email);

            // Get doctor's name
            var doctorName = User.FindFirstValue(ClaimTypes.Name);

            // Check if user is a doctor
            var isDoctor = User.IsInRole("Doctor");

            // Get doctor's ID
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
