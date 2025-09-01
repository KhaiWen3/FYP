using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PatientAppointmentSchedulingSystem.Pages.Data;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderProfileModel : PageModel
    {
        public string ProviderName { get; set; }

        [BindProperty]
        public ProviderDetails ProviderDetails { get; set; }

        private readonly ProviderDbContext _context;
        public ProviderProfileModel(ProviderDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            int? providerIdFromSession = HttpContext.Session.GetInt32("ProviderId");

            if (providerIdFromSession == null)//session die
            {
                //alert -> session die
                return RedirectToPage("/ProviderLogin");
            }
            else
            {
                ProviderDetails = await _context.Provider.FindAsync((int)providerIdFromSession);

                if (ProviderDetails == null)
                {
                    //return to login page
                    //record not found
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

                    if (ProviderDetails.Logo == null)
                    {
                        ProviderDetails.Logo = "Default Image";
                    }

                    if (ProviderDetails.BedCount == 0 || ProviderDetails.BedCount == null)
                    {
                        ProviderDetails.BedCount = 0;
                    }
                    return Page(); //success data get
                }
            }
        }
    }
}
