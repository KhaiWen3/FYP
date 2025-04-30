using Microsoft.EntityFrameworkCore;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext>options) : base (options) { }

        public DbSet<PatientDetails> Patients { get; set; }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatientDetails>()
                .HasKey(p => p.PatientId); //Ensure define a primary key
        }
    }
}
