using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PatientAppointmentSchedulingSystem.Pages.Data;
using Microsoft.EntityFrameworkCore;


namespace PatientAppointmentSchedulingSystem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DoctorDbContext _context;

        private readonly ILogger<IndexModel> _logger;
        public List<DoctorDetails> Doctors { get; set; } = new List<DoctorDetails>();
        public List<Specialty> Specialties { get; set; } = new();
        public List<ProviderDetails> Providers { get; set; } = new();
        
        [BindProperty]
        public string InsuranceName { get; set; }
        public List<InsuranceProvider> InsuranceList { get; set; } = new();

        public IndexModel(DoctorDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void OnGet()
        {
            //base doctor query
            var doctorQuery = _context.Doctor
                .AsNoTracking()
                .Include(d => d.Provider)
                    .ThenInclude(p => p.ProviderSpecialties)
                        .ThenInclude(ps => ps.Specialty)   // only if you want SpecialtyName later
                .AsQueryable();

            Doctors = doctorQuery
                .Select(d => new DoctorDetails
                {
                    DoctorId = d.DoctorId,
                    DoctorFullName = d.DoctorFullName,   // ← THIS maps the DB column to your view model
                    DoctorMedicalService = d.DoctorMedicalService,
                    DoctorProviderName = d.Provider != null ? d.Provider.Name : null,
                    DoctorSpecialtyName = d.Specialty != null ? d.Specialty.SpecialtyName : null
                })
                .ToList(); // ← THIS executes the SQL and actually fetches the data
            
            Providers = _context.ProviderDetails
                .ToList();

        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            OnGet();
            if (!string.IsNullOrWhiteSpace(InsuranceName))
            {
                InsuranceList = await _context.InsuranceProvider
                    .Where(i => i.InsuranceProviderName.ToLower().Contains(InsuranceName.ToLower()))
                    .ToListAsync();
            }
            return Page();
        }
    }
}
