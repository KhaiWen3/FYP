using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;
using System.Reflection.Metadata.Ecma335;
using PatientAppointmentSchedulingSystem.Helpers;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderProfileModel : PageModel
    {
        private readonly FirebaseStorageHelper _firebaseHelper;
        public string ProviderName { get; set; }

        [BindProperty]
        public ProviderDetails ProviderDetails { get; set; }
        [BindProperty]
        public List<Specialty> Specialty { get; set; } //this is to retrieve the name
        [BindProperty]
        public List<ProviderSpecialty> ProviderSpecial { get; set; } // this is retrieve the provider got which one
        [BindProperty]
        public List<int> SelectedSpecialtyIds { get; set; }
        [BindProperty]
        public List<Specialty> AllSpecialty { get; set; }
        [BindProperty]
        public IFormFile ProviderLogo { get; set; }

        private readonly ProviderDbContext _context;
        public ProviderProfileModel(ProviderDbContext context, FirebaseStorageHelper firebaseHelper)
        {
            _context = context;
            _firebaseHelper = firebaseHelper;
        }

        //[BindProperty]
        //public IFormFile ImageFile { get; set; }

        public async Task<IActionResult> OnGet()
        {
            int? providerIdFromSession = HttpContext.Session.GetInt32("ProviderId");

            if (providerIdFromSession == null)//session die
            {
                //alert -> session die
                TempData["AlertMsg"] = "Session Ended, Please Login.";
                return RedirectToPage("/ProviderLogin");
            }
            else
            {
                ProviderDetails = await _context.Provider.FindAsync((int)providerIdFromSession);
                ProviderSpecial = await _context.ProviderSpecialties.Where(ps => ps.ProviderId == (int)providerIdFromSession).ToListAsync();
                Specialty = await _context.Specialty.Where(s => ProviderSpecial.Select(ps=> ps.SpecialtyId).Contains(s.SpecialtyId)).ToListAsync();
                AllSpecialty = await _context.Specialty.ToListAsync();

                if (ProviderDetails == null)
                {
                    //return to login page
                    //record not found
                    TempData["AlertMsg"] = "Provider Not Found.";
                    return RedirectToPage("/ProviderLogin");
                }
                else
                {
                    //required data, Provider type, Provider Name, Ownership type, street address, state, phone number,  available specialties
                    //optional Hospital description, Logo, number of beds,

                    if (ProviderDetails.Description == null)
                    {
                        ProviderDetails.Description = "No Description.";
                    }

                    if (ProviderDetails.ProviderLogo == null)
                    {
                        ProviderDetails.ProviderLogo = "Default Image";
                    }

                    if (ProviderDetails.BedCount == 0 || ProviderDetails.BedCount == null)
                    {
                        ProviderDetails.BedCount = 0;
                    }
                    return Page(); //success data get
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
           /* if (!ModelState.IsValid)
            {
                TempData["AlertMsg"] = "Invalid Form Submission.";
                return Page();
            }*/
            if (ProviderDetails.ProviderType == "Hospital" && ProviderDetails.BedCount is null)
            {
                TempData["AlertMsg"] = "Bed count is required for hospitals.";
                /*ModelState.AddModelError("ProviderDetails.BedCount", "Bed count is required for hospitals.");*/
                return Page();
            }
            if (ProviderDetails.ProviderType == "Clinic") ProviderDetails.BedCount = null;


            int? providerIdFromSession = HttpContext.Session.GetInt32("ProviderId");

            if (providerIdFromSession == null)//session die
            {
                //alert -> session die
                TempData["AlertMsg"] = "Session Ended, Please Login.";
                return RedirectToPage("/ProviderLogin");
            }
            else
            {
                //i need to check is that the email is same or not then only check is existi or not if not same
                ProviderDetails pd = await _context.Provider.FindAsync(providerIdFromSession);
                if (!string.Equals(pd.Email, ProviderDetails.Email, StringComparison.OrdinalIgnoreCase))
                {
                    bool checkExistOrNot = await _context.Provider.AnyAsync(p => p.Email.ToLower() == ProviderDetails.Email.ToLower());

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
                            ProviderDetails.ProviderId = (int)providerIdFromSession;
                            ProviderDetails.Password = pd.Password;
                            _context.Entry(pd).State = EntityState.Detached;

                            //Firebase image upload for Provider
                            string logoUrl = null;
                            if (ProviderLogo != null)
                            {
                                using var stream = ProviderLogo.OpenReadStream();
                                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(ProviderLogo.FileName)}";

                                //upload to firebase
                                logoUrl = await _firebaseHelper.UploadImageAsync(stream, fileName, "ProviderLogo", ProviderLogo.ContentType);
                                TempData["AlertMsg"] = "Logo uploaded to Firebase: " + logoUrl;
                            }
                            else
                            {
                                TempData["AlertMsg"] = "No new logo uploaded, keeping old one.";
                            }
                            if (!string.IsNullOrEmpty(logoUrl))
                            {
                                ProviderDetails.ProviderLogo = logoUrl;   // ? save Firebase URL
                            }
                            else
                            {
                                ProviderDetails.ProviderLogo = pd.ProviderLogo;   // ? keep old
                            }
                                _context.Provider.Update(ProviderDetails);

                            //Remove old record
                            var existing = _context.ProviderSpecialties
                                .Where(ps => ps.ProviderId == (int)providerIdFromSession);
                            _context.ProviderSpecialties.RemoveRange(existing);

                            //Add new record
                            var newRecords = SelectedSpecialtyIds
                                .Distinct()
                                .Select(sid => new ProviderSpecialty { ProviderId = (int)providerIdFromSession, SpecialtyId = sid });
                            _context.ProviderSpecialties.AddRange(newRecords);

                            await _context.SaveChangesAsync();
                            TempData["AlertMsg"] = "Update Provider Successfully.";
                        }
                        catch(DbUpdateException ex)
                        {
                            Console.WriteLine("Inner Msg: " + ex.InnerException?.Message?? ex.Message);
                            TempData["AlertMsg"] = "DB UPDATE ERROR";
                        }
                        catch (Exception ex)
                        {
                            //Errror
                            TempData["AlertMsg"] = "Invalid Update Provider. Error Message : "+ex.Message;
                        }

                        ProviderDetails = await _context.Provider.FindAsync((int)providerIdFromSession);
                        ProviderSpecial = await _context.ProviderSpecialties.Where(ps => ps.ProviderId == (int)providerIdFromSession).ToListAsync();
                        Specialty = await _context.Specialty.Where(s => ProviderSpecial.Select(ps => ps.SpecialtyId).Contains(s.SpecialtyId)).ToListAsync();
                        AllSpecialty = await _context.Specialty.ToListAsync();

                        return Page();
                    }

                }
                else
                {
                    //continue the process
                    try
                    {
                        ProviderDetails.ProviderId = (int)providerIdFromSession;
                        ProviderDetails.Password = pd.Password;
                        _context.Entry(pd).State = EntityState.Detached;

                        //Firebase image upload for Provider
                        string logoUrl = null;
                        if (ProviderLogo != null)
                        {
                            using var stream = ProviderLogo.OpenReadStream();
                            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(ProviderLogo.FileName)}";

                            //upload to firebase
                            logoUrl = await _firebaseHelper.UploadImageAsync(stream, fileName, "ProviderLogo", ProviderLogo.ContentType);
                            TempData["AlertMsg"] = "Logo uploaded to Firebase: " + logoUrl;
                        }
                        else
                        {
                            TempData["AlertMsg"] = "No new logo uploaded, keeping old one.";
                        }
                        if (!string.IsNullOrEmpty(logoUrl))
                        {
                            ProviderDetails.ProviderLogo = logoUrl;   // ? save Firebase URL
                        }
                        else
                        {
                            ProviderDetails.ProviderLogo = pd.ProviderLogo;   // ? keep old
                        }

                        _context.Provider.Update(ProviderDetails);

                        //Remove old record
                        var existing = _context.ProviderSpecialties
                            .Where(ps => ps.ProviderId == (int)providerIdFromSession);
                        _context.ProviderSpecialties.RemoveRange(existing);

                        //Add new record
                        var newRecords = SelectedSpecialtyIds
                            .Distinct()
                            .Select(sid => new ProviderSpecialty { ProviderId = (int)providerIdFromSession, SpecialtyId = sid });
                        _context.ProviderSpecialties.AddRange(newRecords);

                        await _context.SaveChangesAsync();

                        TempData["AlertMsg"] = "Update Provider Successfully.";
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine("Inner Msg: " + ex.InnerException?.Message ?? ex.Message);
                        TempData["AlertMsg"] = "DB UPDATE ERROR";
                    }
                    catch (Exception ex)
                    {
                        //Errror
                        TempData["AlertMsg"] = "Invalid Update Provider. Error Message : " + ex.Message;
                    }

                    ProviderDetails = await _context.Provider.FindAsync((int)providerIdFromSession);
                    ProviderSpecial = await _context.ProviderSpecialties.Where(ps => ps.ProviderId == (int)providerIdFromSession).ToListAsync();
                    Specialty = await _context.Specialty.Where(s => ProviderSpecial.Select(ps => ps.SpecialtyId).Contains(s.SpecialtyId)).ToListAsync();
                    AllSpecialty = await _context.Specialty.ToListAsync();

                    return Page();
                }

            }
        }


    }
}
