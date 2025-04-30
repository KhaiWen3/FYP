using Microsoft.EntityFrameworkCore;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext>options) : base(options) { }

        public DbSet<HospitalDetails> Hospital { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HospitalDetails>()
                .HasKey(h => h.HospitalId); //Ensure define a primary key
        }
    }
}
