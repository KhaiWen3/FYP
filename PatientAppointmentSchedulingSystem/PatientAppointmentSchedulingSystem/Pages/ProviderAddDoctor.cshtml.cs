using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using PatientAppointmentSchedulingSystem.Helpers;


namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderAddDoctorModel : PageModel
    {
        private readonly FirebaseStorageHelper _firebaseHelper;
        private readonly ProviderDbContext _context;
        public ProviderAddDoctorModel(ProviderDbContext context, FirebaseStorageHelper firebaseHelper)
        { 
            _context = context; 
            _firebaseHelper = firebaseHelper;
        
        }
        [BindProperty]
        public IFormFile DoctorPhoto { get; set; }

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
                //TempData["AlertMsg"] = "Session Time Out, Please Login Again";
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
                //TempData["AlertMsg"] = "if else";
                try
                {
                    string photoUrl = null;
                    if(DoctorPhoto != null)
                    {
                        using var stream = DoctorPhoto.OpenReadStream();
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(DoctorPhoto.FileName)}";
                        photoUrl = await _firebaseHelper.UploadImageAsync(stream, fileName, "DoctorImage", DoctorPhoto.ContentType);
                        TempData["AlertMsg"] = "photoUrl : " + photoUrl;
                    }
                    TempData["AlertMsg"] = "Photo Is Null";

                    var duplicateEmail = await _context.Doctor
                        .FirstOrDefaultAsync(d => d.DoctorEmail.ToLower() == Input.DoctorEmail.ToLower());
                    if(duplicateEmail != null)
                    {
                        TempData["AlertMsg"] = "Email " + Input.DoctorEmail + " had been use, try another email.";
                        return RedirectToPage("/ProviderAddDoctor");
                    }

                    var duplicateRoom = await _context.Doctor
                        .FirstOrDefaultAsync(d => d.ProviderId == providerId && d.DoctorRoomNum == Input.DoctorRoomNum);
                    if(duplicateRoom != null)
                    {
                        TempData["AlertMsg"] = "Room Number " + Input.DoctorRoomNum + " had been use, try another room number.";
                        return RedirectToPage("/ProviderAddDoctor");
                    }

                    // set ProviderId + hash password, then save
                    Input.ProviderId = (int)providerId.Value;
                    Input.DoctorPassword = BCrypt.Net.BCrypt.HashPassword(Input.DoctorPassword);

                    // assign Firebase URL if exists
                    if (!string.IsNullOrEmpty(photoUrl))
                    {
                        Input.DoctorPhoto = photoUrl;
                    }

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
        }

    }
}