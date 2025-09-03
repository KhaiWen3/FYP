using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderPatientListModel : PageModel
    {
        private readonly ProviderDbContext _context;
        [BindProperty]
        public List<PatientDetails> PatientList { get; set; }
        [BindProperty]
        public List<AvailabilitySlots> AvailabilitySlots { get; set; }

        public ProviderPatientListModel(ProviderDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            int? providerIdFromSession = HttpContext.Session.GetInt32("ProviderId");
            if(providerIdFromSession == null)
            {
                TempData["AlertMsg"] = "Session Ended, Please Login.";
                return RedirectToPage("/ProviderLogin");
            }
            else
            {
                try
                {
                    var AvailabilitySlotsQuery = from av in _context.AvailabilitySlots
                                                 join pd in _context.PatientDetails on av.PatientId equals pd.PatientId
                                                 join dd in _context.Doctor on av.DoctorId equals dd.DoctorId
                                                 where dd.ProviderId == (int)providerIdFromSession
                                                 select new AvailabilitySlots
                                                 {
                                                     Patient = pd,
                                                     Doctor = dd,
                                                     AppointmentType = av.AppointmentType,
                                                     AptStartTime = av.AptStartTime,
                                                     AptEndTime = av.AptEndTime,
                                                     AppointmentDate = av.AppointmentDate
                                                 };

                    AvailabilitySlots = await AvailabilitySlotsQuery.ToListAsync();

                    return Page();
                }catch(Exception ex)
                {
                    TempData["AlertMsg"] = "Exception : "+ex.Message;
                    return Page();
                }
                
            }

        }
    }
}
