using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System.Security.Claims;
using System.Security.Cryptography;

//namespace PatientAppointmentSchedulingSystem.Pages
//{
//    public class ProviderAddDoctorModel : PageModel
//    {
//        private readonly ProviderDbContext _context;
//        public ProviderAddDoctorModel(ProviderDbContext context) => _context = context;

//        public List<Specialty> Specialties { get; set; } = new();
//        public string? SuccessMessage { get; set; }

//        [BindProperty]
//        public DoctorDetails Input { get; set; } = new();

//        public async Task OnGetAsync()
//        {
//            Specialties = await _context.Specialty.OrderBy(s => s.SpecialtyName).ToListAsync();
//        }
//        public async Task<IActionResult> OnPostAsync()
//        {
//            Specialties = await _context.Specialty.OrderBy(s => s.SpecialtyName).ToListAsync();
//            if (!ModelState.IsValid) return Page();

//            // ?? resolve ProviderId from claims or from email -> DB
//            var providerId = await GetProviderIdForCurrentUserAsync();
//            if (providerId == null)
//            {
//                ModelState.AddModelError("", "You must be signed in as a provider.");
//                return Page();
//            }

//            // ensure (ProviderId, SpecialtyId) exists due to your composite FK
//            await _context.Database.ExecuteSqlRawAsync(@"
//                IF NOT EXISTS (SELECT 1 FROM dbo.ProviderSpecialty WHERE ProviderId=@pid AND SpecialtyId=@sid)
//                    INSERT INTO dbo.ProviderSpecialty (ProviderId, SpecialtyId) VALUES (@pid, @sid);",
//                new SqlParameter("@pid", providerId),
//                new SqlParameter("@sid", Input.SpecialtyId));

//            // set ProviderId + hash password, then save
//            Input.ProviderId = providerId.Value;
//            Input.DoctorPassword = BCrypt.Net.BCrypt.HashPassword(Input.DoctorPassword);

//            _context.Doctor.Add(Input);              // DbSet<DoctorDetails> Doctor { get; set; }
//            await _context.SaveChangesAsync();

//            TempData["Success"] = $"Doctor saved (ID #{Input.DoctorId}).";
//            return RedirectToPage("/ProviderAddDoctor");
//        }

//        private async Task<int?> GetProviderIdForCurrentUserAsync()
//        {
//            // Prefer explicit ProviderId claim if present
//            if (int.TryParse(User.FindFirstValue("ProviderId"), out var pidFromClaim))
//                return pidFromClaim;

//            // Otherwise resolve via email claim (or Name)
//            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
//            if (!string.IsNullOrWhiteSpace(email))
//            {
//                return await _context.Provider
//                    .Where(p => p.Email == email)
//                    .Select(p => (int?)p.ProviderId)
//                    .FirstOrDefaultAsync();
//            }
//            return null;
//        }


//    }
//}

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderAddDoctorModel : PageModel
    {
        private readonly ProviderDbContext _context;
        public ProviderAddDoctorModel(ProviderDbContext context) => _context = context;

        public List<Specialty> Specialties { get; set; } = new();
        public string? SuccessMessage { get; set; }

        [BindProperty]
        public DoctorDetails Input { get; set; } = new();
        
        public List<int> ProviderSpecialtiesId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int? providerId = HttpContext.Session.GetInt32("ProviderId");
            if(providerId == null) 
            {
                TempData["AlertMsg"] = "Session Time Out, Please Login Again";
                return RedirectToPage("/ProviderLogin"); 
            }
            else
            {
                //retreive the provider specialties
                //then based on the provider id select the specialty data
                ProviderSpecialtiesId = await _context.ProviderSpecialties.Where(ps => ps.ProviderId == (int)providerId).Select(ps => ps.SpecialtyId).ToListAsync();
                Specialties = await _context.Specialty.Where(s=>ProviderSpecialtiesId.Contains(s.SpecialtyId)).OrderBy(s => s.SpecialtyName).ToListAsync();
                //SuccessMessage = TempData["Success"] as string;
                return Page();
            }
            
        }
        public async Task<IActionResult> OnPostAsync()
        {
            //Specialties = await _context.Specialty.OrderBy(s => s.SpecialtyName).ToListAsync();
            /*if (!ModelState.IsValid)
            {
                TempData["AlertMsg"] = "Missing Required Field.";
                return Page();
            }*/

            // Validate specialty selection (drop-down default is empty)
            if (Input.SpecialtyId <= 0)
            {
                TempData["AlertMsg"] = "Please Select A Specialty.";
                return Page();
            }

            // ?? resolve ProviderId from claims or from email -> DB
            //var providerId = await GetProviderIdForCurrentUserAsync();
            var providerId = HttpContext.Session.GetInt32("ProviderId");
            if (providerId == null)
            {
                TempData["AlertMsg"] = "Session Time Out, Please Login Again.";
                return RedirectToPage("/ProviderLogin");
            }
            else
            {
                try
                {
                    // set ProviderId + hash password, then save
                    Input.ProviderId = (int)providerId.Value;
                    Input.DoctorPassword = BCrypt.Net.BCrypt.HashPassword(Input.DoctorPassword);

                    _context.Doctor.Add(Input);              // DbSet<DoctorDetails> Doctor { get; set; }
                    await _context.SaveChangesAsync();

                    //TempData["Success"] = $"Doctor saved (ID #{Input.DoctorId}).";
                    TempData["AlertMsg"] = $"Doctor saved (ID #{Input.DoctorId}).";

                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error ------- " +ex.ToString());
                    //TempData["Success"] = $"Exception : {ex}.";
                    TempData["AlertMsg"] = $"Exception : {ex}";
                }
                return Page();
            }

            // ensure (ProviderId, SpecialtyId) exists due to your composite FK
            /*await _context.Database.ExecuteSqlRawAsync(@"
				IF NOT EXISTS (SELECT 1 FROM dbo.ProviderSpecialty WHERE ProviderId=@pid AND SpecialtyId=@sid)
					INSERT INTO dbo.ProviderSpecialty (ProviderId, SpecialtyId) VALUES (@pid, @sid);",
                new SqlParameter("@pid", (int)providerId),
                new SqlParameter("@sid", Input.SpecialtyId));*/
        }

        private async Task<int?> GetProviderIdForCurrentUserAsync()
        {
            // Prefer explicit ProviderId claim if present
            if (int.TryParse(User.FindFirstValue("ProviderId"), out var pidFromClaim))
                return pidFromClaim;

            // Otherwise resolve via email claim (or Name)
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(email))
            {
                var providerIdFromEmail = await _context.Provider
                    .Where(p => p.Email == email)
                    .Select(p => (int?)p.ProviderId)
                    .FirstOrDefaultAsync();
                if (providerIdFromEmail != null)
                    return providerIdFromEmail;
            }

            // Fallback to static ProviderSession used by ProviderLogin
            if (ProviderSession.ProviderId > 0)
                return ProviderSession.ProviderId;

            return null;
        }


    }
}