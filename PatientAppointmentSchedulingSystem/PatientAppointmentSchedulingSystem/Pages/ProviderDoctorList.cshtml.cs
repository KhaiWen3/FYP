using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using PatientAppointmentSchedulingSystem.Helpers; // for Firebase helper

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderDoctorListModel : PageModel
    {
        private readonly FirebaseStorageHelper _firebaseHelper;
        private readonly ProviderDbContext _context;
        [BindProperty]
        public List<DoctorDetails> DoctorList { get; set; }
        [BindProperty]
        public Specialty SpecialtyName { get; set; }

        //for edit modal
        [BindProperty]
        public DoctorDetails DoctorInput { get; set; }
        [BindProperty]
        public IFormFile DoctorPhoto { get; set; }

        public ProviderDoctorListModel(ProviderDbContext context, FirebaseStorageHelper firebaseHelper)
        {
            _context = context;
            _firebaseHelper = firebaseHelper;
        }

        public async Task<IActionResult> OnGet()
        {
            int? providerIdFromSession = HttpContext.Session.GetInt32("ProviderId");

            if (providerIdFromSession == null)//session die
            {
                //alert -> session die
                TempData["AlertMsg"] = "Session Ended, Please Login.";
                return RedirectToPage("/ProviderLogin");
            }
            else
            {
                DoctorList = await _context.Doctor.Where(d => d.ProviderId == (int)providerIdFromSession).ToListAsync();

                foreach (var doctor in DoctorList)
                {
                    doctor.DoctorSpecialtyName = await _context.Specialty
                        .Where(s => s.SpecialtyId == doctor.SpecialtyId)
                        .Select(s => s.SpecialtyName)
                        .FirstOrDefaultAsync();
                }
                return Page();
            }
        }

        // ? Handle doctor edit from modal
        public async Task<IActionResult> OnPostEditDoctorAsync()
        {
            if (DoctorInput == null) return Page();

            var doctor = await _context.Doctor.FindAsync(DoctorInput.DoctorId);
            if (doctor == null) return NotFound();

            // update fields
            doctor.DoctorFullName = DoctorInput.DoctorFullName;
            doctor.DoctorSpecialtyName = DoctorInput.DoctorSpecialtyName;
            doctor.DoctorMedicalService = DoctorInput.DoctorMedicalService;
            doctor.DoctorRoomNum = DoctorInput.DoctorRoomNum;
            doctor.DoctorPhoneNum = DoctorInput.DoctorPhoneNum;
            doctor.DoctorEmail = DoctorInput.DoctorEmail;
            doctor.DoctorLanguageSpoken = DoctorInput.DoctorLanguageSpoken;

            // ? handle photo upload
            if (DoctorPhoto != null)
            {
                using var stream = DoctorPhoto.OpenReadStream();
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(DoctorPhoto.FileName)}";

                // upload to Firebase
                var photoUrl = await _firebaseHelper.UploadImageAsync(
                    stream, fileName, "DoctorImage", DoctorPhoto.ContentType);

                doctor.DoctorPhoto = photoUrl;
            }

            await _context.SaveChangesAsync();

            TempData["AlertMsg"] = "Doctor updated successfully!";
            return RedirectToPage(); // refresh list
        }
    }
}
