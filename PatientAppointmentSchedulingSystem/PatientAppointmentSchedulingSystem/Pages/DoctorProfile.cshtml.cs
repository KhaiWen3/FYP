using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class DoctorProfileModel : PageModel
    {
        private readonly DoctorDbContext _context;
        public DoctorProfileModel(DoctorDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public DoctorDetails DoctorDetails { get; set; }
        public ProviderDetails ProviderDetails { get; set; }
        public Specialty SpecialtyDetails {  get; set; }

        public async Task<IActionResult> OnGet()
        {
            int? idDoctorSession = HttpContext.Session.GetInt32("DoctorId");

            if (idDoctorSession == null)
            {
                TempData["AlertMsg"] = "Session Time Out. Please Login Again.";
                return RedirectToPage("/DoctorLogin");
            }
            else
            {
                DoctorDetails = await _context.Doctor.FindAsync((int)idDoctorSession);
                if (DoctorDetails == null)
                {
                    TempData["AlertMsg"] = "Doctor Not Found.";
                    return RedirectToPage("/DoctorLogin");
                }
                else
                {
                    ProviderDetails = await _context.ProviderDetails.FirstOrDefaultAsync(pd => pd.ProviderId == DoctorDetails.ProviderId);
                    if(ProviderDetails != null) {
                        DoctorDetails.DoctorProviderName = ProviderDetails.Name;
                    }
                    else
                    {
                        DoctorDetails.DoctorProviderName = "NOT_FOUND";
                    }

                    SpecialtyDetails = await _context.SpecialtyDetails.FirstOrDefaultAsync(sd => sd.SpecialtyId == DoctorDetails.SpecialtyId);
                    if(SpecialtyDetails != null)
                    {
                        DoctorDetails.DoctorSpecialtyName = SpecialtyDetails.SpecialtyName;
                    }
                    else
                    {
                        DoctorDetails.DoctorSpecialtyName = "NOT_FOUND";
                    }
                    return Page();
                }
                
            }
        }
    }
}
