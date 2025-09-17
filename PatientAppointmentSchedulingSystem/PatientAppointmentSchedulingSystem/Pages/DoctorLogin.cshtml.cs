using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Principal;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class DoctorLoginModel : PageModel
    {
        private readonly DoctorDbContext _context;
        private readonly EmailService _emailService;
        public string ErrorMessage { get; set; }

        public DoctorLoginModel(DoctorDbContext context,EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public IList<DoctorDetails> Doctor { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Doctor = await _context.Doctor.ToListAsync();
            return Page();
        }
        //hey changest
        //try again
        public class InputModel
        {
            [Required]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            

            // Validate doctor credentials
            var doctor = _context.Doctor
                .FirstOrDefault(d => d.DoctorEmail == Input.Email);

            if (doctor == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }
            else
            {
                var passwordMatch = BCrypt.Net.BCrypt.Verify(Input.Password, doctor.DoctorPassword);

                if (!passwordMatch)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
                else
                {
                    HttpContext.Session.SetInt32("DoctorId", doctor.DoctorId);
                    return RedirectToPage("/DoctorHomePage");
                }
            }
        }

        public async Task<IActionResult> OnPostForgotPasswordAsync(string resetEmail)
        {
            if (string.IsNullOrEmpty(resetEmail))
            {
                TempData["AlertMsg"] = "Please enter an email address.";
                return Page();
            }

            // Check if doctor exists
            var doctor = await _context.Doctor.FirstOrDefaultAsync(d => d.DoctorEmail == resetEmail);
            if (doctor == null)
            {
                TempData["AlertMsg"] = "Email not found in system.";
                return Page();
            }

            var randomPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(randomPassword);

            doctor.DoctorPassword = hashedPassword;

            await _context.SaveChangesAsync();

            string subject = "Reset Your Account Password";
            string body = $@"
                <p>Hello Doctor {doctor.DoctorFullName},</p>
                <p>Your password has been reset. Here is your new password:</p>
                <h3>{randomPassword}</h3>
                <p>If you does not request for new password, kindly ignore it.</p>
                <p>Regards,<br/>MediBook Team</p>";

            await _emailService.SendEmailAsync(doctor.DoctorEmail,subject,body);

            TempData["AlertMsg"] = "Password reset had sent to " + resetEmail;

            return RedirectToPage("/DoctorLogin");
        }
    }
}
