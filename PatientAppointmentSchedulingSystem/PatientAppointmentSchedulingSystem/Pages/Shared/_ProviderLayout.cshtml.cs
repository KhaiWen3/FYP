using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PatientAppointmentSchedulingSystem.Pages.Shared
{
    public class _ProviderLayoutModel : PageModel
    {
        private readonly ILogger<_ProviderLayoutModel> _logger;

        public _ProviderLayoutModel(ILogger<_ProviderLayoutModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
        }
    }
}
