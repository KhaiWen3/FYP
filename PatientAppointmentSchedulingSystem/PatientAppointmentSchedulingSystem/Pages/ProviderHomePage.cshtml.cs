using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PatientAppointmentSchedulingSystem.Pages
{
    public class ProviderHomePageModel : PageModel
    {
        public string Name { get; set; } = "Provider";
        public void OnGet()
        {
            // Replace with your actual provider name source (claims/session/db)
            if (User?.Identity?.IsAuthenticated == true)
                Name = User.Identity?.Name ?? "Provider";
        }
    }
}
