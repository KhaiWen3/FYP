using Microsoft.EntityFrameworkCore;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    public class ProviderDbContext : DbContext
    {
        public ProviderDbContext(DbContextOptions<ProviderDbContext>options) : base(options) { }

        public DbSet<ProviderDetails> Provider => Set<ProviderDetails>();
        public DbSet<Specialty> Specialty => Set<Specialty>();
        public DbSet<ProviderSpecialty> ProviderSpecialties => Set<ProviderSpecialty>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProviderDetails>()
                .HasKey(h => h.ProviderId); //Ensure define a primary key

            modelBuilder.Entity<ProviderSpecialty>()
            .HasKey(ps => new { ps.ProviderId, ps.SpecialtyId });

            // ⬇️ paste your 30-item seed here
            modelBuilder.Entity<Specialty>().HasData(
                new Specialty { SpecialtyId = 1, SpecialtyName = "Cardiology" },
                new Specialty { SpecialtyId = 2, SpecialtyName = "Internal Medicine" },
                new Specialty { SpecialtyId = 3, SpecialtyName = "Family Medicine" },
                new Specialty { SpecialtyId = 4, SpecialtyName = "Pediatrics" },
                new Specialty { SpecialtyId = 5, SpecialtyName = "Obstetrics & Gynecology" },
                new Specialty { SpecialtyId = 6, SpecialtyName = "Orthopedic Surgery" },
                new Specialty { SpecialtyId = 7, SpecialtyName = "General Surgery" },
                new Specialty { SpecialtyId = 8, SpecialtyName = "Neurology" },
                new Specialty { SpecialtyId = 9, SpecialtyName = "Psychiatry" },
                new Specialty { SpecialtyId = 10, SpecialtyName = "Dermatology" },
                new Specialty { SpecialtyId = 11, SpecialtyName = "Ophthalmology" },
                new Specialty { SpecialtyId = 12, SpecialtyName = "Otorhinolaryngology (ENT)" },
                new Specialty { SpecialtyId = 13, SpecialtyName = "Radiology (Diagnostic)" },
                new Specialty { SpecialtyId = 14, SpecialtyName = "Emergency Medicine" },
                new Specialty { SpecialtyId = 15, SpecialtyName = "Anesthesiology" },
                new Specialty { SpecialtyId = 16, SpecialtyName = "Urology" },
                new Specialty { SpecialtyId = 17, SpecialtyName = "Nephrology" },
                new Specialty { SpecialtyId = 18, SpecialtyName = "Endocrinology" },
                new Specialty { SpecialtyId = 19, SpecialtyName = "Gastroenterology" },
                new Specialty { SpecialtyId = 20, SpecialtyName = "Pulmonology (Respiratory)" },
                new Specialty { SpecialtyId = 21, SpecialtyName = "Oncology (Medical)" },
                new Specialty { SpecialtyId = 22, SpecialtyName = "Hematology" },
                new Specialty { SpecialtyId = 23, SpecialtyName = "Rheumatology" },
                new Specialty { SpecialtyId = 24, SpecialtyName = "Infectious Diseases" },
                new Specialty { SpecialtyId = 25, SpecialtyName = "Geriatrics" },
                new Specialty { SpecialtyId = 26, SpecialtyName = "Physical Medicine & Rehabilitation" },
                new Specialty { SpecialtyId = 27, SpecialtyName = "Pathology" },
                new Specialty { SpecialtyId = 28, SpecialtyName = "Nuclear Medicine" },
                new Specialty { SpecialtyId = 29, SpecialtyName = "Plastic Surgery" },
                new Specialty { SpecialtyId = 30, SpecialtyName = "Neurosurgery" }
            );

            modelBuilder.Entity<ProviderSpecialty>()
                .HasOne(ps => ps.Provider)
                .WithMany(p => p.ProviderSpecialties)
                .HasForeignKey(ps => ps.ProviderId);

            modelBuilder.Entity<ProviderSpecialty>()
                .HasOne(ps => ps.Specialty)
                .WithMany(s => s.ProviderSpecialties)
                .HasForeignKey(ps => ps.SpecialtyId);
        }
    }
}
