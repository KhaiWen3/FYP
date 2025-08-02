using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class DoctorLoginModel : PageModel
    {
        private readonly DoctorDbContext _context;
        public string ErrorMessage { get; set; }

        public DoctorLoginModel(DoctorDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public IList<DoctorDetails> Doctor { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Doctor = await _context.Doctor.ToListAsync();
            return Page();
        }

        public class InputModel
        {
            [Required]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validate doctor credentials
            var doctor = _context.Doctor.FirstOrDefault(d => d.DoctorEmail == Input.Email);

            if (doctor == null)
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }


            // Create claims for the authenticated doctor
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, doctor.DoctorId.ToString()),
                new Claim(ClaimTypes.Email, doctor.DoctorEmail),
                new Claim(ClaimTypes.Name, doctor.DoctorFullName),
                new Claim("Specialization", doctor.DoctorSpeciality), // Custom claim
                new Claim ("MedicalService", doctor.DoctorMedicalService),
                new Claim(ClaimTypes.Role, "Doctor")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                // You can configure additional properties here
                IsPersistent = true // For "Remember me" functionality
            };

            // Important - Sign in the doctor
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);


            return RedirectToPage("/DoctorHomePage"); // Redirect to doctor dashboard
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Implement your password verification logic here
            // This is a simple example - use proper password hashing in production
            return enteredPassword == storedHash; // Replace with proper hashing
        }
    }
}
