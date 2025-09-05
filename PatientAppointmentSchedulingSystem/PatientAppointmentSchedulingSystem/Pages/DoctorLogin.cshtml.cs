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
            //else
            //{
            //    HttpContext.Session.SetInt32("DoctorId", doctor.DoctorId); // for this HttpContext u had been setup at Program.cs
            //    //So when u login means that u will get the doctor id from the db
            //    // so the parameter u pass to the session is ("Session Name",value) <-- for the session name u need to remmeber u need to reuse
            //}

            // 2) Store everything you need in Session (not claims)
            //HttpContext.Session.SetInt32("DoctorId", doctor.DoctorId);
            //HttpContext.Session.SetString("DoctorEmail", doctor.DoctorEmail);
            //HttpContext.Session.SetString("DoctorName", doctor.DoctorFullName);
            //HttpContext.Session.SetString("DoctorRole", "Doctor");

            // 3) (Optional) Sign in with an empty identity so [Authorize] works
            //    No user data in claims — you will read from Session everywhere.
            //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            //var principal = new ClaimsPrincipal(identity);

            // Create claims so the Add Slots page knows who the doctor is
            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.NameIdentifier, doctor.DoctorId.ToString()),
            //    new Claim(ClaimTypes.Email, doctor.DoctorEmail),
            //    new Claim(ClaimTypes.Name, doctor.DoctorFullName),
            //    new Claim(ClaimTypes.Role, "Doctor")
            //};

            //var Identity = new ClaimsIdentity(
            //    claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //var principal = new ClaimsPrincipal(Identity);

            //var authProperties = new AuthenticationProperties
            //{
            //    // You can configure additional properties here
            //    IsPersistent = true // For "Remember me" functionality
            //};

            // Important - Sign in the doctor
            //await HttpContext.SignInAsync(
            //    CookieAuthenticationDefaults.AuthenticationScheme,
            //    principal,  
            //    new AuthenticationProperties { IsPersistent = true });


            //return RedirectToPage("/DoctorHomePage"); // Redirect to doctor dashboard
        }

        //private bool VerifyPassword(string enteredPassword, string storedHash)
        //{
        //    // Implement your password verification logic here
        //    // This is a simple example - use proper password hashing in production
        //    return enteredPassword == storedHash; // Replace with proper hashing
        //}
    }
}
