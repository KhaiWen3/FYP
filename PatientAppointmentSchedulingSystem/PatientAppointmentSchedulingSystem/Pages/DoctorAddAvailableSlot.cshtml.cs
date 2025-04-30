using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System.Security.Claims;
using static System.Reflection.Metadata.BlobBuilder;

namespace PatientAppointmentSchedulingSystem.Pages
{
    //[Authorize(AuthenticationSchemes = "DoctorCookie")]
    public class DoctorAddAvailableSlotModel : PageModel
    {
        private readonly AvailabilitySlotDbContext _context;

        [BindProperty(SupportsGet = true)]
        public int? DoctorId { get; set; }

        public string DoctorName { get; set; }
        public string DoctorSpecialty { get; set; }
        public string DoctorMedicalService { get; set; }

        public DoctorAddAvailableSlotModel(AvailabilitySlotDbContext context)
        {
            _context = context;
        }

        public List<AvailabilitySlots> AvailableSlots { get; set; }

        [BindProperty]
        public AvailabilitySlots Input {  get; set; }

        

        public async Task<IActionResult> OnPostAsync()
        {
            //1. check model state valdiation
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 2. add custom time validation
            if (Input.EndTime <= Input.StartTime)
            {
                ModelState.AddModelError("Input.EndTime", "End time must be after start time");
                return Page();
            }

            //3. Get doctor id from sesion
            //var doctorId = DoctorId ?? HttpContext.Session.GetInt32("DoctorId");
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            int doctorId = 0;
            Console.WriteLine("DoctorId: " + doctorIdClaim.Value);
            if (doctorIdClaim == null)
            {
                return RedirectToPage("/DoctorLogin");
            }
            else
            {
                doctorId = int.Parse(doctorIdClaim.Value);
            }

            //4. using entity framework to create and save the slot
            var slot = new AvailabilitySlots
                {
                    AppointmentDate = Input.AppointmentDate,
                    StartTime = Input.StartTime,
                    EndTime = Input.EndTime,
                    AppointmentType = Input.AppointmentType,
                    DoctorId = doctorId,
                    PatientId = null,
                    AppointmentStatus = 0 //0 = available
                };

            try
            {
                _context.AvailabilitySlots.Add(slot);
                await _context.SaveChangesAsync();

                if (DoctorId.HasValue)
                {
                    // Populate the available slots for this doctor
                    AvailableSlots = _context.AvailabilitySlots
                        .Where(s => s.DoctorId == DoctorId.Value)
                        .OrderBy(s => s.AppointmentDate)
                        .ThenBy(s => s.StartTime)
                        .ToList();
                }
                else
                {
                    AvailableSlots = new List<AvailabilitySlots>(); // prevent null reference
                }

                // Redirect back to DoctorDetails with the DoctorId
                return RedirectToPage("/DoctorAddAvailableSlot", new { id = doctorId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving slot: " + ex.Message);
                // Log the exception
                Console.WriteLine(ex.StackTrace);
                return Page();
            }
        }

        public void OnGet()
        {
            // Get doctor ID from claims
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (doctorIdClaim != null)
            {
                DoctorId = int.Parse(doctorIdClaim.Value);
            }

            DoctorName = User.FindFirstValue(ClaimTypes.Name);
            DoctorSpecialty = User.FindFirstValue("Specialization");
            DoctorMedicalService = User.FindFirstValue("MedicalService");

            // Load the slots for the doctor
            if (DoctorId.HasValue)
            {
                AvailableSlots = _context.AvailabilitySlots
                    .Where(s => s.DoctorId == DoctorId.Value)
                    .OrderBy(s => s.AppointmentDate)
                    .ThenBy(s => s.StartTime)
                    .ToList();
            }
            else
            {
                AvailableSlots = new List<AvailabilitySlots>();
            }

        }
    }
}
