using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderDoctorListModel : PageModel
    {
        private readonly ProviderDbContext _context;
        [BindProperty]
        public List<DoctorDetails> DoctorList { get; set; }
        [BindProperty]
        public Specialty SpecialtyName { get; set; }

        public ProviderDoctorListModel(ProviderDbContext context)
        {
            _context = context;
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
    }
}
