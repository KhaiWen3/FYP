using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System.ComponentModel.DataAnnotations;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderLoginModel : PageModel
    {
        private readonly ProviderDbContext _context;
        private readonly EmailService _emailService;
        public string ErrorMessage { get; set; }

        public ProviderLoginModel(ProviderDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<ProviderDetails> Provider { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Provider = await _context.Provider.ToListAsync();
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

            // Retrieve the hospital based on the email
            var provider = await _context.Provider
                .FirstOrDefaultAsync(p => p.Email == Input.Email);

            if (provider == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
            else
            {
                var passwordMatch = BCrypt.Net.BCrypt.Verify(Input.Password, provider.Password);

                if (!passwordMatch)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
                else
                {
                    HttpContext.Session.SetInt32("ProviderId", provider.ProviderId);
                    return RedirectToPage("/ProviderHomePage");
                }
            }

            // Verify the provided password against the stored hashed password
            

            // Redirect to the homepage or another page upon successful login
            //ProviderSession.ProviderId = provider.ProviderId;
            //System.Diagnostics.Debug.WriteLine("Current User : " + ProviderSession.ProviderId);
           
        }

        public async Task<IActionResult> OnPostForgotPasswordAsync(string resetEmail)
        {
            if (string.IsNullOrEmpty(resetEmail))
            {
                TempData["AlertMsg"] = "Please enter an email address.";
                return Page();
            }

            // Check if doctor exists
            var provider = await _context.Provider.FirstOrDefaultAsync(p => p.Email == resetEmail);
            if (provider == null)
            {
                TempData["AlertMsg"] = "Email not found in system.";
                return Page();
            }

            var randomPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(randomPassword);

            provider.Password = hashedPassword;

            await _context.SaveChangesAsync();

            string subject = "Reset Your Account Password";
            string body = $@"
                <p>Hello Provider {provider.Name},</p>
                <p>Your password has been reset. Here is your new password:</p>
                <h3>{randomPassword}</h3>
                <p>If you does not request for new password, kindly ignore it.</p>
                <p>Regards,<br/>MediBook Team</p>";

            await _emailService.SendEmailAsync(provider.Email, subject, body);

            TempData["AlertMsg"] = "Password reset had sent to " + resetEmail;

            return RedirectToPage("/DoctorLogin");
        }

        // A method to verify password (assuming hashed password)
        private bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
    }
}
