using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System.ComponentModel.DataAnnotations;
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
        public string? ErrorMessage { get; set; }

        [BindProperty]
        public PatientDetails Input { get; set; } //Connect to PatientDetails.cs rather than create another InputModel
       
        public IEnumerable<SelectListItem> InsuranceProviderList { get; set; } //for asp-items
        public async Task OnGetAsync()
        {
            InsuranceProviderList = await _context.InsuranceProvider
                .OrderBy(ip => ip.InsuranceProviderName)
                .Select(ip => new SelectListItem
                {
                    Value = ip.InsuranceProviderId.ToString(),
                    Text = ip.InsuranceProviderName
                })
                .ToListAsync(); 
        }

        // Handle POST requests (when the form is submitted)
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Console.WriteLine(Input);
                
                //reload dropdown
                InsuranceProviderList = await _context.InsuranceProvider
                        .OrderBy(ip => ip.InsuranceProviderName)
                        .Select(ip => new SelectListItem
                        {
                            Value = ip.InsuranceProviderId.ToString(),
                            Text = ip.InsuranceProviderName
                        })
                        .ToListAsync();

                if (!ModelState.IsValid)
                {
                    Console.WriteLine(ModelState);
                    return Page();
                }

                // Check if the email already exists
                var email = Input.PatientEmail?.Trim();
                var existingPatient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.PatientEmail.ToLower() == email.ToLower());


                if (existingPatient != null)
                {
                    // Set error message and return to the same page
                    ErrorMessage = "The email address is already in use.";
                    return Page();
                }
                else
                {
                    // --- BEGIN: minimal changes for part 3 + 4 ---
                    //using var tx = await _context.Database.BeginTransactionAsync();

                    // insert patient with EF so we get the PK immediately
                    var patient = new PatientDetails
                    {
                        PatientFirstName = Input.PatientFirstName,
                        PatientLastName = Input.PatientLastName,
                        PatientAge = Input.PatientAge,
                        PatientPhoneNum = Input.PatientPhoneNum,
                        PatientEmail = email,
                        PatientPassword = BCrypt.Net.BCrypt.HashPassword(Input.PatientPassword),
                        InsuranceProviderId = Input.InsuranceProviderId
                    };

                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();// patient.PatientId is now set
                    //var patientId = patient.PatientId;

                    // 2) Insert join rows for specialties (keep your raw SQL)
                    //if (InsuranceProviderList?.Any() == true)
                    //{
                    //    foreach (var ipid in InsuranceProviderList.Distinct())
                    //    {
                    //        await _context.Database.ExecuteSqlRawAsync(
                    //            "INSERT INTO InsuranceProvider (InsuranceProviderId) VALUES (@ipid)",
                    //            new SqlParameter("@ipid", ipid)
                    //        );
                    //    }
                    //}

                    //await tx.CommitAsync();

                    return RedirectToPage("/PatientLogin");
                }
            }catch (SqlException sqlExe)
            {
                Console.WriteLine(sqlExe.Message);
                return RedirectToPage("/PatientLogin");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToPage("/PatientLogin");
            }
        }
    }
}
