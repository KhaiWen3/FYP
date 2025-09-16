using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatientAppointmentSchedulingSystem.Pages.Data;
using Microsoft.EntityFrameworkCore;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class PatientViewAppointmentModel : PageModel
    {
        //database connection to query doctor and slots data
        private readonly AvailabilitySlotDbContext _context;

        //constructor
        public PatientViewAppointmentModel(AvailabilitySlotDbContext context)
        {
            _context = context;
        }

        public List<DoctorDetails> Doctors { get; set; } = new List<DoctorDetails>();

        public List<AvailabilitySlots> AppointmentSlots { get; set; } = new List<AvailabilitySlots>();


        public IActionResult OnGet()
        {
            int? patientIdFromSession = HttpContext.Session.GetInt32("PatientId");
            if(patientIdFromSession == null)
            {
                return RedirectToPage("/PatientLogin");
            }

            //get the slot with status = 1 and correct patient id
            var appointmentSlotQuery = _context.AvailabilitySlots
                .Where(s => s.AppointmentStatus == 1 && s.PatientId == (int)patientIdFromSession)
                .Include(s => s.Doctor)
                .ToList();

            // Step 2: get all specialties in one go (avoid hitting DB in a loop)
            var specialtyDict = _context.SpecialtyDetails
                .ToDictionary(sp => sp.SpecialtyId, sp => sp.SpecialtyName);

            // Step 3: map DoctorSpecialtyName manually
            foreach (var slot in appointmentSlotQuery)
            {
                if (specialtyDict.TryGetValue((int)slot.Doctor.SpecialtyId, out var specName))
                {
                    slot.Doctor.DoctorSpecialtyName = specName;
                }
            }

            AppointmentSlots = appointmentSlotQuery;

            return Page();

        }

        public IActionResult OnPostDelete(int id)
        {
            var slot = _context.AvailabilitySlots.FirstOrDefault(s => s.SlotId == id);
            if (slot != null)
            {
                // Clear patient assignment and mark as available
                slot.PatientId = null;
                slot.AppointmentStatus = 0;

                _context.SaveChanges();
            }

            // Refresh page
            return RedirectToPage();
        }

        public IActionResult OnPostBookAppointment(int slotId)
        {
            var patientId = HttpContext.Session.GetInt32("PatientId");

            if (patientId == null)
            {
                return RedirectToPage("/PatientLogin"); // if not logged in
            }

            var slot = _context.AvailabilitySlots.FirstOrDefault(s => s.SlotId == slotId);

            if (slot == null || slot.AppointmentStatus == 1)
            {
                return NotFound();
            }

            slot.PatientId = patientId.Value;
            slot.AppointmentStatus = 1; // Booked

            _context.SaveChanges();

            // After booking, refresh the page
            return RedirectToPage(); // reloads PatientMakeAppointment page
        }

    }
}
