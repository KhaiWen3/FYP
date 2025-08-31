using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class DoctorHomePageModel : PageModel
    {
        // Expose to the .cshtml if you want to display them
        public int? DoctorId { get; private set; }
        public string? DoctorEmail { get; private set; }
        public string? DoctorName { get; private set; }
        public bool IsDoctor { get; private set; }

        public IActionResult OnGet()
        {
            //// Get doctor's email
            //var doctorEmail = User.FindFirstValue(ClaimTypes.Email);

            //// Get doctor's name
            //var doctorName = User.FindFirstValue(ClaimTypes.Name);

            //// Check if user is a doctor
            //var isDoctor = User.IsInRole("Doctor");

            //// Get doctor's ID
            //var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Read from Session
            DoctorId = HttpContext.Session.GetInt32("DoctorId");
            DoctorEmail = HttpContext.Session.GetString("DoctorEmail");
            DoctorName = HttpContext.Session.GetString("DoctorName");
            IsDoctor = string.Equals(
                            HttpContext.Session.GetString("DoctorRole"),
                            "Doctor",
                            StringComparison.OrdinalIgnoreCase);

            // If session expired or never set, send them to login
            if (DoctorId is null || !IsDoctor)
            {
                return RedirectToPage("/DoctorLogin");
            }

            return Page();
        }
    }
}
