using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class PatientHomePageModel : PageModel
    {
        private readonly DoctorDbContext _context;

        public PatientHomePageModel(DoctorDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchDoctorName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SpecialtyId { get; set; }

        
        
        public SelectList SpecialtyOptions => new SelectList(Specialties, "SpecialtyId", "SpecialtyName", SpecialtyId);

        [BindProperty]
        public PatientDetails PatientDetails { get; set; }
        [BindProperty]
        public ProviderDetails ProviderDetails { get; set; }

        public List<DoctorDetails> Doctors { get; set; } = new List<DoctorDetails>();
        public List<AvailabilitySlots> AppointmentSlots { get; set; } = new List<AvailabilitySlots>();
        public List<Specialty> Specialties { get; set; } = new();


        public void OnGet()
        {
            // Load specialties for dropdown
            Specialties = _context.SpecialtyDetails
                .AsNoTracking()
                .OrderBy(s => s.SpecialtyName)
                .ToList();

            //load patient for header name
            var patientId = HttpContext.Session.GetInt32("PatientId");
            if (patientId != null)
            {
                // Ensure this matches your DbSet name: _context.Patients or _context.PatientDetails
                PatientDetails = _context.PatientDetails
                    .AsNoTracking()
                    .FirstOrDefault(p => p.PatientId == patientId.Value); // <-- use correct property name
            }

            //base doctor query
            var doctorQuery = _context.Doctor
                .AsNoTracking()
                .Include(d => d.Provider)   // <= this line loads Provider for each Doctor
                .AsQueryable();

            Doctors = doctorQuery
                .Select(d => new DoctorDetails
                {
                    DoctorId = d.DoctorId,
                    DoctorFullName = d.DoctorFullName,
                    DoctorMedicalService = d.DoctorMedicalService,

                    // uses the navigation loaded above
                    DoctorProviderName = d.Provider != null ? d.Provider.Name : null
                })
                .ToList();


            //search by doctor name
            if (!string.IsNullOrWhiteSpace(SearchDoctorName))
            {
                var q = SearchDoctorName.Trim();
                doctorQuery = doctorQuery.Where(d =>
                    d.DoctorFullName.ToLower().Contains(q.ToLower())
                    // || d.DoctorMedicalService.ToLower().Contains(q.ToLower())
                    // || d.DoctorSpeciality.ToLower().Contains(q.ToLower())
                );
            }

            // Specialty filter
            if (SpecialtyId.HasValue)
            {
                // === Option A: if you have a many-to-many nav:
                doctorQuery = doctorQuery.Where(d => d.ProviderSpecialties.Any(ps => ps.SpecialtyId == SpecialtyId.Value));
                doctorQuery = doctorQuery.Distinct(); // avoid duplicates
            }

            // LOAD SLOTS
            var doctorIds = Doctors.Select(d => d.DoctorId).ToList();
            if (doctorIds.Count > 0)
            {
                AppointmentSlots = _context.AvailabilitySlots
                    .AsNoTracking()
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
            slot.AppointmentType = "booked";
            slot.AppointmentStatus = 1; // Booked

            _context.SaveChanges();

            // After booking, refresh the page
            return RedirectToPage(); // reloads PatientMakeAppointment page
        }
    }
}
