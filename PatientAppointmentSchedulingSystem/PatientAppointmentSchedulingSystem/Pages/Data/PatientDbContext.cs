using Microsoft.EntityFrameworkCore;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext>options) : base (options) { }

        public DbSet<PatientDetails> Patients { get; set; }
        public DbSet<InsuranceProvider> InsuranceProvider {get; set; }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatientDetails>()
                .ToTable("Patients")
                .HasKey(p => p.PatientId); //Ensure define a primary key

            // InsuranceProvider table mapping
            modelBuilder.Entity<InsuranceProvider>()
                .ToTable("InsuranceProvider")
                .HasKey(ip => ip.InsuranceProviderId);

            // FK: Patients.InsuranceProviderId -> InsuranceProvider.InsuranceProviderId
            modelBuilder.Entity<PatientDetails>()
                .HasOne(p => p.InsuranceProviders)
                .WithMany(ip => ip.Patients)      // keep this if InsuranceProvider has ICollection<PatientDetails> Patients
                .HasForeignKey(p => p.InsuranceProviderId)
                .OnDelete(DeleteBehavior.SetNull); // matches SQL: ON DELETE SET NULL
        }
    }
}
