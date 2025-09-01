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
        public PatientDetails PatientDetails { get; set; } //use this

        //toggle edit mode via query: /PatientProfile?edit=true
        //[BindProperty]
        //public bool IsEditing { get; set; }

        //display purpose
        //public DateTime? LastUpdated { get; set; }
        //public string? ErrorMessage { get; set; }

        //view-modal that matches UI fields
        
        //[BindProperty]
        //public PatientProfileInput Input { get; set; }

        public int? idPatientSession { get; set; }


        //GET: load the patient (by ?id= OR Session) and populate Input
        public async Task<IActionResult> OnGet()
        {
            int? idPatientSession = HttpContext.Session.GetInt32("PatientId");


            //here is to check the session
            if (idPatientSession == null) //session die
            {
                //alert -> session dies
                return RedirectToPage("/PatientLogin");
            }
            else
            {
                //not null retrieve data from ur session adn store into the Patient object thenc an call at frontend
                PatientDetails = await _context.Patients.FindAsync((int)idPatientSession);

                if (PatientDetails == null)
                {
                    //return to login page
                    //record not found
                    return RedirectToPage("/PatientLogin");
                }
                else
                {
                    //required data, PatientFirstName, PatientLastName, PatientPhoneNum, PatientAge, PatientEmail, PatientPassword
                    //optional data, DateOfBirth, Gender, Adrdress, State, EmergencyContact, EmergencyPhone, InsuranceProvider, PolicyNumber, BloodType, Allergies, Medications

                    if (PatientDetails.DateOfBirth == null)
                    {
                        PatientDetails.DateOfBirth = "1900-01-01";
                    }

                    if (PatientDetails.Gender == null)
                    {
                        PatientDetails.Gender = "No Gender";
                    }
                    if (PatientDetails.Adrdress == null)
                    {
                        PatientDetails.Adrdress = "No Address";
                    }
                    if (PatientDetails.State == null)
                    {
                        PatientDetails.State = "No State";
                    }
                    if (PatientDetails.EmergencyContact == null)
                    {
                        PatientDetails.EmergencyContact = "No Contact Person";
                    }
                    if (PatientDetails.EmergencyPhone == null)
                    {
                        PatientDetails.EmergencyPhone = "No Contact Number";
                    }
                    if (PatientDetails.InsuranceProvider == null)
                    {
                        PatientDetails.InsuranceProvider = "No Contact Number";
                    }
                    //if (PatientDetails.PolicyNumber == null)
                    //{
                    //    PatientDetails.PolicyNumber = "No Contact Number";
                    //}
                    if (PatientDetails.BloodType == null)
                    {
                        PatientDetails.BloodType = "No Contact Number";
                    }
                    if (PatientDetails.Allergies == null)
                    {
                        PatientDetails.Allergies = "No Contact Number";
                    }
                    return Page(); //success data get

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

            }
        }

        // POST: Save button
        public async Task<IActionResult> OnPostSaveAsync()
        {
            OnGet;

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
