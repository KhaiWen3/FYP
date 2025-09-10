using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;

namespace PatientAppointmentSchedulingSystem.Pages
{
    //[Authorize(Roles = "Doctor")] // optional but recommended
    public class DoctorAddAvailableSlotModel : PageModel
    {
        // inject your ef core context so you can query/insert AvailabilitySlots
        private readonly AvailabilitySlotDbContext _context;
        public DoctorAddAvailableSlotModel(AvailabilitySlotDbContext context)
        {
            _context = context;
        }


        [BindProperty]
        public int? DoctorId { get; set; }

        public string? DoctorName { get; set; }
        public string? DoctorSpecialty { get; set; }
        public string? DoctorMedicalService { get; set; }

        //AvailableSlots is what will display on the page
        public List<AvailabilitySlots> AvailableSlots { get; set; }

        [BindProperty]
        public AvailabilitySlots Input { get; set; }

        //Use a view-model without DoctorId so validation passes
        public class SlotInput
        {
            [Required]
            public DateTime AppointmentDate { get; set; }

            [Required]
            public TimeSpan AptStartTime { get; set; }

            [Required]
            public TimeSpan AptEndTime { get; set; }

            [Required]
            public string AppointmentType { get; set; } = "Video";
        }

        //[BindProperty]
        //public SlotInput Input { get; set; } = new();

        //idClaim stores DoctorId from session so you can show it in the page.
        public int? idClaim { get; set; }


        public async Task<IActionResult> OnPostAsync()
        {
            // reload list for redisplay on validation errors
            OnGet();

            //Server-side validation: if the bound Input has required fields missing/invalid, redisplay the page.
            if (!ModelState.IsValid) return Page();

            //Fetch doctor id from Session (not from cookies/claims)
            //If the session does not have it/expired, send the user to the doctor login page
            int doctorId = 0;
            idClaim = HttpContext.Session.GetInt32("DoctorId");
            if (idClaim == null)
            {
                Console.WriteLine("Doctor Id Not Found");
                return RedirectToPage("/DoctorLogin");
            }
            else
            {
                doctorId = (int)idClaim;
            }

            Console.WriteLine("Testing");

            var date = Input.AppointmentDate.Date;
            var start = new DateTime(
                date.Year, date.Month, date.Day,
                Input.AptStartTime.Hour, Input.AptStartTime.Minute, Input.AptStartTime.Second);

            var end = new DateTime(
                date.Year, date.Month, date.Day,
                Input.AptEndTime.Hour, Input.AptEndTime.Minute, Input.AptEndTime.Second);

            // 2. add custom time validation
            if (end <= start)
            {
                ModelState.AddModelError("Input.AptEndTime", "End time must be after start time");
                return Page();
            }

            //3.Get doctor id from sesion
            //var doctorId = doctorId ?? HttpContext.Session.GetInt32("DoctorId");
            //var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            //int doctorId = 0;
            //Console.WriteLine("DoctorId: " + doctorIdClaim.Value);
            //if (doctorIdClaim == null)
            //{
            //    return RedirectToPage("/DoctorLogin");
            //}
            //else
            //{
            //    doctorId = int.Parse(doctorIdClaim.Value);
            //}

            //4. using entity framework to create and save the slot
            var slot = new AvailabilitySlots
                {
                    AppointmentDate = Input.AppointmentDate,
                    AptStartTime = start,
                    AptEndTime = end,
                    AppointmentType = Input.AppointmentType,
                    DoctorId = doctorId,
                    PatientId = null,
                    AppointmentStatus = 0 //0 = available
                };
    
            try
            {
                _context.AvailabilitySlots.Add(slot);
                await _context.SaveChangesAsync();

                // Redirect with the correct query key so your property binds:
                //return RedirectToPage("/DoctorAddAvailableSlot", new { DoctorId = doctorId });
                return RedirectToPage("/DoctorAddAvailableSlot");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving slot: " + ex.Message);
                // Log the exception
                Console.WriteLine(ex.StackTrace);
                return Page();
            }
        }
        public IActionResult OnGet()
        {
            // Get doctor ID from claims
            idClaim = HttpContext.Session.GetInt32("DoctorId");
            int doctorId = 0;
            // So now u login as doctor right? so u just get it from the session !! but session will die which means if the user open the page so long time session will die then need to restart
            // if session no die do this V

            if(idClaim != null) //here is to check the session la
            {
                doctorId = (int)idClaim;    // for this because to store the session of doctor id but possible is null because of die, so u need to use (int) casting to cast from int? to int
                // perform the data retrieve from here
                AvailableSlots = _context.AvailabilitySlots
                    .Where(s => s.DoctorId == doctorId)
                    .OrderBy(s => s.AppointmentDate)
                    .ThenBy(s => s.AptStartTime)
                    .ToList();

                return Page();
            }
            else //this else mean session die so ask them go back login page
            {
                Console.WriteLine("Doctor Id Is NuLL");
                // later u add here back to login page 
                //if the session die
                return RedirectToPage("/DoctorLogin");
            }

            //if (User.Identity?.IsAuthenticated == true)
            //{
            //    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            //    if (idClaim != null && int.TryParse(idClaim.Value, out var idFromClaim))
            //    {
            //        DoctorId ??= idFromClaim;
            //    }
            //}

            //DoctorName = User.FindFirstValue(ClaimTypes.Name);
            //DoctorSpecialty = User.FindFirstValue("Specialization");
            //DoctorMedicalService = User.FindFirstValue("MedicalService");

            // Load the slots for the doctor
            //if (DoctorId.HasValue)
            //{
            //    AvailableSlots = _context.AvailabilitySlots
            //        .Where(s => s.DoctorId == DoctorId.Value)
            //        .OrderBy(s => s.AppointmentDate)
            //        .ThenBy(s => s.AptStartTime)
            //        .ToList();
            //}

        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // check doctor session
            var doctorId = HttpContext.Session.GetInt32("DoctorId");
            if (doctorId == null)
            {
                return RedirectToPage("/DoctorLogin");
            }

            var slot = await _context.AvailabilitySlots
                .FirstOrDefaultAsync(s => s.SlotId == id && s.DoctorId == doctorId);

            if (slot == null)
            {
                return NotFound(new { success = false, message = "Slot not found" });
            }

            _context.AvailabilitySlots.Remove(slot);
            await _context.SaveChangesAsync();

            // If you're using AJAX (fetch)
            return new JsonResult(new { success = true, message = "Slot deleted successfully" });

            // If you want to refresh page instead:
            // return RedirectToPage("/DoctorAddAvailableSlot");
        }

    }
}
