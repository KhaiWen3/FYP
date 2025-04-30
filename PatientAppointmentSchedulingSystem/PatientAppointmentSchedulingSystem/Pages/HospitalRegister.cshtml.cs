//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using PatientAppointmentSchedulingSystem.Pages.Data;

//namespace PatientAppointmentSchedulingSystem.Pages
//{
//    public class AdminRegisterModel : PageModel
//    {
//        private readonly HospitalDbContext _context;

//        public AdminRegisterModel (HospitalDbContext context)
//        {
//            _context = context;
//        }
//        public string ErrorMessage { get; set; }
//        [BindProperty]
//        public HospitalDetails Input { get; set; } //Connect to HospitalDetails.cs rather than create another InputModel

//        // Handle POST requests (when the form is submitted)
//        public async Task<IActionResult> OnPostAsync()
//        {
//            if (!ModelState.IsValid)
//            {
//                return Page();
//            }
//            // Check if the email already exists
//            var existingHospital = await _context.Hospital
//                .FirstOrDefaultAsync(h => h.HospitalEmail == Input.HospitalEmail);

//            if (existingHospital != null)
//            {
//                // Set error message and return to the same page
//                ErrorMessage = "The email address is already in use.";
//                return Page();
//            }
//            else
//            {
//                // Proceed with registration if email is not in use
//                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Input.HospitalPassword);

//                var sql = "INSERT INTO Hospital (HospitalName, HospitalDescription, HospitalEstablishedYear, HospitalType, HospitalAdress, PatientPhoneNum) VALUES (@FirstName, @LastName, @Email, @Password, @Age, @PhoneNumber)";

//                var parameters = new[]
//                {
//                new SqlParameter("@FirstName", Input.PatientFirstName),
//                new SqlParameter("@LastName", Input.PatientLastName),
//                new SqlParameter("@Email", Input.PatientEmail),
//                new SqlParameter("@Password", hashedPassword),
//                new SqlParameter("@Age", Input.PatientAge),
//                new SqlParameter("@PhoneNumber", Input.PatientPhoneNum)
//                };

//                await _context.Database.ExecuteSqlRawAsync(sql, parameters);

//                return RedirectToPage("/PatientLogin");
//            }
//        }
//    }
//}
