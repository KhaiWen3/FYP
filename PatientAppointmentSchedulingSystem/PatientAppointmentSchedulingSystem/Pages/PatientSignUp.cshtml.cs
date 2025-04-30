using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net;
using System.Data;


namespace PatientAppointmentSchedulingSystem.Pages
{
    public class PatientSignUpModel : PageModel
    {
        private readonly PatientDbContext _context;

        public PatientSignUpModel(PatientDbContext context)
        {
            _context = context;
        }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public PatientDetails Input { get; set; } //Connect to PatientDetails.cs rather than create another InputModel
        
        public void OnGet()
        {
        }

        // Handle POST requests (when the form is submitted)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Check if the email already exists
            var existingPatient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientEmail == Input.PatientEmail);

            if (existingPatient != null)
            {
                // Set error message and return to the same page
                ErrorMessage = "The email address is already in use.";
                return Page();
            }
            else
            {
                // Proceed with registration if email is not in use
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Input.PatientPassword);

                var sql = "INSERT INTO Patients (PatientFirstName, PatientLastName, PatientEmail, PatientPassword, PatientAge, PatientPhoneNum) VALUES (@FirstName, @LastName, @Email, @Password, @Age, @PhoneNumber)";

                var parameters = new[]
                {
                new SqlParameter("@FirstName", Input.PatientFirstName),
                new SqlParameter("@LastName", Input.PatientLastName),
                new SqlParameter("@Email", Input.PatientEmail),
                new SqlParameter("@Password", hashedPassword),
                new SqlParameter("@Age", Input.PatientAge),
                new SqlParameter("@PhoneNumber", Input.PatientPhoneNum)
                };

                await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                return RedirectToPage("/PatientLogin");
            }
        }
    }
}
