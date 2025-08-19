using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class HospitalRegisterModel : PageModel
    {
        private readonly HospitalDbContext _context;

        public HospitalRegisterModel(HospitalDbContext context)
        {
            _context = context;
        }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public HospitalDetails Input { get; set; } //Connect to HospitalDetails.cs rather than create another InputModel

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
            var existingHospital = await _context.Hospital
                .FirstOrDefaultAsync(h => h.HospitalEmail == Input.HospitalEmail);

            if (existingHospital != null)
            {
                // Set error message and return to the same page
                ErrorMessage = "The email address is already in use.";
                return Page();
            }
            else
            {
                // Proceed with registration if email is not in use
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Input.HospitalPassword);

                var sql = @"INSERT INTO Hospital 
                (
                    HospitalName, 
                    HospitalDesc, 
                    HospitalContactNum, 
                    HospitalEmail, 
                    HospitalPassword, 
                    HospitalAddress, 
                    HospitalBedCount, 
                    HospitalType, 
                    HospitalState, 
                    HospitalLogo, 
                    HospitalServices, 
                    HospitalEstablishedYear
                ) 
                VALUES 
                (
                    @HospitalName, 
                    @HospitalDesc, 
                    @HospitalContactNum, 
                    @HospitalEmail, 
                    @HospitalPassword, 
                    @HospitalAddress, 
                    @HospitalBedCount, 
                    @HospitalType, 
                    @HospitalState, 
                    @HospitalLogo, 
                    @HospitalServices, 
                    @HospitalEstablishedYear
                )";


                var parameters = new[]
                {
                new SqlParameter("@HospitalName", Input.HospitalName),
                new SqlParameter("@HospitalDesc", Input.HospitalDescription),
                new SqlParameter("@HospitalContactNum", Input.HospitalContactNum),
                new SqlParameter("@HospitalEmail", Input.HospitalEmail),
                new SqlParameter("@HospitalPassword", hashedPassword),
                new SqlParameter("@HospitalAddress", Input.HospitalAddress),
                new SqlParameter("@HospitalBedCount", Input.HospitalBedCount),
                new SqlParameter("@HospitalType", Input.HospitalType),
                new SqlParameter("@HospitalState", Input.HospitalState),
                new SqlParameter("@HospitalLogo", Input.HospitalLogo),
                new SqlParameter("@HospitalServices", Input.HospitalServices),
                new SqlParameter("@HospitalEstablishedYear", Input.HospitalEstablishedYear)
                };

                await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                return RedirectToPage("/DoctorLogin");
            }
        }
    }
}
