using Microsoft.EntityFrameworkCore;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    public class ProviderDbContext : DbContext
    {
        public ProviderDbContext(DbContextOptions<ProviderDbContext>options) : base(options) { }

        public DbSet<DoctorDetails> Doctor { get; set; }
        public DbSet<ProviderDetails> Provider => Set<ProviderDetails>();
        public DbSet<Specialty> Specialty => Set<Specialty>();
        public DbSet<ProviderSpecialty> ProviderSpecialties => Set<ProviderSpecialty>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctorDetails>().ToTable("Doctor").HasKey(d => d.DoctorId);

            modelBuilder.Entity<ProviderDetails>()
                .HasKey(h => h.ProviderId); //Ensure define a primary key

            modelBuilder.Entity<ProviderSpecialty>()
            .HasKey(ps => new { ps.ProviderId, ps.SpecialtyId });

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
