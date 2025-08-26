using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderRegisterModel : PageModel
    {
        private readonly ProviderDbContext _context;

        public ProviderRegisterModel(ProviderDbContext context)
        {
            _context = context;
        }
        public string? ErrorMessage { get; set; }

        [BindProperty]
        public ProviderDetails Input { get; set; } //Connect to HospitalDetails.cs rather than create another InputModel

        // 2) List of specialties to display as checkboxes
        public List<Specialty> Specialties { get; set; } = new();

        // 3) IDs of specialties selected by the provider via checkboxes
        [BindProperty]
        public List<int> SelectedSpecialtyIds { get; set; } = new();
        public async Task OnGet()
        {
            Specialties = await _context.Specialty
                .OrderBy(s=> s.SpecialtyName)
                .ToListAsync();

        }

        //public void OnGet()
        //{
        //}

        
        // Handle POST requests (when the form is submitted)
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Console.WriteLine(Input);

                // Reload specials for redisplay if Modelstate fails
                Specialties = await _context.Specialty.OrderBy(s => s.SpecialtyName).ToListAsync();

                if (!ModelState.IsValid)
                {
                    Console.WriteLine(ModelState);
                    return Page();
                }

                // Enforce BedCount rule (DB also has CHECK)
                if (Input.ProviderType == "Hospital" && Input.BedCount is null)
                {
                    ModelState.AddModelError("Input.BedCount", "Bed count is required for hospitals.");
                    return Page();
                }
                if (Input.ProviderType == "Clinic") Input.BedCount = null;

                // Check if the email already exists
                var existingProvider = await _context.Provider
                    .FirstOrDefaultAsync(p => p.Email == Input.Email);

                if (existingProvider != null)
                {
                    // Set error message and return to the same page
                    ErrorMessage = "The email address is already in use.";
                    return Page();
                }
                else
                {
                    // --- BEGIN: minimal changes for part 3 + 4 ---
                    using var tx = await _context.Database.BeginTransactionAsync();

                    // Proceed with registration if email is not in use
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Input.Password);

                    var sql = @"INSERT INTO Provider 
                (
                    ProviderType, Name, Description, ContactNum, Email,
                    Password, Address, BedCount, OwnershipType, State, Logo
                )
                VALUES
                (
                    @ProviderType, @Name, @Description, @ContactNum, @Email,
                    @Password, @Address, @BedCount, @OwnershipType, @State, @Logo
                )";


                    var parameters = new[]
                    {
                    new SqlParameter("@ProviderType",  Input.ProviderType),
                    new SqlParameter("@Name",          Input.Name),
                    new SqlParameter("@Description",   (object?)Input.Description ?? DBNull.Value),
                    new SqlParameter("@ContactNum",    Input.ContactNum),
                    new SqlParameter("@Email",         Input.Email),
                    new SqlParameter("@Password",  hashedPassword),
                    new SqlParameter("@Address",       Input.Address),
                    new SqlParameter("@BedCount",      (object?)Input.BedCount ?? DBNull.Value),
                    new SqlParameter("@OwnershipType", Input.OwnershipType),
                    new SqlParameter("@State",         Input.State),
                    new SqlParameter("@Logo",          (object?)Input.Logo ?? DBNull.Value),
                };
                    //save provider first
                    await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                    // Get ProviderId of the row we just inserted (email is unique, we checked above)
                    var providerId = await _context.Provider
                        .Where(p => p.Email == Input.Email)
                        .Select(p => p.ProviderId)
                        .FirstAsync();

                    // Gather selected specialties from the posted form WITHOUT adding new bound properties
                    // Your markup uses two names ("HospitalServices" and one "hospital.departments"), so read both.
                    var selected1 = Request.Form["HospitalServices"].ToArray();
                    var selected2 = Request.Form["hospital.departments"].ToArray(); // just in case the first checkbox is used
                    var selectedAll = selected1.Concat(selected2)
                                               .Select(v => v?.ToString()?.Trim() ?? "")
                                               .Where(v => !string.IsNullOrWhiteSpace(v))
                                               .Select(v => v.ToLowerInvariant())
                                               .Distinct()
                                               .ToList();

                    if (selectedAll.Count > 0)
                    {
                        // Map posted names (e.g., "cardiology") to SpecialtyIds in DB
                        // Assumes Specialty.SpecialtyName stores names like "cardiology", "orthopedics", etc.
                        // If your names are Title Case, the ToLower() normalization below will still match.
                        var specialtyIds = await _context.Specialty
                            .Where(s => selectedAll.Contains(s.SpecialtyName.ToLower()))
                            .Select(s => s.SpecialtyId)
                            .ToListAsync();

                        // Insert join rows
                        foreach (var sid in specialtyIds.Distinct())
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                                "INSERT INTO ProviderSpecialty (ProviderId, SpecialtyId) VALUES (@pid, @sid)",
                                new SqlParameter("@pid", providerId),
                                new SqlParameter("@sid", sid)
                            );
                        }
                    }

                    await tx.CommitAsync();
                    // --- END: minimal changes for part 3 + 4 ---

                    return RedirectToPage("/ProviderLogin");
                }
            }catch(SqlException sqlExe)
            {
                Console.WriteLine(sqlExe.Message);
                return RedirectToPage("/ProviderLogin");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToPage("/ProviderLogin");
            }
        }
    }
}
