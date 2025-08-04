using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System.ComponentModel.DataAnnotations;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class AdminLoginModel : PageModel
    {
        private readonly HospitalDbContext _context;
        public string ErrorMessage { get; set; }

        public AdminLoginModel(HospitalDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<HospitalDetails> Hospital { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Hospital = await _context.Hospital.ToListAsync();
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
            var hospital = await _context.Hospital
                .FirstOrDefaultAsync(h => h.HospitalEmail == Input.Email);

            if (hospital == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // Verify the provided password against the stored hashed password
            var passwordMatch = BCrypt.Net.BCrypt.Verify(Input.Password, hospital.HospitalPassword);

            if (!passwordMatch)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // Redirect to the homepage or another page upon successful login
            HospitalSession.HospitalId = hospital.HospitalId;
            System.Diagnostics.Debug.WriteLine("Current User : " + PatientSession.PatientId);
            return RedirectToPage("/PatientHomePage");
        }

        // A method to verify password (assuming hashed password)
        private bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
    }
}
