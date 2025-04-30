using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net;
using System.Numerics;
using System.Security.Claims;
using Microsoft.Extensions.Logging;


namespace PatientAppointmentSchedulingSystem.Pages
{
    public class PatientLoginModel : PageModel
    {
        private readonly PatientDbContext _context;
        //private readonly ILogger<PatientLoginModel> _logger;
        public string ErrorMessage { get; set; }

        public PatientLoginModel(PatientDbContext context)
        {
            _context = context;
            //_logger = logger; //initialize logger
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<PatientDetails> Patients { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Patients = await _context.Patients.ToListAsync();
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

            // Retrieve the student based on the email
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientEmail == Input.Email);

            if (patient == null)
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }

            var passwordMatch = BCrypt.Net.BCrypt.Verify(Input.Password, patient.PatientPassword);

            if (!passwordMatch)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt.");
                return Page();
            }

            HttpContext.Session.SetInt32("PatientId", patient.PatientId);
            HttpContext.Session.SetString("PatientFirstName", patient.PatientFirstName);
            HttpContext.Session.SetString("PatientLastName", patient.PatientLastName);
            HttpContext.Session.SetString("PatientEmail", patient.PatientEmail);
            HttpContext.Session.SetString("PatientFullName", patient.PatientFirstName + " " + patient.PatientLastName);


            // Optional: Debug
            System.Diagnostics.Debug.WriteLine("Logged in PatientId: " + patient.PatientId);

            return RedirectToPage("/PatientHomePage"); // Redirect to doctor dashboard

            // Verify the provided password against the stored hashed password
            //var passwordMatch = BCrypt.Net.BCrypt.Verify(Input.Password, patient.PatientPassword);

            //if (!passwordMatch)
            //{
            //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            //    return Page();
            //}

            //// Redirect to the homepage or another page upon successful login
            //PatientSession.PatientId = patient.PatientId;
            //System.Diagnostics.Debug.WriteLine("Current User : " + PatientSession.PatientId);
            //return RedirectToPage("/PatientHomePage");
        }

        // A method to verify password (assuming hashed password)
        private bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
       


        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }

        //    // Retrieve the student based on the email
        //    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientEmail == Input.Email);

        //    if (patient == null)
        //    {
        //        ErrorMessage = "Invalid login attempt.";
        //        return Page();
        //    }

        //    // Create claims for the authenticated doctor
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, patient.PatientId.ToString()),
        //        new Claim(ClaimTypes.Email, patient.PatientEmail),
        //        new Claim(ClaimTypes.Name, patient.PatientFirstName),
        //        new Claim(ClaimTypes.Name, patient.PatientLastName),
        //        new Claim(ClaimTypes.Role, "Patient")
        //    };

        //    var claimsIdentity = new ClaimsIdentity(
        //        claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //    var authProperties = new AuthenticationProperties
        //    {
        //        // You can configure additional properties here
        //        IsPersistent = true // For "Remember me" functionality
        //    };

        //    // Sign in the patient
        //    await HttpContext.SignInAsync(
        //        CookieAuthenticationDefaults.AuthenticationScheme,
        //        new ClaimsPrincipal(claimsIdentity),
        //        authProperties);

        //    return RedirectToPage("/PatientHomePage"); // Redirect to doctor dashboard

        //    // Verify the provided password against the stored hashed password
        //    var passwordMatch = BCrypt.Net.BCrypt.Verify(Input.Password, patient.PatientPassword);

        //    if (!passwordMatch)
        //    {
        //        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //        return Page();
        //    }

        //    // Redirect to the homepage or another page upon successful login
        //    PatientSession.PatientId = patient.PatientId;
        //    System.Diagnostics.Debug.WriteLine("Current User : " + PatientSession.PatientId);
        //    return RedirectToPage("/PatientHomePage");
        //}

        // A method to verify password (assuming hashed password)
        //private bool VerifyPassword(string hashedPassword, string providedPassword)
        //{
        //    return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        //}
    }
}
