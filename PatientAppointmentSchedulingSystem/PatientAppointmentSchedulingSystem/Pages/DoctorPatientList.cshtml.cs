using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class DoctorPatientListModel : PageModel
    {
        private readonly AvailabilitySlotDbContext _context;

        public DoctorPatientListModel(AvailabilitySlotDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AvailabilitySlots AvailableSlots { get; set; }

        public List<AvailabilitySlots> ListAvailableSlots { get; set; }


        public IActionResult OnGet()
        {
            int? idDoctorSession = HttpContext.Session.GetInt32("DoctorId");
            if(idDoctorSession == null)
            {
                return RedirectToPage("/DoctorLogin");
            }
            else
            {
                ListAvailableSlots = _context.AvailabilitySlots
                    .Where(s => s.DoctorId == (int)idDoctorSession && s.AppointmentStatus == 1)
                    .Include(s => s.Patient)
                    .OrderBy(s => s.AppointmentDate)
                    .ThenBy( s => s.AptStartTime)
                    .ToList();

                /*foreach (var slot in ListAvailableSlots)
                {
                    if (slot.PatientId != null)
                    {
                        slot.Patient = _context.PatientDetails
                            .FirstOrDefault(p => p.PatientId == slot.PatientId);
                    }
                }*/

                return Page();
            }

        }
    }
}
