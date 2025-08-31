using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class PatientProfileModel : PageModel
    {
        private readonly PatientDbContext _context;
        public PatientProfileModel(PatientDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PatientDetails Patient { get; set; } //use this

        //toggle edit mode via query: /PatientProfile?edit=true
        [BindProperty]
        public bool IsEditing { get; set; }

        //display purpose
        public DateTime? LastUpdated { get; set; }
        public string? ErrorMessage { get; set; }

        //view-modal that matches UI fields
        public class PatientProfileInput // this is created because in my db table dont have this info that it let me separate outok understand
        {
            // ——— Personal ———
            [Required] 
            public string FirstName { get; set; } = string.Empty;
            [Required] 
            public string LastName { get; set; } = string.Empty;
            public int? Age { get; set; }
            public DateTime? DateOfBirth { get; set; }// Not in DB; used to compute Age on save
            public string? Gender { get; set; } // Not in DB now — keep optional unless you plan to store it somewhere

            // ——— Contact ———
            [Required, EmailAddress] 
            public string Email { get; set; } = string.Empty;
            [Required] 
            public string Phone { get; set; } = string.Empty;


            // The rest are UI-only for now (not saved yet)
            public string? Address { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? ZipCode { get; set; }
            public string? EmergencyContact { get; set; }
            public string? EmergencyPhone { get; set; }
            public string? Insurance { get; set; }
            public string? PolicyNumber { get; set; }
            public string? BloodType { get; set; }
            public string? PrimaryPhysician { get; set; }
            public string? Allergies { get; set; }
            public string? Medications { get; set; }
        }
        [BindProperty]
        public PatientProfileInput Input { get; set; }

        public int? idPatientSession { get; set; }


        //GET: load the patient (by ?id= OR Session) and populate Input
        public async Task<IActionResult> OnGetAsync()
        {
            idPatientSession = HttpContext.Session.GetInt32("PatientId");
            int patientId = 0;
            IsEditing = false;

            //here is to check the session
            if (idPatientSession != null) //this benlai is =! but 
            {
                patientId = (int)idPatientSession;

                //perform data retrieve from here
                Patient =  await _context.Patients.FindAsync(patientId);
                //i think can ignore this warning, because we make sure patient id is there! wahah
                
                return Page();
                    //.OrderBy(p => p.PatientLastName)
                    //.ThenBy(p => p.AptStartTime) here i dk how to change
                    //.ToList();
                    //this error is need return value
                //return RedirectToPage("/PatientLogin");
            }
            //else
            //{
            //    //not null retrieve data from ur session adn store into the Patient object thenc an call at frontend
            //    Patient = (PatientDetails)_context.Patients
            //        .Where(p => p.PatientId == patientId);

            //    LastUpdated = DateTime.UtcNow;
            //    return Page();
            //}
            else //this else mean session die so ask them go back login page
            {
                Console.WriteLine("Patient Id Is NuLL");
                //i planned to add the alert ok
                // later u add here back to login page 
                //if the session die
                return RedirectToPage("/PatientLogin");
            }


            //try
            //{
            //    _context.Patients.Add(patient);
            //    await _context.SaveChangesAsync();

            //    // Redirect with the correct query key so your property binds:
            //    //return RedirectToPage("/DoctorAddAvailableSlot", new { DoctorId = doctorId });
            //    return RedirectToPage("/Doctor");
            //}
            //catch (Exception ex)
            //{
            //    ModelState.AddModelError("", "Error saving slot: " + ex.Message);
            //    // Log the exception
            //    Console.WriteLine(ex.StackTrace);
            //    return Page();
            //}



            //var id = HttpContext.Session.GetInt32("PatientId");
            //Input = await _context.Patients.FindAsync(id);
        }

        // POST: Save button
        public async Task<IActionResult> OnPostSaveAsync()
        {
            OnGetAsync();

            if (!ModelState.IsValid)
            {
                // stay on page with validation messages
                IsEditing = true;
                return Page();
            }

            //Fetch patient id from session
            int patientId = 0;
            idPatientSession = HttpContext.Session.GetInt32("PatientId");
            if(idPatientSession == null)
            {
                Console.WriteLine("Patient Id Not Found");
                return RedirectToPage("/PatientLogin");
            }
            else
            {
                patientId = (int)idPatientSession;
            }

            // Map DB entity -> view-model
            //var patient = new PatientProfileInput
            //{
            //    //for here u want get from session? im not sure need or not i compare with besidde one
            //    //what usage for this -> i think is no need for adding just only need
            //    //
            //    FirstName = PatientDetails.PatientFirstName,
            //    LastName = PatientDetails.PatientLastName,
            //    Email = PatientDetails.PatientEmail,
            //    Phone = PatientDetails.PatientPhoneNum,
            //    //DateOfBirth = Input.Age,

            //    // UI-only fields (not stored yet) can be left null or populated from
            //    // another table if you add one later.
            //    // DateOfBirth is not in DB; if you later add a column/table, load it here.
            //};

            //idPatientSession= ResolvePatientId();
            //if (patientId == null) return RedirectToPage("/PatientLogin");

            //Id = patientId;

            //var patient = await _context.Patients
            //    .FirstOrDefaultAsync(p => p.PatientId == patientId.Value);

            //if (patient == null) return NotFound();

            // Map Input -> DB (only the columns your table actually has)
            //patient.PatientFirstName = Input.FirstName?.Trim() ?? patient.PatientFirstName;
            //patient.PatientLastName = Input.LastName?.Trim() ?? patient.PatientLastName;
            //patient.PatientEmail = Input.Email?.Trim() ?? patient.PatientEmail;
            //patient.PatientPhoneNum = Input.Phone?.Trim() ?? patient.PatientPhoneNum;

            // Your table has Age (int). If DateOfBirth is provided, compute it.
            if (Input.DateOfBirth != null)
            {
                //patient.PatientAge = CalculateAge(Input.DateOfBirth.Value, DateTime.Today);
            }

            await _context.SaveChangesAsync();

            // end edit mode
            IsEditing = false;

            // reload page to show saved values
            return RedirectToPage();
        }

        // POST: Cancel button — discard edits and reload
        //public IActionResult OnPostCancel()
        //{
        //    //var patientId = ResolvePatientId();
        //    if (patientId == null) return RedirectToPage("/PatientLogin");

        //    return RedirectToPage(new { id = patientId, edit = false });
        //}

        // Helpers
        //private int? ResolvePatientId()
        //{
        //    if (Id.HasValue) return Id;

        //    // fallback to Session set at login
        //    var fromSession = HttpContext.Session.GetInt32("PatientId");
        //    return fromSession;
        //}

        private static int CalculateAge(DateTime dob, DateTime today)
        {
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age < 0 ? 0 : age;
        }
    }
}
