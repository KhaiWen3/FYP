using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatientAppointmentSchedulingSystem.Pages.Data;
using Microsoft.EntityFrameworkCore;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class PatientViewAppointmentModel : PageModel
    {
        //database connection to query doctor and slots data
        private readonly DoctorDbContext _context;

        //constructor
        public PatientViewAppointmentModel(DoctorDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        public List<DoctorDetails> Doctors { get; set; } = new List<DoctorDetails>();
        public List<AvailabilitySlots> AppointmentSlots { get; set; } = new List<AvailabilitySlots>();

        public string PatientFullName { get; set; }
        public int? PatientId { get; set; }

        public void OnGet()
        {
            PatientFullName = HttpContext.Session.GetString("PatientFullName");
            // ? Retrieve PatientId from session
            PatientId = HttpContext.Session.GetInt32("PatientId");

            //step 1
            var doctorQuery = _context.Doctor.AsQueryable(); // don't ToList() yet

            //step 2 apply search filter
            if (!string.IsNullOrEmpty(Search))
            {
                doctorQuery = doctorQuery
                    .Where(d => d.DoctorMedicalService.ToLower().Contains(Search.ToLower()) ||
                           d.DoctorFullName.ToLower().Contains(Search.ToLower()));
                //d.DoctorSpeciality.ToLower().Contains(Search.ToLower())
            }

            //step 3 fetch filtered doctors
            Doctors = doctorQuery.ToList();

            // Step 4: Get their IDs
            var doctorIds = Doctors.Select(d => d.DoctorId).ToList();


            if (doctorIds.Any())
            {
                AppointmentSlots = _context.AvailabilitySlots
                    .Where(slot => doctorIds.Contains(slot.DoctorId))
                    .Include(slot => slot.Doctor)
                    .OrderBy(s => s.AppointmentDate)
                    .ThenBy(s => s.AptStartTime)
                    .ToList();
            }
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
