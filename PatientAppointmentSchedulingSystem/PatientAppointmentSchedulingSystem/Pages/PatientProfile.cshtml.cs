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
                    TempData["AlertMsg"] = "Patient Not Found.";
                    return RedirectToPage("/PatientLogin");
                }
                else
                {
                    //required data, PatientFirstName, PatientLastName, PatientPhoneNum, PatientAge, PatientEmail, PatientPassword
                    //optional data, DateOfBirth, Gender, Adrdress, State, EmergencyContact, EmergencyPhone, InsuranceProvider, PolicyNumber, BloodType, Allergies, Medications

                    if (PatientDetails.DateOfBirth == null)
                    {
                        //PatientDetails.DateOfBirth = null;
                        PatientDetails.DateOfBirth = new DateOnly(1900, 1, 1);
                    }

                    if (PatientDetails.Gender == null)
                    {
                        PatientDetails.Gender = "Not Specified";
                    }
                    if (PatientDetails.Address == null)
                    {
                        PatientDetails.Address = "Address Not Provided";
                    }
                    if (PatientDetails.State == null)
                    {
                        PatientDetails.State = "State Not Provided";
                    }
                    if (PatientDetails.EmergencyContact == null)
                    {
                        PatientDetails.EmergencyContact = "No Emergency Contact Person";
                    }
                    if (PatientDetails.EmergencyPhone == null)
                    {
                        PatientDetails.EmergencyPhone = "No Emergency Contact Number";
                    }
                    if (PatientDetails.InsuranceProvider == null)
                    {
                        PatientDetails.InsuranceProvider = "No Insurance Provider";
                    }
                    //if (PatientDetails.PolicyNumber == null)
                    //{
                    //    PatientDetails.PolicyNumber = "No Contact Number";
                    //}
                    if (PatientDetails.BloodType == null)
                    {
                        PatientDetails.BloodType = "Unknown";
                    }
                    if (PatientDetails.Allergies == null)
                    {
                        PatientDetails.Allergies = "No Known Allergies";
                    }
                    return Page(); //success data get

                }

            }
        }

        // POST: Save button
        public async Task<IActionResult> OnPostAsync()
        {
            int? idPatientSession = HttpContext.Session.GetInt32("PatientId");

            if(idPatientSession == null) //session die
            {
                TempData["AlertMsg"] = "Session Ended, Please Login.";
                return RedirectToPage("/PatientLogin");
            }
            else
            {
                // check is that the email is same?
                // then only check is exist? if not same
                PatientDetails PatientD = await _context.Patients.FindAsync(idPatientSession);
                if (!string.Equals(PatientD.PatientEmail, PatientDetails.PatientEmail, StringComparison.OrdinalIgnoreCase))
                {
                    bool checkExistOrNot = await _context.Patients.AnyAsync(p => p.PatientEmail.ToLower() == PatientDetails.PatientEmail.ToLower());

                    if (checkExistOrNot == true)
                    {
                        //exist
                        TempData["AlertMsg"] = "Email Exist.";
                        return Page();
                    }
                    else
                    {
                        //continue the process
                        try
                        {
                            PatientDetails.PatientId = (int)idPatientSession;
                            PatientDetails.PatientPassword = PatientD.PatientPassword;
                            _context.Entry(PatientD).State = EntityState.Detached;
                            _context.Patients.Update(PatientDetails);

                            await _context.SaveChangesAsync();
                            TempData["AlertMsg"] = "Update Patient Successfully.";
                        }
                        catch(DbUpdateException ex)
                        {
                            Console.WriteLine("Inner Msg: " + ex.InnerException?.Message ?? ex.Message);
                            TempData["AlertMsg"] = "DB UPDATE ERROR";
                        }
                        catch (Exception ex)
                        {
                            //Errror
                            TempData["AlertMsg"] = "Invalid Update Patient. Error Message : " + ex.Message;
                        }

                        PatientDetails = await _context.Patients.FindAsync((int)idPatientSession);

                        return Page();
                    }

                }
                else
                {
                    //continue the process
                    try
                    {
                        PatientDetails.PatientId = (int)idPatientSession;
                        PatientDetails.PatientPassword = PatientD.PatientPassword;
                        _context.Entry(PatientD).State = EntityState.Detached;
                        _context.Patients.Update(PatientDetails);

                        await _context.SaveChangesAsync();
                        TempData["AlertMsg"] = "Update Patient Successfully.";
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine("Inner Msg: " + ex.InnerException?.Message ?? ex.Message);
                        TempData["AlertMsg"] = "DB UPDATE ERROR";
                    }
                    catch (Exception ex)
                    {
                        //Errror
                        TempData["AlertMsg"] = "Invalid Update Patient. Error Message : " + ex.Message;
                    }

                    PatientDetails = await _context.Patients.FindAsync((int)idPatientSession);

                    return Page();
                }
            }
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
